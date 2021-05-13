using CZToolKit.Core;
using CZToolKit.Core.Blackboards;
using CZToolKit.Core.SimpleFSM;
using CZToolKit.GraphProcessor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CZToolKit.GOAP
{
    public class GOAPAgent : GraphOwner<GOAPGraph>
    {
        #region 变量

        [SerializeField]
        [Space(10), Tooltip("要达成的目标")]
        List<Goal> goals = new List<Goal>();

        [SerializeField]
        [Tooltip("状态预设")]
        List<State> preState = new List<State>();

        /// <summary> 计划最大深度，<1无限制 </summary>
        [Space(10), Header("Settings")]
        public UpdateMode updateMode = UpdateMode.Normal;

        [Tooltip("计划最大深度，<1无限制")]
        public int maxDepth = 0;

        /// <summary> 两次搜索计划之间的间隔 </summary>
        [Min(0), Tooltip("两次搜索计划之间的间隔")]
        public float interval = 3;

        /// <summary> 计划异常终止是否要重置间隔 </summary>
        [Tooltip("计划异常终止是否立即重新搜寻计划")]
        public bool replanOnFailed = true;
        #endregion

        #region 公共属性

        public IGOAP Provider { get; private set; }
        public FSM FSM { get; private set; }
        public GOAPPlanner Planner { get; private set; }
        public Dictionary<string, ICZType> Blackboard { get; private set; } = new Dictionary<string, ICZType>();
        public List<Goal> Goals { get { return goals; } private set { goals = value; } }
        public Dictionary<string, bool> States { get; private set; }
        /// <summary> 当前计划 </summary>
        public Queue<GOAPAction> StoredActionQueue { get; private set; }
        /// <summary> 当前行为队列 </summary>
        public Queue<GOAPAction> ActionQueue { get; private set; }
        /// <summary> 当前行为 </summary>
        public GOAPAction CurrentAction { get; private set; }
        /// <summary> 当前目的，没有为空 </summary>
        public Goal CurrentGoal { get; private set; }
        public bool HasGoal { get { return CurrentGoal != null; } }
        public bool HasPlan { get { return ActionQueue != null && ActionQueue.Count > 0; } }
        /// <summary> 下此搜寻计划的时间 </summary>
        public float NextPlanTime { get; set; } = 0;
        #endregion

        protected virtual void Awake()
        {
            FSM = new FSM();
            Planner = new GOAPPlanner();
            Provider = GetComponent<IGOAP>();
            Graph = Graph.Clone() as GOAPGraph;
            Graph.Initialize(this);
            Goals = Goals.OrderByDescending(goal => goal.Priority).ToList();
            ActionQueue = new Queue<GOAPAction>();
        }

        protected virtual void Start()
        {
            States = new Dictionary<string, bool>();
            foreach (var item in preState)
            {
                States[item.Key] = item.Value;
            }

            GOAPState idleState = new GOAPState()
            {
                onUpdate = () =>
                {
                    if (TGraph == null) return;
                    if (Time.time < NextPlanTime) return;

                    NextPlanTime = Time.time + interval;

                    Queue<GOAPAction> plan = null;
                    // 搜寻计划
                    foreach (Goal goal in Goals)
                    {
                        plan = Planner.Plan(TGraph.AvailableActions.ToArray(), States, goal, maxDepth);
                        if (plan != null)
                        {
                            CurrentGoal = goal;
                            break;
                        }
                    }

                    if (plan != null)
                    {
                        StoredActionQueue = plan;

                        ActionQueue.Clear();
                        foreach (var action in plan)
                            ActionQueue.Enqueue(action);

                        //通知计划找到
                        //if (Provider != null)
                        //    Provider.PlanFound(CurrentGoal, ActionQueue);
                        //转换状态
                        FSM.ChangeTo("PerformActionState");
                    }
                    else
                    {
                        //通知计划没找到
                        if (Provider != null)
                            Provider.PlanFailed(Goals);
                        CurrentGoal = null;
                    }
                }
            };

            GOAPState performActionState = new GOAPState()
            {
                onUpdate = () =>
                {
                    if (HasPlan)
                    {
                        // 如果当前有计划(目标尚未完成)
                        GOAPAction action = ActionQueue.Peek();
                        if (CurrentAction != action)
                        {
                            CurrentAction = action;
                            action.PrePerform();
                        }
                        // 成功 or 失败
                        ActionStatus status = action.Perform();

                        switch (status)
                        {
                            case ActionStatus.Success:
                                action.PostPerform();
                                action.Success();
                                //foreach (var item in action.Effects)
                                //{
                                //    States[item.Key] = item.Value;
                                //}
                                if (Provider != null)
                                    Provider.ActionFinished(action.Effects);
                                ActionQueue.Dequeue();
                                CurrentAction = null;
                                break;
                            case ActionStatus.Failure:
                                if (replanOnFailed)
                                    EnforceReplan();
                                else
                                    AbortPlan();
                                return;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // 如果没有计划(目标已完成)
                        // 如果目标为一次性，移除掉
                        if (CurrentGoal.Once)
                            Goals.Remove(CurrentGoal);

                        // 通知计划完成
                        if (Provider != null)
                            Provider.PlanFinished();

                        // 当前目标设置为空
                        CurrentGoal = null;
                        FSM.ChangeTo("IdleState");
                    }
                }
            };

            FSM.PushState("IdleState", idleState);
            FSM.PushState("PerformActionState", performActionState);
            FSM.ChangeTo("IdleState");
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.AnimatePhysics) FSM.Update();
        }

        protected virtual void Update()
        {
            if (updateMode == UpdateMode.Normal || updateMode == UpdateMode.UnscaledTime) FSM.Update();
        }

        public void Evaluate()
        {
            if (updateMode == UpdateMode.Manual) FSM.Update();
        }

        /// <summary> 终止计划(在<see cref="interval"/>之后才会重新搜寻计划) </summary>
        public void AbortPlan()
        {
            if (HasPlan)
                ActionQueue.Clear();

            if (CurrentAction != null)
            {
                CurrentAction.PostPerform();
                // 如果动作执行失败，转换到空闲状态，并通知因为该动作导致计划失败  
                CurrentAction.Failed();
                //if (Provider != null)
                //    Provider.PlanAborted(CurrentAction);
            }

            CurrentAction = null;
            CurrentGoal = null;
            FSM.ChangeTo("IdleState");
        }

        /// <summary> 终止当前计划，并立即重新搜寻计划 </summary>
        public void EnforceReplan()
        {
            AbortPlan();
            NextPlanTime = Time.time;
            FSM.Update();
        }

        public virtual bool GetState(string _key, bool _fallback = false)
        {
            bool result;
            if (States.TryGetValue(_key, out result))
                return result;
            return _fallback;
        }

        /// <summary> 设置状态 </summary>
        public virtual void SetState(string _key, bool _value) { States[_key] = _value; }

        /// <summary> 移除状态 </summary>
        public virtual void RemoveState(string _key) { States.Remove(_key); }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "GOAP/GOAP_Icon.png", true);
            if (TGraph != null)
                TGraph.DrawGizmos(this);
        }
    }
}

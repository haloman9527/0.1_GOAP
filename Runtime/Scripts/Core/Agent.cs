using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CZToolKit.Core.Blackboards;

namespace CZToolKit.GOAP
{
    public class Agent : MonoBehaviour
    {
        #region 私有变量
        GOAPFSM fsm;
        GOAPFSMState idleState, performActionState;
        Planner planner;

        [SerializeField]
        [Tooltip("要达成的目标")]
        List<Goal> goals = new List<Goal>();
        Dictionary<string, bool> states = null;

        [SerializeField]
        [Tooltip("状态预设")]
        List<State> preState = new List<State>();


        Dictionary<string, ICZType> blackboard = new Dictionary<string, ICZType>();

        IGOAP provider = null;
        Goal currentGoal = null;
        Action[] availableActions = null;
        Queue<Action> storedActionQueue = new Queue<Action>();
        Queue<Action> actionQueue;
        Action currentAction = null;
        /// <summary> 下此搜寻计划的时间 </summary>
        float nextPlanTime = 0;

        [Header("Settings")]
        /// <summary> 计划最大深度，<1无限制 </summary>
        [Tooltip("计划最大深度，<1无限制")]
        public int maxDepth = 0;

        [Min(0)]
        [Tooltip("两次搜索计划之间的间隔")]
        /// <summary> 两次搜索计划之间的间隔 </summary>
        public float interval = 3;

        [Tooltip("计划异常终止是否要重置间隔")]
        /// <summary> 计划异常终止是否要重置间隔 </summary>
        public bool resetInterval = true;
        #endregion

        #region 公共属性
        public Planner Planner
        {
            get
            {
                if (planner == null)
                    planner = new Planner();
                return planner;
            }
        }
        public IGOAP Provider { get { return provider; } }

        public List<Goal> Goals { get { return goals; } }

        public Dictionary<string, bool> States
        {
            get
            {
                if (states == null)
                {
                    states = new Dictionary<string, bool>();
                    foreach (var item in preState)
                    {
                        states[item.Key] = item.Value;
                    }
                }
                return states;
            }
        }

        public Dictionary<string, ICZType> Blackboard { get { return blackboard; } }

        /// <summary> 当前目标，没有为空 </summary>
        public Goal CurrentGoal { get { return currentGoal; } }

        /// <summary> 当前计划 </summary>
        public Queue<Action> StoredActionQueue { get { return storedActionQueue; } }

        /// <summary> 当前行为队列 </summary>
        public Queue<Action> ActionQueue { get { return actionQueue; } }

        /// <summary> 所有可用的行为 </summary>
        public Action[] AvailableActions { get { return availableActions; } }

        /// <summary> 当前行为 </summary>
        public Action CurrentAction { get { return currentAction; } }

        public bool HasGoal { get { return CurrentGoal != null; } }

        public bool HasPlan { get { return ActionQueue != null && ActionQueue.Count > 0; } }
        #endregion

        protected virtual void Awake()
        {
            provider = GetComponent<IGOAP>();
            actionQueue = new Queue<Action>();
            fsm = new GOAPFSM(this);
            goals = Goals.OrderByDescending(goal => goal.Priority).ToList();
            availableActions = GetComponentsInChildren<Action>();
            foreach (Action action in availableActions)
            {
                action.Initialize(this);
            }
        }

        protected virtual void Start()
        {
            idleState = new GOAPFSMState()
            {
                onUpdate = () =>
                {
                    if (Time.time < nextPlanTime)
                        return;
                    nextPlanTime = Time.time + interval;

                    Queue<Action> plan = null;
                    // 搜寻计划
                    foreach (Goal goal in Goals)
                    {
                        plan = Planner.Plan(this, availableActions, States, goal);
                        if (plan != null)
                        {
                            currentGoal = goal;
                            break;
                        }
                    }

                    if (plan != null)
                    {
                        storedActionQueue = plan;

                        ActionQueue.Clear();
                        foreach (var action in plan)
                            ActionQueue.Enqueue(action);

                        //通知计划找到
                        if (provider != null)
                            provider.PlanFound(currentGoal, actionQueue);
                        //转换状态
                        fsm.ChangeTo("PerformActionState");
                    }
                    else
                    {
                        //通知计划没找到
                        if (provider != null)
                            provider.PlanFailed(Goals);
                        currentGoal = null;
                    }
                }
            };
            performActionState = new GOAPFSMState()
            {
                onUpdate = () =>
                {
                    if (HasPlan)
                    {
                        // 如果当前有计划(目标尚未完成)
                        Action action = actionQueue.Peek();
                        if (currentAction != action)
                        {
                            currentAction = action;
                            action.PrePerform();
                        }
                        // 成功 or 失败
                        bool performStatus = action.Perform();

                        if (performStatus)
                        {
                            //如果栈顶行为完成，出栈
                            if (action.IsDone())
                            {
                                action.Success();
                                action.PostPerform();

                                if (provider != null)
                                    provider.ActionFinished(action.Effects);
                                actionQueue.Dequeue();
                                currentAction = null;
                            }
                        }
                        else
                        {
                            AbortPlan();
                            return;
                        }
                    }
                    else
                    {
                        // 如果没有计划(目标已完成)
                        // 如果目标为一次性，移除掉
                        if (CurrentGoal.Once)
                            Goals.Remove(CurrentGoal);

                        // 通知计划完成
                        if (provider != null)
                            provider.PlanFinished();

                        // 当前目标设置为空
                        currentGoal = null;
                        fsm.ChangeTo("IdleState");
                    }
                }
            };

            fsm.PushState("IdleState", idleState);
            fsm.PushState("PerformActionState", performActionState);
            fsm.ChangeTo("IdleState");
        }

        protected virtual void LateUpdate() { fsm.LateUpdate(); }

        /// <summary> 终止计划(在<see cref="interval"/>之后才会重新搜寻计划) </summary>
        public void AbortPlan()
        {
            if (HasPlan && currentAction != null)
            {
                // 如果动作执行失败，转换到空闲状态，并通知因为该动作导致计划失败  
                currentAction.Failed();
                currentAction.PostPerform();
                if (Provider != null)
                    Provider.PlanAborted(currentAction);
                actionQueue.Clear();
            }
            currentAction = null;
            currentGoal = null;

            if (resetInterval)
                nextPlanTime = Time.time + interval;
            else
                nextPlanTime = Time.time;

            fsm.ChangeTo("IdleState");
        }

        /// <summary> 立即重新搜寻计划 </summary>
        public void EnforceReplan()
        {
            AbortPlan();
            nextPlanTime = Time.time;
            idleState.onUpdate();
        }

        /// <summary> 设置状态 </summary>
        public virtual void SetState(string _key, bool _value) { States[_key] = _value; }

        /// <summary> 移除状态 </summary>
        public virtual void RemoveState(string _key) { States.Remove(_key); }
    }
}
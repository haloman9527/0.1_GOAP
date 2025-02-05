#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using Moyo;
using System.Collections.Generic;
using System.Linq;
using Moyo.GraphProcessor;
using UnityEngine;

namespace Moyo.GOAP
{
    public class GOAPAgent : GraphAssetOwner<GOAPGraphAsset, GOAPGraphProcessor>
    {
        #region 变量

        [SerializeField]
        [Space(10), Tooltip("要达成的目标")]
        List<GOAPGoal> goals = new List<GOAPGoal>();

        [SerializeField]
        [Tooltip("状态预设")]
        List<GOAPState> preState = new List<GOAPState>();

        /// <summary> 计划最大深度，<1无限制 </summary>
        [Space(10), Header("Settings")]
        [SerializeField]
        UpdateMode updateMode = UpdateMode.Normal;

        [Tooltip("计划最大深度，<1无限制")]
        public int maxDepth = 0;

        /// <summary> 两次搜索计划之间的间隔 </summary>
        [Min(0), Tooltip("两次搜索计划之间的间隔")]
        public float interval = 3;

        /// <summary> 计划异常终止是否要重置间隔 </summary>
        [Tooltip("计划异常终止是否立即重新搜寻计划")]
        public bool replanOnFailed = true;

        private List<GOAPActionProcessor> plan = new List<GOAPActionProcessor>();
        private int currentActionIndex = -1;
        private GOAPGoal currentGoal;
        #endregion

        #region 公共属性

        public IGOAP Provider { get; private set; }
        public GOAPFSM FSM { get; private set; }
        public GOAPPlanner Planner { get; private set; }
        public Blackboard<string> Memory { get; private set; } = new Blackboard<string>();
        public List<GOAPGoal> Goals { get { return goals; } private set { goals = value; } }
        public Dictionary<string, bool> States { get; private set; }
        /// <summary> 当前计划 </summary>
        public IReadOnlyList<GOAPActionProcessor> Plan => plan;
        public int CurrentActionIndex => currentActionIndex;
        /// <summary> 当前行为 </summary>
        public GOAPActionProcessor CurrentAction => plan[currentActionIndex];
        /// <summary> 当前目的，没有为空 </summary>
        public GOAPGoal CurrentGoal => currentGoal;
        public bool HasGoal { get { return CurrentGoal != null; } }
        public bool HasPlan { get { return plan != null && plan.Count > 0 && currentActionIndex >= 0; } }
        /// <summary> 下此搜寻计划的时间 </summary>
        public float NextPlanTime { get; set; } = 0;

        #endregion

        protected virtual void Awake()
        {
            Provider = GetComponent<IGOAP>();
            Planner = new GOAPPlanner();
            FSM = new GOAPFSM();

            T_Graph.Initialize(this);
            Goals = Goals.OrderByDescending(goal => goal.Priority).ToList();
        }

        protected virtual void Start()
        {
            States = new Dictionary<string, bool>();
            foreach (var item in preState)
            {
                States[item.Key] = item.Value;
            }

            IdleState idleState = new IdleState(FSM)
            {
                onStart = () => { },
                onExit = () => { }
            };
            idleState.onUpdate = () =>
                 {
                     if (NextPlanTime > FSM.time) return;
                     if (T_GraphAsset == null) return;

                     NextPlanTime = FSM.time + interval;

                     // 搜寻计划
                     foreach (GOAPGoal goal in Goals)
                     {
                         plan.Clear();
                         Planner.Plan(T_Graph.AvailableActions.ToArray(), States, goal, maxDepth, plan);
                         // 如果找到了计划
                         if (plan.Count != 0)
                         {
                             currentGoal = goal;
                             break;
                         }
                     }

                     if (plan.Count > 0)
                     {
                         //通知计划找到
                         if (Provider != null)
                             Provider.PlanFound(CurrentGoal, plan);
                         //转换状态
                         FSM.JumpTo("PerformActionState");
                     }
                     else
                     {
                         //通知计划没找到
                         if (Provider != null)
                             Provider.PlanFailed(Goals);
                         currentGoal = null;
                         currentActionIndex = -1;
                     }
                 };

            GOAPFSMState performActionState = new GOAPFSMState(FSM)
            {
                onStart = () => { },
                onExit = () => { }
            };
            performActionState.onUpdate = () =>
            {
                if (HasPlan)
                {
                    // 如果当前有计划(目标尚未完成)
                    var action = CurrentAction;
                    // 成功 or 失败
                    GOAPActionStatus status = action.OnPerform();

                    switch (status)
                    {
                        case GOAPActionStatus.Success:
                        {
                            foreach (var effect in action.Effects)
                            {
                                SetState(effect.Key, effect.Value);
                            }

                            action.OnPostPerform(true);
                            if (Provider != null)
                                Provider.ActionFinished(action.Effects);

                            if (currentActionIndex + 1 >= plan.Count)
                            {
                                plan.Clear();
                                currentActionIndex = -1;
                            }
                            else
                            {
                                currentActionIndex++;
                            }

                            break;
                        }
                        case GOAPActionStatus.Failure:
                        {
                            if (replanOnFailed)
                                EnforceReplan();
                            else
                                AbortPlan();
                            return;
                        }
                    }
                }
                else
                {
                    // 如果没有计划(目标已完成)
                    // 如果目标为一次性，移除掉
                    if (currentGoal != null && currentGoal.Once)
                        Goals.Remove(currentGoal);

                    // 通知计划完成
                    if (Provider != null)
                        Provider.PlanFinished();

                    // 当前目标设置为空
                    currentGoal = null;
                    FSM.JumpTo("IdleState");
                }
            };

            FSM.PushState("IdleState", idleState);
            FSM.PushState("PerformActionState", performActionState);
            FSM.JumpTo("IdleState");
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.AnimatePhysics)
                Evaluate(Time.fixedDeltaTime);
        }

        protected virtual void Update()
        {
            switch (updateMode)
            {
                case UpdateMode.Normal:
                    Evaluate(Time.deltaTime);
                    break;
                case UpdateMode.UnscaledTime:
                    Evaluate(Time.unscaledDeltaTime);
                    break;
            }
        }

        public void Evaluate(float _deltaTime)
        {
            FSM.time += _deltaTime;
            FSM.Update();
        }

        /// <summary> 终止计划(在<see cref="interval"/>之后才会重新搜寻计划) </summary>
        public void AbortPlan()
        {
            if (!HasPlan)
            {
                return;
            }

            if (CurrentAction != null)
            {
                CurrentAction.OnPostPerform(false);
                // 如果动作执行失败，转换到空闲状态，并通知因为该动作导致计划失败
                if (Provider != null)
                    Provider.PlanAborted(CurrentAction);
            }

            plan.Clear();
            currentActionIndex = -1;
            currentGoal = null;
            FSM.JumpTo("IdleState");
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
            Gizmos.DrawIcon(transform.position, "GOAP/GOAP_Scene_Icon.png", true);
            if (enabled)
                T_Graph?.DrawGizmos(this);
        }
    }
}

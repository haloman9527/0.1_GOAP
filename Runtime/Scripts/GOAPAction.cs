using CZToolKit.GraphProcessor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.GOAP
{
    public enum ActionStatus
    {
        Running,
        Success,
        Failure
    }

    public interface IGOAPAction
    {
        string Name { get; }
        bool IsUsable { get; }

        void DynamicallyEvaluateCost();

        bool IsProceduralPrecondition(Dictionary<string, bool> currentState);

        void PrePerform();

        bool Perform();

        void PostPerform();

        void Success();

        void Failed();

        bool IsDone();

        void OnDrawGizmos();
    }

    public abstract class GOAPAction : BaseNode
    {
        /// <summary> 行为的名称 </summary>
        [Tooltip("行为名称")] [SerializeField, HideInInspector] string name;
        /// <summary> 行为的执行成本 </summary>
        [Tooltip("此行为的执行成本")] [HideInInspector] public float cost = 1;

        /// <summary> 执行此行为的前提条件 </summary>
        [Tooltip("此行为的前提条件")] [HideInInspector] public List<State> m_Preconditions = new List<State>();
        private Dictionary<string, bool> preconditions = null;

        /// <summary> 行为执行成功造成的效果 </summary>
        [Tooltip("行为可以造成的效果")] [HideInInspector] public List<State> m_Effects = new List<State>();

        private Dictionary<string, bool> effects = null;

        [Port(PortDirection.Input, IsMulti = true, TypeConstraint = PortTypeConstraint.None)]
        [PortSize(12), PortColor(0.1f, 0.5f, 0.1f)]
        [Tooltip("进入行为时触发")]
        public Action onEnter;

        [Port(PortDirection.Output, IsMulti = true, TypeConstraint = PortTypeConstraint.None)]
        [PortSize(12), PortColor(0.5f, 0.1f, 0.5f)]
        [Tooltip("退出行为时触发")]
        public Action onExit;

        public string Name { get { return name; } set { name = value; } }
        /// <summary> 行为所属代理 </summary>
        public GOAPAgent Agent { get; private set; }
        /// <summary> 冷却时间(可重载) </summary>
        public virtual float CooldownTime => 0f;
        /// <summary> 冷却计时器 </summary>
        public Cooldown Cooldown { get; private set; } = new Cooldown();

        /// <summary> 执行此行为的前提条件 </summary>
        public Dictionary<string, bool> Preconditions
        {
            get
            {
                if (preconditions == null)
                {
                    preconditions = new Dictionary<string, bool>();
                    foreach (var item in m_Preconditions)
                        preconditions[item.Key] = item.Value;
                }
                return preconditions;
            }
        }

        /// <summary> 此技能对世界状态造成的修改 </summary>
        public Dictionary<string, bool> Effects
        {
            get
            {
                if (effects == null)
                {
                    effects = new Dictionary<string, bool>();
                    foreach (var item in m_Effects)
                        effects[item.Key] = item.Value;
                }
                return effects;
            }
        }

        public override void Initialize(GraphOwner _graphOwner)
        {
            base.Initialize(_graphOwner);
            Agent = _graphOwner as GOAPAgent;
            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        /// <summary> 是否行为是否可用(可重载) </summary>
        public virtual bool IsUsable() { return true; }

        /// <summary> 动态评估成本 </summary>
        public virtual void DynamicallyEvaluateCost() { }

        /// <summary> 匹配计划过程中检查能否执行(应用计划执行过程中会导致的状态改变) </summary>
        public virtual bool IsProceduralPrecondition(Dictionary<string, bool> currentState) { return true; }

        public virtual void PrePerform() { }

        public abstract ActionStatus Perform();

        public virtual void PostPerform()
        {
            foreach (var effect in effects)
            {
                Agent.SetState(effect.Key, effect.Value);
            }
        }

        public virtual void Success() { }

        public virtual void Failed() { }

        /// <summary> 添加一条前提条件 </summary>
        public void SetPrecondition(string key, bool value)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                State state = m_Preconditions.Find(item => item.Key == key);
                if (state == null)
                    m_Preconditions.Add(new State(key, value));
                else
                    state.Value = value;
            }
            else
#endif
                Preconditions[key] = value;
        }

        /// <summary> 移除一条前提条件 </summary>
        public void RemovePrecondition(string key)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_Preconditions.RemoveAll(item => item.Key == key);
            else
#endif
                Preconditions.Remove(key);
        }

        /// <summary> 添加一条效果 </summary>
        public void SetEffect(string key, bool value)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                State state = m_Effects.Find(item => item.Key == key);
                if (state == null)
                    m_Effects.Add(new State(key, value));
                else
                    state.Value = value;
            }
            else
#endif
                Effects[key] = value;
        }

        /// <summary> 移除一条效果 </summary>
        public void RemoveEffect(string key)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_Effects.RemoveAll(item => item.Key == key);
            else
#endif
                Effects.Remove(key);
        }
    }
}

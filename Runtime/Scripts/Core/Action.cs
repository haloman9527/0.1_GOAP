using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using State = CZToolKit.GOAP.State;

namespace CZToolKit.GOAP
{
    [Serializable]
    public abstract class Action : MonoBehaviour
    {
        /// <summary> 行为的名称 </summary>
        [Tooltip("行为名称")] public string Name;

        /// <summary> 行为的执行成本 </summary>
        [Tooltip("此行为的执行成本")] public float cost = 1;

        /// <summary> 执行此行为的前提条件 </summary>
        [Tooltip("此行为的前提条件")] [SerializeField] private List<State> m_Preconditions = new List<State>();
        private Dictionary<string, bool> preconditions = null;

        /// <summary> 行为执行成功造成的效果 </summary>
        [Tooltip("行为成功造成的效果")] [SerializeField] private List<State> m_Effects = new List<State>();
        private Dictionary<string, bool> effects = null;

        /// <summary> 行为所属代理 </summary>
        public Agent Agent { get; private set; }
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

        /// <summary> 行为初始化 </summary>
        /// <param name="_agent"></param>
        public void Initialize(Agent _agent)
        {
            this.Agent = _agent;

            preconditions = new Dictionary<string, bool>();
            foreach (var item in m_Preconditions)
            {
                preconditions[item.Key] = item.Value;
            }

            effects = new Dictionary<string, bool>();
            foreach (var item in m_Effects)
            {
                effects[item.Key] = item.Value;
            }

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        /// <summary> 是否行为是否可用(可重载) </summary>
        public virtual bool IsUsable() { return true; }

        /// <summary> 动态评估成本 </summary>
        public virtual void DynamicallyEvaluateCost() { }

        /// <summary> 检查能否执行 </summary>
        public virtual bool IsProceduralPrecondition(Dictionary<string, bool> currentState) { return true; }

        /// <summary> 在行为开始前执行一次 </summary>
        public virtual void PrePerform() { }

        /// <summary> 执行行为 </summary>
        public abstract bool Perform();

        public virtual void Success() { }

        public virtual void Failed() { }

        /// <summary> 在行为结束后执行一次 </summary>
        public virtual void PostPerform() { }

        /// <summary> 是否完成动作 </summary>
        public abstract bool IsDone();

        /// <summary> 添加一条前提条件 </summary>
        public void AddPrecondition(string key, bool value)
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
        public void AddEffect(string key, bool value)
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

        protected virtual void Reset()
        {
            Name = GetType().Name;
        }
    }
}
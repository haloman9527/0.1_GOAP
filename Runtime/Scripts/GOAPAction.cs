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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using CZToolKit.GraphProcessor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.GOAP
{
    public abstract class GOAPAction : BaseNode
    {
        /// <summary> 行为的名称 </summary>
        [Tooltip("行为名称")]
        [SerializeField] protected string name;

        /// <summary> 行为的执行成本 </summary>
        [Tooltip("此行为的执行成本")]
        [SerializeField] protected float cost = 1;

        /// <summary> 执行此行为的前提条件 </summary>
        [Tooltip("此行为的前提条件")]
        [SerializeField] protected List<GOAPState> preconditions = new List<GOAPState>();

        /// <summary> 行为执行成功造成的效果 </summary>
        [Tooltip("行为可以造成的效果")]
        [SerializeField] protected List<GOAPState> effects = new List<GOAPState>();

        #region ViewModel
        public event Action<GOAPState> onPreconditionAdded;
        public event Action<GOAPState> onPreconditionRemoved;
        public event Action<GOAPState> onEffectAdded;
        public event Action<GOAPState> onEffectRemoved;

        public string Name
        {
            get { return GetPropertyValue<string>(nameof(Name)); }
            set { SetPropertyValue(nameof(Name), value); }
        }
        public float Cost
        {
            get { return GetPropertyValue<float>(nameof(Cost)); }
            set { SetPropertyValue(nameof(Cost), value); }
        }
        public IReadOnlyList<GOAPState> Preconditions { get { return preconditions; } }
        public IReadOnlyList<GOAPState> Effects { get { return effects; } }
        /// <summary> 冷却时间(可重载) </summary>
        public virtual float CooldownTime => 0f;
        /// <summary> 冷却计时器 </summary>
        public Cooldown Cooldown { get; private set; } = new Cooldown();

        public GOAPAgent Agent { get; set; }

        public override void InitializeBindableProperties()
        {
            base.InitializeBindableProperties();
            SetBindableProperty(nameof(Name), new BindableProperty<string>(name, v => { name = v; }));
            SetBindableProperty(nameof(Cost), new BindableProperty<float>(cost, v => { cost = v; }));
        }

        public override void Initialize(IGraphOwner _graphOwner)
        {
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

        public virtual void OnPrePerform() { }

        public abstract GOAPActionStatus OnPerform();

        public virtual void OnPostPerform(bool _successed) { }

        public void AddPrecondition(GOAPState _precondition)
        {
            preconditions.Add(_precondition);
            onPreconditionAdded?.Invoke(_precondition);
        }

        public void AddEffect(GOAPState _effect)
        {
            effects.Add(_effect);
            onEffectAdded?.Invoke(_effect);
        }

        public void RemovePrecondition(GOAPState _precondition)
        {
            onPreconditionRemoved?.Invoke(null);
        }

        public void RemoveEffect(GOAPState _effect)
        {
            onEffectRemoved?.Invoke(null);
        }
        #endregion
    }
}

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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using CZToolKit.VM;
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
        public string name;

        /// <summary> 行为的执行成本 </summary>
        [Tooltip("此行为的执行成本")]
        public float cost = 1;

        /// <summary> 执行此行为的前提条件 </summary>
        [Tooltip("此行为的前提条件")]
        public List<GOAPState> preconditions = new List<GOAPState>();

        /// <summary> 行为执行成功造成的效果 </summary>
        public List<GOAPState> effects = new List<GOAPState>();
    }

    public abstract class GOAPActionVM : BaseNodeProcessor
    {
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
        public IReadOnlyList<GOAPState> Preconditions
        {
            get { return (Model as GOAPAction).preconditions; }
        }
        public IReadOnlyList<GOAPState> Effects
        {
            get { return (Model as GOAPAction).effects; }
        }
        /// <summary> 冷却时间(可重载) </summary>
        public virtual float CooldownTime
        {
            get { return 0; }
        }
        /// <summary> 冷却计时器 </summary>
        public Cooldown Cooldown
        {
            get;
            private set;
        } = new Cooldown();
        public GOAPAgent Agent
        {
            get;
            set;
        }

        public GOAPActionVM(BaseNode model) : base(model)
        {
            var t_model = Model as GOAPAction;
            this[nameof(Name)] = new BindableProperty<string>(() => t_model.name, v => t_model.name = v);
            this[nameof(Cost)] = new BindableProperty<float>(() => t_model.cost, v => t_model.cost = v);
        }

        public virtual void Initialized(GOAPAgent agent)
        {
            Agent = agent;
        }

        public virtual void OnAdded() { }

        /// <summary> 行为是否可用(可重载) </summary>
        public virtual bool IsUsable()
        {
            return true;
        }

        /// <summary> 匹配计划过程中检查能否执行(应用计划执行过程中会导致的状态改变) </summary>
        public virtual bool IsProceduralPrecondition(Dictionary<string, bool> currentState)
        {
            return true;
        }

        /// <summary> 动态评估成本 </summary>
        public virtual void DynamicallyEvaluateCost() { }

        public virtual void OnPrePerform() { }

        public abstract GOAPActionStatus OnPerform();

        public virtual void OnPostPerform(bool successed) { }

        public void AddPrecondition(GOAPState precondition)
        {
            var t_model = Model as GOAPAction;
            t_model.preconditions.Add(precondition);
            onPreconditionAdded?.Invoke(precondition);
        }

        public void RemovePrecondition(GOAPState precondition)
        {
            var t_model = Model as GOAPAction;
            if (t_model.preconditions.Remove(precondition))
                onPreconditionRemoved?.Invoke(precondition);
        }

        public void AddEffect(GOAPState _effect)
        {
            var t_model = Model as GOAPAction;
            t_model.effects.Add(_effect);
            onEffectAdded?.Invoke(_effect);
        }

        public void RemoveEffect(GOAPState effect)
        {
            var t_model = Model as GOAPAction;
            if (t_model.effects.Remove(effect))
                onEffectRemoved?.Invoke(effect);
        }
    }
}

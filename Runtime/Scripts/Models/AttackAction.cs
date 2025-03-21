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
using Atom;
using System.Collections.Generic;
using Atom.GraphProcessor;

namespace Atom.GOAP
{
    [NodeMenu("Attack")]
    public class AttackAction : GOAPAction { }

    [ViewModel(typeof(AttackAction))]
    public class AttackActionProcessor : GOAPActionProcessor
    {
        public AttackActionProcessor(BaseNode model) : base(model)
        {
            var t_model = model as AttackAction;

            t_model.name = "攻击";
            t_model.preconditions.Add(new GOAPState() { Key = "HasTarget", Value = true });
            t_model.preconditions.Add(new GOAPState() { Key = "InAttackRange", Value = true });
            t_model.effects.Add(new GOAPState() { Key = "KillTarget", Value = true });
        }

        public override GOAPActionStatus OnPerform()
        {
            return GOAPActionStatus.Running;
        }

        public override void OnPostPerform(bool _successed)
        {
            //base.PostPerform();
            // 如果没有击杀敌人
            // 设置HasTarget = true
            // 设置InAttackRange = false
            // 设置KillTarget = false

            // 如果击杀了敌人
            // 设置HasTarget = false
            // 设置InAttackRange = false
            // 设置KillTarget = true
        }
    }
}

using CZToolKit.GraphProcessor;

namespace CZToolKit.GOAP
{
    [NodeMenuItem("Attack")]
    public class AttackAction : GOAPAction
    {
        public AttackAction() : base()
        {
            name = "攻击";

            preconditions.Add(new GOAPState() { Key = "HasTarget", Value = true });
            preconditions.Add(new GOAPState() { Key = "InAttackRange", Value = true });

            effects.Add(new GOAPState() { Key = "KillTarget", Value = true });
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

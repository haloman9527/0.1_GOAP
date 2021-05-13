using CZToolKit.GraphProcessor;

namespace CZToolKit.GOAP
{
    [NodeMenuItem("Attack")]
    public class AttackAction : GOAPAction
    {
        public override void OnCreated()
        {
            base.OnCreated();

            Name = "攻击";

            SetPrecondition("HasTarget", true);
            SetPrecondition("InAttackRange", true);

            SetEffect("KillTarget", true);
        }

        public override ActionStatus Perform()
        {
            return ActionStatus.Running;
        }

        public override void PostPerform()
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

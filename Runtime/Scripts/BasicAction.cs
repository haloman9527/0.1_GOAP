using UnityEngine;

namespace CZToolKit.GOAP
{
    public abstract class BasicAction : GOAPAction
    {
        public Vector2 cooldownRange = new Vector2(0, 5);
        public override float CooldownTime => Random.Range(cooldownRange.x, cooldownRange.y);

        public override bool IsUsable()
        {
            return !Cooldown.Active;
        }

        public override abstract bool Perform();

        public override void PostPerform()
        {
            Cooldown.Run(CooldownTime);
        }

        public override abstract bool IsDone();
    }
}

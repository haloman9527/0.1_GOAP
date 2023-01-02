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
using CZToolKit.Core.ViewModel;
using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeMenu("Movement/Seek")]
    public class Seek : NavMeshMovement
    {
        [Tooltip("If target is null then use the target position")]
        public Vector3 targetPosition;
    }

    [ViewModel(typeof(Seek))]
    public class SeekVM : NavMeshMovementVM
    {
        public GameObject target;

        public SeekVM(BaseNode model) : base(model) { }

        public override void OnAdded()
        {
            base.OnAdded();
            var t_model = Model as Seek;
            t_model.preconditions.Add(new GOAPState("HasTarget", true));
            t_model.effects.Add(new GOAPState("InXXXRange", true));
            t_model.targetPosition = Vector3.zero;
        }

        public override bool IsUsable()
        {
            return !Agent.GetState("HasTarget");
        }

        public override void OnPrePerform()
        {
            base.OnPrePerform();
            Agent.Memory.TryGet("Target", out target);
            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override GOAPActionStatus OnPerform()
        {
            if (HasArrived())
                return GOAPActionStatus.Success;
            SetDestination(Target());
            return GOAPActionStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target != null)
                return target.transform.position;
            var t_model = Model as Seek;
            return t_model.targetPosition;
        }
    }
}
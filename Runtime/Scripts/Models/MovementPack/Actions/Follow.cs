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
using CZToolKit.SharedVariable;
using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeTooltip("跟随一个目标，使用NavMesh移动")]
    [NodeMenu("Movement/Follow")]
    public class Follow : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is following")]
        public SharedGameObject target;
        [Tooltip("Start moving towards the target if the target is further than the specified distance")]
        public SharedFloat moveDistance = new SharedFloat(2);
    }

    [ViewModel(typeof(Follow))]
    public class FollowVM : NavMeshMovementVM
    {
        Vector3 lastTargetPosition;
        bool hasMoved;

        public FollowVM(BaseNode model) : base(model) { }

        public override void OnAdded()
        {
            base.OnAdded();
            var t_model = Model as Follow;
            t_model.name = "跟随";
        }

        public override void OnPrePerform()
        {
            base.OnPrePerform();
            var t_model = Model as Follow;
            if (t_model.target.Value == null)
                return;
            lastTargetPosition = t_model.target.Value.transform.position + Vector3.one * (t_model.moveDistance.Value + 1);
            hasMoved = false;
        }

        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as Follow;
            if (t_model.target.Value == null)
                return GOAPActionStatus.Failure;

            // Move if the target has moved more than the moveDistance since the last time the agent moved.
            var targetPosition = t_model.target.Value.transform.position;
            if ((targetPosition - lastTargetPosition).magnitude >= t_model.moveDistance.Value)
            {
                SetDestination(targetPosition);
                lastTargetPosition = targetPosition;
                hasMoved = true;
            }
            else
            {
                // Stop moving if the agent is within the moveDistance of the target.
                if (hasMoved && (targetPosition - Agent.transform.position).magnitude < t_model.moveDistance.Value)
                {
                    Stop();
                    hasMoved = false;
                    lastTargetPosition = targetPosition;
                }
            }

            return GOAPActionStatus.Running;
        }
    }
}
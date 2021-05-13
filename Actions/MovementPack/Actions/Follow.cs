#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using CZToolKit.Core.SharedVariable;
using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    //[TaskDescription("Follows the specified target using the Unity NavMesh.")]
    //[TaskCategory("Movement")]
    //[HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    //[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FollowIcon.png")]
    [NodeMenuItem("Follow(MP)")]
    public class Follow : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is following")]
        public SharedGameObject target;
        [Tooltip("Start moving towards the target if the target is further than the specified distance")]
        public SharedFloat moveDistance = new SharedFloat(2);

        private Vector3 lastTargetPosition;
        private bool hasMoved;

        public override void PrePerform()
        {
            base.PrePerform();
            if (target.Value == null)
            {
                return;
            }

            lastTargetPosition = target.Value.transform.position + Vector3.one * (moveDistance.Value + 1);
            hasMoved = false;
        }

        public override ActionStatus Perform()
        {
            if (target.Value == null)
                return ActionStatus.Failure;

            // Move if the target has moved more than the moveDistance since the last time the agent moved.
            var targetPosition = target.Value.transform.position;
            if ((targetPosition - lastTargetPosition).magnitude >= moveDistance.Value)
            {
                SetDestination(targetPosition);
                lastTargetPosition = targetPosition;
                hasMoved = true;
            }
            else
            {
                // Stop moving if the agent is within the moveDistance of the target.
                if (hasMoved && (targetPosition - Agent.transform.position).magnitude < moveDistance.Value)
                {
                    Stop();
                    hasMoved = false;
                    lastTargetPosition = targetPosition;
                }
            }

            return ActionStatus.Running;
        }
    }
}
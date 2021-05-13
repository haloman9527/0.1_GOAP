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
    //[TaskDescription("Move towards the specified position. The position can either be specified by a transform or position. If the transform " +
    //                 "is used then the position will not be used.")]
    //[TaskCategory("Movement")]
    //[HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    //[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}MoveTowardsIcon.png")]
    [NodeMenuItem("MoveTowards(MP)")]
    public class MoveTowards : GOAPAction
    {
        [Tooltip("The speed of the agent")]
        public SharedFloat speed;
        [Tooltip("The agent has arrived when the magnitude is less than this value")]
        public SharedFloat arriveDistance = new SharedFloat(0.1f);
        [Tooltip("Should the agent be looking at the target position?")]
        public SharedBool lookAtTarget = new SharedBool(true);
        [Tooltip("Max rotation delta if lookAtTarget is enabled")]
        public SharedFloat maxLookAtRotationDelta;
        [Tooltip("The GameObject that the agent is moving towards")]
        public SharedGameObject target;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;


        // Return targetPosition if targetTransform is null
        private Vector3 Target()
        {
            if (target == null || target.Value == null)
            {
                return targetPosition.Value;
            }
            return target.Value.transform.position;
        }

        public override ActionStatus Perform()
        {
            var position = Target();
            // Return a task status of success once we've reached the target
            if (Vector3.Magnitude(Agent.transform.position - position) < arriveDistance.Value)
            {
                return ActionStatus.Success;
            }
            // We haven't reached the target yet so keep moving towards it
            Agent.transform.position = Vector3.MoveTowards(Agent.transform.position, position, speed.Value * Time.deltaTime);
            if (lookAtTarget.Value && (position - Agent.transform.position).sqrMagnitude > 0.01f)
            {
                Agent.transform.rotation = Quaternion.RotateTowards(Agent.transform.rotation, Quaternion.LookRotation(position - Agent.transform.position), maxLookAtRotationDelta.Value);
            }
            return ActionStatus.Running;
        }
    }
}
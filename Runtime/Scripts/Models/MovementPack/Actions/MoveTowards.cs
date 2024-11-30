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
using Moyo;
using Moyo.GraphProcessor;
using Moyo.SharedVariable;
using UnityEngine;

namespace Moyo.GOAP.Actions.Movement
{
    [NodeTooltip("Move towards the specified position. The position can either be specified by a transform or position. If the transform " +
                     "is used then the position will not be used.")]
    [NodeMenu("Movement/MoveTowards")]
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
    }

    [ViewModel(typeof(MoveTowards))]
    public class MoveTowardsProcessor : GOAPActionProcessor
    {
        public MoveTowardsProcessor(BaseNode model) : base(model) { }

        // Return targetPosition if targetTransform is null
        private Vector3 Target()
        {
            var t_model = Model as MoveTowards;
            if (t_model.target == null || t_model.target.Value == null)
            {
                return t_model.targetPosition.Value;
            }
            return t_model.target.Value.transform.position;
        }

        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as MoveTowards;
            var position = Target();
            // Return a task status of success once we've reached the target
            if (Vector3.Magnitude(Agent.transform.position - position) < t_model.arriveDistance.Value)
            {
                return GOAPActionStatus.Success;
            }
            // We haven't reached the target yet so keep moving towards it
            Agent.transform.position = Vector3.MoveTowards(Agent.transform.position, position, t_model.speed.Value * Time.deltaTime);
            if (t_model.lookAtTarget.Value && (position - Agent.transform.position).sqrMagnitude > 0.01f)
            {
                Agent.transform.rotation = Quaternion.RotateTowards(Agent.transform.rotation, Quaternion.LookRotation(position - Agent.transform.position), t_model.maxLookAtRotationDelta.Value);
            }
            return GOAPActionStatus.Running;
        }
    }
}
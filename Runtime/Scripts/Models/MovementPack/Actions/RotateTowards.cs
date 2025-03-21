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

using Atom.GraphProcessor;
using Atom.SharedVariable;
using Atom;
using UnityEngine;

namespace Atom.GOAP.Actions.Movement
{
    [NodeTooltip("Rotates towards the specified rotation. The rotation can either be specified by a transform or rotation. If the transform " +
                     "is used then the rotation will not be used.")]
    [NodeMenu("Movement/RotateTowards")]
    public class RotateTowards : GOAPAction
    {
        [Tooltip("Should the 2D version be used?")]
        public bool usePhysics2D;
        [Tooltip("The agent is done rotating when the angle is less than this value")]
        public SharedFloat rotationEpsilon = new SharedFloat();
        [Tooltip("The maximum number of angles the agent can rotate in a single tick")]
        public SharedFloat maxLookAtRotationDelta = new SharedFloat(1);
        [Tooltip("Should the rotation only affect the Y axis?")]
        public SharedBool onlyY;
        [Tooltip("The GameObject that the agent is rotating towards")]
        public SharedGameObject target;
        [Tooltip("If target is null then use the target rotation")]
        public SharedVector3 targetRotation;
    }

    [ViewModel(typeof(RotateTowards))]
    public class RotateTowardsProcessor : GOAPActionProcessor
    {
        public RotateTowardsProcessor(BaseNode model) : base(model) { }

        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as RotateTowards;
            var rotation = Target();
            // Return a task status of success once we are done rotating
            if (Quaternion.Angle(Agent.transform.rotation, rotation) < t_model.rotationEpsilon.Value)
            {
                return GOAPActionStatus.Success;
            }
            // We haven't reached the target yet so keep rotating towards it
            Agent.transform.rotation = Quaternion.RotateTowards(Agent.transform.rotation, rotation, t_model.maxLookAtRotationDelta.Value);
            return GOAPActionStatus.Running;
        }

        // Return targetPosition if targetTransform is null
        private Quaternion Target()
        {
            var t_model = Model as RotateTowards;
            if (t_model.target == null || t_model.target.Value == null)
            {
                return Quaternion.Euler(t_model.targetRotation.Value);
            }
            var position = t_model.target.Value.transform.position - Agent.transform.position;
            if (t_model.onlyY.Value)
            {
                position.y = 0;
            }
            if (t_model.usePhysics2D)
            {
                var angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
                return Quaternion.AngleAxis(angle, Vector3.forward);
            }
            return Quaternion.LookRotation(position);
        }
    }
}
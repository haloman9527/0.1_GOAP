using CZToolKit.Core.SharedVariable;
using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeMenuItem("Movement", "Flee")]
    public class Flee : NavMeshMovement
    {
        [Tooltip("The agent has fleed when the magnitude is greater than this value")]
        public SharedFloat fleedDistance = new SharedFloat(20);
        [Tooltip("The distance to look ahead when fleeing")]
        public float lookAheadDistance = 5;
        [Tooltip("The GameObject that the agent is fleeing from")]
        public SharedGameObject target;

        private bool hasMoved;

        public override void OnCreated()
        {
            base.OnCreated();
            Name = "ÌÓÀë";
        }

        public override void PrePerform()
        {
            base.PrePerform();
            hasMoved = false;
            SetDestination(Target());
        }

        // Flee from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override ActionStatus Perform()
        {
            if (Vector3.Magnitude(Agent.transform.position - target.Value.transform.position) > fleedDistance.Value)
            {
                return ActionStatus.Success;
            }

            if (HasArrived())
            {
                if (!hasMoved)
                {
                    return ActionStatus.Failure;
                }
                if (!SetDestination(Target()))
                {
                    return ActionStatus.Failure;
                }
                hasMoved = false;
            }
            else
            {
                // If the agent is stuck the task shouldn't continue to return a status of running.
                var velocityMagnitude = Velocity().sqrMagnitude;
                if (hasMoved && velocityMagnitude <= 0f)
                {
                    return ActionStatus.Failure;
                }
                hasMoved = velocityMagnitude > 0f;
            }

            return ActionStatus.Running;
        }

        // Flee in the opposite direction
        private Vector3 Target()
        {
            return Agent.transform.position + (Agent.transform.position - target.Value.transform.position).normalized * lookAheadDistance;
        }

        // Return false if the position isn't valid on the NavMesh.
        protected override bool SetDestination(Vector3 destination)
        {
            if (!SamplePosition(destination))
            {
                return false;
            }
            return base.SetDestination(destination);
        }
    }
}
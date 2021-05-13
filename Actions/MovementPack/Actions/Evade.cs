using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeMenuItem("Evade(MP)")]
    public class Evade : NavMeshMovement
    {
        [Tooltip("The agent has evaded when the magnitude is greater than this value")]
        public float evadeDistance = 10;
        [Tooltip("The distance to look ahead when evading")]
        public float lookAheadDistance = 5;
        [Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
        public float targetDistPrediction = 20;
        [Tooltip("Multiplier for predicting the look ahead distance")]
        public float targetDistPredictionMult = 20;
        [Tooltip("The GameObject that the agent is evading")]
        public GameObject target;

        // The position of the target at the last frame
        private Vector3 targetPosition;

        public override void PrePerform()
        {
            base.PrePerform();
            targetPosition = target.transform.position;
            SetDestination(Target());
        }

        // Evade from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override ActionStatus Perform()
        {
            SetDestination(Target());
            if (Vector3.Magnitude(Agent.transform.position - target.transform.position) > evadeDistance)
                return ActionStatus.Success;

            return ActionStatus.Running;
        }

        // Evade in the opposite direction
        private Vector3 Target()
        {
            // Calculate the current distance to the target and the current speed
            var distance = (target.transform.position - Agent.transform.position).magnitude;
            var speed = Velocity().magnitude;

            float futurePrediction = 0;
            // Set the future prediction to max prediction if the speed is too small to give an accurate prediction
            if (speed <= distance / targetDistPrediction)
            {
                futurePrediction = targetDistPrediction;
            }
            else
            {
                futurePrediction = (distance / speed) * targetDistPredictionMult; // the prediction should be accurate enough
            }

            // Predict the future by taking the velocity of the target and multiply it by the future prediction
            var prevTargetPosition = targetPosition;
            targetPosition = target.transform.position;
            var position = targetPosition + (targetPosition - prevTargetPosition) * futurePrediction;

            return Agent.transform.position + (Agent.transform.position - position).normalized * lookAheadDistance;
        }
    }
}
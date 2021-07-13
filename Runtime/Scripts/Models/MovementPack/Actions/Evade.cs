using CZToolKit.Core.SharedVariable;
using CZToolKit.GraphProcessor;
using System;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeMenuItem("Movement", "Evade")]
    public class Evade : NavMeshMovement
    {
        [Tooltip("The agent has evaded when the magnitude is greater than this value")]
        public SharedFloat evadeDistance = new SharedFloat(10);
        [Tooltip("The distance to look ahead when evading")]
        public SharedFloat lookAheadDistance = new SharedFloat(5);
        [Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
        public SharedFloat targetDistPrediction = new SharedFloat(20);
        [Tooltip("Multiplier for predicting the look ahead distance")]
        public SharedFloat targetDistPredictionMult = new SharedFloat(20);
        [Tooltip("The GameObject that the agent is evading")]
        public SharedGameObject target;

        #region ViewModel
        // The position of the target at the last frame
        [NonSerialized] Vector3 targetPosition;

        public override void OnPrePerform()
        {
            base.OnPrePerform();
            targetPosition = target.Value.transform.position;
            SetDestination(Target());
        }

        // Evade from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override GOAPActionStatus OnPerform()
        {
            SetDestination(Target());
            if (Vector3.Magnitude(Agent.transform.position - target.Value.transform.position) > evadeDistance.Value)
                return GOAPActionStatus.Success;

            return GOAPActionStatus.Running;
        }

        // Evade in the opposite direction
        private Vector3 Target()
        {
            // Calculate the current distance to the target and the current speed
            var distance = (target.Value.transform.position - Agent.transform.position).magnitude;
            var speed = Velocity().magnitude;

            float futurePrediction = 0;
            // Set the future prediction to max prediction if the speed is too small to give an accurate prediction
            if (speed <= distance / targetDistPrediction.Value)
            {
                futurePrediction = targetDistPrediction.Value;
            }
            else
            {
                futurePrediction = (distance / speed) * targetDistPredictionMult.Value; // the prediction should be accurate enough
            }

            // Predict the future by taking the velocity of the target and multiply it by the future prediction
            var prevTargetPosition = targetPosition;
            targetPosition = target.Value.transform.position;
            var position = targetPosition + (targetPosition - prevTargetPosition) * futurePrediction;

            return Agent.transform.position + (Agent.transform.position - position).normalized * lookAheadDistance.Value;
        }
        #endregion
    }
}
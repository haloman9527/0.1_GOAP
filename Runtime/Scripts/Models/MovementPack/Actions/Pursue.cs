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
using CZToolKit.Core.SharedVariable;
using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeMenu("Movement", "Pursue")]
    public class Pursue : NavMeshMovement
    {
        [Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
        public float targetDistPrediction = 20;
        [Tooltip("Multiplier for predicting the look ahead distance")]
        public float targetDistPredictionMult = 20;
        [Tooltip("The GameObject that the agent is pursuing")]
        public SharedGameObject target = new SharedGameObject(null);
    }

    [ViewModel(typeof(Pursue))]
    public class PursueVM : NavMeshMovementVM
    {
        // The position of the target at the last frame
        Vector3 targetPosition;

        public PursueVM(BaseNode model) : base(model) { }

        public override void OnPrePerform()
        {
            base.OnPrePerform();
            var t_model = Model as Pursue;
            targetPosition = t_model.target.Value.transform.position;
            SetDestination(Target());
        }

        // Pursue the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override GOAPActionStatus OnPerform()
        {
            if (HasArrived())
                return GOAPActionStatus.Success;

            // Target will return the predicated position
            SetDestination(Target());
            return GOAPActionStatus.Running;
        }

        // Predict the position of the target
        private Vector3 Target()
        {
            var t_model = Model as Pursue;
            // Calculate the current distance to the target and the current speed
            var distance = (t_model.target.Value.transform.position - Agent.transform.position).magnitude;
            var speed = Velocity().magnitude;

            float futurePrediction = 0;
            // Set the future prediction to max prediction if the speed is too small to give an accurate prediction
            if (speed <= distance / t_model.targetDistPrediction)
            {
                futurePrediction = t_model.targetDistPrediction;
            }
            else
            {
                futurePrediction = (distance / speed) * t_model.targetDistPredictionMult; // the prediction should be accurate enough
            }

            // Predict the future by taking the velocity of the target and multiply it by the future prediction
            var prevTargetPosition = targetPosition;
            targetPosition = t_model.target.Value.transform.position;
            return targetPosition + (targetPosition - prevTargetPosition) * futurePrediction;
        }
    }
}
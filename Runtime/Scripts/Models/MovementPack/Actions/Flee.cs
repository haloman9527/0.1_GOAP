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
using System;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeMenu("Movement/Flee")]
    public class Flee : NavMeshMovement
    {
        [Tooltip("The agent has fleed when the magnitude is greater than this value")]
        public SharedFloat fleedDistance = new SharedFloat(20);
        [Tooltip("The distance to look ahead when fleeing")]
        public float lookAheadDistance = 5;
        [Tooltip("The GameObject that the agent is fleeing from")]
        public SharedGameObject target;
    }

    [ViewModel(typeof(Flee))]
    public class FleeVM : NavMeshMovementVM
    {
        [NonSerialized] bool hasMoved;

        public FleeVM(BaseNode model) : base(model) { }

        public override void OnPrePerform()
        {
            base.OnPrePerform();
            hasMoved = false;
            SetDestination(Target());
        }

        // Flee from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as Flee;
            if (Vector3.Magnitude(Agent.transform.position - t_model.target.Value.transform.position) > t_model.fleedDistance.Value)
                return GOAPActionStatus.Success;

            if (HasArrived())
            {
                if (!hasMoved)
                    return GOAPActionStatus.Failure;
                if (!SetDestination(Target()))
                    return GOAPActionStatus.Failure;
                hasMoved = false;
            }
            else
            {
                // If the agent is stuck the task shouldn't continue to return a status of running.
                var velocityMagnitude = Velocity().sqrMagnitude;
                if (hasMoved && velocityMagnitude <= 0f)
                    return GOAPActionStatus.Failure;
                hasMoved = velocityMagnitude > 0f;
            }

            return GOAPActionStatus.Running;
        }

        // Flee in the opposite direction
        private Vector3 Target()
        {
            var t_model = Model as Flee;
            return Agent.transform.position + (Agent.transform.position - t_model.target.Value.transform.position).normalized * t_model.lookAheadDistance;
        }

        // Return false if the position isn't valid on the NavMesh.
        public override bool SetDestination(Vector3 destination)
        {
            if (!SamplePosition(destination))
            {
                return false;
            }
            return base.SetDestination(destination);
        }
    }
}
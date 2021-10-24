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
using CZToolKit.Core.SharedVariable;
using CZToolKit.GraphProcessor;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeTooltip("在NavMesh上徘徊")]
    [NodeMenuItem("Movement", "Wander")]
    public class Wander : NavMeshMovement
    {
        [Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
        public SharedFloat minWanderDistance = new SharedFloat(20);
        [Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
        public SharedFloat maxWanderDistance = new SharedFloat(20);
        [Tooltip("The amount that the agent rotates direction")]
        public SharedFloat wanderRate = new SharedFloat(2);
        [Tooltip("The minimum length of time that the agent should pause at each destination")]
        public SharedFloat minPauseDuration = new SharedFloat(0);
        [Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
        public SharedFloat maxPauseDuration = new SharedFloat(0);
        [Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
        public SharedInt targetRetries = new SharedInt(1);

        public Wander() : base()
        {
            name = "徘徊";
        }
        #region ViewModel
        [NonSerialized] float pauseTime;
        [NonSerialized] float destinationReachTime;

        public override void OnInitializedPropertyMapping(IVariableOwner variableOwner)
        {
            base.OnInitializedPropertyMapping(variableOwner);
        }

        // There is no success or fail state with wander - the agent will just keep wandering
        public override GOAPActionStatus OnPerform()
        {
            if (HasArrived())
            {
                // The agent should pause at the destination only if the max pause duration is greater than 0
                if (maxPauseDuration.Value > 0)
                {
                    if (destinationReachTime == -1)
                    {
                        destinationReachTime = Time.time;
                        pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
                    }
                    if (destinationReachTime + pauseTime <= Time.time)
                    {
                        // Only reset the time if a destination has been set.
                        if (TrySetTarget())
                        {
                            destinationReachTime = -1;
                        }
                    }
                }
                else
                {
                    TrySetTarget();
                }
            }
            return GOAPActionStatus.Running;
        }

        private bool TrySetTarget()
        {
            var direction = Agent.transform.forward;
            var validDestination = false;
            var attempts = targetRetries.Value;
            var destination = Agent.transform.position;
            while (!validDestination && attempts > 0)
            {
                direction = direction + Random.insideUnitSphere * wanderRate.Value;
                destination = Agent.transform.position + direction.normalized * Random.Range(minWanderDistance.Value, maxWanderDistance.Value);
                validDestination = SamplePosition(destination);
                attempts--;
            }
            if (validDestination)
            {
                SetDestination(destination);
            }
            return validDestination;
        }
        #endregion
    }
}
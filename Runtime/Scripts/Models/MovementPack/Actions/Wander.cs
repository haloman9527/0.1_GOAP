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

using Random = UnityEngine.Random;

namespace Atom.GOAP.Actions.Movement
{
    [NodeTooltip("在NavMesh上徘徊")]
    [NodeMenu("Movement/Wander")]
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
    }

    [ViewModel(typeof(Wander))]
    public class WanderProcessor : NavMeshMovementProcessor
    {
        float pauseTime;
        float destinationReachTime;

        public WanderProcessor(BaseNode model) : base(model) { }

        public override void OnAdded()
        {
            base.OnAdded();
            var t_model = Model as Wander;
            t_model.name = "徘徊";
        }

        // There is no success or fail state with wander - the agent will just keep wandering
        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as Wander;
            if (HasArrived())
            {
                // The agent should pause at the destination only if the max pause duration is greater than 0
                if (t_model.maxPauseDuration.Value > 0)
                {
                    if (destinationReachTime == -1)
                    {
                        destinationReachTime = Time.time;
                        pauseTime = Random.Range(t_model.minPauseDuration.Value, t_model.maxPauseDuration.Value);
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
            var t_model = Model as Wander;
            var direction = Agent.transform.forward;
            var validDestination = false;
            var attempts = t_model.targetRetries.Value;
            var destination = Agent.transform.position;
            while (!validDestination && attempts > 0)
            {
                direction = direction + Random.insideUnitSphere * t_model.wanderRate.Value;
                destination = Agent.transform.position + direction.normalized * Random.Range(t_model.minWanderDistance.Value, t_model.maxWanderDistance.Value);
                validDestination = SamplePosition(destination);
                attempts--;
            }
            if (validDestination)
            {
                SetDestination(destination);
            }
            return validDestination;
        }
    }
}
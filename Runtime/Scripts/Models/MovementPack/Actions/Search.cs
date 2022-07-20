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
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeTooltip("Search for a target by combining the wander, within hearing range, and the within seeing range tasks using the Unity NavMesh.")]
    [NodeMenuItem("Movement", "Search")]
    public class Search : NavMeshMovement
    {
        [Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
        public SharedFloat minWanderDistance = new SharedFloat(20);
        [Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
        public SharedFloat maxWanderDistance = new SharedFloat(20);
        [Tooltip("The amount that the agent rotates direction")]
        public SharedFloat wanderRate = new SharedFloat(1);
        [Tooltip("The minimum length of time that the agent should pause at each destination")]
        public SharedFloat minPauseDuration = new SharedFloat(0);
        [Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
        public SharedFloat maxPauseDuration = new SharedFloat(0);
        [Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
        public SharedInt targetRetries = new SharedInt(1);
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = new SharedFloat(90);
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = new SharedFloat(30);
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("Should the search end if audio was heard?")]
        public SharedBool senseAudio = new SharedBool(true);
        [Tooltip("How far away the unit can hear")]
        public SharedFloat hearingRadius = new SharedFloat(30);
        [Tooltip("The raycast offset relative to the pivot position")]
        public SharedVector3 offset;
        [Tooltip("The target raycast offset relative to the pivot position")]
        public SharedVector3 targetOffset;
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("Specifies the maximum number of colliders that the physics cast can collide with")]
        public int maxCollisionCount = 200;
        [Tooltip("Should the target bone be used?")]
        public SharedBool useTargetBone;
        [Tooltip("The target's bone if the target is a humanoid")]
        public HumanBodyBones targetBone;
        [Tooltip("Should a debug look ray be drawn to the scene view?")]
        public SharedBool drawDebugRay;
        [Tooltip("The further away a sound source is the less likely the agent will be able to hear it. " +
                 "Set a threshold for the the minimum audibility level that the agent can hear")]
        public SharedFloat audibilityThreshold = new SharedFloat(0.05f);
        [Tooltip("The object that is found")]
        public SharedGameObject returnedObject;
    }

    [ViewModel(typeof(Search))]
    public class SearchVM : NavMeshMovementVM
    {
        private float pauseTime;
        private float destinationReachTime;
        private Collider[] overlapColliders;

        public SearchVM(BaseNode model) : base(model) { }

        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as Search;
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

            // Detect if any objects are within sight
            if (overlapColliders == null)
            {
                overlapColliders = new Collider[t_model.maxCollisionCount];
            }
            t_model.returnedObject.Value = MovementUtility.WithinSight(Agent.transform, t_model.offset.Value, t_model.fieldOfViewAngle.Value, t_model.viewDistance.Value, overlapColliders, t_model.objectLayerMask, t_model.targetOffset.Value, t_model.ignoreLayerMask, t_model.useTargetBone.Value, t_model.targetBone, t_model.drawDebugRay.Value);
            // If an object was seen then return success
            if (t_model.returnedObject.Value != null)
            {
                return GOAPActionStatus.Success;
            }
            // Detect if any object are within audio range (if enabled)
            if (t_model.senseAudio.Value)
            {
                t_model.returnedObject.Value = MovementUtility.WithinHearingRange(Agent.transform, t_model.offset.Value, t_model.audibilityThreshold.Value, t_model.hearingRadius.Value, overlapColliders, t_model.objectLayerMask);
                // If an object was heard then return success
                if (t_model.returnedObject.Value != null)
                {
                    return GOAPActionStatus.Success;
                }
            }

            // No object has been seen or heard so keep searching
            return GOAPActionStatus.Running;
        }

        private bool TrySetTarget()
        {
            var t_model = Model as Search;
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
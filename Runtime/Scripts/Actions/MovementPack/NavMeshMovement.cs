using UnityEngine;
using UnityEngine.AI;

namespace CZToolKit.GOAP.Actions.Movement
{
    public abstract class NavMeshMovement : Movement
    {
        [Tooltip("The speed of the agent")]
        public float speed = 10;
        [Tooltip("The angular speed of the agent")]
        public float angularSpeed = 120;
        [Tooltip("The agent has arrived when the destination is less than the specified amount. This distance should be greater than or equal to the NavMeshAgent StoppingDistance.")]
        public float arriveDistance = 0.2f;
        [Tooltip("Should the NavMeshAgent be stopped when the task ends?")]
        public bool stopOnTaskEnd = true;
        [Tooltip("Should the NavMeshAgent rotation be updated when the task ends?")]
        public bool updateRotation = true;

        // Component references
        protected NavMeshAgent navMeshAgent;
        private bool startUpdateRotation;

        public override void OnCreated()
        {
            base.OnCreated();
        }

        /// <summary>
        /// Cache the component references.
        /// </summary>
        protected override void OnInitialized()
        {
            navMeshAgent = Agent.GetComponent<NavMeshAgent>();
        }

        public override void PrePerform()
        {
            navMeshAgent.speed = speed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.isStopped = false;
            startUpdateRotation = navMeshAgent.updateRotation;
            UpdateRotation(updateRotation);
        }

        /// <summary>
        /// Set a new pathfinding destination.
        /// </summary>
        /// <param name="destination">The destination to set.</param>
        /// <returns>True if the destination is valid.</returns>
        protected override bool SetDestination(Vector3 destination)
        {
            navMeshAgent.isStopped = false;
            return navMeshAgent.SetDestination(destination);
        }

        /// <summary>
        /// Specifies if the rotation should be updated.
        /// </summary>
        /// <param name="update">Should the rotation be updated?</param>
        protected override void UpdateRotation(bool update)
        {
            navMeshAgent.updateRotation = update;
            navMeshAgent.updateUpAxis = update;
        }

        /// <summary>
        /// Does the agent have a pathfinding path?
        /// </summary>
        /// <returns>True if the agent has a pathfinding path.</returns>
        protected override bool HasPath()
        {
            return navMeshAgent.hasPath && navMeshAgent.remainingDistance > arriveDistance;
        }

        /// <summary>
        /// Returns the velocity of the agent.
        /// </summary>
        /// <returns>The velocity of the agent.</returns>
        protected override Vector3 Velocity()
        {
            return navMeshAgent.velocity;
        }

        /// <summary>
        /// Returns true if the position is a valid pathfinding position.
        /// </summary>
        /// <param name="position">The position to sample.</param>
        /// <returns>True if the position is a valid pathfinding position.</returns>
        protected bool SamplePosition(Vector3 position)
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(position, out hit, navMeshAgent.height * 2, NavMesh.AllAreas);
        }

        /// <summary>
        /// Has the agent arrived at the destination?
        /// </summary>
        /// <returns>True if the agent has arrived at the destination.</returns>
        protected override bool HasArrived()
        {
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (navMeshAgent.pathPending)
            {
                remainingDistance = float.PositiveInfinity;
            }
            else
            {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            return remainingDistance <= arriveDistance;
        }

        /// <summary>
        /// Stop pathfinding.
        /// </summary>
        protected override void Stop()
        {
            UpdateRotation(startUpdateRotation);
            if (navMeshAgent.hasPath)
            {
                navMeshAgent.isStopped = true;
            }
        }

        /// <summary>
        /// The behavior tree has ended.Stop moving.
        /// </summary>
        public override void PostPerform()
        {
            if (stopOnTaskEnd)
                Stop();
            else
                UpdateRotation(startUpdateRotation);
        }

        //public override void PostPerform()
        //{
        //    Stop();
        //    Debug.Log(1);
        //}
    }
}
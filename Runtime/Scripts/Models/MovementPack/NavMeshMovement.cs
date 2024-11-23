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

using Jiange.GraphProcessor;
using UnityEngine;
using UnityEngine.AI;

namespace Jiange.GOAP.Actions.Movement
{
    public abstract class NavMeshMovement : GOAPAction
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
    }

    public abstract class NavMeshMovementProcessor : GOAPActionProcessor, IMovementAction
    {
        protected NavMeshAgent navMeshAgent;
        private bool startUpdateRotation;

        protected NavMeshMovementProcessor(BaseNode model) : base(model) { }

        public override void Initialized(GOAPAgent agent)
        {
            base.Initialized(agent);
            navMeshAgent = Agent.GetComponent<NavMeshAgent>();
        }

        public override void OnPrePerform()
        {
            var t_model = Model as NavMeshMovement;
            navMeshAgent.speed = t_model.speed;
            navMeshAgent.angularSpeed = t_model.angularSpeed;
            navMeshAgent.isStopped = false;
            startUpdateRotation = navMeshAgent.updateRotation;
            UpdateRotation(t_model.updateRotation);
        }

        /// <summary> Set a new pathfinding destination. </summary>
        /// <param name="destination">The destination to set.</param>
        /// <returns>True if the destination is valid.</returns>
        public virtual bool SetDestination(Vector3 destination)
        {
            navMeshAgent.isStopped = false;
            return navMeshAgent.SetDestination(destination);
        }

        /// <summary> Specifies if the rotation should be updated. </summary>
        /// <param name="update">Should the rotation be updated?</param>
        public virtual void UpdateRotation(bool update)
        {
            navMeshAgent.updateRotation = update;
            navMeshAgent.updateUpAxis = update;
        }

        /// <summary> Does the agent have a pathfinding path? </summary>
        /// <returns>True if the agent has a pathfinding path.</returns>
        public virtual bool HasPath()
        {
            var t_model = Model as NavMeshMovement;
            return navMeshAgent.hasPath && navMeshAgent.remainingDistance > t_model.arriveDistance;
        }

        /// <summary> Returns the velocity of the agent. </summary>
        /// <returns>The velocity of the agent.</returns>
        public virtual Vector3 Velocity()
        {
            return navMeshAgent.velocity;
        }

        /// <summary> Returns true if the position is a valid pathfinding position. </summary>
        /// <param name="position">The position to sample.</param>
        /// <returns>True if the position is a valid pathfinding position.</returns>
        public virtual bool SamplePosition(Vector3 position)
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(position, out hit, navMeshAgent.height * 2, NavMesh.AllAreas);
        }

        /// <summary> Has the agent arrived at the destination?  </summary>
        /// <returns>True if the agent has arrived at the destination.</returns>
        public virtual bool HasArrived()
        {
            var t_model = Model as NavMeshMovement;
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (navMeshAgent.pathPending)
                remainingDistance = float.PositiveInfinity;
            else
                remainingDistance = navMeshAgent.remainingDistance;

            return remainingDistance <= t_model.arriveDistance;
        }

        /// <summary> Stop pathfinding. </summary>
        public virtual void Stop()
        {
            UpdateRotation(startUpdateRotation);
            if (navMeshAgent.hasPath)
                navMeshAgent.isStopped = true;
        }

        /// <summary> The behavior tree has ended.Stop moving. </summary>
        public override void OnPostPerform(bool _successed)
        {
            var t_model = Model as NavMeshMovement;
            if (t_model.stopOnTaskEnd)
                Stop();
            else
                UpdateRotation(startUpdateRotation);
        }
    }
}
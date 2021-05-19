#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using CZToolKit.Core.SharedVariable;
using UnityEngine;
using UnityEngine.AI;

namespace CZToolKit.GOAP.Actions.Movement
{
    public abstract class NavMeshGroupMovement : GroupMovement
    {
        [Tooltip("All of the agents")]
        public SharedGameObjectList agents = null;
        [Tooltip("The speed of the agents")]
        public SharedFloat speed = new SharedFloat(10);
        [Tooltip("The angular speed of the agents")]
        public SharedFloat angularSpeed = new SharedFloat(120);


        // A cache of the NavMeshAgents
        private NavMeshAgent[] navMeshAgents;
        protected Transform[] transforms;

        public override void OnPrePerform()
        {
            navMeshAgents = new NavMeshAgent[agents.Value.Count];
            transforms = new Transform[agents.Value.Count];
            for (int i = 0; i < agents.Value.Count; ++i)
            {
                transforms[i] = agents.Value[i].transform;
                navMeshAgents[i] = agents.Value[i].GetComponent<NavMeshAgent>();
                navMeshAgents[i].speed = speed.Value;
                navMeshAgents[i].angularSpeed = angularSpeed.Value;
                navMeshAgents[i].isStopped = false;
            }
        }

        protected override bool SetDestination(int index, Vector3 target)
        {
            if (navMeshAgents[index].destination == target)
            {
                return true;
            }
            return navMeshAgents[index].SetDestination(target);
        }

        protected override Vector3 Velocity(int index)
        {
            return navMeshAgents[index].velocity;
        }

        public override void OnPostPerform(bool _successed)
        {
            base.OnPostPerform(_successed);
            // Disable the nav mesh
            for (int i = 0; i < navMeshAgents.Length; ++i)
            {
                if (navMeshAgents[i] != null)
                {
                    navMeshAgents[i].isStopped = true;
                }
            }
        }
    }
}
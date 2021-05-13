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
using CZToolKit.GraphProcessor;
using UnityEngine;
using UnityEngine.AI;

namespace CZToolKit.GOAP.Actions.Movement
{
    //[TaskDescription("Follow the leader using the Unity NavMesh.")]
    //[TaskCategory("Movement")]
    //[HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    //[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}LeaderFollowIcon.png")]
    [NodeMenuItem("LeaderFollow(MP)")]
    public class LeaderFollow : NavMeshGroupMovement
    {
        [Tooltip("Agents less than this distance apart are neighbors")]
        public SharedFloat neighborDistance = new SharedFloat(10);
        [Tooltip("How far behind the leader the agents should follow the leader")]
        public SharedFloat leaderBehindDistance = new SharedFloat(2);
        [Tooltip("The distance that the agents should be separated")]
        public SharedFloat separationDistance = new SharedFloat(2);
        [Tooltip("The agent is getting too close to the front of the leader if they are within the aheadDistance")]
        public SharedFloat aheadDistance = new SharedFloat(2);
        [Tooltip("The leader to follow")]
        public SharedGameObject leader = null;

        // component cache
        private Transform leaderTransform;
        private NavMeshAgent leaderAgent;

        public override void PrePerform()
        {
            leaderTransform = leader.Value.transform;
            leaderAgent = leader.Value.GetComponent<NavMeshAgent>();

            base.PrePerform();
        }

        public override ActionStatus Perform()
        {
            var behindPosition = LeaderBehindPosition();
            // Determine a destination for each agent
            for (int i = 0; i < agents.Value.Count; ++i)
            {
                // Get out of the way of the leader if the leader is currently looking at the agent and is getting close
                if (LeaderLookingAtAgent(i) && Vector3.Magnitude(leaderTransform.position - transforms[i].position) < aheadDistance.Value)
                {
                    SetDestination(i, transforms[i].position + (transforms[i].position - leaderTransform.position).normalized * aheadDistance.Value);
                }
                else
                {
                    // The destination is the behind position added to the separation vector
                    SetDestination(i, behindPosition + DetermineSeparation(i));
                }
            }
            return ActionStatus.Running;
        }

        private Vector3 LeaderBehindPosition()
        {
            // The behind position is the normalized inverse of the leader's velocity multiplied by the leaderBehindDistance
            return leaderTransform.position + (-leaderAgent.velocity).normalized * leaderBehindDistance.Value;
        }

        // Determine the separation between the current agent and all of the other agents also following the leader
        private Vector3 DetermineSeparation(int agentIndex)
        {
            var separation = Vector3.zero;
            int neighborCount = 0;
            var agentTransform = transforms[agentIndex];
            // Loop through each agent to determine the separation
            for (int i = 0; i < agents.Value.Count; ++i)
            {
                // The agent can't compare against itself
                if (agentIndex != i)
                {
                    // Only determine the parameters if the other agent is its neighbor
                    if (Vector3.SqrMagnitude(transforms[i].position - agentTransform.position) < neighborDistance.Value)
                    {
                        // This agent is the neighbor of the original agent so add the separation
                        separation += transforms[i].position - agentTransform.position;
                        neighborCount++;
                    }
                }
            }

            // Don't move if there are no neighbors
            if (neighborCount == 0)
            {
                return Vector3.zero;
            }
            // Normalize the value
            return ((separation / neighborCount) * -1).normalized * separationDistance.Value;
        }

        // Use the dot product to determine if the leader is looking at the current agent
        public bool LeaderLookingAtAgent(int agentIndex)
        {
            return Vector3.Dot(leaderTransform.forward, transforms[agentIndex].forward) < -0.5f;
        }
    }
}
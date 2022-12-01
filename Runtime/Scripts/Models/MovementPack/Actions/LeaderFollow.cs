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
using System;
using UnityEngine;
using UnityEngine.AI;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeTooltip("控制多个NavMesh跟随一个目标")]
    [NodeMenu("Movement", "LeaderFollow")]
    public class LeaderFollow : NavMeshGroupMoveMent
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

    }

    [ViewModel(typeof(LeaderFollow))]
    public class LeaderFollowVM : NavMeshGroupMovementVM
    {
        Transform leaderTransform;
        NavMeshAgent leaderAgent;

        public LeaderFollowVM(BaseNode model) : base(model) { }

        public override void OnPrePerform()
        {
            var t_model = Model as LeaderFollow;
            leaderTransform = t_model.leader.Value.transform;
            leaderAgent = t_model.leader.Value.GetComponent<NavMeshAgent>();
            base.OnPrePerform();
        }

        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as LeaderFollow;
            var behindPosition = LeaderBehindPosition();
            // Determine a destination for each agent
            for (int i = 0; i < t_model.agents.Value.Count; ++i)
            {
                // Get out of the way of the leader if the leader is currently looking at the agent and is getting close
                if (LeaderLookingAtAgent(i) && Vector3.Magnitude(leaderTransform.position - transforms[i].position) < t_model.aheadDistance.Value)
                {
                    SetDestination(i, transforms[i].position + (transforms[i].position - leaderTransform.position).normalized * t_model.aheadDistance.Value);
                }
                else
                {
                    // The destination is the behind position added to the separation vector
                    SetDestination(i, behindPosition + DetermineSeparation(i));
                }
            }
            return GOAPActionStatus.Running;
        }

        private Vector3 LeaderBehindPosition()
        {
            var t_model = Model as LeaderFollow;
            // The behind position is the normalized inverse of the leader's velocity multiplied by the leaderBehindDistance
            return leaderTransform.position + (-leaderAgent.velocity).normalized * t_model.leaderBehindDistance.Value;
        }

        // Determine the separation between the current agent and all of the other agents also following the leader
        private Vector3 DetermineSeparation(int agentIndex)
        {
            var t_model = Model as LeaderFollow;
            var separation = Vector3.zero;
            int neighborCount = 0;
            var agentTransform = transforms[agentIndex];
            // Loop through each agent to determine the separation
            for (int i = 0; i < t_model.agents.Value.Count; ++i)
            {
                // The agent can't compare against itself
                if (agentIndex != i)
                {
                    // Only determine the parameters if the other agent is its neighbor
                    if (Vector3.SqrMagnitude(transforms[i].position - agentTransform.position) < t_model.neighborDistance.Value)
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
            return ((separation / neighborCount) * -1).normalized * t_model.separationDistance.Value;
        }

        // Use the dot product to determine if the leader is looking at the current agent
        public bool LeaderLookingAtAgent(int agentIndex)
        {
            return Vector3.Dot(leaderTransform.forward, transforms[agentIndex].forward) < -0.5f;
        }
    }
}
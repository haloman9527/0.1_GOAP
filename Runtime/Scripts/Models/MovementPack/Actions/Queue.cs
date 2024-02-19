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
using CZToolKit;
using CZToolKit.SharedVariable;
using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeTooltip("控制多个NavMesh前往目标")]
    [NodeMenu("Movement/Queue")]
    public class Queue : NavMeshGroupMoveMent
    {
        [Tooltip("Agents less than this distance apart are neighbors")]
        public SharedFloat neighborDistance = new SharedFloat(10);
        [Tooltip("The distance that the agents should be separated")]
        public SharedFloat separationDistance = new SharedFloat(2);
        [Tooltip("The distance the the agent should look ahead to see if another agent is in the way")]
        public SharedFloat maxQueueAheadDistance = new SharedFloat(2);
        [Tooltip("The radius that the agent should check to see if another agent is in the way")]
        public SharedFloat maxQueueRadius = new SharedFloat(20);
        [Tooltip("The multiplier to slow down if an agent is in front of the current agent")]
        public SharedFloat slowDownSpeed = new SharedFloat(0.15f);
        [Tooltip("The target to seek towards")]
        public SharedGameObject target;
    }

    [ViewModel(typeof(Queue))]
    public class QueueProcessor : NavMeshGroupMovementProcessor
    {
        public QueueProcessor(BaseNode model) : base(model) { }

        public override void OnAdded()
        {
            base.OnAdded();
            var t_model = Model as Queue;
            t_model.name = "排队";
        }

        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as Queue;
            // Determine a destination for each agent
            for (int i = 0; i < t_model.agents.Value.Count; ++i)
            {
                if (AgentAhead(i))
                {
                    SetDestination(i, transforms[i].position + transforms[i].forward * t_model.slowDownSpeed.Value + DetermineSeparation(i));
                }
                else
                {
                    SetDestination(i, t_model.target.Value.transform.position);
                }
            }
            return GOAPActionStatus.Running;
        }

        // Returns the agent that is ahead of the current agent
        private bool AgentAhead(int index)
        {
            var t_model = Model as Queue;
            // queueAhead is the distance in front of the current agent
            var queueAhead = Velocity(index) * t_model.maxQueueAheadDistance.Value;
            for (int i = 0; i < t_model.agents.Value.Count; ++i)
            {
                // Return the first agent that is ahead of the current agent
                if (index != i && Vector3.SqrMagnitude(queueAhead - transforms[i].position) < t_model.maxQueueRadius.Value)
                {
                    return true;
                }
            }
            return false;
        }

        // Determine the separation between the current agent and all of the other agents also queuing
        private Vector3 DetermineSeparation(int agentIndex)
        {
            var t_model = Model as Queue;
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
    }
}
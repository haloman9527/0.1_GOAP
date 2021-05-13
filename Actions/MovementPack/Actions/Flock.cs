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

namespace CZToolKit.GOAP.Actions.Movement
{
    //[TaskDescription("Flock around the scene using the Unity NavMesh.")]
    //[TaskCategory("Movement")]
    //[HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    //[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FlockIcon.png")]
    [NodeMenuItem("Flock(MP)")]
    public class Flock : NavMeshGroupMovement
    {
        [Tooltip("Agents less than this distance apart are neighbors")]
        public SharedFloat neighborDistance = new SharedFloat(100);
        [Tooltip("How far the agent should look ahead when determine its pathfinding destination")]
        public SharedFloat lookAheadDistance = new SharedFloat(5);
        [Tooltip("The greater the alignmentWeight is the more likely it is that the agents will be facing the same direction")]
        public SharedFloat alignmentWeight = new SharedFloat(0.4f);
        [Tooltip("The greater the cohesionWeight is the more likely it is that the agents will be moving towards a common position")]
        public SharedFloat cohesionWeight = new SharedFloat(0.5f);
        [Tooltip("The greater the separationWeight is the more likely it is that the agents will be separated")]
        public SharedFloat separationWeight = new SharedFloat(0.6f);


        // Determine the three flock parameters: alignment, cohesion, and separation.
        // Alignment: determines which direction to move
        // Cohesion: Determines a common position to move towards
        // Separation: Determines how far apart the agent is from all other agents
        private void DetermineFlockParameters(int index, out Vector3 alignment, out Vector3 cohesion, out Vector3 separation)
        {
            alignment = cohesion = separation = Vector3.zero;
            int neighborCount = 0;
            var agentPosition = transforms[index].position;
            // Loop through each agent to determine the alignment, cohesion, and separation
            for (int i = 0; i < agents.Value.Count; ++i)
            {
                // The agent can't compare against itself
                if (index != i)
                {
                    var position = transforms[i].position;
                    // Only determine the parameters if the other agent is its neighbor
                    if (Vector3.Magnitude(position - agentPosition) < neighborDistance.Value)
                    {
                        // This agent is the neighbor of the original agent so add the alignment, cohesion, and separation
                        alignment += Velocity(i);
                        cohesion += position;
                        separation += position - agentPosition;
                        neighborCount++;
                    }
                }
            }

            // Don't move if there are no neighbors
            if (neighborCount == 0)
            {
                return;
            }
            // Normalize all of the values
            alignment = (alignment / neighborCount).normalized;
            cohesion = ((cohesion / neighborCount) - agentPosition).normalized;
            separation = ((separation / neighborCount) * -1).normalized;
        }

        public override ActionStatus Perform()
        {
            // Determine a destination for each agent
            for (int i = 0; i < agents.Value.Count; ++i)
            {
                Vector3 alignment, cohesion, separation;
                // determineFlockAttributes will determine which direction to head, which common position to move toward, and how far apart each agent is from one another,
                DetermineFlockParameters(i, out alignment, out cohesion, out separation);
                // Weigh each parameter to give one more of an influence than another
                var velocity = alignment * alignmentWeight.Value + cohesion * cohesionWeight.Value + separation * separationWeight.Value;
                // Set the destination based on the velocity multiplied by the look ahead distance
                if (!SetDestination(i, transforms[i].position + velocity * lookAheadDistance.Value))
                {
                    // Go the opposite direction if the destination is invalid
                    velocity *= -1;
                    SetDestination(i, transforms[i].position + velocity * lookAheadDistance.Value);
                }
            }
            return ActionStatus.Running;
        }
    }
}
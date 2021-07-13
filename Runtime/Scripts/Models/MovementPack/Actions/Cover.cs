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
    [NodeTooltip("寻找一个掩体躲起来，使用NavMesh移动")]
    [NodeMenuItem("Movement", "Cover")]
    public class Cover : NavMeshMovement
    {
        #region Model
        [Tooltip("The distance to search for cover")]
        public SharedFloat maxCoverDistance = new SharedFloat(1000);
        [Tooltip("The layermask of the available cover positions")]
        public LayerMask availableLayerCovers;
        [Tooltip("The maximum number of raycasts that should be fired before the agent gives up looking for an agent to find cover behind")]
        public SharedInt maxRaycasts = new SharedInt(100);
        [Tooltip("How large the step should be between raycasts")]
        public SharedFloat rayStep = new SharedFloat(1);
        [Tooltip("Once a cover point has been found, multiply this offset by the normal to prevent the agent from hugging the wall")]
        public SharedFloat coverOffset = new SharedFloat(2);
        [Tooltip("Should the agent look at the cover point after it has arrived?")]
        public SharedBool lookAtCoverPoint = new SharedBool(false);
        [Tooltip("The agent is done rotating to the cover point when the square magnitude is less than this value")]
        public SharedFloat rotationEpsilon = new SharedFloat(0.5f);
        [Tooltip("Max rotation delta if lookAtCoverPoint")]
        public SharedFloat maxLookAtRotationDelta;


        public Cover() : base()
        {
            name = "Cover";
            cost = 1;
        }

        #endregion

        #region ViewModel
        Vector3 coverPoint;
        // The position to reach, offsetted from coverPoint
        Vector3 coverTarget;
        // Was cover found?
        bool foundCover;

        public override void OnPrePerform()
        {
            RaycastHit hit;
            int raycastCount = 0;
            var direction = Agent.transform.forward;
            float step = 0;
            coverTarget = Agent.transform.position;
            foundCover = false;
            // Keep firing a ray until too many rays have been fired
            while (raycastCount < maxRaycasts.Value)
            {
                var ray = new Ray(Agent.transform.position, direction);
                if (Physics.Raycast(ray, out hit, maxCoverDistance.Value, availableLayerCovers.value))
                {
                    // A suitable agent has been found. Find the opposite side of that agent by shooting a ray in the opposite direction from a point far away
                    if (hit.collider.Raycast(new Ray(hit.point - hit.normal * maxCoverDistance.Value, hit.normal), out hit, Mathf.Infinity))
                    {
                        coverPoint = hit.point;
                        coverTarget = hit.point + hit.normal * coverOffset.Value;
                        foundCover = true;
                        break;
                    }
                }
                // Keep sweeiping along the y axis
                step += rayStep.Value;
                direction = Quaternion.Euler(0, Agent.transform.eulerAngles.y + step, 0) * Vector3.forward;
                raycastCount++;
            }

            if (foundCover)
                SetDestination(coverTarget);

            base.OnPrePerform();
        }

        public override GOAPActionStatus OnPerform()
        {
            if (!foundCover) return GOAPActionStatus.Failure;
            if (HasArrived())
            {
                var rotation = Quaternion.LookRotation(coverPoint - Agent.transform.position);
                // Return success if the agent isn't going to look at the cover point or it has completely rotated to look at the cover point
                if (!lookAtCoverPoint.Value || Quaternion.Angle(Agent.transform.rotation, rotation) < rotationEpsilon.Value)
                {
                    return GOAPActionStatus.Success;
                }
                else
                {
                    // Still needs to rotate towards the target
                    Agent.transform.rotation = Quaternion.RotateTowards(Agent.transform.rotation, rotation, maxLookAtRotationDelta.Value);
                }
            }
            return GOAPActionStatus.Running;
        }
        #endregion
    }
}
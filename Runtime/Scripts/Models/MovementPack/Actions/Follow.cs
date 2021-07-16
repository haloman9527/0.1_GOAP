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
using System;
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeTooltip("跟随一个目标，使用NavMesh移动")]
    [NodeMenuItem("Movement", "Follow")]
    public class Follow : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is following")]
        public SharedGameObject target;
        [Tooltip("Start moving towards the target if the target is further than the specified distance")]
        public SharedFloat moveDistance = new SharedFloat(2);

        public Follow() : base()
        {
            name = "跟随";
        }

        #region ViewModel
        [NonSerialized] Vector3 lastTargetPosition;
        [NonSerialized] bool hasMoved;

        public override void OnPrePerform()
        {
            base.OnPrePerform();
            if (target.Value == null)
            {
                return;
            }

            lastTargetPosition = target.Value.transform.position + Vector3.one * (moveDistance.Value + 1);
            hasMoved = false;
        }

        public override GOAPActionStatus OnPerform()
        {
            if (target.Value == null)
                return GOAPActionStatus.Failure;

            // Move if the target has moved more than the moveDistance since the last time the agent moved.
            var targetPosition = target.Value.transform.position;
            if ((targetPosition - lastTargetPosition).magnitude >= moveDistance.Value)
            {
                SetDestination(targetPosition);
                lastTargetPosition = targetPosition;
                hasMoved = true;
            }
            else
            {
                // Stop moving if the agent is within the moveDistance of the target.
                if (hasMoved && (targetPosition - Agent.transform.position).magnitude < moveDistance.Value)
                {
                    Stop();
                    hasMoved = false;
                    lastTargetPosition = targetPosition;
                }
            }

            return GOAPActionStatus.Running;
        }
        #endregion
    }
}
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

using Random = UnityEngine.Random;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeTooltip("���趨·��Ѳ�ߣ�ʹ��NavMesh�ƶ�")]
    [NodeMenuItem("Movement", "Patrol")]
    public class Patrol : NavMeshMovement
    {
        [Tooltip("Should the agent patrol the waypoints randomly?")]
        public bool randomPatrol = false;
        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public float waypointPauseDuration = 0;
        [Tooltip("The waypoints to move to")]
        public SharedGameObjectList waypoints = new SharedGameObjectList();

        public Patrol() : base()
        {
            preconditions.Add(new GOAPState("HasTarget", false));

            effects.Add(new GOAPState("HasTarget", true));

            randomPatrol = false;
            waypointPauseDuration = 0;
        }

        #region ViewModel
        // The current index that we are heading towards within the waypoints array
        [NonSerialized] int waypointIndex;
        [NonSerialized] float waypointReachedTime;

        public override void OnPrePerform()
        {
            base.OnPrePerform();

            // initially move towards the closest waypoint
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < waypoints.Value.Count; ++i)
            {
                if ((localDistance = Vector3.Magnitude(Agent.transform.position - waypoints.Value[i].transform.position)) < distance)
                {
                    distance = localDistance;
                    waypointIndex = i;
                }
            }
            waypointReachedTime = -1;
            SetDestination(Target());
        }

        // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
        public override GOAPActionStatus OnPerform()
        {
            if (waypoints.Value.Count == 0)
            {
                return GOAPActionStatus.Failure;
            }
            if (HasArrived())
            {
                if (waypointReachedTime == -1)
                {
                    waypointReachedTime = Time.time;
                }
                // wait the required duration before switching waypoints.
                if (waypointReachedTime + waypointPauseDuration <= Time.time)
                {
                    if (randomPatrol)
                    {
                        if (waypoints.Value.Count == 1)
                        {
                            waypointIndex = 0;
                        }
                        else
                        {
                            // prevent the same waypoint from being selected
                            var newWaypointIndex = waypointIndex;
                            while (newWaypointIndex == waypointIndex)
                            {
                                newWaypointIndex = Random.Range(0, waypoints.Value.Count);
                            }
                            waypointIndex = newWaypointIndex;
                        }
                    }
                    else
                    {
                        waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
                    }
                    SetDestination(Target());
                    waypointReachedTime = -1;
                }
            }

            return GOAPActionStatus.Running;
        }

        // Return the current waypoint index position
        private Vector3 Target()
        {
            if (waypointIndex >= waypoints.Value.Count)
            {
                return Agent.transform.position;
            }
            return waypoints.Value[waypointIndex].transform.position;
        }

        public override void DrawGizmos(IGraphOwner _graphOwner)
        {
#if UNITY_EDITOR
            if (waypoints == null)
            {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < waypoints.Value.Count; ++i)
            {
                if (waypoints.Value[i] != null)
                {
                    UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
                }
            }
            UnityEditor.Handles.color = oldColor;
#endif
        }
        #endregion
    }
}
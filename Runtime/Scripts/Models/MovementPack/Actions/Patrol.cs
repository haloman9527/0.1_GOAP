#region ע ��
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: ��ֻ��Ϻ��
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using CZToolKit.VM;
using CZToolKit.SharedVariable;
using CZToolKit.GraphProcessor;
using UnityEngine;

using Random = UnityEngine.Random;

namespace CZToolKit.GOAP.Actions.Movement
{
    [NodeMenu("Movement/Patrol")]
    public class Patrol : NavMeshMovement
    {
        [Tooltip("Should the agent patrol the waypoints randomly?")]
        public bool randomPatrol = false;
        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public float waypointPauseDuration = 0;
        [Tooltip("The waypoints to move to")]
        public SharedGameObjectList waypoints = new SharedGameObjectList();
    }

    [ViewModel(typeof(Patrol))]
    public class PatrolVM : NavMeshMovementVM
    {
        // The current index that we are heading towards within the waypoints array
        int waypointIndex;
        float waypointReachedTime;

        public PatrolVM(BaseNode model) : base(model) { }

        public override void OnAdded()
        {
            base.OnAdded();
            var t_model = Model as Patrol;
            t_model.preconditions.Add(new GOAPState("HasTarget", false));
            t_model.effects.Add(new GOAPState("HasTarget", true));
            t_model.randomPatrol = false;
            t_model.waypointPauseDuration = 0;
        }

        public override void OnPrePerform()
        {
            base.OnPrePerform();
            var t_model = Model as Patrol;
            // initially move towards the closest waypoint
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < t_model.waypoints.Value.Count; ++i)
            {
                if ((localDistance = Vector3.Magnitude(Agent.transform.position - t_model.waypoints.Value[i].transform.position)) < distance)
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
            var t_model = Model as Patrol;
            if (t_model.waypoints.Value.Count == 0)
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
                if (waypointReachedTime + t_model.waypointPauseDuration <= Time.time)
                {
                    if (t_model.randomPatrol)
                    {
                        if (t_model.waypoints.Value.Count == 1)
                        {
                            waypointIndex = 0;
                        }
                        else
                        {
                            // prevent the same waypoint from being selected
                            var newWaypointIndex = waypointIndex;
                            while (newWaypointIndex == waypointIndex)
                            {
                                newWaypointIndex = Random.Range(0, t_model.waypoints.Value.Count);
                            }
                            waypointIndex = newWaypointIndex;
                        }
                    }
                    else
                    {
                        waypointIndex = (waypointIndex + 1) % t_model.waypoints.Value.Count;
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
            var t_model = Model as Patrol;
            if (waypointIndex >= t_model.waypoints.Value.Count)
            {
                return Agent.transform.position;
            }
            return t_model.waypoints.Value[waypointIndex].transform.position;
        }

        public override void DrawGizmos(IGraphOwner _graphOwner)
        {
#if UNITY_EDITOR
            var t_model = Model as Patrol;
            if (t_model.waypoints == null)
            {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < t_model.waypoints.Value.Count; ++i)
            {
                if (t_model.waypoints.Value[i] != null)
                {
                    UnityEditor.Handles.SphereHandleCap(0, t_model.waypoints.Value[i].transform.position, t_model.waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
                }
            }
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}
using CZToolKit.Core.SharedVariable;
using CZToolKit.GraphProcessor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using Random = UnityEngine.Random;

namespace CZToolKit.GOAP
{
    [NodeMenuItem("Wander")]
    [NodeTooltip("在指定区域内徘徊，直到看到敌人")]
    [NodeIcon("Assets/CZToolKit/2.0_UIFramework/Icons/Simple/constellation, zodiac, astronomy, horoscope.png", width = 25, height = 25)]
    public class WanderAction : GOAPAction
    {
        [Header("巡逻范围")]
        public SharedGameObject center;

        public float range = 10;

        [Header("视野范围")]
        public float radius = 5;
        [Range(0, 360)]
        [Header("视野角度")]
        public float sector = 90;
        public LayerMask layer;

        private NavMeshAgent navMeshAgent;

        [Vertical, Port(PortDirection.Input, IsMulti = true, TypeConstraint = PortTypeConstraint.None)]
        [Tooltip("前往下个地点时触发")]
        [NonSerialized]
        UnityAction onRefindTarget;

        [Vertical, Port(PortDirection.Input, IsMulti = true, TypeConstraint = PortTypeConstraint.None)]
        [Tooltip("看到敌人时触发")]
        [NonSerialized]
        UnityAction onFindedTarget;

        public override void OnCreated()
        {
            base.OnCreated();
            Name = "徘徊";
            SetPrecondition("HasTarget", false);
            SetEffect("HasTarget", true);
        }

        protected override void OnInitialized()
        {
            navMeshAgent = Agent.GetComponent<NavMeshAgent>();
        }

        public override bool IsProceduralPrecondition(Dictionary<string, bool> currentState)
        {
            return base.IsProceduralPrecondition(currentState);
        }

        public override bool IsUsable()
        {
            //return Agent.States["HasTarget"] = false;
            //Agent.SetState("HasTarget", Agent.Blackboard.TryGetData("Target", out GameObject go) && go != null);
            return !Agent.Blackboard.TryGetData("Target", out GameObject go) || go == null;
        }

        Vector3 targetPos;
        float stayTime;

        public override void OnPrePerform()
        {
            targetPos = Random.insideUnitSphere * range + center.Value.transform.position;
            targetPos.y = 0;
            stayTime = Random.Range(2, 5);
            navMeshAgent.stoppingDistance = 0;
            navMeshAgent.isStopped = false;
            Debug.Log("徘徊");
        }

        public override ActionStatus OnPerform()
        {
            if (Vector3.Distance(targetPos, Agent.transform.position) <= 2)
            {
                stayTime -= Time.deltaTime;
                if (stayTime <= 0)
                {
                    onRefindTarget?.Invoke();
                    ExecuteConnections(nameof(onRefindTarget));
                    targetPos = Random.insideUnitSphere * range + center.Value.transform.position;
                    targetPos.y = 0;
                    stayTime = Random.Range(2, 5);
                }
            }
            navMeshAgent.SetDestination(targetPos);

            Collider[] colliders = Physics.OverlapSphere(Agent.transform.position, radius, layer);
            if (colliders.Length > 0)
            {
                foreach (var item in colliders)
                {
                    if (Vector3.Angle(Agent.transform.forward, item.transform.position - Agent.transform.position) <= sector / 2)
                    {
                        Agent.Blackboard.SetData("Target", item.gameObject);
                        return ActionStatus.Success;
                    }
                }
            }

            return ActionStatus.Running;
        }

        public override void OnPostPerform(bool _successed)
        {
            navMeshAgent.isStopped = true;
            if (_successed)
            {
                Agent.SetState("HasTarget", true);
                ExecuteConnections(nameof(onFindedTarget));
            }
            else
            {
                Agent.SetState("HasTarget", false);
            }
        }

        public override void DrawGizmos(GraphOwner _graphOwner)
        {
#if UNITY_EDITOR
            SharedGameObject variable = _graphOwner.GetVariable(center.GUID) as SharedGameObject;
            Gizmos.color = Color.green;
            if (variable != null && variable.Value != null)
            {
                Gizmos.DrawWireSphere(variable.Value.transform.position, range);
                Gizmos.DrawSphere(variable.Value.transform.position, 0.5f);
            }

            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawMesh(SemicircleMesh(radius, (int)sector, Vector3.up), _graphOwner.transform.position + Vector3.up * 0.2f, _graphOwner.transform.rotation);
#endif
            if (Application.isPlaying)
                Gizmos.DrawSphere(targetPos, 0.5f);
        }

#if UNITY_EDITOR
        /// <summary> 绘制半圆 </summary>
        public static void DrawWireSemicircle(Vector3 origin, Vector3 direction, float radius, int angle)
        {
            DrawWireSemicircle(origin, direction, radius, angle, Vector3.up);
        }

        public static void DrawWireSemicircle(Vector3 origin, Vector3 direction, float radius, int angle, Vector3 axis)
        {
            Vector3 leftdir = Quaternion.AngleAxis(-angle / 2, axis) * direction;
            Vector3 rightdir = Quaternion.AngleAxis(angle / 2, axis) * direction;

            Vector3 currentP = origin + leftdir * radius;
            Vector3 oldP;
            if (angle != 360)
            {
                Gizmos.DrawLine(origin, currentP);
            }
            for (int i = 0; i < angle / 10; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(10 * i, axis) * leftdir;
                oldP = currentP;
                currentP = origin + dir * radius;
                Gizmos.DrawLine(oldP, currentP);
            }
            oldP = currentP;
            currentP = origin + rightdir * radius;
            Gizmos.DrawLine(oldP, currentP);
            if (angle != 360)
                Gizmos.DrawLine(currentP, origin);
        }

        public static Mesh SemicircleMesh(float radius, int angle, Vector3 axis)
        {
            Vector3 leftdir = Quaternion.AngleAxis(-angle / 2, axis) * Vector3.forward;
            Vector3 rightdir = Quaternion.AngleAxis(angle / 2, axis) * Vector3.forward;
            int pcount = angle / 10;
            //顶点
            Vector3[] vertexs = new Vector3[3 + pcount];
            vertexs[0] = Vector3.zero;
            int index = 1;
            vertexs[index] = leftdir * radius;
            index++;
            for (int i = 0; i < pcount; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(10 * i, axis) * leftdir;
                vertexs[index] = dir * radius;
                index++;
            }
            vertexs[index] = rightdir * radius;
            //三角面
            int[] triangles = new int[3 * (1 + pcount)];
            for (int i = 0; i < 1 + pcount; i++)
            {
                triangles[3 * i] = 0;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = i + 2;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertexs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
#endif
    }
}

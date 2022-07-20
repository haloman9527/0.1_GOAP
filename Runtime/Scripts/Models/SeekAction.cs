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
using CZToolKit.GraphProcessor;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace CZToolKit.GOAP
{
    [NodeTooltip("追逐敌人到一定距离后停下")]
    [NodeMenuItem("Seek")]
    public class SeekAction : GOAPAction
    {
        public float stopDistance = 2;
        [Header("超时")]
        [Tooltip("超时将不再追击敌人")]
        public float timeout = 10;

    }

    [ViewModel(typeof(SeekAction))]
    public class SeekActionVM : GOAPActionVM
    {
        public SeekActionVM(BaseNode model) : base(model) { }


        [NonSerialized] GameObject target;
        [NonSerialized] NavMeshAgent navMeshAgent;

        private float startTime;
        public UnityAction onPrePerform { get; }
        public UnityAction onPerform { get; }
        public UnityAction onSuccess { get; }
        public UnityAction onFailed { get; }

        public override void OnAdded()
        {
            base.OnAdded();
            var t_model = Model as SeekAction;
            t_model.name = "追逐";
            t_model.cost = 1;
            t_model.preconditions.Add(new GOAPState() { Key = "HasTarget", Value = true });
            t_model.preconditions.Add(new GOAPState() { Key = "InAttackRange", Value = false });
            t_model.effects.Add(new GOAPState() { Key = "InAttackRange", Value = true });
        }

        public override void Initialized(GOAPAgent agent)
        {
            base.Initialized(agent);
            navMeshAgent = Agent.GetComponent<NavMeshAgent>();
        }

        public override void OnPrePerform()
        {
            var t_model = Model as SeekAction;
            Agent.Memory.TryGetData("Target", out target);
            startTime = Time.time;
            navMeshAgent.stoppingDistance = t_model.stopDistance;
            navMeshAgent.updateRotation = true;
            navMeshAgent.isStopped = false;
            onPrePerform?.Invoke();
            Debug.Log("追逐");
        }

        public override GOAPActionStatus OnPerform()
        {
            var t_model = Model as SeekAction;
            if (target == null || !target.activeSelf || Time.time - startTime > t_model.timeout)
            {
                Debug.Log("追不上");
                return GOAPActionStatus.Failure;
            }
            if (Vector3.Distance(Agent.transform.position, target.transform.position) <= t_model.stopDistance)
            {
                return GOAPActionStatus.Success;
            }
            navMeshAgent.destination = target.transform.position;
            onPerform?.Invoke();
            return GOAPActionStatus.Running;
        }

        public override void OnPostPerform(bool _successed)
        {
            navMeshAgent.isStopped = true;
            if (_successed)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onFailed?.Invoke();
                Agent.Memory.SetData<GameObject>("Target", null);
                Agent.SetState("HasTarget", false);
                Agent.SetState("InAttackRange", false);
            }
        }
    }
}

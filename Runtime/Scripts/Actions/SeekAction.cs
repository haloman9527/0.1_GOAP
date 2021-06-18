using CZToolKit.GraphProcessor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace CZToolKit.GOAP
{
    [NodeMenuItem("Seek")]
    [NodeTooltip("追逐敌人到一定距离后停下")]
    [NodeIcon("Assets/CZToolKit/0.1_GOAP/Editor/Icons/Running.png", width = 15, height = 18)]
    public class SeekAction : GOAPAction
    {
        private GameObject target;
        private NavMeshAgent navMeshAgent;

        public float stopDistance = 2;
        private float startTime;

        [Header("超时")]
        [Tooltip("超时将不再追击敌人")]
        public float timeout = 10;

        public UnityAction onPrePerform { get; }
        public UnityAction onPerform { get; }
        public UnityAction onSuccess { get; }
        public UnityAction onFailed { get; }

        public override void OnCreated()
        {
            base.OnCreated();

            Name = "追逐";
            cost = 1;
            SetPrecondition("HasTarget", true);
            SetPrecondition("InAttackRange", false);
            SetEffect("InAttackRange", true);
        }

        protected override void OnInitialized()
        {
            navMeshAgent = Agent.GetComponent<NavMeshAgent>();
        }

        public override void OnPrePerform()
        {
            Agent.Memory.TryGetData("Target", out target);
            startTime = Time.time;
            navMeshAgent.stoppingDistance = stopDistance;
            navMeshAgent.updateRotation = true;
            navMeshAgent.isStopped = false;
            onPrePerform?.Invoke();
            Debug.Log("追逐");
        }

        public override GOAPActionStatus OnPerform()
        {
            if (target == null || !target.activeSelf || Time.time - startTime > timeout)
            {
                Debug.Log("追不上");
                return GOAPActionStatus.Failure;
            }
            if (Vector3.Distance(Agent.transform.position, target.transform.position) <= stopDistance)
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

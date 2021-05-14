using CZToolKit.GraphProcessor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace CZToolKit.GOAP
{
    [NodeMenuItem("Seek")]
    [NodeDescription("追逐敌人到一定距离后停下")]
    [NodeIcon("Assets/CZToolKit/2.0_UIFramework/Icons/Awesome/running.png", width = 22, height = 25)]
    public class SeekAction : GOAPAction
    {
        private GameObject target;
        private NavMeshAgent navMeshAgent;

        public float distance = 2;
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

        public override void PrePerform()
        {
            Agent.Blackboard.TryGetData("Target", out target);
            startTime = Time.time;
            navMeshAgent.stoppingDistance = distance; 
            navMeshAgent.updateRotation = true;
            navMeshAgent.isStopped = false;
            onPrePerform?.Invoke();
            Debug.Log("追逐");
        }

        public override ActionStatus Perform()
        {
            if (target == null || !target.activeSelf || Time.time - startTime > timeout)
            {
                Debug.Log("追不上");
                return ActionStatus.Failure;
            }
            if (Vector3.Distance(Agent.transform.position, target.transform.position) <= distance)
            {
                return ActionStatus.Success;
            }
            navMeshAgent.destination = target.transform.position;
            onPerform?.Invoke();
            return ActionStatus.Running;
        }

        public override void PostPerform()
        {
            navMeshAgent.isStopped = true;
        }

        public override void Success()
        {
            onSuccess?.Invoke();
        }

        public override void Failed()
        {
            onFailed?.Invoke();
            Agent.Blackboard.SetData<GameObject>("Target", null);
            Agent.SetState("HasTarget", false);
        }
    }
}

using CZToolKit.GOAP;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SeekAction : Action
{
    private GameObject target;
    private NavMeshAgent navMeshAgent;

    public float distance = 2;
    private float startTime;

    [Header("超时")]
    [Tooltip("超时将不再追击敌人")]
    public float timeout = 10;

    public UnityAction onPrePerform;
    public UnityAction onPerform;
    public UnityAction onSuccess;
    public UnityAction onFailed;

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
    }

    public override bool Perform()
    {
        if (target == null || !target.activeSelf || Time.time - startTime > timeout)
        {
            Agent.Blackboard.SetData<GameObject>("Target", null);
            return false;
        }
        navMeshAgent.destination = target.transform.position;
        onPerform?.Invoke();
        return true;
    }

    public override void Success()
    {
        onSuccess?.Invoke();
    }

    public override void Failed()
    {
        onFailed?.Invoke();
    }

    public override void PostPerform()
    {
        navMeshAgent.isStopped = true;
    }

    public override bool IsDone()
    {
        return Vector3.Distance(Agent.transform.position, target.transform.position) <= distance;
    }

    protected override void Reset()
    {
        base.Reset();
        cost = 1;
        AddPrecondition("HasTarget", true);
        AddPrecondition("InAttackRange", false);
        AddEffect("InAttackRange", true);
    }
}

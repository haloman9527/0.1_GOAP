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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using CZToolKit;
using CZToolKit.SharedVariable;
using UnityEngine;
using UnityEngine.AI;
using CZToolKit.GraphProcessor;

namespace CZToolKit.GOAP.Actions.Movement
{
    public abstract class NavMeshGroupMoveMent : GOAPAction
    {
        [Tooltip("All of the agents")] public SharedGameObjectList agents = null;
        [Tooltip("The speed of the agents")] public SharedFloat speed = new SharedFloat(10);

        [Tooltip("The angular speed of the agents")]
        public SharedFloat angularSpeed = new SharedFloat(120);
    }

    public abstract class NavMeshGroupMovementProcessor : GOAPActionProcessor, IGroupMovement
    {
        // A cache of the NavMeshAgents
        NavMeshAgent[] navMeshAgents;
        protected Transform[] transforms;

        protected NavMeshGroupMovementProcessor(BaseNode model) : base(model)
        {
            var t_model = Model as NavMeshGroupMoveMent;
        }

        public float Speed
        {
            get { return (Model as NavMeshGroupMoveMent).speed.Value; }
            set { (Model as NavMeshGroupMoveMent).speed.Value = value; }
        }

        public float AgularSpeed
        {
            get { return (Model as NavMeshGroupMoveMent).angularSpeed.Value; }
            set { (Model as NavMeshGroupMoveMent).angularSpeed.Value = value; }
        }

        public override void OnPrePerform()
        {
            var t_model = Model as NavMeshGroupMoveMent;
            navMeshAgents = new NavMeshAgent[t_model.agents.Value.Count];
            transforms = new Transform[t_model.agents.Value.Count];
            for (int i = 0; i < t_model.agents.Value.Count; ++i)
            {
                transforms[i] = t_model.agents.Value[i].transform;
                navMeshAgents[i] = t_model.agents.Value[i].GetComponent<NavMeshAgent>();
                navMeshAgents[i].speed = t_model.speed.Value;
                navMeshAgents[i].angularSpeed = t_model.angularSpeed.Value;
                navMeshAgents[i].isStopped = false;
            }
        }

        public virtual bool SetDestination(int index, Vector3 target)
        {
            if (navMeshAgents[index].destination == target)
            {
                return true;
            }

            return navMeshAgents[index].SetDestination(target);
        }

        public virtual Vector3 Velocity(int index)
        {
            return navMeshAgents[index].velocity;
        }

        public override void OnPostPerform(bool _successed)
        {
            base.OnPostPerform(_successed);
            // Disable the nav mesh
            for (int i = 0; i < navMeshAgents.Length; ++i)
            {
                if (navMeshAgents[i] != null)
                {
                    navMeshAgents[i].isStopped = true;
                }
            }
        }
    }
}
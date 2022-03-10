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
using CZToolKit.Core.ViewModel;
using CZToolKit.Core.SharedVariable;
using UnityEngine;
using UnityEngine.AI;

namespace CZToolKit.GOAP.Actions.Movement
{
    public abstract class NavMeshGroupMovement : GroupMovement
    {
        [Tooltip("All of the agents")]
        public SharedGameObjectList agents = null;
        [Tooltip("The speed of the agents")]
        public SharedFloat speed = new SharedFloat(10);
        [Tooltip("The angular speed of the agents")]
        public SharedFloat angularSpeed = new SharedFloat(120);

        #region ViewModel
        // A cache of the NavMeshAgents
        NavMeshAgent[] navMeshAgents;
        protected Transform[] transforms;

        public BindableList<GameObject> Agents
        {
            get { return this[nameof(Agents)] as BindableList<GameObject>; }
            set { this[nameof(Agents)] = value; }
        }

        public float Speed
        {
            get { return GetPropertyValue<float>(nameof(Speed)); }
            set { SetPropertyValue(nameof(Speed), value); }
        }

        public float AgularSpeed
        {
            get { return GetPropertyValue<float>(nameof(AgularSpeed)); }
            set { SetPropertyValue(nameof(AgularSpeed), value); }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            this[nameof(Agents)] = new BindableList<GameObject>(() => agents.Value, v => agents.Value = v);
            this[nameof(AgularSpeed)] = new BindableProperty<float>(() => angularSpeed.Value, v => angularSpeed.Value = v);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Agents.SetValueWithoutNotify(agents.Value);
            this[nameof(Speed)].AsBindableProperty<float>().SetValueWithoutNotify(speed.Value);
            this[nameof(AgularSpeed)].AsBindableProperty<float>().SetValueWithoutNotify(angularSpeed.Value);
        }

        public override void OnPrePerform()
        {
            navMeshAgents = new NavMeshAgent[agents.Value.Count];
            transforms = new Transform[agents.Value.Count];
            for (int i = 0; i < agents.Value.Count; ++i)
            {
                transforms[i] = agents.Value[i].transform;
                navMeshAgents[i] = agents.Value[i].GetComponent<NavMeshAgent>();
                navMeshAgents[i].speed = speed.Value;
                navMeshAgents[i].angularSpeed = angularSpeed.Value;
                navMeshAgents[i].isStopped = false;
            }
        }

        protected override bool SetDestination(int index, Vector3 target)
        {
            if (navMeshAgents[index].destination == target)
            {
                return true;
            }
            return navMeshAgents[index].SetDestination(target);
        }

        protected override Vector3 Velocity(int index)
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
        #endregion
    }
}
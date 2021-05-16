using CZToolKit.Core.SimpleFSM;
using System;

namespace CZToolKit.GOAP
{
    public class GOAPFSM : FSM
    {
        public float time;
    }

    public class GOAPState : IFSMState
    {
        public Action onStart;
        public Action onUpdate;
        public Action onExit;

        public FSM Owner { get; set; }

        public GOAPState(GOAPFSM _owner)
        {
            Owner = _owner;
        }

        public virtual void OnStart()
        {
            onStart?.Invoke();
        }

        public virtual void OnUpdate()
        {
            onUpdate?.Invoke();
        }

        public virtual void OnExit()
        {
            onExit?.Invoke();
        }
    }

    public class IdleState : GOAPState
    {
        public IdleState(GOAPFSM _owner) : base(_owner) { }

        public override void OnStart()
        {
            //await Task.Run(onStart);
            onStart?.Invoke();
        }

        public override void OnUpdate()
        {
            onUpdate?.Invoke();
            //await Task.Run(onUpdate);
        }

        public override void OnExit()
        {
            onExit?.Invoke();
            //await Task.Run(onExit);
        }
    }
}

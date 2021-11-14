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
using CZToolKit.Core.SimpleFSM;
using System;

namespace CZToolKit.GOAP
{
    public class GOAPFSM : Core.SimpleFSM.FSM
    {
        public float time;
    }

    public class GOAPFSMState : IFSMState
    {
        public Action onStart;
        public Action onUpdate;
        public Action onExit;

        public Core.SimpleFSM.FSM Owner { get; set; }

        public GOAPFSMState(GOAPFSM _owner)
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

    public class IdleState : GOAPFSMState
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

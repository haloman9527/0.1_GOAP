using CZToolKit.Core.SimpleFSM;
using System;

namespace CZToolKit.GOAP
{
    public class GOAPState : IFSMState
    {
        public Action onStart;
        public Action onUpdate;
        public Action onExit;

        public void OnStart()
        {
            onStart?.Invoke();
        }

        public void OnUpdate()
        {
            onUpdate?.Invoke();
        }

        public void OnExit()
        {
            onExit?.Invoke();
        }
    }
}

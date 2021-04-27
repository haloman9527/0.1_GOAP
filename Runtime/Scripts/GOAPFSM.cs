using System;
using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    public class GOAPFSM
    {
        private Dictionary<string, GOAPFSMState> states = new Dictionary<string, GOAPFSMState>();
        private GOAPFSMState currentState;

        public void LateUpdate()
        {
            if (currentState.onUpdate != null)
                currentState.onUpdate();
        }

        public void PushState(string stateName, GOAPFSMState state)
        {
            states[stateName] = state;
        }

        public void ChangeTo(string stateName)
        {
            if (currentState == states[stateName])
                return;

            if (currentState != null && currentState.onExit != null)
                currentState.onExit();

            currentState = states[stateName];
            if (currentState != null && currentState.onStart != null)
                currentState.onStart();
        }
    }

    public class GOAPFSMState
    {
        public System.Action onStart;
        public System.Action onUpdate;
        public System.Action onExit;
    }
}
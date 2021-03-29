using System;

namespace CZToolKit.GOAP
{
    [Serializable]
    public class Goal : State
    {
        public bool Once;
        public float Priority;
    }
}
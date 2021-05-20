using System;

namespace CZToolKit.GOAP
{
    [Serializable]
    public class GOAPGoal : GOAPState
    {
        public bool Once;
        public float Priority;
    }
}
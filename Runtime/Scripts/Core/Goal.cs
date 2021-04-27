using System;

namespace CZToolKit.GOAP
{
    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
    public class Goal : State
    {
        public bool Once;
        public float Priority;
    }
}
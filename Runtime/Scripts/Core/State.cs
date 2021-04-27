using System;

namespace CZToolKit.GOAP
{
    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
    public class State
    {
        public string Key = "None";
        public bool Value;

        public State() { }

        public State(string _key, bool _value)
        {
            Key = _key;
            Value = _value;
        }
    }
}

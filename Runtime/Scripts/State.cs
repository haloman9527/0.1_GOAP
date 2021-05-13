using System;

namespace CZToolKit.GOAP
{
    [Serializable]
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

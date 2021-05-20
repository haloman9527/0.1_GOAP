using System;

namespace CZToolKit.GOAP
{
    [Serializable]
    public class GOAPState
    {
        public string Key = "None";
        public bool Value;

        public GOAPState() { }

        public GOAPState(string _key, bool _value)
        {
            Key = _key;
            Value = _value;
        }
    }
}

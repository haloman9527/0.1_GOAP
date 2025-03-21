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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System;

namespace Atom.GOAP
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

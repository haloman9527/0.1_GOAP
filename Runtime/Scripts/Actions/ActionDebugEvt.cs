#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using CZToolKit.GraphProcessor;
using UnityEngine;

namespace CZToolKit.GOAP
{
    [NodeMenuItem("ActionDebug")]
    public class ActionDebugEvt : GOAPActionEvtNode
    {
        public string text = "";
        public override void Execute(NodePort _port, params object[] _params)
        {
            Debug.Log(text);
        }
    }
}

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
    [NodeIcon("Assets/CZToolKit/2.0_UIFramework/Icons/Awesome/broom.png", width = 18, height = 14)]
    public class ActionDebugEvt : GOAPActionEvtNode
    {
        public string text = "";
        public override void Execute(NodePort _port, params object[] _params)
        {
            Debug.Log(text);
        }
    }
}

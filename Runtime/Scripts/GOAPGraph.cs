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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using CZToolKit.GraphProcessor;
using System;
using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    [Serializable]
    public class GOAPGraph : BaseGraph
    {
        public List<GOAPAction> AvailableActions { get; private set; } = new List<GOAPAction>();

        public override void Initialize(IGraphOwner _graphOwner)
        {
            base.Initialize(_graphOwner);
            GOAPAgent agent = _graphOwner as GOAPAgent;

            if (AvailableActions == null)
                AvailableActions = new List<GOAPAction>();
            else
                AvailableActions.Clear();
            foreach (var node in Nodes)
            {
                if (node.Value is GOAPAction action)
                    AvailableActions.Add(action);
            }
            AvailableActions.QuickSort((a, b) =>
            {
                if (a.Position.y > b.Position.y)
                    return 1;
                if (a.Position.y == b.Position.y)
                    return 0;
                return -1;
            });
            foreach (var action in AvailableActions)
            {
                action.Initialize(agent);
            }
        }

        public void DrawGizmos(GOAPAgent _agent)
        {
            foreach (var node in Nodes)
            {
                GOAPAction action = node.Value as GOAPAction;
                if (action != null)
                    action.DrawGizmos(_agent);
            }
        }
    }
}

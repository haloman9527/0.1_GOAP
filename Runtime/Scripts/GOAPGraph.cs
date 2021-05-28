using CZToolKit.GraphProcessor;
using System;
using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    [Serializable]
    public class GOAPGraph : BaseGraph
    {
        public List<GOAPAction> AvailableActions { get; private set; } = new List<GOAPAction>();

        public override void Initialize(GraphOwner _graphOwner)
        {
            base.Initialize(_graphOwner);
            GOAPAgent agent = _graphOwner as GOAPAgent;

            if (AvailableActions == null)
                AvailableActions = new List<GOAPAction>();
            else
                AvailableActions.Clear();
            foreach (var node in NodesGUIDMapping)
            {
                if (node.Value is GOAPAction action)
                    AvailableActions.Add(action);
            }
            AvailableActions.QuickSort((a, center) =>
            {
                if (a.position.y > center.position.y)
                    return true;
                return false;
            });
            foreach (var action in AvailableActions)
            {
                action.InitializeGraphOwner(agent);
            }
        }

        public void DrawGizmos(GOAPAgent _agent)
        {
            foreach (var node in NodesGUIDMapping)
            {
                GOAPAction action = node.Value as GOAPAction;
                if (action != null)
                    action.DrawGizmos(_agent);
            }
        }
    }
}

using CZToolKit.GraphProcessor;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.GOAP
{
    [CreateAssetMenu]
    public class GOAPGraph : BaseGraph
    {
        public List<GOAPAction> AvailableActions { get; private set; } = new List<GOAPAction>();

        public override void Initialize(GraphOwner _graphOwner)
        {
            base.Initialize(_graphOwner);
            GOAPAgent agent = _graphOwner as GOAPAgent;

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
                action.Initialize(agent);
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

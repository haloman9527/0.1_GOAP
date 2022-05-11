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
using CZToolKit.Core.SharedVariable;
using CZToolKit.GraphProcessor;
using System;
using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    [Serializable]
    public class GOAPGraph : BaseGraph, IGraphForMono
    {
        [NonSerialized] internal List<SharedVariable> variables;

        public IGraphOwner GraphOwner
        {
            get;
            private set;
        }
        public IVariableOwner VarialbeOwner
        {
            get { return GraphOwner as IVariableOwner; }
        }
        public IReadOnlyList<SharedVariable> Variables
        {
            get { return variables; }
        }
        public List<GOAPAction> AvailableActions { get; private set; } = new List<GOAPAction>();

        protected override void OnEnabled()
        {
            base.OnEnabled();

            OnNodeAdded += NodeAdded;
        }

        public void Initialize(IGraphOwner graphOwner)
        {
            GraphOwner = graphOwner;

            foreach (var node in Nodes.Values)
            {
                if (node is INodeForMono monoNode)
                    monoNode.Initialize();
            }

            variables = new List<SharedVariable>();
            foreach (var node in Nodes.Values)
            {
                variables.AddRange(SharedVariableUtility.CollectionObjectSharedVariables(node));
            }
            foreach (var variable in variables)
            {
                variable.InitializePropertyMapping(VarialbeOwner);
            }

            GOAPAgent agent = GraphOwner as GOAPAgent;
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
        }

        public void NodeAdded(BaseNode node)
        {
            if (!(node is INodeForMono monoNode))
                return;
            if (GraphOwner != null)
                monoNode.Initialize();

            IEnumerable<SharedVariable> nodeVariables = SharedVariableUtility.CollectionObjectSharedVariables(node);
            variables.AddRange(nodeVariables);
            if (VarialbeOwner != null)
            {
                foreach (var variable in nodeVariables)
                {
                    variable.InitializePropertyMapping(VarialbeOwner);
                }
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

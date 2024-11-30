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
using Moyo;
using System;
using System.Collections.Generic;
using Moyo.GraphProcessor;
using Moyo.SharedVariable;

namespace Moyo.GOAP
{
    [Serializable]
    public class GOAPGraph : BaseGraph { }

    [ViewModel(typeof(GOAPGraph))]
    public class GOAPGraphProcessor : BaseGraphProcessor
    {
        [NonSerialized] internal List<SharedVariable.SharedVariable> variables;

        public IGraphOwner GraphOwner
        {
            get;
            private set;
        }
        public IVariableOwner VarialbeOwner
        {
            get { return GraphOwner as IVariableOwner; }
        }
        public IReadOnlyList<SharedVariable.SharedVariable> Variables
        {
            get { return variables; }
        }
        public List<GOAPActionProcessor> AvailableActions { get; private set; } = new List<GOAPActionProcessor>();

        public GOAPGraphProcessor(BaseGraph model) : base(model)
        {
            OnNodeAdded += NodeAdded;
        }

        public void Initialize(IGraphOwner graphOwner)
        {
            GraphOwner = graphOwner;

            foreach (var node in Nodes.Values)
            {
                if (node is IGOAPAction goapAction)
                    goapAction.Initialize();
            }

            variables = new List<SharedVariable.SharedVariable>();
            foreach (var node in Nodes.Values)
            {
                variables.AddRange(SharedVariableUtility.CollectionObjectSharedVariables(node));
            }
            foreach (var variable in variables)
            {
                variable.SetVariableOwner(VarialbeOwner);
            }

            GOAPAgent agent = GraphOwner as GOAPAgent;
            if (AvailableActions == null)
                AvailableActions = new List<GOAPActionProcessor>();
            else
                AvailableActions.Clear();
            foreach (var node in Nodes)
            {
                if (node.Value is GOAPActionProcessor action)
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

        public void NodeAdded(BaseNodeProcessor node)
        {
            if (!(node is IGOAPAction goapAction))
                return;
            if (GraphOwner != null)
                goapAction.Initialize();

            IEnumerable<SharedVariable.SharedVariable> nodeVariables = SharedVariableUtility.CollectionObjectSharedVariables(node);
            variables.AddRange(nodeVariables);
            if (VarialbeOwner != null)
            {
                foreach (var variable in nodeVariables)
                {
                    variable.SetVariableOwner(VarialbeOwner);
                }
            }
        }

        public void DrawGizmos(GOAPAgent _agent)
        {
            foreach (var node in Nodes)
            {
                GOAPActionProcessor action = node.Value as GOAPActionProcessor;
                if (action != null)
                    action.DrawGizmos(_agent);
            }
        }
    }
}

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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using CZToolKit.VM;
using CZToolKit.SharedVariable;
using CZToolKit.GraphProcessor;
using System;
using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    [Serializable]
    public class GOAPGraph : BaseGraph { }

    [ViewModel(typeof(GOAPGraph))]
    public class GOAPGraphVM : BaseGraphVM
    {
        [NonSerialized] internal List<CZToolKit.SharedVariable.SharedVariable> variables;

        public IGraphOwner GraphOwner
        {
            get;
            private set;
        }
        public IVariableOwner VarialbeOwner
        {
            get { return GraphOwner as IVariableOwner; }
        }
        public IReadOnlyList<CZToolKit.SharedVariable.SharedVariable> Variables
        {
            get { return variables; }
        }
        public List<GOAPActionVM> AvailableActions { get; private set; } = new List<GOAPActionVM>();

        public GOAPGraphVM(BaseGraph model) : base(model)
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

            variables = new List<CZToolKit.SharedVariable.SharedVariable>();
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
                AvailableActions = new List<GOAPActionVM>();
            else
                AvailableActions.Clear();
            foreach (var node in Nodes)
            {
                if (node.Value is GOAPActionVM action)
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

        public void NodeAdded(BaseNodeVM node)
        {
            if (!(node is IGOAPAction goapAction))
                return;
            if (GraphOwner != null)
                goapAction.Initialize();

            IEnumerable<CZToolKit.SharedVariable.SharedVariable> nodeVariables = SharedVariableUtility.CollectionObjectSharedVariables(node);
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
                GOAPActionVM action = node.Value as GOAPActionVM;
                if (action != null)
                    action.DrawGizmos(_agent);
            }
        }
    }
}

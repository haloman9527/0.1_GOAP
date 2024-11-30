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
using System.Collections.Generic;
using Moyo.GraphProcessor;
using Moyo.GraphProcessor.Editors;
using UnityObject = UnityEngine.Object;

namespace Moyo.GOAP.Editors
{
    [CustomView(typeof(GOAPGraph))]
    public class GOAPGraphWindow : BaseGraphWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "Goap Graph";
        }

        protected override BaseGraphView NewGraphView()
        {
            return new GOAPGraphView(Graph, this, new CommandDispatcher());
        }
    }

    public class GOAPGraphView : BaseGraphView
    {
        public GOAPGraphView(BaseGraphProcessor graph, BaseGraphWindow window, CommandDispatcher commandDispatcher) : base(graph, window, commandDispatcher)
        {
        }

        protected override void BuildNodeMenu(NodeMenuWindow nodeMenu)
        {
            foreach (var nodeType in GetNodeTypes())
            {
                if (nodeType.IsAbstract) 
                    continue;
                var nodeStaticInfo = GraphProcessorUtil.NodeStaticInfos[nodeType];
                if (nodeStaticInfo.hidden)
                    continue;
                
                var path = nodeStaticInfo.path;
                var menu = nodeStaticInfo.menu;
                nodeMenu.entries.Add(new NodeMenuWindow.NodeEntry(path, menu, nodeType));
            }
        }
        
        private IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in Util_TypeCache.GetTypesDerivedFrom<GOAPAction>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }

            foreach (var type in Util_TypeCache.GetTypesDerivedFrom<GOAPActionEvtNode>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }
        }

        protected override BaseNodeView NewNodeView(BaseNodeProcessor node)
        {
            if (typeof(GOAPAction).IsAssignableFrom(node.ModelType))
                return new GOAPNodeView();
            else
                return base.NewNodeView(node);
        }
    }
}
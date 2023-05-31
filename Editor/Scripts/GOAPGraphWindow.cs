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

using CZToolKit.Common;
using CZToolKit.Common.Collection;
using CZToolKit.GraphProcessor;
using CZToolKit.GraphProcessor.Editors;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.GOAP.Editors
{
    [CustomView(typeof(GOAPGraph))]
    public class GOAPGraphWindow : BaseGraphWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "Goap Graph";
        }

        protected override BaseGraphView NewGraphView(object argument)
        {
            var graphView = new GOAPGraphView(Graph, this, new CommandDispatcher());
            GraphView.RegisterCallback<KeyDownEvent>(KeyDownCallback);
            return graphView;
        }

        protected override void BuildToolBar()
        {
            base.BuildToolBar();
            
            ToolbarButton btnSave = new ToolbarButton();
            btnSave.text = "Save";
            btnSave.style.width = 80;
            btnSave.style.unityTextAlign = TextAnchor.MiddleCenter;
            btnSave.clicked += Save;
            ToolbarRight.Add(btnSave);
        }

        void KeyDownCallback(KeyDownEvent evt)
        {
            if (evt.commandKey || evt.ctrlKey)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.S:
                        Save();
                        break;
                }
            }
        }

        void Save()
        {
            if (GraphAsset is IGraphSerialization graphSerialization)
                graphSerialization.SaveGraph(Graph.Model);
            GraphView.SetDirty();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GraphView.SetUnDirty();
        }
    }

    public class GOAPGraphView : BaseGraphView
    {
        public GOAPGraphView(BaseGraphVM graph, BaseGraphWindow window, CommandDispatcher commandDispatcher) : base(graph, window, commandDispatcher)
        {
        }

        protected override void NodeCreationRequest(NodeCreationContext c)
        {
            var multiLayereEntryCount = 0;
            var entries = new List<NodeEntry>(16);
            foreach (var nodeType in GetNodeTypes())
            {
                if (nodeType.IsAbstract) 
                    continue;
                var nodeStaticInfo = GraphProcessorUtil.NodeStaticInfos[nodeType];
                var path = nodeStaticInfo.path;
                var menu = nodeStaticInfo.menu;
                var hidden = nodeStaticInfo.hidden;

                if (menu.Length > 1)
                    multiLayereEntryCount++;
                entries.Add(new NodeEntry(nodeType, path, menu, hidden));
            }

            entries.QuickSort((a, b) => -(a.menu.Length.CompareTo(b.menu.Length)));
            entries.QuickSort(0, multiLayereEntryCount - 1, (a, b) => String.Compare(a.path, b.path, StringComparison.Ordinal));
            entries.QuickSort(multiLayereEntryCount, entries.Count - 1, (a, b) => String.Compare(a.path, b.path, StringComparison.Ordinal));

            var nodeMenu = ScriptableObject.CreateInstance<NodeMenuWindow>();
            nodeMenu.Initialize("Nodes", this, entries);
            SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), nodeMenu);
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

        protected override BaseNodeView NewNodeView(BaseNodeVM node)
        {
            if (typeof(GOAPAction).IsAssignableFrom(node.ModelType))
                return new GOAPNodeView();
            else
                return base.NewNodeView(node);
        }
    }
}
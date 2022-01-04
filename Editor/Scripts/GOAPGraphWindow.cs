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
using CZToolKit.Core;
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
    [CustomGraphWindow(typeof(GOAPGraph))]
    public class GOAPGraphWindow : BaseGraphWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "Goap Graph";
        }

        protected override BaseGraphView NewGraphView(BaseGraph graph, CommandDispatcher commandDispatcher)
        {
            return new GOAPGraphView(graph as GOAPGraph, this, commandDispatcher);
        }

        protected override void BuildToolbar(ToolbarView toolbar)
        {
            base.BuildToolbar(toolbar);
            ToolbarButton btnSave = new ToolbarButton();
            btnSave.text = "Save";
            btnSave.clicked += Save;
            toolbar.AddButtonToRight(btnSave);
        }

        protected override void KeyDownCallback(KeyDownEvent evt)
        {
            base.KeyDownCallback(evt);
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
            if (GraphAsset is IGraphAsset graphAsset)
            {
                graphAsset.SaveGraph(Graph);
            }
            if (GraphOwner is IGraphOwner graphOwner)
            {
                graphOwner.SaveVariables();
            }
            GraphView.SetDirty(true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GraphView.UnsetDirty();
        }
    }

    public class GOAPGraphView : BaseGraphView
    {
        public GOAPGraphView(GOAPGraph _graph, BaseGraphWindow _window, CommandDispatcher _commandDispatcher) : base(_graph, _window, _commandDispatcher) { }

        protected override IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in Util_Reflection.GetChildTypes<GOAPAction>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }
            foreach (var type in Util_Reflection.GetChildTypes<GOAPActionEvtNode>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }
        }

        protected override Type GetNodeViewType(BaseNode node)
        {
            if (typeof(GOAPAction).IsAssignableFrom(node.GetType()))
                return typeof(GOAPNodeView);
            else
                return base.GetNodeViewType(node);
        }
    }
}

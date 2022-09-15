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
    [CustomView(typeof(GOAPGraph))]
    public class GOAPGraphWindow : BaseGraphWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "Goap Graph";
        }

        protected override BaseGraphView NewGraphView(BaseGraphVM graph)
        {
            return new GOAPGraphView();
        }

        protected override void OnGraphLoaded()
        {
            base.OnGraphLoaded();

            ToolbarButton btnSave = new ToolbarButton();
            btnSave.text = "Save";
            btnSave.style.width = 80;
            btnSave.style.unityTextAlign = TextAnchor.MiddleCenter;
            btnSave.clicked += Save;
            ToolbarRight.Add(btnSave);

            GraphView.RegisterCallback<KeyDownEvent>(KeyDownCallback);
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
            GraphView.SetUndirty();
        }
    }

    public class GOAPGraphView : BaseGraphView
    {
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

        protected override BaseNodeView NewNodeView(BaseNodeVM node)
        {
            if (typeof(GOAPAction).IsAssignableFrom(node.ModelType))
                return new GOAPNodeView();
            else
                return base.NewNodeView(node);
        }
    }
}

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

        protected override BaseGraphView NewGraphView(BaseGraph graph)
        {
            return new GOAPGraphView(graph as GOAPGraph, this, new CommandDispatcher());
        }
    }

    public class GOAPGraphView : BaseGraphView
    {
        Label label;

        public GOAPGraphView(GOAPGraph _graph, BaseGraphWindow _window, CommandDispatcher _commandDispatcher) : base(_graph, _window, _commandDispatcher)
        {
            label = new Label();
            label.style.fontSize = 30;
            label.style.color = new StyleColor(new Color(0.18f, 1f, 0.63f));
            label.style.alignSelf = Align.FlexEnd;
            Add(label);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateLabel();
            Add(new IMGUIContainer(UpdateLabel));

            MiniMap miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(10, 10, 200, 200));
            Add(miniMap);

            miniMap.MarkDirtyRepaint();

            ToolbarToggle tglMiniMap = new ToolbarToggle() { text = "MiniMap" };
            tglMiniMap.RegisterValueChangedCallback(e =>
            {
                miniMap.visible = e.newValue;
            });
            tglMiniMap.value = true;
            GraphWindow.Toolbar.AddToggleToLeft(tglMiniMap, 80);


            // 添加模拟节点

            // 模拟节点所有的世界状态
        }

        protected override IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in Utility_Reflection.GetChildTypes<GOAPAction>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }
            foreach (var type in Utility_Reflection.GetChildTypes<GOAPActionEvtNode>())
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

        void UpdateLabel()
        {
            string assetName = "";
            string ownerName = "";
            if (GraphAsset != null)
                assetName = GraphAsset.name;
            if (GraphWindow.GraphOwner != null)
                ownerName = (GraphWindow.GraphOwner as UnityObject)?.name;
            label.text = $"{assetName}/{ownerName}";
        }
    }
}

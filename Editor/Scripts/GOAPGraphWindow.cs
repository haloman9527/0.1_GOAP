using CZToolKit.Core;
using CZToolKit.GraphProcessor;
using CZToolKit.GraphProcessor.Editors;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

        protected override BaseGraphView CreateGraphView(IGraph _graph)
        {
            GOAPGraphView graphView = new GOAPGraphView(_graph, CommandDispatcher, this);
            return graphView;
        }
    }

    public class GOAPGraphView : BaseGraphView
    {
        protected override Type GetDefaultNodeViewType(Type _nodeDataType)
        {
            if (typeof(GOAPAction).IsAssignableFrom(_nodeDataType))
                return typeof(GOAPNodeView);
            else
                return typeof(SimpleNodeView);
        }

        protected override IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in Utility_Refelection.GetChildrenTypes<GOAPAction>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }
            foreach (var type in Utility_Refelection.GetChildrenTypes<GOAPActionEvtNode>())
            {
                if (type.IsAbstract) continue;
                yield return type;
            }
        }

        Label label;

        public GOAPGraphView(IGraph _graph, CommandDispatcher _commandDispatcher, BaseGraphWindow _window) : base(_graph, _commandDispatcher, _window)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            label = new Label();
            label.style.fontSize = 30;
            label.style.color = new StyleColor(new Color(0.18f, 1f, 0.63f));
            label.style.alignSelf = Align.FlexEnd;
            //label.BindProperty(SerializedObject.FindProperty("m_Name"));
            Add(label);

            Add(new IMGUIContainer(UpdateLabel));

            MiniMap miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(10, 10, 200, 200));
            Add(miniMap);

            miniMap.MarkDirtyRepaint();

            ToolbarToggle tglMiniMap = new ToolbarToggle() { text = "MiniMap", style = { alignSelf = Align.Center, width = 80, unityTextAlign = TextAnchor.MiddleCenter } };
            tglMiniMap.RegisterValueChangedCallback(e =>
            {
                miniMap.visible = e.newValue;
            });
            tglMiniMap.value = true;
            Parent.Toolbar.AddToggleToLeft(tglMiniMap);


            // 添加模拟节点

            // 模拟节点所有的世界状态
        }

        void UpdateLabel()
        {
            string assetName = "";
            string ownerName = "";
            if (GraphAsset != null)
                assetName = GraphAsset.name;
            if (GraphWindow.GraphOwner != null && GraphWindow.GraphOwner.Self() != null)
                ownerName = GraphWindow.GraphOwner.GetOwnerName();
            label.text = $"{assetName}/{ownerName}";
        }
    }
}

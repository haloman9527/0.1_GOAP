using CZToolKit.Core;
using CZToolKit.GraphProcessor;
using CZToolKit.GraphProcessor.Editors;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CZToolKit.GOAP.Editors
{
    [CustomGraphWindow(typeof(GOAPGraphAsset))]
    public class GOAPGraphWindow : BaseGraphWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "Goap Graph";
        }

        protected override BaseGraphView InitializeGraphView(BaseGraphAsset _graphAsset)
        {
            GOAPGraphView graphView = new GOAPGraphView();
            graphView.Initialize(this, _graphAsset);
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
            foreach (var type in Utility.GetChildrenTypes<GOAPAction>())
            {
                yield return type;
            }
            foreach (var type in Utility.GetChildrenTypes<GOAPActionEvtNode>())
            {
                yield return type;
            }
        }

        Label label;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            label = new Label();
            label.style.fontSize = 30;
            label.style.color = new StyleColor(new Color(0.18f, 1f, 0.63f));
            label.style.alignSelf = Align.FlexEnd;
            //label.BindProperty(SerializedObject.FindProperty("m_Name"));
            Add(label);

            MiniMap miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(10, 10, 200, 200));
            Add(miniMap);
            miniMap.MarkDirtyRepaint();

            GraphWindow.Toolbar.AddToggle("MiniMap", true, b =>
            {
                miniMap.visible = b;
            }, true);

            // 添加模拟节点

            // 模拟节点所有的世界状态
        }

        public override void OnGUI()
        {
            base.OnGUI();
            string assetName = "";
            string ownerName = "";
            if (GraphAsset != null)
                assetName = GraphAsset.name;
            if (GraphWindow.GraphOwner != null && GraphWindow.GraphOwner.GetObject() != null)
                ownerName = GraphWindow.GraphOwner.GetOwnerName();
            label.text = $"{assetName}/{ownerName}";
        }
    }
}

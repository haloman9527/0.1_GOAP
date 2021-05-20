using CZToolKit.Core;
using CZToolKit.GraphProcessor;
using CZToolKit.GraphProcessor.Editors;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CZToolKit.GOAP
{
    public class GOAPNodeView : SimpleNodeView, IOnGUIObserver
    {
        string rawTitle;

        //public override bool RefreshPorts()
        //{
        //    GOAPAction action = NodeData as GOAPAction;
        //    bool result = base.RefreshPorts();
        //    outputContainer.Add(PortViews[nameof(action.onExit)]);
        //    return result;
        //}

        protected override void OnInitialized()
        {
            base.OnInitialized();

            titleContainer.style.height = 30;
            style.minWidth = 150;
            rawTitle = title;

            if (!typeof(GOAPAction).IsAssignableFrom(NodeDataType)) return;

            title = (NodeData as GOAPAction).Name;

            GOAPAction action = NodeData as GOAPAction;

            // 名字
            TextField nameField = new TextField("Name", 100, false, false, '*') { value = action.Name };
            nameField.labelElement.style.minWidth = nameField.labelElement.style.maxWidth = 50;
            nameField.RegisterValueChangedCallback(evt =>
            {
                action.Name = evt.newValue;
            });
            nameField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            nameField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
            controlsContainer.Add(nameField);

            // 成本
            FloatField costField = new FloatField("Cost") { value = action.cost };
            costField.labelElement.style.minWidth = costField.labelElement.style.maxWidth = 50;
            costField.RegisterValueChangedCallback(evt => { action.cost = evt.newValue; });
            controlsContainer.Add(costField);

            // 条件列表
            Foldout conditionFoldout = new Foldout() { text = "条件" };
            conditionFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            controlsContainer.Add(conditionFoldout);
            Button btnAddCondition = new Button(() =>
            {
                GOAP.GOAPState state = new GOAP.GOAPState();
                action.Preconditions.Add(state);
                conditionFoldout.Add(CreateToggle(state, action.Preconditions));
            });
            btnAddCondition.text = "添加条件";
            btnAddCondition.style.alignItems = Align.Center;
            conditionFoldout.Add(btnAddCondition);
            for (int i = 0; i < action.Preconditions.Count; i++)
            {
                conditionFoldout.Add(CreateToggle(action.Preconditions[i], action.Preconditions));
            }

            // 效果列表
            Foldout effectFoldout = new Foldout() { text = "可以造成的效果" };
            effectFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            controlsContainer.Add(effectFoldout);
            Button btnAddEffect = new Button(() =>
            {
                GOAP.GOAPState state = new GOAP.GOAPState();
                action.Effects.Add(state);
                effectFoldout.Add(CreateToggle(state, action.Effects));
            });
            btnAddEffect.text = "添加效果";
            btnAddEffect.style.alignItems = Align.Center;
            effectFoldout.Add(btnAddEffect);
            for (int i = 0; i < action.Effects.Count; i++)
            {
                effectFoldout.Add(CreateToggle(action.Effects[i], action.Effects));
            }
        }

        VisualElement CreateToggle(GOAP.GOAPState _state, List<GOAP.GOAPState> _states)
        {
            VisualElement box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;

            Toggle toggle = new Toggle();
            toggle.value = _state.Value;
            toggle.RegisterValueChangedCallback(evt =>
            {
                _state.Value = evt.newValue;
            });
            box.Add(toggle);

            TextField keyField = new TextField();
            keyField.value = _state.Key;
            keyField.RegisterValueChangedCallback(evt =>
            {
                _state.Key = evt.newValue;
            });
            keyField.style.flexGrow = 1;
            keyField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            keyField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
            box.Add(keyField);

            box.Add(CreateSmallButton(EditorGUIUtility.FindTexture("P4_DeletedLocal"), 14,
                () =>
                {
                    _states.Remove(_state);
                    box.RemoveFromHierarchy();
                }));
            return box;
        }

        Button CreateSmallButton(string _text, float _size, Action _onClick)
        {
            Button btn = new Button(_onClick);
            btn.style.height = btn.style.width = _size;
            btn.text = _text;
            btn.style.backgroundImage = EditorGUIUtility.FindTexture("P4_DeletedLocal");
            return btn;
        }

        Button CreateSmallButton(Texture2D _texture, float _size, Action _onClick)
        {
            Button btn = new Button(_onClick);
            btn.style.height = btn.style.width = _size;
            btn.style.backgroundImage = _texture;
            return btn;
        }

        public void OnGUI()
        {
            if (!typeof(GOAPAction).IsAssignableFrom(NodeDataType)) return;
            title = (NodeData as GOAPAction).Name + $"({rawTitle})";
        }
    }
}

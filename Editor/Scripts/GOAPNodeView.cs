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
    public class GOAPNodeView : SimpleNodeView<GOAPAction>
    {
        string rawTitle;
        TextField nameField;
        FloatField costField;

        Foldout conditionFoldout;
        Button btnAddCondition;
        Foldout effectFoldout;
        Button btnAddEffect;

        public GOAPNodeView() : base()
        {
            nameField = new TextField();
            controlsContainer.Add(nameField);

            costField = new FloatField();
            controlsContainer.Add(costField);

            conditionFoldout = new Foldout() { text = "条件" };
            conditionFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            controlsContainer.Add(conditionFoldout);

            btnAddCondition = new Button();
            btnAddCondition.text = "添加条件";
            btnAddCondition.style.alignItems = Align.Center;
            conditionFoldout.Add(btnAddCondition);

            effectFoldout = new Foldout() { text = "可以造成的效果" };
            effectFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            controlsContainer.Add(effectFoldout);

            btnAddEffect = new Button();
            btnAddEffect.text = "添加效果";
            btnAddEffect.style.alignItems = Align.Center;
            effectFoldout.Add(btnAddEffect);

            titleContainer.style.height = 30;
            style.minWidth = 150;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();


            if (!typeof(GOAPAction).IsAssignableFrom(T_Model.GetType())) return;

            nameField.RegisterValueChangedCallback(evt =>
            {
                T_Model.Name = evt.newValue;
            });
            nameField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            nameField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });

            costField.RegisterValueChangedCallback(evt =>
            {
                T_Model.Cost = evt.newValue;
            });

            btnAddCondition.clicked += () =>
            {
                T_Model.AddPrecondition(new GOAPState());
                Owner.SetDirty();
            };
            btnAddEffect.clicked += () =>
            {
                T_Model.AddEffect(new GOAPState());
                Owner.SetDirty();
            };

            for (int i = 0; i < T_Model.Preconditions.Count; i++)
            {
                conditionFoldout.Add(CreateToggle(T_Model.Preconditions[i], (ele, state) =>
                {
                    T_Model.RemovePrecondition(state);
                    ele.RemoveFromHierarchy();
                }));
            }

            for (int i = 0; i < T_Model.Effects.Count; i++)
            {
                effectFoldout.Add(CreateToggle(T_Model.Effects[i], (ele, state) =>
                {
                    T_Model.RemoveEffect(state);
                    ele.RemoveFromHierarchy();
                }));
            }
        }

        protected override void BindingPropertiesBeforeUpdate()
        {
            base.BindingPropertiesBeforeUpdate();
            rawTitle = T_Model.Title;

            T_Model.RegisterValueChangedEvent<string>(nameof(T_Model.Name), v =>
            {
                Model.Title = T_Model.Name + $"({rawTitle})";
                nameField.SetValueWithoutNotify(v);
            });

            T_Model.RegisterValueChangedEvent<float>(nameof(T_Model.Cost), v =>
            {
                costField.SetValueWithoutNotify(v);
            });

            T_Model.onPreconditionAdded += v =>
            {
                conditionFoldout.Add(CreateToggle(v, (ele, state) =>
                {
                    T_Model.RemovePrecondition(state);
                }));
            };
            T_Model.onPreconditionRemoved += v =>
            {

            };

            T_Model.onEffectAdded += v =>
            {
                effectFoldout.Add(CreateToggle(v, (ele, state) =>
                {
                    T_Model.RemoveEffect(state);
                }));
            };
            T_Model.onEffectRemoved += v =>
            {

            };
        }

        VisualElement CreateToggle(GOAPState _state, Action<VisualElement, GOAPState> _onBtnRemoveClicked)
        {
            VisualElement box = new VisualElement();
            box.style.flexDirection = FlexDirection.Row;

            Toggle toggle = new Toggle();
            toggle.value = _state.Value;
            toggle.RegisterValueChangedCallback(evt =>
            {
                _state.Value = evt.newValue;
            });
            toggle.style.width = 20;
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
                    _onBtnRemoveClicked?.Invoke(box, _state);
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
    }
}

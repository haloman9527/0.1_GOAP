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

        Dictionary<GOAPState, VisualElement> ConditionElements = new Dictionary<GOAPState, VisualElement>();
        Dictionary<GOAPState, VisualElement> EffectElements = new Dictionary<GOAPState, VisualElement>();

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

            nameField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            nameField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (!typeof(GOAPAction).IsAssignableFrom(T_Model.GetType())) return;

            for (int i = 0; i < T_Model.Preconditions.Count; i++)
            {
                VisualElement v = CreateToggle(T_Model.Preconditions[i], (ele, state) =>
                {
                    T_Model.RemovePrecondition(state);
                    ele.RemoveFromHierarchy();
                });
                conditionFoldout.Add(v);
                ConditionElements[T_Model.Preconditions[i]] = v;
            }

            for (int i = 0; i < T_Model.Effects.Count; i++)
            {
                VisualElement v = CreateToggle(T_Model.Effects[i], (ele, state) =>
                {
                    T_Model.RemoveEffect(state);
                    ele.RemoveFromHierarchy();
                });
                effectFoldout.Add(v);
                EffectElements[T_Model.Effects[i]] = v;
            }
        }

        void OnNameChanged(string _newName)
        {
            Model.Title = T_Model.Name + $"({rawTitle})";
            nameField.SetValueWithoutNotify(_newName);
        }

        void OnCostChanged(float _newCost)
        {
            costField.SetValueWithoutNotify(_newCost);
        }

        void OnPreconditionAdded(GOAPState _newCondition)
        {
            VisualElement v = CreateToggle(_newCondition, (ele, state) =>
            {
                T_Model.RemovePrecondition(state);
            });
            conditionFoldout.Add(v);
            ConditionElements[_newCondition] = v;
        }

        void OnPreconditionRemoved(GOAPState _condition)
        {
            if (ConditionElements.TryGetValue(_condition, out VisualElement e))
            {
                conditionFoldout.Remove(e);
                ConditionElements.Remove(_condition);
            }
        }

        void OnEffectAdded(GOAPState _newEffect)
        {
            VisualElement v = CreateToggle(_newEffect, (ele, state) =>
            {
                T_Model.RemoveEffect(state);
            });
            effectFoldout.Add(v);
            EffectElements[_newEffect] = v;
        }

        void OnEffectRemoved(GOAPState _newEffect)
        {
            if (EffectElements.TryGetValue(_newEffect, out VisualElement e))
            {
                effectFoldout.Remove(e);
                EffectElements.Remove(_newEffect);
            }
        }

        protected override void BindingProperties()
        {
            base.BindingProperties();
            rawTitle = T_Model.Title;

            nameField.SetValueWithoutNotify(T_Model.Name);
            costField.SetValueWithoutNotify(T_Model.Cost);
            Model.Title = T_Model.Name + $"({rawTitle})";

            nameField.RegisterValueChangedCallback(evt =>
            {
                T_Model.Name = evt.newValue;
            });

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

            T_Model.BindingProperty<string>(nameof(T_Model.Name), OnNameChanged);
            T_Model.BindingProperty<float>(nameof(T_Model.Cost), OnCostChanged);

            T_Model.onPreconditionAdded += OnPreconditionAdded;
            T_Model.onPreconditionRemoved += OnPreconditionRemoved;

            T_Model.onEffectAdded += OnEffectAdded;
            T_Model.onEffectRemoved += OnEffectRemoved;
        }

        public override void UnBindingProperties()
        {
            base.UnBindingProperties();

            T_Model.UnBindingProperty<string>(nameof(T_Model.Name), OnNameChanged);
            T_Model.UnBindingProperty<float>(nameof(T_Model.Cost), OnCostChanged);

            T_Model.onPreconditionAdded -= OnPreconditionAdded;
            T_Model.onPreconditionRemoved -= OnPreconditionRemoved;

            T_Model.onEffectAdded -= OnEffectAdded;
            T_Model.onEffectRemoved -= OnEffectRemoved;
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
            box.userData = _state;
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

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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using CZToolKit;
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
    public class GOAPNodeView : BaseSimpleNodeView<GOAPActionVM>
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
            contentContainer.Add(nameField);

            costField = new FloatField();
            contentContainer.Add(costField);

            conditionFoldout = new Foldout() { text = "条件" };
            conditionFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            contentContainer.Add(conditionFoldout);

            btnAddCondition = new Button();
            btnAddCondition.text = "添加条件";
            btnAddCondition.style.alignItems = Align.Center;
            conditionFoldout.Add(btnAddCondition);

            effectFoldout = new Foldout() { text = "可以造成的效果" };
            effectFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            contentContainer.Add(effectFoldout);

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
            if (!typeof(GOAPAction).IsAssignableFrom(T_ViewModel.GetType())) return;

            rawTitle = T_ViewModel.Title;

            title = T_ViewModel.Name + $"({rawTitle})";
            nameField.SetValueWithoutNotify(T_ViewModel.Name);
            costField.SetValueWithoutNotify(T_ViewModel.Cost);

            for (int i = 0; i < T_ViewModel.Preconditions.Count; i++)
            {
                VisualElement v = CreateToggle(T_ViewModel.Preconditions[i], (ele, state) =>
                {
                    T_ViewModel.RemovePrecondition(state);
                    ele.RemoveFromHierarchy();
                });
                conditionFoldout.Add(v);
                ConditionElements[T_ViewModel.Preconditions[i]] = v;
            }

            for (int i = 0; i < T_ViewModel.Effects.Count; i++)
            {
                VisualElement v = CreateToggle(T_ViewModel.Effects[i], (ele, state) =>
                {
                    T_ViewModel.RemoveEffect(state);
                    ele.RemoveFromHierarchy();
                });
                effectFoldout.Add(v);
                EffectElements[T_ViewModel.Effects[i]] = v;
            }
        }

        void OnNameChanged(string oldName, string newName)
        {
            title = T_ViewModel.Name + $"({rawTitle})";
            nameField.SetValueWithoutNotify(newName);
        }

        void OnCostChanged(float oldCost, float newCost)
        {
            costField.SetValueWithoutNotify(newCost);
        }

        void OnPreconditionAdded(GOAPState _newCondition)
        {
            VisualElement v = CreateToggle(_newCondition, (ele, state) =>
            {
                T_ViewModel.RemovePrecondition(state);
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
                T_ViewModel.RemoveEffect(state);
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

        protected override void OnBindingProperties()
        {
            base.OnBindingProperties();

            nameField.RegisterValueChangedCallback(evt =>
            {
                T_ViewModel.Name = evt.newValue;
            });

            costField.RegisterValueChangedCallback(evt =>
            {
                T_ViewModel.Cost = evt.newValue;
            });

            btnAddCondition.clicked += () =>
            {
                T_ViewModel.AddPrecondition(new GOAPState());
                Owner.SetDirty();
            };
            btnAddEffect.clicked += () =>
            {
                T_ViewModel.AddEffect(new GOAPState());
                Owner.SetDirty();
            };

            T_ViewModel.BindingProperty<string>(nameof(T_ViewModel.Name), OnNameChanged);
            T_ViewModel.BindingProperty<float>(nameof(T_ViewModel.Cost), OnCostChanged);

            T_ViewModel.onPreconditionAdded += OnPreconditionAdded;
            T_ViewModel.onPreconditionRemoved += OnPreconditionRemoved;

            T_ViewModel.onEffectAdded += OnEffectAdded;
            T_ViewModel.onEffectRemoved += OnEffectRemoved;
        }

        protected override void OnUnBindingProperties()
        {
            base.OnUnBindingProperties();

            T_ViewModel.UnBindingProperty<string>(nameof(T_ViewModel.Name), OnNameChanged);
            T_ViewModel.UnBindingProperty<float>(nameof(T_ViewModel.Cost), OnCostChanged);

            T_ViewModel.onPreconditionAdded -= OnPreconditionAdded;
            T_ViewModel.onPreconditionRemoved -= OnPreconditionRemoved;

            T_ViewModel.onEffectAdded -= OnEffectAdded;
            T_ViewModel.onEffectRemoved -= OnEffectRemoved;
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

using UnityEngine;
using CZToolKit.Core.Editors;
using UnityEditorInternal;
using UnityEditor;

namespace CZToolKit.GOAP.Editors
{
    [CustomEditor(typeof(GOAPAction), true)]
    public class ActionEditor : BasicEditor
    {
        public static bool preconditionsFoldout, effectsFoldout;

        GOAP.GOAPAction action;
        SerializedProperty preconditionsProperty, effectsProperty;
        ReorderableList preconditionsReorderableList, effectsReorderableList;

        protected override void OnEnable()
        {
            base.OnEnable();
            action = target as GOAPAction;

            preconditionsProperty = serializedObject.FindProperty("m_Preconditions");
            preconditionsReorderableList = new ReorderableList(serializedObject, preconditionsProperty, true, false, true, true);
            preconditionsReorderableList.headerHeight = 3;
            preconditionsReorderableList.elementHeight = 20;
            preconditionsReorderableList.drawElementCallback += (a, b, c, d) => DrawElement(a, b, c, d, preconditionsProperty);

            effectsProperty = serializedObject.FindProperty("m_Effects");
            effectsReorderableList = new ReorderableList(serializedObject, effectsProperty, true, false, true, true);
            effectsReorderableList.headerHeight = 3;
            effectsReorderableList.elementHeight = 20;
            effectsReorderableList.drawElementCallback += (a, b, c, d) => DrawElement(a, b, c, d, effectsProperty);
        }

        protected override void RegisterDrawers()
        {
            base.RegisterDrawers();

            RegisterDrawer("m_Preconditions", property =>
            {
                preconditionsFoldout = EditorGUILayout.Foldout(preconditionsFoldout, "Preconditions", true);
                if (preconditionsFoldout)
                    preconditionsReorderableList.DoLayoutList();
            });
            RegisterDrawer("m_Effects", property =>
            {
                effectsFoldout = EditorGUILayout.Foldout(effectsFoldout, "Effects", true);
                if (effectsFoldout)
                    effectsReorderableList.DoLayoutList();
            });
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty property)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element);
        }

        private void DrawHeader(Rect rect, SerializedProperty property)
        {
            GUI.Label(rect, new GUIContent(property.displayName));
        }
    }
}

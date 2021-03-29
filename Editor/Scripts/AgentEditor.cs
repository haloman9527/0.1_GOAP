using CZToolKit.Core.Editors;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CZToolKit.GOAP.Editors
{
    [CustomEditor(typeof(Agent))]
    public class AgentEditor : BasicEditor
    {
        public static bool goalsFoldout, preStatesFoldout;

        private Agent agent;
        SerializedProperty goalsProperty, preStateProperty;
        ReorderableList goalsReorderableList, preStateReorderableList;

        protected override void OnEnable()
        {
            base.OnEnable();
            agent = target as Agent;

            goalsProperty = serializedObject.FindProperty("goals");
            goalsReorderableList = new ReorderableList(serializedObject, goalsProperty, true, false, true, true);
            goalsReorderableList.headerHeight = 3;
            goalsReorderableList.elementHeight = 20;
            goalsReorderableList.drawElementCallback += (a, b, c, d) => DrawElement(a, b, c, d, goalsProperty);

            preStateProperty = serializedObject.FindProperty("preState");
            preStateReorderableList = new ReorderableList(serializedObject, preStateProperty, true, false, true, true);
            preStateReorderableList.headerHeight = 3;
            preStateReorderableList.elementHeight = 20;
            preStateReorderableList.drawElementCallback += (a, b, c, d) => DrawElement(a, b, c, d, preStateProperty);
        }

        protected override void RegisterDrawers()
        {
            base.RegisterDrawers();

            RegisterDrawer("goals", property =>
            {
                goalsFoldout = EditorGUILayout.Foldout(goalsFoldout, "Goals", true);
                if (goalsFoldout)
                    goalsReorderableList.DoLayoutList();
            });
            RegisterDrawer("preState", property =>
            {
                preStatesFoldout = EditorGUILayout.Foldout(preStatesFoldout, "PreStates", true);
                if (preStatesFoldout)
                    preStateReorderableList.DoLayoutList();
            });
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(20);
            if (GUILayout.Button("Open Debuger Window"))
                AgentEditorWindow.Open();
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty property)
        {
            EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index));
        }
    }
}

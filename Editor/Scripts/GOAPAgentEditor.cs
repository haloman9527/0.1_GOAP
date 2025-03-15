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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion

using Atom.UnityEditors;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Atom.GOAP.Editors
{
    [CustomEditor(typeof(GOAPAgent))]
    public class GOAPAgentEditor : BaseEditor
    {
        GOAPAgent agent;
        SerializedProperty goalsProperty, preStateProperty;
        ReorderableList goalsReorderableList, preStateReorderableList;
        
        void OnEnable()
        {
            agent = target as GOAPAgent;

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

        protected override void OnPropertyGUI(SerializedProperty property)
        {
            switch (property.propertyPath)
            {
                case "goals":
                {
                    if (EditorGUILayoutExtension.DrawFoldout(agent.GetHashCode(), GUIHelper.TextContent("Goals")))
                        goalsReorderableList.DoLayoutList();
                    break;
                }
                case "preState":
                {
                    if (EditorGUILayoutExtension.DrawFoldout(agent.GetHashCode(), GUIHelper.TextContent("PreStates")))
                        preStateReorderableList.DoLayoutList();
                    break;
                }
                default:
                {
                    base.OnPropertyGUI(property);
                    break;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(20);
            //if (GUILayout.Button("Open Debuger Window"))
            //    AgentEditorWindow.Open();
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty property)
        {
            EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index));
        }
    }
}

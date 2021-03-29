using UnityEditor;
using UnityEngine;
using Goal = CZToolKit.GOAP.Goal;

namespace CZToolKit.GOAP.Editors
{
    [CustomPropertyDrawer(typeof(Goal))]
    public class GoalDrawer : PropertyDrawer
    {
        public float height = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var key = property.FindPropertyRelative("Key");
            var value = property.FindPropertyRelative("Value");
            var once = property.FindPropertyRelative("Once");
            var priority = property.FindPropertyRelative("Priority");


            EditorGUI.indentLevel++;
            GUI.Box(position, "");

            float width = position.width;

            position.width = 30;
            value.boolValue = EditorGUI.Toggle(position, "", value.boolValue);

            position.x += position.width;
            position.width = width - 200;
            key.stringValue = EditorGUI.TextField(position, key.stringValue);

            position.x += position.width + 5;
            position.width = 50;
            once.boolValue = GUI.Toggle(position, once.boolValue, new GUIContent("Once", "是否是一次性目标，若是一次性，则在完成目标后移除该目标"), "ButtonMid");

            position.x += position.width - 10;
            position.width = 120;
            float lableWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 75;
            priority.floatValue = EditorGUI.FloatField(position, new GUIContent("Priority", "优先级，越高越先执行"), priority.floatValue);
            EditorGUIUtility.labelWidth = lableWidth;

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return 20; }
    }
}

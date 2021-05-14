#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using System;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.GOAP.Editors
{
    [InitializeOnLoad]
    public class HierarchyIcon : ScriptableObject
    {
        private static Texture2D icon = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/GOAP/GOAP_Hierarchy_Icon.png", typeof(Texture2D)) as Texture2D;

        static HierarchyIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI =
                (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, new EditorApplication.HierarchyWindowItemCallback(HierarchyIcon.HierarchyWindowItemOnGUI));
        }

        static void HierarchyWindowItemOnGUI(int _id, Rect _rect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(_id) as GameObject;
            if (gameObject == null) return;
            GOAPAgent agent = gameObject.GetComponent<GOAPAgent>();
            if (agent == null) return;
            _rect.x = _rect.width + (_rect.x - 16f);
            _rect.width = _rect.height = 16;
            GUI.DrawTexture(_rect, icon);
        }
    }
}

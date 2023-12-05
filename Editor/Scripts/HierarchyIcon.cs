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
using System;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.GOAP.Editors
{
    [InitializeOnLoad]
    public class HierarchyIcon : ScriptableObject
    {
        static Texture2D icon;

        static HierarchyIcon()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/GOAP/GOAP_Hierarchy_Icon.png");
            EditorApplication.hierarchyWindowItemOnGUI =
                (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, new EditorApplication.HierarchyWindowItemCallback(HierarchyIcon.HierarchyWindowItemOnGUI));
        }

        static void HierarchyWindowItemOnGUI(int _id, Rect _rect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(_id) as GameObject;
            if (gameObject == null) return;
            GOAPAgent agent = gameObject.GetComponent<GOAPAgent>();
            if (agent == null) return;
            _rect.x = _rect.width + (_rect.x - 15f);
            _rect.width = _rect.height = 15;
            GUI.DrawTexture(_rect, icon);
        }
    }
}

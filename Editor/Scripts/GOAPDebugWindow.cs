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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using CZToolKit;
using CZToolKitEditor;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.GOAP.Editors
{
    public class GOAPDebugWindow : BaseEditorWindow
    {

        [MenuItem("Tools/CZToolKit/GraphGOAP", priority = 0)]
        public static void Open()
        {
            GOAPDebugWindow w = GetWindow<GOAPDebugWindow>("Agent Debuger");
            w.position = new Rect(200, 200, 500, 700);
        }

        static GOAPDebugWindow instance;

        public static GOAPDebugWindow Instance
        {
            get { return instance; }
            protected set { instance = value; }
        }

        string[] toolbarTabTexts = new string[] { "Overview", "Setting", "Realtime" };
        private int selectedTabIndex = 0;

        GUIStyle acBoldHeader;
        GUIStyle ACBoldHeader
        {
            get
            {
                if (acBoldHeader == null)
                {
                    acBoldHeader = new GUIStyle("dockareaStandalone");
                    acBoldHeader.alignment = TextAnchor.MiddleLeft;
                    acBoldHeader.fontSize = 20;
                    acBoldHeader.fixedHeight = 30;
                }
                return acBoldHeader;
            }
        }

        GOAPAgent agent;

        void OnEnable()
        {
            instance = this;
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            foreach (GameObject gameOjbect in Selection.gameObjects)
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(gameOjbect))
                {
                    GOAPAgent tempAgent = gameOjbect.GetComponentInChildren<GOAPAgent>();
                    if (tempAgent == null)
                        tempAgent = gameOjbect.GetComponentInParent<GOAPAgent>();
                    if (tempAgent != null)
                        agent = tempAgent;
                }
            }
        }

        private void OnGUI()
        {
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, toolbarTabTexts, GUILayout.Height(30));

            Rect r = GUILayoutUtility.GetLastRect();
            GUILayout.BeginArea(new Rect(r.x, 30, position.width, position.height));
            GUILayout.Space(10);
            switch (selectedTabIndex)
            {
                case 0:
                    //DrawOverview();
                    break;
                case 1:

                    break;
                case 2:
                    DrawRuntime();
                    break;
            }
            GUILayout.EndArea();
            Repaint();
        }

        private void DrawOverview()
        {

        }

        private void DrawRuntime()
        {
            if (EditorApplication.isPlaying)
            {
                if (agent == null)
                    GUILayout.Label("选择一个Agent", ACBoldHeader, GUILayout.ExpandWidth(true));
                else
                {
                    EditorGUILayout.ObjectField(agent, typeof(GOAPAgent), true);
                    GUILayout.Label("Goal(当前目的)", ACBoldHeader, GUILayout.ExpandWidth(true));
                    GUILayout.BeginHorizontal();
                    if (agent.HasGoal)
                    {
                        GUILayout.Label(agent.CurrentGoal.Key);
                        GUILayout.Toggle(agent.CurrentGoal.Value, "");
                    }
                    else
                        GUILayout.Label("Haven't Goal");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);
                    GUILayout.Label("Plan(当前计划)", ACBoldHeader, GUILayout.ExpandWidth(true));
                    GUILayout.BeginHorizontal();
                    if (agent.HasPlan)
                    {
                        foreach (var action in agent.StoredActionQueue)
                        {
                            if (agent.CurrentAction == action)
                                GUILayout.Label($"{action.Name}-->", "BoldLabel");
                            else
                                GUILayout.Label($"{action.Name}-->");
                        }
                    }
                    else
                        GUILayout.Label("No Plan");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);
                    GUILayout.Label("Actions(Agent拥有的行为)", ACBoldHeader, GUILayout.ExpandWidth(true));
                    float f = position.width / 4;
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(f);
                    GUILayout.Label("Runnings", (GUIStyle)"dockareaStandalone", GUILayout.Width(f));
                    GUILayout.Label("Precondtions", (GUIStyle)"dockareaStandalone", GUILayout.Width(f));
                    GUILayout.Label("Cost", (GUIStyle)"dockareaStandalone", GUILayout.Width(f));
                    GUILayout.EndHorizontal();
                    for (int i = 0; i < agent.T_Graph.AvailableActions.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(agent.T_Graph.AvailableActions[i].GetType().Name, (GUIStyle)"dockareaStandalone", GUILayout.Width(f));
                        GUILayout.Toggle(agent.CurrentAction == agent.T_Graph.AvailableActions[i], "", GUILayout.Width(f));
                        GUILayout.Toggle(agent.CurrentAction == agent.T_Graph.AvailableActions[i], "", GUILayout.Width(f));
                        GUILayout.Label(agent.T_Graph.AvailableActions[i].Cost.ToString(), (GUIStyle)"dockareaStandalone", GUILayout.Width(f));
                        GUILayout.EndHorizontal();
                    }


                    GUILayout.Space(20);
                    GUILayout.Label("World State(Agent状态)", ACBoldHeader, GUILayout.ExpandWidth(true));
                    int count = 0;
                    GUILayout.BeginHorizontal();
                    foreach (var item in agent.States)
                    {
                        GUILayout.Toggle(item.Value, item.Key, GUILayout.Width(f));
                        count++;
                        if ((count + 1) % 4 == 0)
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("此页面功能需要在运行时使用", ACBoldHeader, GUILayout.ExpandWidth(true));
            }
        }
    }
}
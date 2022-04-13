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
using CZToolKit.GraphProcessor;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.GOAP
{
    [CreateAssetMenu(menuName = "CZToolKit/GOAP/New GOAPGraph")]
    public class GOAPGraphAsset : ScriptableObject, IGraphAsset, IGraphAsset<GOAPGraph>
    {
        [HideInInspector]
        [SerializeField]
        byte[] serializedGraph;
        [HideInInspector]
        [SerializeField]
        List<UnityObject> graphUnityReferences = new List<UnityObject>();

        public Type GraphType => typeof(GOAPGraph);

        public void SaveGraph(IGraph graph)
        {
            serializedGraph = SerializationUtility.SerializeValue(graph, DataFormat.JSON, out graphUnityReferences);
        }

        public BaseGraph DeserializeGraph()
        {
            return DeserializeTGraph();
        }

        public GOAPGraph DeserializeTGraph()
        {
            GOAPGraph graph = null;
            if (serializedGraph != null && serializedGraph.Length > 0)
                graph = SerializationUtility.DeserializeValue<GOAPGraph>(serializedGraph, DataFormat.JSON, graphUnityReferences);
            if (graph == null)
                graph = new GOAPGraph();
            graph.Enable();
            return graph;
        }
    }
}
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

using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using Jiange.GraphProcessor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Jiange.GOAP
{
    [CreateAssetMenu(menuName = "Jiange/GOAP/New GOAPGraph")]
    public class GOAPGraphAsset : ScriptableObject, IGraphAsset, IGraphAsset<GOAPGraph>
    {
        [HideInInspector]
        [SerializeField]
        byte[] serializedGraph;
        [HideInInspector]
        [SerializeField]
        List<UnityObject> graphUnityReferences = new List<UnityObject>();

        public UnityObject UnityAsset => this;

        public Type GraphType => typeof(GOAPGraph);

        public void SaveGraph(BaseGraph graph)
        {
            serializedGraph = SerializationUtility.SerializeValue(graph as GOAPGraph, DataFormat.JSON, out graphUnityReferences);
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
            return graph;
        }
    }
}
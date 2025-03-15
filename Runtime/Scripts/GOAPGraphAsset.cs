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

using System;
using Atom.GraphProcessor;
using UnityEngine;

namespace Atom.GOAP
{
    [CreateAssetMenu(menuName = "Moyo/GOAP/New GOAPGraph")]
    public class GOAPGraphAsset : ScriptableObject, IGraphAsset
    {
        [SerializeField]
        private GOAPGraph data;
        
        public Type GraphType => typeof(GOAPGraph);

        public void SaveGraph(BaseGraph graph) => this.data = (GOAPGraph)graph;

        public BaseGraph LoadGraph() => data;
    }
}
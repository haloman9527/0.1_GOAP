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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.GOAP
{
    public interface IGOAPActionViewModel
    {
        string Name { get; }
        float Cost { get; }
        IReadOnlyList<GOAPState> Preconditions { get; }
        IReadOnlyList<GOAPState> Effects { get; }

        bool IsUsable();
        void DynamicallyEvaluateCost();
        bool IsProceduralPrecondition(Dictionary<string, bool> currentState);
        void OnPrePerform();
        GOAPActionStatus OnPerform();
        void OnPostPerform(bool _successed);    }
}
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
using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    public interface IGOAPAction
    {
        string Name { get; }
        float Cost { get; }
        IReadOnlyList<GOAPState> Preconditions { get; }
        IReadOnlyList<GOAPState> Effects { get; }

        bool Initialize();
        bool IsUsable();
        void DynamicallyEvaluateCost();
        bool IsProceduralPrecondition(Dictionary<string, bool> currentState);
        void OnPrePerform();
        GOAPActionStatus OnPerform();
        void OnPostPerform(bool _successed);
    }
}
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
using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    public interface IGOAP
    {
        /// <summary> 没有找到可以完成目标的路线 </summary>  
        void PlanFailed(List<GOAPGoal> failedGoals);

        /// <summary> 一个行为完成 </summary>  
        void ActionFinished(IReadOnlyList<GOAPState> effect);

        /// <summary> 找到可以完成目标的一系列动作 </summary>  
        void PlanFound(GOAPGoal goal, Queue<GOAPAction> actions);

        /// <summary> 动作全部完成，达成目标 </summary>
        void PlanFinished();

        /// <summary> 计划被一个动作打断 </summary>
        void PlanAborted(GOAPAction abortAction);
    }
}

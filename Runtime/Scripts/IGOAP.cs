using System.Collections.Generic;

namespace CZToolKit.GOAP
{
    public interface IGOAP
    {
        /// <summary> 没有找到可以完成目标的路线 </summary>  
        void PlanFailed(List<Goal> failedGoals);

        /// <summary> 一个行为完成 </summary>  
        void ActionFinished(List<State> effect);

        /// <summary> 找到可以完成目标的一系列动作 </summary>  
        void PlanFound(Goal goal, Queue<GOAPAction> actions);

        /// <summary> 动作全部完成，达成目标 </summary>
        void PlanFinished();

        /// <summary> 计划被一个动作打断 </summary>
        void PlanAborted(GOAPAction abortAction);
    }
}

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
using UnityEngine;

namespace Moyo.GOAP.Actions.Movement
{
    public interface IMovementAction
    {
        /// <summary> Set a new pathfinding destination. </summary>
        /// <param name="destination">The destination to set.</param>
        /// <returns>True if the destination is valid.</returns>
        bool SetDestination(Vector3 destination);

        /// <summary> Specifies if the rotation should be updated. </summary>
        /// <param name="update">Should the rotation be updated?</param>
        void UpdateRotation(bool update);

        /// <summary> Does the agent have a pathfinding path? </summary>
        /// <returns>True if the agent has a pathfinding path.</returns>
        bool HasPath();

        /// <summary> Returns the velocity of the agent. </summary>
        /// <returns>The velocity of the agent.</returns>
        Vector3 Velocity();

        /// <summary> Has the agent arrived at the destination? </summary>
        /// <returns>True if the agent has arrived at the destination.</returns>
        bool HasArrived();

        /// <summary> Stop pathfinding. </summary>
        void Stop();
    }
}
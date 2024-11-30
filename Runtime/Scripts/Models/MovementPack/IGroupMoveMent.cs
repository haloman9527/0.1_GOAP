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
    public interface IGroupMovement
    {
        bool SetDestination(int index, Vector3 target);

        Vector3 Velocity(int index);
    }
}
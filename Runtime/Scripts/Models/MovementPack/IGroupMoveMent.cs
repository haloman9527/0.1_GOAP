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
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    public interface IGroupMovement
    {
        bool SetDestination(int index, Vector3 target);

        Vector3 Velocity(int index);
    }
}
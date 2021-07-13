#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using UnityEngine;

namespace CZToolKit.GOAP.Actions.Movement
{
    public abstract class GroupMovement : GOAPAction
    {
        protected abstract bool SetDestination(int index, Vector3 target);

        protected abstract Vector3 Velocity(int index);
    }
}
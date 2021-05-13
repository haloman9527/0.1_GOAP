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
using CZToolKit.GraphProcessor;
using System;

namespace CZToolKit.GOAP
{
    public abstract class GOAPActionEvtNode : BaseNode
    {
        [Vertical, Port(PortDirection.Output, IsMulti = true, TypeConstraint = PortTypeConstraint.Inherited)]
        [NonSerialized]
        GOAPAction action;
    }
}

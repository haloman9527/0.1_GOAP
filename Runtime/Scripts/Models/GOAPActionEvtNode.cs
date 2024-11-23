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
using Jiange.GraphProcessor;

namespace Jiange.GOAP
{
    public abstract class GOAPActionEvtNode : BaseNode
    {
        [NonSerialized]
        GOAPAction action;
    }
}

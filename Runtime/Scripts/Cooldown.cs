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
using System.Collections;
using UnityEngine;

namespace CZToolKit.GOAP
{
    public class Cooldown
    {
        public bool Active { get; private set; }
        public Coroutine Run(float _duration)
        {
            if (Active)
                return null;
            return CoroutineService.Instance.StartCoroutine(Routine(_duration));
        }

        IEnumerator Routine(float _duration)
        {
            Active = true;
            float startTime = Time.time;
            while (Time.time - startTime < _duration)
                yield return null;
            Active = false;
        }
    }
}
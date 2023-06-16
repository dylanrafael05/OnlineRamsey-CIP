using System;
using System.Collections;
using UnityEngine;

namespace Ramsey.Utilities
{
    public static class Coroutines 
    {
        public static Coroutine Start(IEnumerator coro) 
            => CoroRunner.Instance.StartCoroutine(coro);
        public static Coroutine Start(Func<IEnumerator> coro) 
            => CoroRunner.Instance.StartCoroutine(coro());
        public static void Kill(IEnumerator coro) 
        {
            if(coro is not null)
                CoroRunner.Instance.StopCoroutine(coro);
        }
        public static void Kill(Coroutine coro) 
        {
            if(coro is not null)
                CoroRunner.Instance.StopCoroutine(coro);
        }
        public static void Kill(Func<IEnumerator> coro) 
        {
            var c = coro();
            if(c is not null)
                CoroRunner.Instance.StopCoroutine(c);
        }

        public static void KillAll()
        {
            CoroRunner.Instance.StopAllCoroutines();
        }
    }
}
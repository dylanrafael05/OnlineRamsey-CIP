using System;
using System.Collections;
using UnityEngine;

namespace Ramsey.Utilities
{
    public static class Coroutines 
    {
        public static Coroutine StartCoroutine(IEnumerator coro) 
            => CoroRunner.Instance.StartCoroutine(coro);
        public static Coroutine StartCoroutine(Func<IEnumerator> coro) 
            => CoroRunner.Instance.StartCoroutine(coro());
        public static void KillCoroutine(IEnumerator coro) 
        {
            if(coro is not null)
                CoroRunner.Instance.StopCoroutine(coro);
        }
        public static void KillCoroutine(Coroutine coro) 
        {
            if(coro is not null)
                CoroRunner.Instance.StopCoroutine(coro);
        }
        public static void KillCoroutine(Func<IEnumerator> coro) 
        {
            var c = coro();
            if(c is not null)
                CoroRunner.Instance.StopCoroutine(c);
        }
    }
}
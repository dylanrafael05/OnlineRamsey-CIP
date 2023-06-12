using System;
using System.Collections;

namespace Ramsey.Utilities
{
    public static class Coroutines 
    {
        public static void StartCoroutine(IEnumerator coro) 
            => CoroRunner.Instance.StartCoroutine(coro);
        public static void StartCoroutine(Func<IEnumerator> coro) 
            => CoroRunner.Instance.StartCoroutine(coro());
        public static void KillCoroutine(IEnumerator coro) 
            => CoroRunner.Instance.StopCoroutine(coro);
        public static void KillCoroutine(Func<IEnumerator> coro) 
            => CoroRunner.Instance.StopCoroutine(coro());
    }
}
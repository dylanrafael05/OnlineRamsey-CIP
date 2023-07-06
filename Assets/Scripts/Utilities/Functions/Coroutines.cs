using System;
using System.Collections;
using UnityEngine;

namespace Ramsey.Utilities
{
    /// <summary>
    /// Statically permits the calling of coroutines.
    /// </summary>
    public static class Coroutines 
    {
        /// <inheritdoc cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>
        public static Coroutine Start(IEnumerator coro) 
            => CoroRunner.Instance.StartCoroutine(coro);
        /// <inheritdoc cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>
        public static Coroutine Start(Func<IEnumerator> coro) 
            => CoroRunner.Instance.StartCoroutine(coro());
            
        /// <inheritdoc cref="MonoBehaviour.StopCoroutine(IEnumerator)"/>
        public static void Kill(IEnumerator coro) 
        {
            if(coro is not null)
                CoroRunner.Instance.StopCoroutine(coro);
        }
        /// <inheritdoc cref="MonoBehaviour.StopCoroutine(Coroutine)"/>
        public static void Kill(Coroutine coro) 
        {
            if(coro is not null)
                CoroRunner.Instance.StopCoroutine(coro);
        }

        /// <inheritdoc cref="MonoBehaviour.StopAllCoroutines"/>
        public static void KillAll()
        {
            CoroRunner.Instance.StopAllCoroutines();
        }
    }
}
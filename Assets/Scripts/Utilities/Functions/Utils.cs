using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;

namespace Ramsey.Utilities
{
    public static class Utils
    {
        public static void Toggle(ref bool value)
            => value = !value;

        /// <summary>
        /// Get a random element in a thread safe manner 
        /// from the given list.
        /// </summary>
        public static T RandomElement<T>(this IReadOnlyList<T> L)
            => L[UnityEngine.Random.Range(0, L.Count)];

        /// <summary>
        /// Parse a color from a hexadecimal string into a color object.
        /// </summary>
        public static Color ColorFromHex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex[0] == '#' ? hex : "#"+hex, out var c);
            return c;
        }

        /// <summary>
        /// Enumerate the given values alongside their index.
        /// </summary>
        public static IEnumerable<(T value, int index)> Indexed<T>(this IEnumerable<T> e) 
            => e.Select((v, i) => (v, i));

        /// <summary>
        /// A condensed version of . . .
        /// <code>
        /// for (int i = 0; i &lt; length; i++) 
        ///     action(i);
        /// </code>
        /// </summary>
        public static void ForLength(int length, Action<int> action)
        { for (int i = 0; i < length; i++) action(i); }

        /// <summary>
        /// Swap the contents of two references.
        /// </summary>
        public static void Swap<T>(ref T a, ref T b)
        {
            (b, a) = (a, b);
        }

        /// <summary>
        /// Get the world matrix of a RectTransform.
        /// </summary>
        public static Matrix4x4 WorldMatrix(this RectTransform transform) 
        {
            var corners = new Vector3[4];

            transform.GetWorldCorners(corners);

            return Matrix4x4.TRS(
                corners.Aggregate((a, b) => a + b) / 4,
                transform.rotation,
                new(
                    (corners.Select(c => c.x).Max() - corners.Select(c => c.x).Min()) / 2, 
                    (corners.Select(c => c.y).Max() - corners.Select(c => c.y).Min()) / 2, 
                    1
                )
            );
        }

        public static int ToDecimal(this IEnumerable<int> num, int previousBase)
            => num.Select((n, i) => n * (int) pow(previousBase, i)).Sum();

        /// <summary>
        /// A condensed version of . . .
        /// <code>
        /// Debug.Log(string.Join("\n", items.Select(e => e.ToString())));
        /// </code>
        /// </summary>
        public static void Print<T>(this IEnumerable<T> E)
        { Debug.Log(string.Join("\n", E.Select(e => e.ToString()))); }

        /// <summary>
        /// A condensed version of . . .
        /// <code>
        /// for (int i = 0; i &lt; items.Count; i++)
        ///     items[i] = value;
        /// </code>
        /// </summary>
        public static void Fill<T>(this IList<T> E, T e)
        {
            for (int i = 0; i < E.Count; i++)
                E[i] = e;
        }

        public static void ForEachIndex<T>(this IEnumerable<T> e, Action<T, int> action)
        {
            int i = 0;
            foreach(T elem in e)
            {
                action(elem, i);
                i++;
            }    
        }

        /// <summary>
        /// Run code conditionally synchronously.
        /// </summary>
        public static async UniTask RunOnThreadPool(bool synchronous, Action action)
        {
            if(synchronous) action();
            else await UniTask.RunOnThreadPool(action);
        }
        /// <summary>
        /// Run code conditionally synchronously.
        /// </summary>
        public static async UniTask<T> RunOnThreadPool<T>(bool synchronous, Func<T> func)
        {
            if(synchronous) return func();
            else return await UniTask.RunOnThreadPool(func);
        }

        /// <summary>
        /// Ensure that the given UniTask is synchronous (i.e. already completed) if necessary.
        /// </summary>
        public static UniTask AssertSync(this UniTask t, bool synchronous)
        {
            if(synchronous && !t.Status.IsCompleted())
            {
                Debug.LogError("UniTask asserted to be synchronous was not!");
            }

            return t;
        }
        /// <summary>
        /// Ensure that the given UniTask is synchronous (i.e. already completed) if necessary.
        /// </summary>
        public static UniTask<T> AssertSync<T>(this UniTask<T> t, bool synchronous)
        {
            if(synchronous && !t.Status.IsCompleted())
            {
                Debug.LogError("UniTask asserted to be synchronous was not!");
            }

            return t;
        }

        /// <summary>
        /// Convert a boolean to an integer with true being '1' and false being '0'.
        /// </summary>
        public static int ToInt(this bool b)
            => b ? 1 : 0;

        /// <summary>
        /// Copy the contents of one list into another
        /// </summary>
        public static List<T> Copy<T>(this List<T> list)
        {
            List<T> rList = new();
            list.ForEach(e => rList.Add(e));
            return rList;
        }

        public static void Foreach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (T elem in self)
                action(elem);
        }
        public static async void ForeachAsync<T>(this IEnumerable<T> self, Func<T, UniTask> action)
        {
            foreach (T elem in self)
                await action(elem);
        }

        public static async UniTask<bool> WaitUntil(Func<bool> func, CancellationToss cancel = null, int milliDelay = 10, int timeout = -1)
        {
            var totalt = 0;

            bool IsCanceled()
            {
                return cancel != null && cancel.IsRequested;
            }

            while (!func.Invoke() && !IsCanceled() && (timeout < 0 || totalt < timeout))
            {
                await UniTask.Delay(milliDelay);
                totalt += milliDelay;
            }

            return totalt < timeout && !IsCanceled();
        }

        /// <summary>
        /// Flatten the given enumerable.
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> self)
        {
            return self.SelectMany(t => t);
        }

        /// <summary>
        /// Create a unique ID for each element in the given enumerable.
        /// </summary>
        public static Dictionary<T, int> AssignUniqueIDs<T>(this IEnumerable<T> self)
        {
            return self.Indexed().ToDictionary(k => k.value, k => k.index);
        }

        /// <summary>
        /// Convert the given boolean sequence to a bit set immediately.
        /// </summary>
        public static BitSet ToBitSet(this IEnumerable<bool> self) 
        {
            var bs = new BitSet();
            var i = 0;

            foreach(var item in self)
            {
                if(item) bs.Set(i);
                i++;
            }

            return bs;
        }
        /// <summary>
        /// Convert the given sequence to a bit set immediately, using
        /// the provided function to convert values to booleans.
        /// </summary>
        public static BitSet ToBitSet<T>(this IEnumerable<T> self, Func<T, bool> pred) 
            => self.Select(pred).ToBitSet();

        /// <summary>
        /// Get the element in the given sequence with the largest value 
        /// according to the provided function.
        /// </summary>
        public static T MaxBy<T>(this IEnumerable<T> self, Func<T, float> value)
        {
            var v = default(T);
            var f = float.NaN;

            foreach(var n in self)
            {
                var fn = value(n);
                if(float.IsNaN(f) || fn > f)
                {
                    v = n;
                    f = fn;
                }
            }

            if(float.IsNaN(f))
                throw new InvalidOperationException("Cannot get a 'max by' on an empty collection");
            
            return v;
        }
        /// <summary>
        /// Get the element in the given sequence with the largest value 
        /// according to the provided function.
        /// </summary>
        public static T MinBy<T>(this IEnumerable<T> self, Func<T, float> value)
        {
            var v = default(T);
            var f = float.NaN;

            foreach(var n in self)
            {
                var fn = value(n);
                if(float.IsNaN(f) || fn < f)
                {
                    v = n;
                    f = fn;
                }
            }

            if(float.IsNaN(f))
                throw new InvalidOperationException("Cannot get a 'max by' on an empty collection");
            
            return v;
        }

        /// <summary>
        /// Pad the list up to the given index with the default value 
        /// of its element type.
        /// </summary>
        public static void PadDefaultUpto<T>(this IList<T> t, int index)
        {
            while(t.Count <= index) 
            {
                t.Add(default);
            }
        }
        /// <summary>
        /// Pad the list up to the given index with a new value 
        /// of its element type.
        /// </summary>
        public static void PadNewUpto<T>(this IList<T> t, int index) 
            where T : new()
        {
            while(t.Count <= index) 
            {
                t.Add(new());
            }
        }
        /// <summary>
        /// Pad the list up to the given index with a value 
        /// of its element type constructed by the given function.
        /// </summary>
        public static void PadUpto<T>(this IList<T> t, int index, Func<T> cons)
        {
            while(t.Count <= index) 
            {
                t.Add(cons());
            }
        }

        /// <summary>
        /// Trick C# into believing an object is of the given type.
        /// Throws a runtime error if it is not of that type.
        /// </summary>
        public static T Pun<T>(this object obj)
            => (T)obj;
    }
}
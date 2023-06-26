using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;

namespace Ramsey.Utilities
{
    public static class Utils
    {
        public static Color ColorFromHex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex[0] == '#' ? hex : "#"+hex, out var c);
            return c;
        }

        public static IEnumerable<(T value, int index)> Indexed<T>(this IEnumerable<T> e) 
            => e.Select((v, i) => (v, i));

        public static void ForLength(int length, Action<int> action)
        { for (int i = 0; i < length; i++) action(i); }

        public static void Swap<T>(ref T a, ref T b)
        {
            T tempA = a;
            a = b;
            b = tempA;
        }

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

        public static void Print<T>(this IEnumerable<T> E)
        { foreach (T e in E) Debug.Log(e.ToString()); }

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

        public static Task UnityReport(this Task task) 
        {
            return task.ContinueWith(t => 
            {
                if(t.IsFaulted)
                {
                    Debug.LogException(t.Exception);
                }
            });
        }
        public static async Task<T> UnityReport<T>(this Task<T> task) 
        {
            try 
            {
                return await task;
            }
            catch(Exception e) 
            {
                Debug.LogException(e);
                throw;
            }
        }

        public static async Task Run(bool synchronous, Action action)
        {
            if(synchronous) action();
            else await Task.Run(action).UnityReport();
        }
        public static async Task<T> Run<T>(bool synchronous, Func<T> func)
        {
            if(synchronous) return func();
            else return await Task.Run(func).UnityReport();
        }

        public static TaskT AssertSync<TaskT>(this TaskT t, bool synchronous) where TaskT : Task
        {
            if(synchronous && !t.IsCompleted)
            {
                Debug.LogError("Task asserted to be synchronous was not!");
            }

            return t;
        }

        public static int ToInt(this bool b)
            => b ? 1 : 0;

        public static List<T> Copy<T>(this List<T> list) where T : struct
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
        public static async void ForeachAsync<T>(this IEnumerable<T> self, Func<T, Task> action)
        {
            foreach (T elem in self)
                await action(elem);
        }
        
        public static Task ForeachParallel<T>(this IEnumerable<T> self, Func<T, Task> action, int parallelCount = 20) 
        {
            async Task DoPartition(IEnumerator<T> partition)
            {
                using(partition)
                {
                    while(partition.MoveNext())
                    {
                        await action(partition.Current);
                    }
                }
            }

            return Task.WhenAll(
                Partitioner.Create(self)
                    .GetPartitions(parallelCount)
                    .AsParallel()
                    .Select(DoPartition)
            );
        }

        public static bool WaitUntil(Func<bool> func, int milliDelay = 10, int timeout = 1000)
        {
            var totalt = 0;

            while (!func.Invoke() && totalt < timeout)
            {
                Thread.Sleep(milliDelay);
                totalt += milliDelay;
            }

            return totalt < timeout;
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> self)
        {
            return self.SelectMany(t => t);
        }

        public static IEnumerable<(T item, int index)> Enumerate<T>(this IEnumerable<T> self)
        {
            return self.Select((x, i) => (x, i));
        }

        public static Dictionary<T, int> AssignUniqueIDs<T>(this IEnumerable<T> self)
        {
            return self.Enumerate().ToDictionary(k => k.item, k => k.index);
        }

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
        public static BitSet ToBitSet<T>(this IEnumerable<T> self, Func<T, bool> pred) 
            => self.Select(pred).ToBitSet();

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

        public static void PadDefaultUpto<T>(this IList<T> t, int index)
        {
            while(t.Count <= index) 
            {
                t.Add(default);
            }
        }
        public static void PadNewUpto<T>(this IList<T> t, int index) 
            where T : new()
        {
            while(t.Count <= index) 
            {
                t.Add(new());
            }
        }
        public static void PadUpto<T>(this IList<T> t, int index, Func<T> cons)
        {
            while(t.Count <= index) 
            {
                t.Add(cons());
            }
        }
    }
}
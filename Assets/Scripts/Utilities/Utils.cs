using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Ramsey.Utilities
{
    public static class Utils
    {

        public static void ForEachIndex<T>(this IEnumerable<T> e, Action<T, int> action)
        {
            int i = 0;
            foreach(T elem in e)
            {
                action(elem, i);
                i++;
            }    
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

        public static async Task WaitUntil(Func<bool> func, int milliDelay = 10)
        {
            while (!func.Invoke())
            {
                await Task.Delay(milliDelay);
            }
        }

        public static async Task WaitUntil(Func<bool> func, Action action, int milliDelay = 10)
        {
            while(!func.Invoke())
            {
                action();
                await Task.Delay(milliDelay);
            }
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
    }

    public static class MathUtils
    {
        public static float3 xyz(this float2 v) 
            => float3(v.x, v.y, 0);
        public static float4 xyzw(this float2 v)
            => float4(v.x, v.y, 0, 0);
        public static float4 xyzw(this float3 v) 
            => float4(v.x, v.y, v.z, 0);
        
        public static float2 mul(this float2 self, float2 other) 
            => float2(self.x * other.x, self.y * other.y);
        public static float2 div(this float2 self, float2 other)
            => float2(self.x / other.x, self.y / other.y);
        public static float3 mul(this float3 self, float3 other) 
            => float3(self.x * other.x, self.y * other.y, self.z * other.z);
        public static float3 div(this float3 self, float3 other)
            => float3(self.x / other.x, self.y / other.y, self.z / other.z);
        public static float4 mul(this float4 self, float4 other) 
            => float4(self.x * other.x, self.y * other.y, self.z * other.z, self.w * other.w);
        public static float4 div(this float4 self, float4 other)
            => float4(self.x / other.x, self.y / other.y, self.z / other.z, self.w / other.w);

        public static float2 rescale(this float2 xy, float2 inSize, float2 outSize)
            => xy.div(inSize).mul(outSize);
        public static float3 rescale(this float3 xy, float2 inSize, float2 outSize)
            => xy.div(float3(inSize, 1)).mul(float3(outSize, 1));
    }
}
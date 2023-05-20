using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace Ramsey.Utilities
{
    public static class Utils
    {

        public static void Foreach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (T elem in self)
                action(elem);
        }

        public static async Task WaitUntil(Func<bool> func, int milliDelay)
        {
            while (!func.Invoke())
            {
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
    }

    public static class MathUtils
    {
        public static float3 xyz(this float2 v) 
            => float3(v.x, v.y, 0);
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
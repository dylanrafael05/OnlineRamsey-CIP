using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace Ramsey.Utilities
{
    public static class MathUtils
    {
        public static readonly float TAU = 6.28318f;

        public static float amod(float v, float g)
        {
            float m = fmod(abs(v), g);
            return m + (-sign(v) * .5f + .5f) * (g - 2f * m);
        }

        public static Matrix4x4 Inverse(this Matrix4x4 m)
            => Matrix4x4.Inverse(m);

        public static float2 ToCartesian(this float2 polar)
            => polar.x * float2(cos(polar.y), sin(polar.y));
        public static float2 ToPolar(this float2 cartesian)
            => new(length(cartesian), fmod(2f*PI + atan2(cartesian.y, cartesian.x), 2f*PI));

        public static float2 xy(this float3 v)
            => float2(v.x, v.y);
        public static float2 xy(this Vector3 v)
            => float2(v.x, v.y);
        public static float2 xy(this float4 v)
            => float2(v.x, v.y);
        public static float2 xy(this Vector4 v)
            => float2(v.x, v.y);
        public static ulong bitposmask(int b)
            => 1ul << b;
        public static bool bit(ulong ul, int b) 
            => (ul & bitposmask(b)) != 0;

        public static ulong setbit(ulong ul, int b)
            => ul | bitposmask(b);
        public static ulong unsetbit(ulong ul, int b)
            => ul & ~bitposmask(b);
        public static ulong flipbit(ulong ul, int b) 
            => ul ^ bitposmask(b);
        public static void setbit(ref ulong ul, int b)
            => ul = setbit(ul, b);
        public static void unsetbit(ref ulong ul, int b)
            => ul = unsetbit(ul, b);
        public static void flipbit(ref ulong ul, int b)
            => ul = flipbit(ul, b);

        public static float3 xyz(this float2 v) 
            => float3(v.x, v.y, 0);
        public static float3 xyz(this float2 v, float z)
            => float3(v.x, v.y, z);
        public static Vector3 xyzV(this float2 v)
            => new(v.x, v.y, 0f);
        public static float4 xyzw(this float2 v)
            => float4(v.x, v.y, 0, 0);
        public static float4 xyzw(this float3 v) 
            => float4(v.x, v.y, v.z, 0);
        public static float4 xyzw(this float2 v, float z, float w)
            => float4(v.x, v.y, z, w);
        public static float4 xyzw(this float3 v, float w)
            => float4(v.x, v.y, v.z, w);

        public static float2 perp(this float2 v)
            => cross(v.xyz(), new float3(0f, 0f, -1f)).xy();


        public static float mul(this float2 v)
            => v.x * v.y;
        public static float mul(this float3 v)
            => v.x * v.y * v.z;
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

        public static IReadOnlyList<int> BitPositions(Bit256 mask) 
        {
            var res = new List<int>();
            var i = 0;

            while(i < 256)
            {
                var b = mask & 1;
                if(b == 1) res.Add(i);
                mask >>= 1;
                i++;
            }

            return res;
        }

        public static float Round(float value, float amt)
            => value - value % amt;
    }
}
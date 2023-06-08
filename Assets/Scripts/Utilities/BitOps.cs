using Unity.Burst;

namespace Ramsey.Utilities
{
    [BurstCompile(CompileSynchronously = true)]
    public static class BitOps
    {
        public static void SplitShift64(int s, out int s1, out int s2) 
        {
            s1 = s >> 1;
            s2 = s - s1;
        }

        public static ulong Shl(ulong x, int s) 
        {
            SplitShift64(s, out var s1, out var s2);
            return (x << s1) << s2;
        }
        public static ulong Shr(ulong x, int s) 
        {
            SplitShift64(s, out var s1, out var s2);
            return (x >> s1) >> s2;
        }

        public static ulong ShlCarry(ulong x, int s, out ulong carry)
        {
            var r = x << s;
            carry = Shr(x, 64 - s);

            return r;
        }

        public static ulong ShrCarry(ulong x, int s, out ulong carry)
        {
            var r = x >> s;
            carry = Shl(x, 64 - s);

            return r;
        }
    }
}
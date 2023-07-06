using Unity.Burst;

namespace Ramsey.Utilities
{
    /// <summary>
    /// Implements various bitwise operations otherwise unsupported
    /// by C#.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    public static class BitOps
    {
        /// <summary>
        /// Split a given shift value into two shift values
        /// which will not cause issue with C#'s bit shift
        /// value wrapping.
        /// </summary>
        public static void SplitShift64(int s, out int s1, out int s2) 
        {
            // Clamp to beneath 128
            s &= 0x7F;

            s1 = s >> 1;
            s2 = s - s1;
        }

        /// <summary>
        /// Shift the given value left by the given amount,
        /// without wrapping the shift value to 0 if it exceeds 64.
        /// </summary>
        public static ulong Shl(ulong x, int s) 
        {
            SplitShift64(s, out var s1, out var s2);
            return (x << s1) << s2;
        }
        /// <summary>
        /// Shift the given value right by the given amount,
        /// without wrapping the shift value to 0 if it exceeds 64.
        /// </summary>
        public static ulong Shr(ulong x, int s) 
        {
            SplitShift64(s, out var s1, out var s2);
            return (x >> s1) >> s2;
        }

        /// <summary>
        /// Shift the given value left by the given amount from 0 to 64,
        /// and save the values which were shifted "out" of the 8 bytes
        /// to <paramref name="carry"/>.
        /// </summary>
        public static ulong ShlCarry(ulong x, int s, out ulong carry)
        {
            var r = x << s;
            carry = Shr(x, 64 - s);

            return r;
        }

        /// <summary>
        /// Shift the given value right by the given amount from 0 to 64,
        /// and save the values which were shifted "out" of the 8 bytes
        /// to <paramref name="carry"/>.
        /// </summary>
        public static ulong ShrCarry(ulong x, int s, out ulong carry)
        {
            var r = x >> s;
            carry = Shl(x, 64 - s);

            return r;
        }
    }
}
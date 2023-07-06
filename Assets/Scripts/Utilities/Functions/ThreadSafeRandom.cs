using Unity.Mathematics;

namespace Ramsey.Utilities
{
    /// <summary>
    /// Perform thread-safe random calculations.
    /// 
    /// Use this instead of any unity methods if retreiving
    /// random information for generating a move for a strategy.
    /// </summary>
    public static class ThreadSafeRandom
    {
        private static readonly System.Random rand = new();

        public static int Range(int min, int max)
        {
            lock(rand) 
            {
                return rand.Next(min, max);
            }
        }

        public static float Range(float min, float max)
        {
            lock(rand) 
            {
                return (float)math.lerp(min, max, rand.NextDouble());
            }
        }
    }
}
using Unity.Mathematics;

namespace Ramsey.Utilities
{
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
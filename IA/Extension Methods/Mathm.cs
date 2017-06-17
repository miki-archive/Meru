using System;

namespace Meru
{
    public class Mathm
    {
        private static Random random = new Random();

        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static bool IsEven(int value)
        {
            return (value & 1) == 0;
        }

        public static int Roll(int max = 100, int min = 0)
        {
            return random.Next(min, max) + 1;
        }
    }
}
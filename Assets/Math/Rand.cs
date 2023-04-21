using System.Collections;
using System.Collections.Generic;
using System;

namespace YX
{
    public static class Rand
    {
        static Random _r;

        static Rand()
        {
            _r = new Random(System.DateTime.Now.Millisecond);
        }

        public static void Reset(int seed)
        {
            _r = new Random(seed);
        }

        public static int Next()
        {
            return _r.Next();
        }

        public static int Next(int maxValue)
        {
            return _r.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return _r.Next(minValue, maxValue);
        }

        public static void NextBytes(byte[] buffer)
        {
            _r.NextBytes(buffer);
        }

        public static double NextDouble()
        {
            return _r.NextDouble();
        }
    }
}

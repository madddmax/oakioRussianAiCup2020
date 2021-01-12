using System;
using System.Collections.Generic;

namespace Aicup2020.Game
{
    public static class Dice
    {
        private static readonly Random Random = new Random(42);

        public static bool Roll(double p) => Random.NextDouble() < p;

        public static T Roll<T>(IList<T> items)
        {
            int index = Random.Next(items.Count);
            return items[index];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
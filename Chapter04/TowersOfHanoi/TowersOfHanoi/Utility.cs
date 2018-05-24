using System;
using System.Collections.Generic;

namespace TowerOfHanoi
{
    class Utility
    {
        public static double GetMax(double[,] Q, int init, List<int> availableMoves)
        {
            double max = -1;
            if (availableMoves.Count == 3)
                max = Math.Max(Q[init, availableMoves[0]], Math.Max(Q[init, availableMoves[1]], Q[init, availableMoves[2]]));
            else
                max = Math.Max(Q[init, availableMoves[0]], Q[init, availableMoves[1]]);

            return max;
        }

        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }
    }
}
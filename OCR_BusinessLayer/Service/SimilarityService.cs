﻿using System;

namespace OCR_BusinessLayer.Service
{
    class SimilarityService
    {
        private static int percent = 100;
        public static int GetSimilarity(string string1, string string2)
        {
            if (string2.Contains(string1))
            {
                return percent;
            }
            float dis = ComputeDistance(string1, string2);
            float maxLen = string1.Length;
            if (maxLen < string2.Length)
                maxLen = string2.Length;
            if (maxLen == 0.0F)
                return (int)(1.0F * percent);
            else
                return (int)((1.0F - dis / maxLen)* percent);
        }

        private static int ComputeDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] distance = new int[n + 1, m + 1]; // matrix
            int cost = 0;
            if (n == 0) return m;
            if (m == 0) return n;
            //init1
            for (int i = 0; i <= n; distance[i, 0] = i++) ;
            for (int j = 0; j <= m; distance[0, j] = j++) ;
            //find min distance
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    cost = (t.Substring(j - 1, 1) ==
                        s.Substring(i - 1, 1) ? 0 : 1);
                    distance[i, j] = Min3(distance[i - 1, j] + 1,
                    distance[i, j - 1] + 1,
                    distance[i - 1, j - 1] + cost);
                }
            }
            return distance[n, m];
        }

        private static int Min3(int a, int b, int c)
        {
            return Math.Min(Math.Min(a, b), c);
        }
    }
}

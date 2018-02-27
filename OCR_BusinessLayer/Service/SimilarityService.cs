using System;

namespace OCR_BusinessLayer.Service
{
    class SimilarityService
    {
        private static int percent = 100;
        private static int one = 1;
        private static int zero = 0;
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
            int[,] distance = new int[n + one, m + one]; // matrix
            int cost = zero;
            if (n == zero) return m;
            if (m == zero) return n;
            //init1
            for (int i = zero; i <= n; distance[i, zero] = i++) ;
            for (int j = zero; j <= m; distance[zero, j] = j++) ;
            //find min distance
            for (int i = one; i <= n; i++)
            {
                for (int j = one; j <= m; j++)
                {
                    cost = (t.Substring(j - one, one) ==
                        s.Substring(i - one, one) ? zero : one);
                    distance[i, j] = Min3(distance[i - one, j] + one,
                    distance[i, j - one] + one,
                    distance[i - one, j - one] + cost);
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

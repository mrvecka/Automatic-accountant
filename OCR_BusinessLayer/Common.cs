using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer
{
    public static class Common
    {
        private static char[] IlegalChars = { '\'', ';', '\\', '|', '\n', ')', '(', '!', '&', '=', '´', '%', 'ˇ', '+', '“', '$', '?', '!' };

        public static string SQLString(string text)
        {
            foreach (char c in IlegalChars)
            {
                if (text.Contains(c))
                {
                    text = text.Replace(c.ToString(),"");
                }
            }
            if (text.Length > CONSTANTS.MAX_LENGTH_OF_SQL_STRING)
            {
                text = text.Substring(0, CONSTANTS.MAX_LENGTH_OF_SQL_STRING);
            }
            return text;
        }
        /// <summary>
        /// Overrides all existing keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="dicToAdd"></param>
        public static void AddRangeOverride<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic[x.Key] = x.Value);
        }

        /// <summary>
        /// Adds new keys only
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="dicToAdd"></param>
        public static void AddRangeNewOnly<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => { if (!dic.ContainsKey(x.Key)) dic.Add(x.Key, x.Value); });
        }

        /// <summary>
        /// Throws an error if keys already exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="dicToAdd"></param>
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
        }

        /// <summary>
        /// Checks if keys don't exist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool ContainsKeys<TKey, TValue>(this Dictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
        {
            bool result = false;
            keys.ForEachOrBreak((x) => { result = dic.ContainsKey(x); return result; });
            return result;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void ForEachOrBreak<T>(this IEnumerable<T> source, Func<T, bool> func)
        {
            foreach (var item in source)
            {
                bool result = func(item);
                if (result) break;
            }
        }

    }




}

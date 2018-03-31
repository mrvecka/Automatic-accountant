using OCR_BusinessLayer.Classes;
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
					text = text.Replace(c.ToString(), string.Empty);
				}
			}
			if (text.Length > CONSTANTS.MAX_LENGTH_OF_SQL_STRING)
			{
				text = text.Substring(0, CONSTANTS.MAX_LENGTH_OF_SQL_STRING);
			}
			return text;
		}

		public static string RemoveDiacritism(string Text)
		{
			string stringFormD = Text.Normalize(System.Text.NormalizationForm.FormD);
			System.Text.StringBuilder retVal = new System.Text.StringBuilder();
			for (int index = 0; index < stringFormD.Length; index++)
			{
				if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stringFormD[index]) != System.Globalization.UnicodeCategory.NonSpacingMark)
			        retVal.Append(stringFormD[index]);

			}

            return retVal.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }


		/// <summary>
		/// Methode return text for Column according to position of words 
		/// </summary>
		/// <param name="col">Colum for which I m looking for words</param>
		/// <param name="line">Line where I m looking</param>
		/// <returns></returns>
		public static string GetWordsForColumn(Column col, TextLine line)
		{
			string a = string.Empty;

			foreach (Word w in line.Words)
			{
				if (w.Bounds.Left < col.Right && w.Bounds.Width < CONSTANTS.MAX_LENGTH_OF_ONE_WORD)
				{
					if (((w.Bounds.Left <= col.Left && w.Bounds.Right > col.Left) || w.Bounds.Left >= col.Left) && ((w.Bounds.Right >= col.Right && w.Bounds.Left < col.Right) || w.Bounds.Right <= col.Right))

					{
						if (col.Left > w.Bounds.Left)
							col.Left = w.Bounds.Left;

						a += w.Text + " ";

					}
				}
			}
			return a.Trim();

		}

		/// <summary>
		/// Methode return path with specified extension
		/// </summary>
		/// <param name="path">Path where the file shoul be saved</param>
		/// <param name="extension">Type of file without dot '.'</param>
		/// <returns></returns>
		public static string ModifyPath(string path, string extension)
		{
			var s = path.Remove(path.LastIndexOf('.')-1);
			s += "." + extension;
			return s;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Service
{
    public class ValidationHelper
    {
        public static List<char> numbersOnly = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public static string NumbersOnly(string symbol)
        {
            if (!string.IsNullOrWhiteSpace(symbol))
            {
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (numbersOnly.Contains(symbol[i]))
                    {
                        continue;
                    }
                    else
                    {
                        symbol = symbol.Remove(i, 1);
                        i--;
                    }
                }
            }
            return symbol.Trim();
        }



        public static string LettersOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122))
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol.Trim();
        }

        public static string LettersSpacesOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122) || symbol[i] == 32)
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol.Trim();
        }

        public static string LettersDotsOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122) || symbol[i] == 46)
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol.Trim();
        }

        public static string LettersSpacesDotsOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122) || symbol[i] == 32 || symbol[i] == 46)
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol.Trim();
        }


        public static string ValidateDate(string symbol)
        {
            string date = string.Empty;
            //120314
            // 12032014 or 12.03.14 or 12 03 14
            //12.03. 14
            // 12 03 2014
            // 12.03. 2014

            //day
            int index = symbol.IndexOfAny(numbersOnly.ToArray());
            var s = string.Empty;
            if (index != -1)
            {
                s = symbol.Substring(index, 2);
                date += s;
                date += ".";
                symbol = symbol.Replace(s, string.Empty);
            }

            // month
            index = symbol.IndexOfAny(numbersOnly.ToArray()); // ak mam mesiac napr 01 a rok 2011 tak mi to vymaze aj z roku
            if (index != -1)
            {
                s = symbol.Substring(index, 2);
                date += s;
                date += ". ";
                symbol = symbol.Replace(s, string.Empty);
            }

            //year
            index = symbol.IndexOfAny(numbersOnly.ToArray());
            if (index != -1)
            {
                s = symbol.Substring(index);
                if (s.Length == 2)
                {
                    date += "20";
                }
                date += s;
            }


            return date;
        }


        public static bool ContainOnlyLetters(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i] >= 65 && text[i] <= 90) || (text[i] >= 97 && text[i] <= 122))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer
{
    public class Common
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
            if (text.Length > 100)
            {
                text = text.Substring(0, 100);
            }
            return text;
        }
    }
}

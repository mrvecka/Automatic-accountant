using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Classes
{
    public class PossitionOfWord
    {
    
        public string Key { get; set; }
        public Rectangle KeyBounds { get; set; }
        public string Value { get; set; }
        public Rectangle ValueBounds { get; set; }
        public bool IsActive { get; set; } = false;
        public string Confidence { get; set; } = "0.00";
        public string DictionaryKey { get; set; }

        public PossitionOfWord(string k,Rectangle KB,string val,Rectangle VB)
        {
            Key = k;
            KeyBounds = KB;
            Value = val;
            ValueBounds = VB;
        }
        public PossitionOfWord() { }
    }
}

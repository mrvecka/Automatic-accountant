using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace Bakalarska_praca.Classes
{
    public class TextLine
    {
        public PageIteratorLevel Level { get; set; }
        public Rectangle Bounds { get; set; }
        public string text;
        public List<Word> Words;
    }
}

﻿using System;
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
        public string Confidence { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Classes
{
    public class PreviewObject
    {
        public Image Img;

        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Example Displaying hint 2")]
        [Category("Global info")]
        [DisplayName("Confidence")]
        public string Confidence { get; set; }
        public List<TextLine> Lines;

        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Example Displaying hint 2")]
        [Category("Global info")]
        [DisplayName("Path")]
        public string Path { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Example Displaying hint 2")]
        [Category("Global info")]
        [DisplayName("Language")]
        public string Lang { get; set; }
        public List<PossitionOfWord> ListOfKeyPossitions;
        public List<Column> ListOfKeyColumn;

        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Example Displaying hint 2")]
        [Category("Evidence")]
        [DisplayName("Evidence")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Evidence Evidence { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Example Displaying hint 2")]
        [Category("Clients")]
        [DisplayName("Clients")]
        public List<Client> Clients { get; set; }

    }
}

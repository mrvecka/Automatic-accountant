using OCR_BusinessLayer.Classes.Client;
using OCR_BusinessLayer.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Classes
{
    public class PreviewObject
    {
        public PreviewObject()
        {
            Confidence = "0.00 %";
            Lines = new List<TextLine>();
            //path
            Lang = "Not Specificed";
            ListOfKeyPossitions = new List<PossitionOfWord>();
            ListOfKeyColumn = new List<Column>();
            Evidence = new Evidence();
            Clients = new ClientCollection() { Capacity = 5};
        }
        public Image Img;

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Global info")]
        [DisplayName("Confidence")]
        [Description("Confidence with recognition")]
        public string Confidence { get; set; }
        public List<TextLine> Lines;

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Global info")]
        [DisplayName("File location")]
        public string Path { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Global info")]
        [DisplayName("Language")]
        public string Lang { get; set; }
        public List<PossitionOfWord> ListOfKeyPossitions;
        public List<Column> ListOfKeyColumn;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Evidence")]
        [DisplayName("Evidence")]
        [TypeConverter(typeof(EvidenceConverter))]
        public Evidence Evidence { get; set; }

        [Browsable(true)]
        [Category("Clients")]
        [ReadOnly(false)]
        [Editor(typeof(ClientCollectionEditor),typeof(UITypeEditor))]
        [TypeConverter(typeof(ClinetCollectionConverter))]
        public ClientCollection Clients { get; set; }

    }
}

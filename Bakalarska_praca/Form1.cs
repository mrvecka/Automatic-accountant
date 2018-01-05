using Leptonica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace Bakalarska_praca
{
    public partial class Form1 : Form
    {

        TessBaseAPI engine;
        static string testImagePath = "./phototest.tif";
        Bitmap originalBmp = null;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            panel1.AutoScroll = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

        }


        void loadImg()
        {
            if (originalBmp == null)
            {
                originalBmp = new Bitmap(Image.FromFile(testImagePath));
            }
            pictureBox1.Image = new Bitmap(originalBmp);
            panel1.AutoScrollMinSize = pictureBox1.Image.Size;
        }

        void IterateFullPage(ResultIterator iter, ref List<Border> blocks, ref List<Border> paras, ref List<Border> textLines, ref List<Border> words, ref List<Border> symbols)
        {
            int left, top, right, bottom;


            StringBuilder ss = new StringBuilder("");
            PageIteratorLevel level = PageIteratorLevel.RIL_TEXTLINE;

            do
            {
                ss.Append(iter.GetUTF8Text(level));
                iter.BoundingBox(level, out left, out top, out right, out bottom);
                textLines.Add(
                    new Border
                    {
                        Bounds = new Rectangle(left, top, right - left, bottom - top)
                    });
                ss.Append(System.Environment.NewLine);
            } while (iter.Next(level));



            richTextBox1.Text = ss.ToString();

        }

        static Bitmap RescaleToDpi(System.Drawing.Image image, int dpiX, int dpiY)
        {
            Bitmap bm = new Bitmap(
                (int)(image.Width * dpiX / image.HorizontalResolution),
                (int)(image.Height * dpiY / image.VerticalResolution),
                image.PixelFormat);
            bm.SetResolution(dpiX, dpiY);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(image, 0, 0, bm.Width, bm.Height);
            }
            return bm;
        }

        byte[] GetTiffBytes(Bitmap img)
        {
            using (var buffer = new MemoryStream())
            {
                img.Save(buffer, System.Drawing.Imaging.ImageFormat.Tiff);
                return buffer.ToArray();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //load
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = open.FileName;
                originalBmp = new Bitmap(open.FileName);
                open.RestoreDirectory = true;
                loadImg();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //start
            loadImg();
            richTextBox1.Text = "";
            textBox1.Text = "";
            pictureBox1.Refresh();
            richTextBox1.Refresh();
            textBox1.Refresh();


            // List<Word> words = IteratePage(page).ToList();

            List<Border> blocks = new List<Border>();
            List<Border> paras = new List<Border>();
            List<Border> textLines = new List<Border>();
            List<Border> words = new List<Border>();
            List<Border> symbols = new List<Border>();

            if (!checkBox1.Checked)
            {
                blocks = null;
            }
            if (!checkBox2.Checked)
            {
                paras = null;
            }
            if (!checkBox3.Checked)
            {
                textLines = null;                
            }
            if (!checkBox4.Checked)
            {
                words = null;
            }
            if (!checkBox5.Checked)
            {
                symbols = null;
            }

            //OCR
            string lang = comboBox1.SelectedItem.ToString();

            using (engine = new TessBaseAPI(@".\tessdata", lang))
            {
                var pix = Pix.Read(textBox2.Text);
                engine.InitForAnalysePage();
                engine.Init(null, lang);
                engine.SetInputImage(pix);
                engine.Recognize();
                ResultIterator iterator = engine.GetIterator();
                
                IterateFullPage(iterator, ref blocks, ref paras, ref textLines, ref words, ref symbols);

                textBox1.Text = ((Double)(engine.MeanTextConf) / 100).ToString("P2");
                iterator.Dispose();

            }

            //DRAW
            Pen myPen;
            System.Drawing.Graphics newGraphics = System.Drawing.Graphics.FromImage(pictureBox1.Image);
            if (blocks != null)
            {
                myPen = new Pen(Color.Blue, 3);
                foreach (Border block in blocks)
                {
                    newGraphics.DrawRectangle(myPen, block.Bounds);
                }
            }
            if (paras != null)
            {
                myPen = new Pen(Color.Green, 2);
                foreach (Border para in paras)
                {
                    newGraphics.DrawRectangle(myPen, para.Bounds);
                }
            }
            if (textLines != null)
            {
                myPen = new Pen(Color.Violet, 1.5f);
                foreach (Border textLine in textLines)
                {
                    newGraphics.DrawRectangle(myPen, textLine.Bounds);
                }
            }
            if (words != null)
            {
                myPen = new Pen(Color.Red, 1);
                foreach (Border word in words)
                {
                    newGraphics.DrawRectangle(myPen, word.Bounds);
                }
            }
            if (symbols != null)
            {
                myPen = new Pen(Color.DarkBlue, 0.5f);
                foreach (Border symbol in symbols)
                {
                    newGraphics.DrawRectangle(myPen, symbol.Bounds);
                }
            }
            pictureBox1.Refresh();

        }
    }

    class Word
    {
        public string Text { get; set; }
        public Rectangle Bounds { get; set; }
    }


    class Border
    {
        public PageIteratorLevel Level { get; set; }
        public Rectangle Bounds { get; set; }
    }
}

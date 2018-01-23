using AForge.Imaging;
using AForge.Imaging.Filters;
using Bakalarska_praca.Classes;
using Bakalarska_praca.Service;
using Emgu.CV;
using Emgu.CV.Structure;
using Leptonica;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        
        static string testImagePath = "./phototest.tif";
        Bitmap originalBmp = null;
        OpenCvSharp.Mat img;
        OpenCvSharp.Mat grayImg;

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
                originalBmp = new Bitmap(System.Drawing.Image.FromFile(testImagePath));
            }
            pictureBox1.Image = new Bitmap(originalBmp);
            panel1.AutoScrollMinSize = pictureBox1.Image.Size;
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
                grayImg = Cv2.ImRead(open.FileName, ImreadModes.GrayScale);
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

            List<TextLine> blocks = new List<TextLine>();
            List<TextLine> paras = new List<TextLine>();
            List<TextLine> textLines = new List<TextLine>();
            List<TextLine> words = new List<TextLine>();
            List<TextLine> symbols = new List<TextLine>();

            if (!checkBox1.Checked)
            {
                blocks = null;
            }
            if (!checkBox2.Checked)
            {
                paras = null;
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


            TesseractService tess = new TesseractService(blocks, paras, textLines, words, symbols, lang);
            System.Drawing.Image img = tess.ProcessImage(textBox2.Text, pictureBox1.Image);

            richTextBox1.Text = tess.text;
            textBox1.Text = tess.confidence;
            pictureBox1.Image = img;
            pictureBox1.Refresh();


        }
    }

}

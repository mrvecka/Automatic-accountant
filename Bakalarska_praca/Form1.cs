using OCR_BusinessLayer.Classes;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Bakalarska_praca
{
    public partial class Form1 : Form
    {

        OpenCvSharp.Mat img;
        private System.Drawing.Image newImage;
        private Label selectedLabel;
        private const int fontSize = 13;


        public Form1(PreviewObject file)
        {
            InitializeComponent();

            pictureBox1.Image = (Bitmap)file.img;
            panel1.AutoScroll = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            txtCoinfidence.Text = file.confidence;
            txtLang.Text = file.lang;
            newImage = file.img;
            FillListView(file.Lines);
        }



        private void FillListView(List<TextLine> lines)
        {
            int loc = 0;
            foreach (TextLine line in lines)
            {
                var l = new Label { Text = line.text };
                l.SetBounds(0, loc, panel3.Width, 30);
                l.Click += delegate { AddRectangle(line,l); };
                panel3.Controls.Add(l);
                loc += l.Height;
            }

            panel3.AutoScrollMinSize = new System.Drawing.Size(0, panel3.Height);
            panel3.Refresh();

        }

        private void AddRectangle(TextLine line,Label l)
        {
            if (selectedLabel != null)
            {
                selectedLabel.BackColor = Color.Transparent;
                selectedLabel.Font = l.Font;
            }
            l.BackColor = Color.FromArgb(102,140,255);
            l.Font = new Font(FontFamily.GenericSerif,fontSize,FontStyle.Bold);
            selectedLabel = l;
            ClearImageAndText();
            System.Drawing.Image img = (System.Drawing.Image)newImage.Clone();
            Pen myPenword = new Pen(Color.Blue, 2f);
            System.Drawing.Graphics newGraphics = System.Drawing.Graphics.FromImage(img);


            newGraphics.DrawRectangle(myPenword, line.Bounds);
            pictureBox1.Image = img;
        }
        private void ClearImageAndText()
        {
            pictureBox1.Image = newImage;

        }

    }

}

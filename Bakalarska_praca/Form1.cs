using OCR_BusinessLayer.Classes;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Bakalarska_praca
{
    public partial class Form1 : Form
    {

        private const int fontSize = 13;
        OpenCvSharp.Mat img;
        private System.Drawing.Image newImage;
        private System.Drawing.Image OrgImg;
        private Label selectedLabel;
        private bool isMouseDown = false;
        private bool firstMove = true;
        private PreviewObject _p;
        private Rectangle newRect;
        private Rectangle oldRect;
        int deltaX = 0;
        int deltaY = 0;
        System.Drawing.Graphics newGraphics;
        Pen myPenword;

        public Form1(PreviewObject file)
        {
            InitializeComponent();

            pictureBox1.Image = (Bitmap)file.img;
            panel1.AutoScroll = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            txtCoinfidence.Text = file.confidence;
            txtLang.Text = file.lang;
            newImage = file.img;
            OrgImg = newImage;
            _p = file;
            FillListView(file);
        }

        private void FillListView(PreviewObject prew)
        {
            int loc = 0;
            foreach (PossitionOfWord key in prew.listOfKeyPossitions)
            {
                var l = new Label { Text = key.Key + " : " + key.Value  };
                l.SetBounds(0, loc, panel3.Width, 30);
                l.Click += delegate { AddRectangle(key,l); };
                panel3.Controls.Add(l);
                loc += l.Height;
            }

            panel3.AutoScrollMinSize = new System.Drawing.Size(0, panel3.Height);
            panel3.Refresh();

        }

        private void AddRectangle(PossitionOfWord key,Label l)
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
            myPenword = new Pen(Color.Blue, 2f);
            newGraphics = System.Drawing.Graphics.FromImage(img);


            newGraphics.DrawRectangle(myPenword, key.KeyBounds);
            newGraphics.DrawRectangle(myPenword, key.ValueBounds);

            pictureBox1.Image = img;
        }
        private void ClearImageAndText()
        {            
            pictureBox1.Image = newImage;

        }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            newRect = oldRect = getRectangle(e);
        }

        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true && oldRect.X != 0 && oldRect.Y != 0)
            {
              
                if (firstMove)
                {
                    deltaX = e.Location.X - newRect.X;
                    deltaY = e.Location.Y - newRect.Y;
                    firstMove = false;
                }

                newRect.Location = e.Location;

                    newRect.X -= deltaX;
                    newRect.Y -= deltaY;


                if (newRect.Right > pictureBox1.Width)
                {
                    newRect.X = pictureBox1.Width - newRect.Width;
                }
                if (newRect.Top < 0)
                {
                    newRect.Y = 0;
                }
                if (newRect.Left < 0)
                {
                    newRect.X = 0;
                }
                if (newRect.Bottom > pictureBox1.Height)
                {
                    newRect.Y = pictureBox1.Height - newRect.Height;
                }
                Refresh();
            }
        }

        private void pictureBox1_MouseUp_1(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            firstMove = true;
            setRectangle();


        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (oldRect != null && newRect != null)
            {
                e.Graphics.DrawRectangle(new Pen(Color.White, 2f), oldRect);
                e.Graphics.DrawRectangle(new Pen(Color.Blue, 2f), newRect);
            }
        }
        private Rectangle getRectangle(MouseEventArgs e)
        {
            foreach (PossitionOfWord p in _p.listOfKeyPossitions)
            {
                if (e.X > p.KeyBounds.X && e.X < p.KeyBounds.X + p.KeyBounds.Width && e.Y > p.KeyBounds.Y && e.Y < p.KeyBounds.Y + p.KeyBounds.Height)
                {
                    return p.KeyBounds;
                }
                else if (e.X > p.ValueBounds.X && e.X < p.ValueBounds.X + p.ValueBounds.Width && e.Y > p.ValueBounds.Y && e.Y < p.ValueBounds.Y + p.ValueBounds.Height)
                {
                    return p.ValueBounds;
                }
            }
            return new Rectangle(0,0,0,0);
        }

        private void setRectangle()
        {

            foreach (PossitionOfWord p in _p.listOfKeyPossitions)
            {
                if (p.KeyBounds.Equals(oldRect))
                {
                    p.KeyBounds = newRect;
                }
                else if (p.ValueBounds.Equals(oldRect))
                {
                    p.ValueBounds = newRect;
                }
            }
        }

        //save pattern
        private void button1_Click(object sender, System.EventArgs e)
        {

        }
    }

}

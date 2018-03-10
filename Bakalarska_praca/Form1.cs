using Bakalarska_praca.Dictioneries;
using OCR_BusinessLayer;
using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Service;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using static OCR_BusinessLayer.CONSTANTS;

namespace Bakalarska_praca
{
    public partial class Form1 : Form
    {

        private const int _fontSize = 13;
        private System.Drawing.Image _newImage;
        private List<string> _files;
        private PreviewObject _def;
        private bool _isMouseDown = false;
        private bool _firstMove = true;
        private bool _resizingRight = false;
        private bool _resizingBottom = false;
        private bool _drawingNewRect = false;
        private bool _canDraw = false;
        private int _countOfNewRect = 0;
        private PreviewObject _p;
        private PossitionOfWord _newPositions;
        private Rectangle _newRect;
        private Rectangle _oldRect;
        private int _deltaX = 0;
        private int _deltaY = 0;
        private System.Drawing.Graphics _newGraphics;
        private Pen _myPenword;

        public Form1(PreviewObject file)
        {
            InitializeComponent();
            _myPenword = new Pen(Color.Blue, 2f);
            if (file != null)
            {
                btnRemove.Enabled = false;
                _def = file;
                pictureBox1.Image = (Bitmap)file.Img;
                txtConfidence.Text = file.Confidence;
                txtLang.Text = file.Lang;
                _newImage = file.Img;
                _p = file;
                txtPathPattern.Visible = false;
                btnNewFile.Visible = false;
                FillListView(file);
                FillCombo();
            }
            else
            {
                //idem vytvarat novy pattern
                lblConfidence.Visible = false;
                lblPatConfidence.Visible = false;
                txtConfidence.Visible = false;
                txtPartConfidence.Visible = false;
                lblLanguage.Visible = false;
                txtLang.Visible = false;

            }
        }

        private void FillListView(PreviewObject prew)
        {
            BindingList<PossitionOfWord> bindingList = new BindingList<PossitionOfWord>(prew.ListOfKeyPossitions);
            foreach (var col in prew.ListOfKeyColumn)
            {
                var rec = new Rectangle(col.Left,col.Top,col.Right-col.Left,col.Bottom-col.Top);
                bindingList.Add(new PossitionOfWord(col.Text,rec,string.Empty,rec));
            }
            dataGridValues.AutoGenerateColumns = false;
            dataGridValues.DataSource = bindingList;
        }

        private void FillCombo()
        {
            var a = new Dictionary();
            Common.AddRangeNewOnly(a.header, a.columns);
            Common.AddRangeNewOnly(a.header, a.clients);
            cmbKey.DataSource = new BindingSource(a.header, null);
            cmbKey.DisplayMember = "Key";
            cmbKey.ValueMember = "Key";

        }


        private PossitionOfWord GetSelectedWords()
        {
            if (dataGridValues.SelectedRows.Count == 1)
            {
                PossitionOfWord b = dataGridValues.SelectedRows[0].DataBoundItem as PossitionOfWord;
                btnRemove.Enabled = true;
                return b;
            }
            else
            {
                btnRemove.Enabled = false;
                return null;
            }
        }

        private void AddRectangle(PossitionOfWord key)
        {

            txtPartConfidence.Text = key.Confidence.ToString();
            if (key != null)
            {
                ClearImageAndText();
                System.Drawing.Image img = (System.Drawing.Image)_newImage.Clone();
                _newGraphics = System.Drawing.Graphics.FromImage(img);

                _p.ListOfKeyPossitions.ForEach(c => c.IsActive = false);
                key.IsActive = true;
                if (key.KeyBounds.Equals(key.ValueBounds))
                {
                    _newGraphics.DrawRectangle(_myPenword, key.ValueBounds);
                }
                else
                {
                    _newGraphics.DrawRectangle(_myPenword, key.KeyBounds);
                    _newGraphics.DrawRectangle(_myPenword, key.ValueBounds);
                }

                pictureBox1.Image = img;
            }
        }
        private void ClearImageAndText()
        {
            pictureBox1.Image = _newImage;
            pictureBox1.Refresh();

        }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
            _newRect = _oldRect = getRectangle(e.Location);
            if (_newRect.X == 0 && _newRect.Y == 0 && _newRect.Width == 0 && _newRect.Height == 0 && _canDraw)
            {
                _drawingNewRect = true;
                _newRect.X = e.Location.X;
                _newRect.Y = e.Location.Y;
                Cursor.Current = Cursors.PanNW;
            }
            else if (e.X > _newRect.Right - 4 && e.X < _newRect.Right + 4 && e.Y > _newRect.Top && e.Y < _newRect.Bottom)
            {
                _resizingRight = true;
                Cursor.Current = Cursors.SizeWE;
            }
            else if (e.Y > _newRect.Bottom - 4 && e.Y < _newRect.Bottom + 4 && e.X > _newRect.Left && e.X < _newRect.Right)
            {
                _resizingBottom = true;
                Cursor.Current = Cursors.SizeNS;
            }
        }

        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
            pictureBox1.SuspendLayout();
            if (_isMouseDown == true)
            {
                if (_resizingBottom || _resizingRight)
                {
                    if (_resizingBottom)
                    {
                        _newRect.Height += (_newRect.Bottom - e.Y) * -1;
                    }
                    if (_resizingRight)
                    {
                        _newRect.Width += (_newRect.Right - e.X) * -1;
                    }
                }
                else if (_drawingNewRect)
                {
                    _newRect.Width = e.Location.X - _newRect.X;
                    _newRect.Height = e.Location.Y - _newRect.Y;
                }
                else
                {
                    if (_oldRect.X != 0 && _oldRect.Y != 0)
                    {
                        if (_firstMove)
                        {
                            _deltaX = e.Location.X - _newRect.X;
                            _deltaY = e.Location.Y - _newRect.Y;
                            _firstMove = false;
                        }

                        _newRect.Location = e.Location;

                        _newRect.X -= _deltaX;
                        _newRect.Y -= _deltaY;


                        if (_newRect.Right > pictureBox1.Width)
                        {
                            _newRect.X = pictureBox1.Width - _newRect.Width;
                        }
                        if (_newRect.Top < 0)
                        {
                            _newRect.Y = 0;
                        }
                        if (_newRect.Left < 0)
                        {
                            _newRect.X = 0;
                        }
                        if (_newRect.Bottom > pictureBox1.Height)
                        {
                            _newRect.Y = pictureBox1.Height - _newRect.Height;


                        }
                    }
                }
            }
            pictureBox1.Invalidate();
            pictureBox1.ResumeLayout();
        }

        private void pictureBox1_MouseUp_1(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
            _firstMove = true;
            _resizingBottom = false;
            _resizingRight = false;

            if (_drawingNewRect)
            {
                if (_newPositions == null)
                {
                    _newPositions = new PossitionOfWord();
                }
                _drawingNewRect = false;

                if (!chkOnlyValue.Checked && _countOfNewRect == 2)
                {
                    _newPositions.KeyBounds = _newRect;
                }
                else if (!chkOnlyValue.Checked && _countOfNewRect == 1)
                {
                    _newPositions.ValueBounds = _newRect;
                }
                else if (chkOnlyValue.Checked && _countOfNewRect == 1)
                {
                    _newPositions.ValueBounds = _newRect;
                }

                _countOfNewRect--;

                if (_countOfNewRect == 0)
                {
                    _newPositions.Key = cmbKey.SelectedValue.ToString();
                    _newPositions.Value = string.Empty;

                    _p.ListOfKeyPossitions.Add(_newPositions);
                    AddRectangle(_newPositions);
                    _newPositions = null;
                    _canDraw = false;
                    panel4.Enabled = true;
                    panel5.Enabled = true;
                    panel6.Enabled = true;
                }

            }
            setRectangle();
            FillListView(_p);

        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Blue, 2f), _newRect);
            if (_oldRect != null)
            {
                e.Graphics.DrawRectangle(new Pen(Color.White, 2f), _oldRect);
            }
        }
        private Rectangle getRectangle(Point e)
        {
            foreach (PossitionOfWord p in _p.ListOfKeyPossitions)
            {
                if (e.X > p.KeyBounds.X && e.X < p.KeyBounds.X + p.KeyBounds.Width && e.Y > p.KeyBounds.Y && e.Y < p.KeyBounds.Y + p.KeyBounds.Height && p.IsActive)
                {
                    return p.KeyBounds;
                }
                else if (e.X > p.ValueBounds.X && e.X < p.ValueBounds.X + p.ValueBounds.Width && e.Y > p.ValueBounds.Y && e.Y < p.ValueBounds.Y + p.ValueBounds.Height && p.IsActive)
                {
                    return p.ValueBounds;
                }
            }
            return new Rectangle(0, 0, 0, 0);
        }

        private void setRectangle()
        {

            foreach (PossitionOfWord p in _p.ListOfKeyPossitions)
            {
                if (p.KeyBounds.Equals(_oldRect))
                {
                    p.KeyBounds = _newRect;
                }
                else if (p.ValueBounds.Equals(_oldRect))
                {
                    p.ValueBounds = _newRect;
                }
            }
        }

        //save pattern
        private void button1_Click(object sender, System.EventArgs e)
        {
            SavePatternClick();
        }
        private void SavePatternClick()
        {
            Database db = new Database();

            string table = "OCR_2018.dbo.T003_Possitions";
            //if (!db.CheckTableExists(table))
            //{
            //    db.CreateTableIfNotExists(table);
            //}

            string SQL = $"INSERT INTO OCR_2018.dbo.T003_Pattern(Lang) VALUES ('{_p.Lang}')";
            db.Execute(SQL, Operation.INSERT);
            SQL = "SELECT TOP 1 * FROM OCR_2018.dbo.T003_Pattern ORDER BY Pattern_ID desc";
            SqlDataReader o = (SqlDataReader)db.Execute(SQL, Operation.SELECT);
            int id = 0;
            while (o.Read())
            {
                id = (int)o[0];
            }
            o.Close();
            int rows = 0;
            foreach (PossitionOfWord p in _p.ListOfKeyPossitions)
            {
                SQL = "INSERT INTO OCR_2018.dbo.T004_Possitions(Pattern_ID,Word_Key,Word_Value,K_X,K_Y,K_Width,K_Height,V_X,V_Y,V_Width,V_Height)" +
                             $" Values({id},'{Common.SQLString(p.Key)}','{Common.SQLString(p.Value)}',{p.KeyBounds.X},{p.KeyBounds.Y},{p.KeyBounds.Width},{p.KeyBounds.Height}," +
                                                       $"{p.ValueBounds.X},{p.ValueBounds.Y},{p.ValueBounds.Width},{p.ValueBounds.Height});";
                object d = db.Execute(SQL, Operation.INSERT);
                if (d.GetType() != typeof(SqlDataReader))
                {
                    rows += (int)d;
                }
            }

            db.Close();

            MessageBox.Show("Pattern successfully saved", "Save pattern", MessageBoxButtons.OK);



        }
        private void btn_default_Click(object sender, System.EventArgs e)
        {
            FillListView(_def);
        }

        private void dataGridValues_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            AddRectangle(GetSelectedWords());
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            var w = GetSelectedWords();
            if (w != null)
            {
                _p.ListOfKeyPossitions.Remove(w);
                _def = _p;
                FillListView(_p);
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("If \"Only value\" is selected draw just one rectangle on value position else draw first rectangle on key position and second on value position.", "Positioning wizard", MessageBoxButtons.OK);
            _canDraw = true;
            _countOfNewRect = chkOnlyValue.Checked ? 1 : 2;
            panel4.Enabled = false;
            panel5.Enabled = false;
            panel6.Enabled = false;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            using (var fbd = new OpenFileDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    txtPathPattern.Text = fbd.FileName;
                }
            }
            _files = FileService.FindFiles(txtPathPattern.Text, CONSTANTS.filter);
            if (_files == null)
            {
                MessageBox.Show("Unsupported file format OR you don't have rights to that file!!!", "Invalid format", MessageBoxButtons.OK);
                return;
            }

            var _cvService = OpenCVImageService.GetInstance();
            _cvService.PrepareImage(_files[0]);
            pictureBox1.Image = _cvService.bmp;
            _newImage = _cvService.bmp;
            _p = new PreviewObject();
            _p.ListOfKeyPossitions = new List<PossitionOfWord>();
            _p.ListOfKeyColumn = new List<Column>();
            FillCombo();
        }


    }

}

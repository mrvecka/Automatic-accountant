using OCR_BusinessLayer;
using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Bakalarska_praca
{
    public partial class frmOCR : Form
    {

        private List<string> _files;
        private List<FileToProcess> _filesToProcess;
        private List<PreviewObject> _previewObjects;
        private ThreadService _service;

        public frmOCR()
        {
            InitializeComponent();
            _filesToProcess = new List<FileToProcess>();
            txtPath.Text = @"C:\Users\Lukáš\Pictures\2018-02-01 faktura\vyskusane";
            var data = FileService.FindTrainedData("tessdata", CONSTANTS.trainedData);
            comboBox1.DataSource = data;
            comboBox1.SelectedIndex = data.IndexOf("slk");
            btnGenerate.Enabled = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _filesToProcess.Clear();
            pnlMain.Controls.Clear();
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtPath.Text = fbd.SelectedPath;
                }
            }

            _files = FileService.FindFiles(txtPath.Text,CONSTANTS.filter);
            if (_files != null)
            {
                if (_files.Count == 0)
                {
                    btnStart.Enabled = false;
                    lblFound.Text = $"{_files.Count} file found";
                }
                else
                {
                    btnStart.Enabled = true;
                    btnGenerate.Enabled = true;
                    if (_files.Count > 1)
                        lblFound.Text = $"{_files.Count} files found";
                    else
                        lblFound.Text = $"{_files.Count} file found";

                    FillPanel();
                }
            }
            else
            {
                MessageBox.Show("You don't have rights to that folder OR no files ware found!!!", "Invalid path", MessageBoxButtons.OK);
            }

        }

        private async void btnStart_Click(object sender, EventArgs e)
        {

            DisableControls();
            //prepare service
            _service = new ThreadService(_filesToProcess,comboBox1.SelectedItem.ToString());
            await _service.StartService();
            _previewObjects = _service.Preview;
            btnStart.Enabled = true;
            btnStart.Enabled = true;
            comboBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            btnGenerate.Enabled = true;
        }

        private void FillPanel()
        {
            pnlMain.Controls.Clear();
            const int lineHeight = 23;
            int width = pnlMain.Width -30;
            int labelWidth = (int)(pnlMain.Width * 0.70);
            int progressBtnWidth = (int)(pnlMain.Width * 0.14);
            int loc = 0;

            foreach (string path in _files)
            {
                var panel = new Panel();
                var label = new Label();
                var progress = new ProgressBar();
                var button = new Button();
                panel.SetBounds(0, loc, width, lineHeight);


                progress.Maximum = 100;

                label.Text = $"...{path.Substring(path.LastIndexOf("\\"))}";


                label.SetBounds(0, 0, labelWidth, lineHeight);
                button.SetBounds(panel.Width- progressBtnWidth, 0, progressBtnWidth, lineHeight);
                progress.SetBounds(button.Location.X - progressBtnWidth, 0, progressBtnWidth, lineHeight);

                var f = new FileToProcess(path, progress, button);
                button.Text = "Preview";
                button.Enabled = false;
                button.Click += delegate { ShowPreview(path); };

                _filesToProcess.Add(f);

                panel.Controls.Add(label);
                panel.Controls.Add(progress);
                panel.Controls.Add(button);

                pnlMain.Controls.Add(panel);

                loc += panel.Height;
            }


        }

        private void ShowPreview(string path)
        {
            if (_previewObjects == null)
            {
                _previewObjects = _service.Preview;
            }

            Form1 f = new Form1(_previewObjects.Where(c => c.Path.Equals(path)).FirstOrDefault());
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1(null);
            f.ShowDialog();
        }

        private void frmOCR_FormClosed(object sender, FormClosedEventArgs e)
        {
            OpenCVImageService.DeleteFiles();
        }
        private void DisableControls()
        {
            btnGenerate.Enabled = false;
            btnStart.Enabled = false;
            comboBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            _filesToProcess.ForEach(c => c.ProgressBar.Value = 0);
            _filesToProcess.ForEach(c => c.Button.Enabled = false);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _previewObjects = _service.Preview;
            if (_previewObjects.Count>0)
                FileService.GenerateTxtFiles(_previewObjects);
            else
                MessageBox.Show("No data to import", "No data", MessageBoxButtons.OK);

        }
    }
}

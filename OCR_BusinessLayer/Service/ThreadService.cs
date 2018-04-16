using Google.Apis.Vision.v1.Data;
using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Google;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static OCR_BusinessLayer.CONSTANTS;

namespace OCR_BusinessLayer.Service
{
    public class ThreadService
    {
        private List<FileToProcess> _filesToProcess;
        private List<PreviewObject> _previewObjects;
        private string _lang = string.Empty;
        public List<PreviewObject> Preview { get { return _previewObjects; } }

        public ThreadService(List<FileToProcess> filesTorocess, string lang)
        {
            _filesToProcess = filesTorocess;
            _previewObjects = new List<PreviewObject>();
            _lang = lang;
        }

        public async Task StartService()
        {
            foreach (FileToProcess s in _filesToProcess)
            {
                var progress = new Progress<int>(percent =>
                {
                    if ((s.ProgressBar.Value + percent) > 100)
                        s.ProgressBar.Value = 100;
                    else
                        s.ProgressBar.Value = s.ProgressBar.Value + percent;

                });
                await Task.Run(async () =>
                {
                    var _cvService = OpenCVImageService.GetInstance();

                    if (SETTINGS.UseGoogleVision)
                    {
                        Annotate anotate = new Annotate();
                        _cvService.PrepareImageForGoogle(s.Path, progress, _lang);
                        
                        await anotate.GetText(_cvService.Rotated, _lang, "TEXT_DETECTION");
                        var response = anotate.Response;
                        PreviewObject p = new PreviewObject();
                        p.Path = s.Path;
                        p.Lang = _lang;
                        p.Img = _cvService.bmp;
                        p.Lines = MakeLinesFromWord(response);
                        ProcessLines(p, progress);
                        _previewObjects.Add(p);

                    }
                    else
                    {
                        TesseractService tess = new TesseractService(_lang);
                        _cvService.PrepareImage(s.Path, progress, _lang);
                        Mat image = _cvService.Rotated;
                        double ratioX = 1;
                        double ratioY = 1;
                        var id = CheckForPattern(tess, image, ref ratioX, ref ratioY);
                        if (id == -1)
                        {
                            PreviewObject p = tess.ProcessImage(image, progress);
                            p.Path = s.Path;
                            ProcessLines(p, progress);
                            _previewObjects.Add(p);
                        }
                        else
                        {
                            PreviewObject prew;
                            //nasiel som pattern tak idem podla neho
                            tess.CheckImageForPatternAndGetDataFromIt(image, GetKeysPossitions(id), progress, out prew, ratioX, ratioY);
                            prew.Path = s.Path;
                            _previewObjects.Add(prew);
                        }

                    }

                });

                s.ProgressBar.Value = 100;
                s.Button.Enabled = true;
            }
        }

        private int CheckForPattern(TesseractService tess, Mat image, ref double ratioX, ref double ratioY)
        {
            Database db = new Database();
            string SQL = "SELECT * FROM OCR_2018.dbo.T003_Pattern";
            SqlDataReader data = (SqlDataReader)db.Execute(SQL, CONSTANTS.Operation.SELECT);
            List<Pattern> patterns = new List<Pattern>();
            while (data.Read())
            {
                var pat = new Pattern();
                pat.Patter_ID = (int)data[0];
                pat.Lang = (string)data[1];
                pat.Resolution_X = (int)data[2];
                pat.Resolution_Y = (int)data[3];
                patterns.Add(pat);
            }
            data.Close();

            foreach (Pattern id in patterns)
            {
                PreviewObject p;
                ratioX = image.Width / id.Resolution_X;
                ratioY = image.Height / id.Resolution_Y;
                if (tess.CheckImageForPatternAndGetDataFromIt(image, GetKeysPossitions(id.Patter_ID, db, true), null, out p, ratioX, ratioY, true))
                {
                    return id.Patter_ID;
                }
            }

            return -1;

        }
        private List<PossitionOfWord> GetKeysPossitions(int id, Database db = null, bool test = false)
        {
            List<PossitionOfWord> list = new List<PossitionOfWord>();
            if (db == null)
            {
                db = new Database();
            }
            string SQL;
            if (test)
            {
                SQL = $"SELECT TOP 5 * FROM OCR_2018.dbo.T004_Possitions WHERE Pattern_ID = {id} AND (Word_Key NOT LIKE '%Meno%' OR Word_Key NOT LIKE '%Ulica%' OR " +
                    $"Word_Key NOT LIKE '%Psč%' OR Word_Key NOT LIKE '%Štát%') AND K_X != 0 AND K_Y != 0";

            }
            else
            {
                SQL = $"SELECT * FROM OCR_2018.dbo.T004_Possitions WHERE Pattern_ID = {id}";
            }
            SqlDataReader data = (SqlDataReader)db.Execute(SQL, CONSTANTS.Operation.SELECT);
            while (data.Read())
            {
                var pos = new PossitionOfWord();
                pos.Key = data[2].ToString();
                pos.Value = data[3].ToString();
                pos.KeyBounds = new System.Drawing.Rectangle((int)data[4], (int)data[5], (int)data[6], (int)data[7]);
                pos.ValueBounds = new System.Drawing.Rectangle((int)data[8], (int)data[9], (int)data[10], (int)data[11]);
                pos.DictionaryKey = data[12].ToString();
                list.Add(pos);
            }
            data.Close();
            db.Close();
            return list;
        }


        private List<TextLine> MakeLinesFromWord(AnnotateImageResponse response)
        {
            List<TextLine> lines = new List<TextLine>();
            for (int i = 1; i < response.TextAnnotations.Count; i++)
            {
                if (lines.Count == 0)
                {
                    TextLine l = new TextLine();
                    l.Words = new List<Classes.Word>();
                    GetDataFromResponse(response, lines, i, l);
                }
                else
                {
                    int bottom = response.TextAnnotations[i].BoundingPoly.Vertices[2].Y.Value;
                    var line = lines.Where(c => Math.Abs(c.Bounds.Bottom - bottom) <= 10).FirstOrDefault();
                    if (line == null)
                    {
                        TextLine l = new TextLine();
                        l.Words = new List<Classes.Word>();
                        GetDataFromResponse(response, lines, i, l);
                    }
                    else
                    {
                        Classes.Word w = new Classes.Word();

                        w.Text = response.TextAnnotations[i].Description;
                        w.Confidence = 100;
                        int x = response.TextAnnotations[i].BoundingPoly.Vertices[0].X.Value;
                        int y = response.TextAnnotations[i].BoundingPoly.Vertices[0].Y.Value;
                        int width = response.TextAnnotations[i].BoundingPoly.Vertices[1].X.Value - response.TextAnnotations[i].BoundingPoly.Vertices[0].X.Value;
                        int height = response.TextAnnotations[i].BoundingPoly.Vertices[2].Y.Value - response.TextAnnotations[i].BoundingPoly.Vertices[0].Y.Value;
                        w.Bounds = new System.Drawing.Rectangle(x, y, width, height);

                        line.Words.Add(w);
                    }

                }
            }

            lines.All(l =>
            {
                l.Words.Sort(delegate (Classes.Word w1, Classes.Word w2)
                {
                    return w1.Bounds.X.CompareTo(w2.Bounds.X);
                });
                int x = l.Words.FirstOrDefault().Bounds.X;
                int y = l.Words.FirstOrDefault().Bounds.Y;
                int x2 = l.Words.LastOrDefault().Bounds.Right;
                l.Text = GetTextForLine(l);
                l.Bounds = new System.Drawing.Rectangle(x,y,x2-x,l.Bounds.Bottom-y);
                return true;
            });

            return lines;
        }
        private string GetTextForLine(TextLine l)
        {
            string text = string.Empty;
            foreach (Classes.Word w in l.Words)
            {
                text += w.Text + " ";
            }
            return text.Trim();
        }

        private static void GetDataFromResponse(AnnotateImageResponse response, List<TextLine> lines, int i, TextLine l)
        {
            Classes.Word w = new Classes.Word();

            w.Text = response.TextAnnotations[i].Description;
            w.Confidence = 100;
            int x = response.TextAnnotations[i].BoundingPoly.Vertices[0].X.Value;
            int y = response.TextAnnotations[i].BoundingPoly.Vertices[0].Y.Value;
            int width = response.TextAnnotations[i].BoundingPoly.Vertices[1].X.Value - response.TextAnnotations[i].BoundingPoly.Vertices[0].X.Value;
            int height = response.TextAnnotations[i].BoundingPoly.Vertices[2].Y.Value - response.TextAnnotations[i].BoundingPoly.Vertices[0].Y.Value;
            w.Bounds = new System.Drawing.Rectangle(x, y, width, height);

            l.Words.Add(w);
            l.Bounds = w.Bounds;
            lines.Add(l);
        }

        private void ProcessLines(PreviewObject p, IProgress<int> progress)
        {
            using (DictionaryService dict = new DictionaryService())
            {
                dict.MakeObjectsFromLines(p, progress);
            }
        }

    }
}

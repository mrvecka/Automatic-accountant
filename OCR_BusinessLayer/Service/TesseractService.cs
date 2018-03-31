using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Tesseract;
using OCR_BusinessLayer.Classes;
using OpenCvSharp;
using Bakalarska_praca.Dictioneries;
using System.Linq;

namespace OCR_BusinessLayer.Service
{
    class TesseractService
    {
        private string _lang;
        private TessBaseAPI engine;
        private List<TextLine> _textLines;
        public string confidence;
        public string text;
        OpenCVImageService _cvService;

        public TesseractService(string lang)
        {
            _textLines = new List<TextLine>();
            _lang = lang;
        }

        public PreviewObject ProcessImage(Mat image, IProgress<int> progress)
        {
            PreviewObject p = new PreviewObject();
            Image img = null;
            try
            {
                using (engine = new TessBaseAPI(@".\tessdata", _lang, OcrEngineMode.TESSERACT_LSTM_COMBINED))
                {

                    try
                    {
                        img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
                    }
                    catch (NullReferenceException e)
                    {
                        throw new NullReferenceException(e.Message, e.InnerException);
                    }


                    engine.InitForAnalysePage();
                    engine.Init(null, _lang);
                    //engine.SetInputImage(pix);
                    engine.SetImage(new UIntPtr(BitConverter.ToUInt64(BitConverter.GetBytes(image.Data.ToInt64()), 0)), image.Size().Width, image.Size().Height, image.Channels(), (int)image.Step1());
                    engine.Recognize();
                    ResultIterator iterator = engine.GetIterator();

                    IterateFullPage(iterator, ref _textLines);

                    confidence = ((Double)(engine.MeanTextConf) / 100).ToString("P2");
                    iterator.Dispose();

                    p.Confidence = confidence;
                    p.Img = img;
                    p.Lines = _textLines;
                    p.Lang = _lang;

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
            progress.Report(40);
            if (img != null)
            {
                ProcessLines(p,progress);
            }

            return p;
        }


        public bool CheckImageForPatternAndGetDataFromIt(Mat image, List<PossitionOfWord> pos, IProgress<int> progress, out PreviewObject prew, double ratioX, double ratioY,
                                        bool checkPattern = false)
        {

            PreviewObject p = new PreviewObject();
            if (pos.Count > 0)
            {
                int step = 1;
                bool isPattern = true;
                p.ListOfKeyColumn = new List<Column>();
                try
                {
                    using (engine = new TessBaseAPI(@".\tessdata", _lang, OcrEngineMode.TESSERACT_LSTM_COMBINED,PageSegmentationMode.AUTO_OSD))
                    {                        

                        p.ListOfKeyPossitions = new List<PossitionOfWord>();
                        p.Lines = new List<TextLine>();

                        if (!checkPattern)
                        {

                            engine.InitForAnalysePage();
                            engine.Init(null, _lang);
                            //engine.SetInputImage(pix);
                            engine.SetImage(new UIntPtr(BitConverter.ToUInt64(BitConverter.GetBytes(image.Data.ToInt64()), 0)), image.Size().Width, image.Size().Height, image.Channels(), (int)image.Step1());
                            engine.Recognize();
                            ResultIterator iterator = engine.GetIterator();

                            IterateFullPage(iterator, ref _textLines);
                            iterator.Dispose();

                            progress.Report(20);

                            foreach (PossitionOfWord w in pos)
                            {
                                if (w.KeyBounds.Equals(w.ValueBounds))
                                    p.ListOfKeyColumn.Add(new Column(w.Value, w.ValueBounds.Left, w.ValueBounds.Right, w.ValueBounds.Bottom, w.ValueBounds.Top));


                                var line = _textLines.Where(l => (l.Bounds.Top > w.ValueBounds.Top) && (l.Bounds.Top > w.KeyBounds.Top)).FirstOrDefault();
                                string text = Common.GetWordsForColumn(new Column("",w.ValueBounds.Left,w.ValueBounds.Right,0,0),line);
                                w.Value = text.Trim(CONSTANTS.charsToTrimLineForpossition);
                                progress.Report(step);


                                p.ListOfKeyPossitions.Add(w);
                                p.Lines.AddRange(_textLines);
                            }

                        }
                        else
                        {
                            foreach (PossitionOfWord w in pos)
                            {                               
                                    OpenCvSharp.Rect rec;
                                    // ak kontrolujem pattern tak pozeram len kluce
                                    rec = new Rect(w.KeyBounds.X, w.KeyBounds.Y, w.KeyBounds.Width, w.KeyBounds.Height);


                                    rec.X -= CONSTANTS.PATTERN_CHECK_XY_PROXIMITY;
                                    rec.Y -= CONSTANTS.PATTERN_CHECK_XY_PROXIMITY;
                                    rec.Width += CONSTANTS.PATTERN_CHECK_WIDTHHEIGHT_PROXIMITY; //ak pozeram ci je to patern tak potrebujem co najmensie
                                    rec.Height += CONSTANTS.PATTERN_CHECK_WIDTHHEIGHT_PROXIMITY;

                                    //nastavim pozicie vzhladom na rozlisenie pretoze mozem dostat rovnaku fakturu s inym rozlisenim
                                    rec.X = (int)(rec.X * ratioX);
                                    rec.Y = (int)(rec.Y * ratioY);
                                    rec.Width = (int)(rec.Width * ratioX);
                                    rec.Height = (int)(rec.Height * ratioY);

                                    //osetrim pozicie aby som sa nedostal mimo obrazka
                                    if (image.Cols < rec.X + rec.Width)
                                        rec.Width -= (rec.X + rec.Width) - image.Cols;
                                    if (image.Rows < rec.Y + rec.Height)
                                        rec.Height -= (rec.Y + rec.Height) - image.Rows;
                                    Mat im = new Mat(image, rec);

                                    engine.InitForAnalysePage();
                                    engine.Init(null, _lang);
                                    //engine.SetInputImage(pix);
                                    engine.SetImage(new UIntPtr(BitConverter.ToUInt64(BitConverter.GetBytes(im.Data.ToInt64()), 0)), im.Size().Width, im.Size().Height, im.Channels(), (int)im.Step1());
                                    engine.Recognize();
                                    ResultIterator iterator = engine.GetIterator();

                                    IterateFullPage(iterator, ref _textLines);
                                    iterator.Dispose();
                                    bool pat = false;
                                    foreach (TextLine line in _textLines)
                                    {
                                        var s = Common.RemoveDiacritism(line.Text.Trim(CONSTANTS.charsToTrimLineForpossition));
                                        var k = Common.RemoveDiacritism(w.Key);
                                        if (SimilarityService.GetSimilarity(s,k) > 90 || s.Contains(k) || k.Contains(s))
                                        {
                                            pat = true;
                                        }
                                    
                                    }

                                    if (!pat)
                                    {
                                        isPattern = false;
                                            break;
                                    }

                                    w.Confidence = GetConfForLine(_textLines[0]);
                                    p.ListOfKeyPossitions.Add(w);
                                    p.Lines.AddRange(_textLines);
                                    _textLines.Clear();
                                
                            }

                        }

                        p.Confidence = string.Format("{0:N2}%", (engine.MeanTextConf) / 100);
                        p.Img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
                        p.Lang = _lang;

                    }

                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e.InnerException);
                }
                prew = p;
                return isPattern;
            }
            prew = p;
            return false;
        }

        /// <summary>
        /// Methode only for confidence
        /// </summary>
        /// <param name="img"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public double GetConfidenceForOrientation(Mat img, int angle)
        {
            double conf = 0;
            try
            {
                using (engine = new TessBaseAPI(@".\tessdata", _lang, OcrEngineMode.TESSERACT_LSTM_COMBINED))
                {
                    _cvService = OpenCVImageService.GetInstance();
                    _cvService.RotateImage(img, ref img, angle, 1);

                    engine.InitForAnalysePage();
                    engine.Init(null, _lang);
                    //engine.SetInputImage(pix);
                    engine.SetImage(new UIntPtr(BitConverter.ToUInt64(BitConverter.GetBytes(img.Data.ToInt64()), 0)), img.Size().Width, img.Size().Height, img.Channels(), (int)img.Step1());
                    engine.Recognize();
                    conf = engine.MeanTextConf;

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }


            return conf;
        }

        private void IterateFullPage(ResultIterator iter, ref List<TextLine> _textLines)
        {
            int left, top, right, bottom;


            StringBuilder ss = new StringBuilder(string.Empty);
            PageIteratorLevel level = PageIteratorLevel.RIL_TEXTLINE;
            string t;

            do
            {
                TextLine l = new TextLine();
                t = iter.GetUTF8Text(level);

                ss.Append(t);
                iter.BoundingBox(level, out left, out top, out right, out bottom);

                l.Bounds = new Rectangle(left, top, right - left, bottom - top);

                l.Text = t ?? string.Empty;


                level = PageIteratorLevel.RIL_WORD;
                l.Words = new List<Word>();
                do
                {
                    Word w = new Word();
                    iter.BoundingBox(level, out left, out top, out right, out bottom);
                    w.Text = iter.GetUTF8Text(level);
                    w.Confidence = iter.Confidence(level);
                    w.Bounds = new Rectangle(left, top, right - left, bottom - top);
                    l.Words.Add(w);
                    if (iter.IsAtFinalElement(PageIteratorLevel.RIL_TEXTLINE, PageIteratorLevel.RIL_WORD))
                        break;
                } while (iter.Next(level));
                level = PageIteratorLevel.RIL_TEXTLINE;

                ss.Append(System.Environment.NewLine);
                _textLines.Add(l);

            } while (iter.Next(level));



            text = ss.ToString();

        }

        private void ProcessLines(PreviewObject p,IProgress<int> progress)
        {
            using (DictionaryService dict = new DictionaryService())
            {
                dict.MakeObjectsFromLines(p,progress);
            }
        }

        private string GetConfForLine(TextLine line)
        {
            float conf = 0;
            int count = 0;
            foreach (var w in line.Words)
            {
                conf += w.Confidence;
                count++;
            }

            return ((float)(conf / count)).ToString("P2");
        }

        private string GetKey(string key)
        {
            foreach (var col in Dictionary.GetInstance().columns)
            {
                if (key.Contains(col.Key))
                {
                    key.Replace(col.Key, "");
                    break;
                }
            }
            return Common.RemoveDiacritism(key);
        }

    }




}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Tesseract;
using OCR_BusinessLayer.Classes;
using OpenCvSharp;

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

        public PreviewObject ProcessImage(FileToProcess file, IProgress<int> progress)
        {
            PreviewObject p = new PreviewObject();
            Image img = null;
            using (engine = new TessBaseAPI(@".\tessdata", _lang,OcrEngineMode.TESSERACT_LSTM_COMBINED))
            {

                _cvService = OpenCVImageService.GetInstance();
                _cvService.PrepareImage(file.Path);
                Mat image = _cvService.Rotated;

                img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);


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
            progress.Report(40);
            if (img != null)
            {
                ProcessLines(p,file,progress);
            }

            return p;
        }


        public bool CheckImageForPattern(FileToProcess file,List<PossitionOfWord> pos, bool checkPattern = false)
        {
            bool isPattern = true;
            PreviewObject p = new PreviewObject();
            using (engine = new TessBaseAPI(@".\tessdata", _lang, OcrEngineMode.TESSERACT_LSTM_COMBINED))
            {

                _cvService = OpenCVImageService.GetInstance();
                _cvService.PrepareImage(file.Path);
                Mat image = _cvService.Rotated;

                
                foreach (PossitionOfWord w in pos)
                {
                    OpenCvSharp.Rect rec = new Rect(w.KeyBounds.X, w.KeyBounds.Y, w.KeyBounds.Width, w.KeyBounds.Height);
                    rec.X -= 10;
                    rec.Y -= 10;
                    rec.Width += 20; //ak pozram ci je to patern tak potrebujem co najmensie
                    rec.Height += 20;
                    
                    Mat im = new Mat(image, rec);

                    engine.InitForAnalysePage();
                    engine.Init(null, _lang);
                    //engine.SetInputImage(pix);
                    engine.SetImage(new UIntPtr(BitConverter.ToUInt64(BitConverter.GetBytes(im.Data.ToInt64()), 0)), im.Size().Width, im.Size().Height, im.Channels(), (int)im.Step1());
                    engine.Recognize();
                    ResultIterator iterator = engine.GetIterator();

                    IterateFullPage(iterator, ref _textLines);
                    iterator.Dispose();

                    if (checkPattern && !_textLines[0].Text.Trim(CONSTANTS.charsToTrimLine).Equals(w.Key))
                    {
                        isPattern = false;
                    }
                    else if (!checkPattern)
                    {
                        w.Value = _textLines[0].Text;
                    }
                    else
                    {

                    }
                    _textLines.Clear();
                }
                p.Img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
                p.Confidence = ((Double)(engine.MeanTextConf) / 100).ToString("P2");
                p.Lines = _textLines;
                p.Lang = _lang;

            }
            return isPattern;
        }

        private void IterateFullPage(ResultIterator iter, ref List<TextLine> _textLines)
        {
            int left, top, right, bottom;


            StringBuilder ss = new StringBuilder("");
            PageIteratorLevel level = PageIteratorLevel.RIL_TEXTLINE;
            string t;
            do
            {
                TextLine l = new TextLine();
                t = iter.GetUTF8Text(level);
                
                ss.Append(t);
                iter.BoundingBox(level, out left, out top, out right, out bottom);

                l.Bounds = new Rectangle(left, top, right - left, bottom - top);
                    
                l.Text = t ?? "";

                
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

        private void ProcessLines(PreviewObject p,FileToProcess file, IProgress<int> progress)
        {
            using (DictionaryService dict = new DictionaryService())
            {
                dict.MakeObjectsFromLines(p,file,progress);
            }
        }

    }




}

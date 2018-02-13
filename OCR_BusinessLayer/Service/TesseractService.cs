using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Tesseract;
using OCR_BusinessLayer.Classes;

namespace OCR_BusinessLayer.Service
{
    class TesseractService
    {
        private string _lang;
        private TessBaseAPI engine;
        private List<TextLine> _textLines;
        public string confidence;
        public string text;

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
                            
                Mat rotated = GetMatImageForTesseract(file.path);

                Bitmap bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(rotated);
                img = bmp;

                engine.InitForAnalysePage();
                engine.Init(null, _lang);
                //engine.SetInputImage(pix);
                engine.SetImage(new UIntPtr(BitConverter.ToUInt64(BitConverter.GetBytes(rotated.Data.ToInt64()), 0)), rotated.Size().Width, rotated.Size().Height, rotated.Channels(), (int)rotated.Step1());
                engine.Recognize();
                ResultIterator iterator = engine.GetIterator();

                IterateFullPage(iterator, ref _textLines);

                confidence = ((Double)(engine.MeanTextConf) / 100).ToString("P2");
                iterator.Dispose();

                p.confidence = confidence;
                p.img = img;
                p.Lines = _textLines;
                p.lang = _lang;

            }
            progress.Report(40);
            if (img != null)
            {
                ProcessLines(p,file,progress);
            }

            return p;
        }

        private Mat GetMatImageForTesseract(string path)
        {
            Mat original = Cv2.ImRead(path);
            Mat rotated = new Mat();
            RotateImage(original, rotated, 0, 1);
            return rotated;
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
                l.text = t;

                
                level = PageIteratorLevel.RIL_WORD;
                l.Words = new List<Word>();
                do
                {
                    Word w = new Word();
                    iter.BoundingBox(level, out left, out top, out right, out bottom);
                    w.Text = iter.GetUTF8Text(level);
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
        private void RotateImage(Mat src, Mat dst, double angle, double scale)
        {
            var imageCenter = new Point2f(src.Cols / 2f, src.Rows / 2f);
            var rotationMat = Cv2.GetRotationMatrix2D(imageCenter, angle, scale);
            Cv2.WarpAffine(src, dst, rotationMat, src.Size());
        }
    }




}

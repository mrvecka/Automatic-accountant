using Bakalarska_praca.Classes;
using Leptonica;
using OpenCvSharp;
using OpenCvSharp.UserInterface;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace Bakalarska_praca.Service
{
    class TesseractService
    {
        private string _lang;
        private TessBaseAPI engine;
        private List<TextLine> _blocks;
        private List<TextLine> _paras;
        private List<TextLine> _textLines;
        private List<TextLine> _words;
        private List<TextLine> _symbols;
        public string confidence;
        public string text;

        public TesseractService(List<TextLine> blocks, List<TextLine> paras, List<TextLine> textLines, List<TextLine> words, List<TextLine> symbols, string lang)
        {
            _blocks = blocks;
            _paras = paras;
            _textLines = textLines;
            _words = words;
            _symbols = symbols;
            _lang = lang;


        }

        public Image ProcessImage(string path,Image img)
        {

            float sim = SimilarityService.GetSimilarity("test","tes");

            using (engine = new TessBaseAPI(@".\tessdata", _lang,OcrEngineMode.TESSERACT_LSTM_COMBINED))
            {
                            
                Mat rotated = GetMatImageForTesseract(path);

                Bitmap bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(rotated);
                img = bmp;

                engine.InitForAnalysePage();
                engine.Init(null, _lang);
                //engine.SetInputImage(pix);
                engine.SetImage(new UIntPtr(BitConverter.ToUInt64(BitConverter.GetBytes(rotated.Data.ToInt64()), 0)), rotated.Size().Width, rotated.Size().Height, rotated.Channels(), (int)rotated.Step1());
                engine.Recognize();
                ResultIterator iterator = engine.GetIterator();

                IterateFullPage(iterator, ref _blocks, ref _paras, ref _textLines, ref _words, ref _symbols);

                confidence = ((Double)(engine.MeanTextConf) / 100).ToString("P2");
                iterator.Dispose();

            }

            //DRAW
            Pen myPen;
            System.Drawing.Graphics newGraphics = System.Drawing.Graphics.FromImage(img);
            //if (_blocks != null)
            //{
            //    myPen = new Pen(Color.Blue, 3);
            //    foreach (TextLine block in _blocks)
            //    {
            //        newGraphics.DrawRectangle(myPen, block.Bounds);
            //    }
            //}
            //if (_paras != null)
            //{
            //    myPen = new Pen(Color.Green, 2);
            //    foreach (TextLine para in _paras)
            //    {
            //        newGraphics.DrawRectangle(myPen, para.Bounds);
            //    }
            //}
            if (_textLines != null)
            {
                Pen myPenline = new Pen(Color.Violet, 1.5f);
                Pen myPenword = new Pen(Color.Blue, 1.5f);

                foreach (TextLine textLine in _textLines)
                {
                    newGraphics.DrawRectangle(myPenline, textLine.Bounds);
                    foreach (Word w in textLine.Words)
                    {
                        newGraphics.DrawRectangle(myPenword, w.Bounds);
                    }
                }
            }
            //if (_words != null)
            //{
            //    myPen = new Pen(Color.Red, 1);
            //    foreach (TextLine word in _words)
            //    {
            //        newGraphics.DrawRectangle(myPen, word.Bounds);
            //    }
            //}
            //if (_symbols != null)
            //{
            //    myPen = new Pen(Color.DarkBlue, 0.5f);
            //    foreach (TextLine symbol in _symbols)
            //    {
            //        newGraphics.DrawRectangle(myPen, symbol.Bounds);
            //    }
            //}

            ProcessLines(img.Width);

            return img;
        }

        private Mat GetMatImageForTesseract(string path)
        {
            Mat original = Cv2.ImRead(path);
            Mat rotated = new Mat();
            RotateImage(original, rotated, 0, 1);
            return rotated;
        }

        private void IterateFullPage(ResultIterator iter, ref List<TextLine> _blocks, ref List<TextLine> _paras, ref List<TextLine> _textLines, ref List<TextLine> _words, ref List<TextLine> _symbols)
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

        private void ProcessLines(int width)
        {
            using (DictionaryService dict = new DictionaryService())
            {
            dict.MakeObjectsFromLines(_textLines,width);
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

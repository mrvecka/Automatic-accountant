using Emgu.CV;
using Emgu.CV.Structure;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Service
{

    public class OpenCVImageService
    {
        private static OpenCVImageService _service;
        private OpenCvSharp.Mat _original;
        private Bitmap cBmp;
        private double cAlphaStart = -20;
        private double cAlphaStep = 0.2;
        private int cSteps = 40 * 5;
        private double[] cSinA;
        private double[] cCosA;
        private double cDMin;
        private double cDStep = 1;
        private int cDCount;
        private int[] cHMatrix;


        public OpenCvSharp.Mat Rotated;
        public Bitmap bmp;
        public static OpenCVImageService GetInstance()
        {
            if (_service == null)
                _service = new OpenCVImageService();

            return _service;
        }
        private OpenCVImageService() { }

        public void PrepareImage(string path)
        {
            _original = Cv2.ImRead(path);
            //using (var window = new Window("original", image: _original, flags: WindowMode.AutoSize))
            //{
            //    Cv2.WaitKey();
            //}
            Rotated = GetRotatedImage();
            //using (var window = new Window("rotated", image: Rotated, flags: WindowMode.AutoSize))
            //{
            //    Cv2.WaitKey();
            //}
            OpenCvSharp.Mat newImage = new OpenCvSharp.Mat();

            Cv2.Threshold(_original, newImage, 127, 255, ThresholdTypes.Tozero);
            //using (var window = new Window("tresshold", image: newImage, flags: WindowMode.AutoSize))
            //{
            //    Cv2.WaitKey();
            //}
            OpenCvSharp.Mat a = new OpenCvSharp.Mat();
            Cv2.Erode(newImage, a, new OpenCvSharp.Mat());
            //using (var window = new Window("erode", image: a, flags: WindowMode.AutoSize))
            //{
            //    Cv2.WaitKey();
            //}

            cBmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(a);
            DeskewImage(ref a);
            bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(a);

            //if (IsUpSideDownBitmap(bmp))
            //{
            //    RotateImage(a, ref a, 180, 1);
            //}
    
            //using (var window = new Window("isupsidedown", image: a, flags: WindowMode.AutoSize))
            //{
            //    Cv2.WaitKey();
            //}

            Rotated = a;
            bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotated);
        }

        private OpenCvSharp.Mat GetRotatedImage()
        {
            OpenCvSharp.Mat rotated = new OpenCvSharp.Mat();
            RotateImage(_original, ref rotated, 0, 1);
            return rotated;
        }

        private void RotateImage(OpenCvSharp.Mat src, ref OpenCvSharp.Mat dst, double angle, double scale)
        {
            var imageCenter = new Point2f(src.Cols / 2f, src.Rows / 2f);
            var rotationMat = Cv2.GetRotationMatrix2D(imageCenter, angle, scale);
            Cv2.WarpAffine(src, dst, rotationMat, src.Size());
        }

        private void DeskewImage(ref OpenCvSharp.Mat a)
        {
            double angle = GetSkewAngle();
            RotateImage(a, ref a, angle, 1);

            //using (var window = new Window("erode", image: a, flags: WindowMode.AutoSize))
            //{
            //    Cv2.WaitKey();
            //}

        }

        public double GetSkewAngle()
        {
            HougLine[] hl;
            int i;
            double sum = 0;
            int count = 0;

            //' Hough Transformation
            Calc();
            //' Top 20 of the detected lines in the image.
            hl = GetTop(20);
            //' Average angle of the lines
            for (i = 0; i < 19; i++)
            {
                sum += hl[i].Alpha;
                count += 1;
            }
            return sum / count;
        }
        private HougLine[] GetTop(int Count)
        {
            HougLine[] hl;
            int j;
            HougLine tmp;
            int AlphaIndex, dIndex;
            hl = new HougLine[Count];
            for (int i = 0; i < Count; i++)
            {
                hl[i] = new HougLine();
            }
            for (int i = 0; i < cHMatrix.Length - 1; i++)
            {
                if (cHMatrix[i] > hl[Count - 1].Count)
                {
                    hl[Count - 1].Count = cHMatrix[i];
                    hl[Count - 1].Index = i;
                    j = Count - 1;
                    while (j > 0 && hl[j].Count > hl[j - 1].Count)
                    {
                        tmp = hl[j];
                        hl[j] = hl[j - 1];
                        hl[j - 1] = tmp;
                        j -= 1;
                    }
                }
            }
            for (int i = 0; i < Count; i++)
            {
                dIndex = hl[i].Index / cSteps;
                AlphaIndex = hl[i].Index - dIndex * cSteps;
                hl[i].Alpha = GetAlpha(AlphaIndex);
                hl[i].d = dIndex + cDMin;
            }
            return hl;
        }
        private void Calc()
        {
            int x;
            int y;
            int hMin = cBmp.Height / 4;
            int hMax = cBmp.Height * 3 / 4;
            Init();
            for (y = hMin; y < hMax; y++)
            {
                for (x = 1; x < cBmp.Width - 2; x++)
                {
                    //' Only lower edges are considered.
                    if (IsBlack(x, y) == true)
                    {
                        if (IsBlack(x, y + 1) == false)
                        {
                            Calc(x, y);
                        }
                    }
                }
            }
        }
        private void Calc(int x, int y)
        {
            double d;
            int dIndex;
            int Index;
            for (int alpha = 0; alpha < cSteps - 1; alpha++)
            {
                d = y * cCosA[alpha] - x * cSinA[alpha];
                dIndex = (int)CalcDIndex(d);
                Index = dIndex * cSteps + alpha;

                try
                {
                    cHMatrix[Index] += 1;
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
        private double CalcDIndex(double d)
        {
            return Convert.ToInt32(d - cDMin);
        }
        private bool IsBlack(int x, int y)
        {
            Color c;
            double luminance;
            c = cBmp.GetPixel(x, y);
            luminance = (c.R * 0.299) + (c.G * 0.587) + (c.B * 0.114);
            return luminance < 140;
        }
        private void Init()
        {
            double angle;
            //' Precalculation of sin and cos.
            cSinA = new double[cSteps - 1];
            cCosA = new double[cSteps - 1];
            for (int i = 0; i < cSteps - 1; i++)
            {
                angle = GetAlpha(i) * Math.PI / 180.0;
                cSinA[i] = Math.Sin(angle);
                cCosA[i] = Math.Cos(angle);
            }
         //' Range of d
         cDMin = -cBmp.Width;
            cDCount = (int)(2 * (cBmp.Width + cBmp.Height) / cDStep);
            cHMatrix = new int[cDCount * cSteps];
        }
        public double GetAlpha(int Index)
        {
            return cAlphaStart + Index * cAlphaStep;
        }

        private bool IsUpSideDownBitmap(Bitmap bmp1)
        {
            Bitmap bmp = (Bitmap)bmp1.Clone();
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            return bmp1 != null && bmpData.Stride > 0;




        }

    }

    public class HougLine
    {
        //' Count of points in the line.
        public int Count;
        //' Index in Matrix.
        public int Index;
        //' The line is represented as all x,y that solve y*cos(alpha)-x*sin(alpha)=d
        public double Alpha;
        public double d;
    }
}

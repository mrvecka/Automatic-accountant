using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Service
{

    class OpenCVImageService
    {

    public List<LineSegmentPoint> verticalLines;
    public List<LineSegmentPoint> horizontalLines;

        public void ImagePreProcess(Mat grayImg,Bitmap originalBmp)
        {
            verticalLines = new List<LineSegmentPoint>();
            horizontalLines = new List<LineSegmentPoint>();
            OpenCvSharp.Mat edges = new OpenCvSharp.Mat();
            Cv2.Canny(grayImg, edges, 95, 100);

            System.Drawing.Graphics newGraphics = System.Drawing.Graphics.FromImage(originalBmp);
            Pen myPen = new Pen(Color.Red, 3);

            LineSegmentPoint[] segHoughP = Cv2.HoughLinesP(edges, 1, Math.PI / 180, 100, 100, 10);
            OpenCvSharp.Mat imgOutP = grayImg.EmptyClone();

            foreach (LineSegmentPoint s in segHoughP)
            {
                if (s.P1.X == s.P2.X)
                {
                    if (s.P1.X > 40 && s.P2.Y > 40 && (s.P1.Y - s.P2.Y) > 100)
                    {
                        verticalLines.Add(s);
                        continue;
                    }
                }
                else if (s.P1.Y == s.P2.Y)
                {
                    if (s.P2.Y > 40 && s.P1.X > 40 && (s.P2.X - s.P1.X) > 200)
                    {
                        horizontalLines.Add(s);
                        continue;
                    }
                }

            }

            //Bitmap m = imgOutP.ToBitmap();
            //m.Save(@"lines.png");
        }

    }
}

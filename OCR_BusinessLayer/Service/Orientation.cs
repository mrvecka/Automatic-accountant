using OpenCvSharp;
using System.Threading;

namespace OCR_BusinessLayer.Service
{
    public class Orientation
    {
        public double Confidence { get; set; }
        public bool Finished { get; set; } = false;
        public int Angle { get; set; }
        private Mat _img;
        private string _lang;
        public Orientation(int angle, Mat img, string lang)
        {
            Angle = angle;
            _img = img;
            _lang = lang;
            Thread newThread = new Thread(new ThreadStart(GetConfidence));
            newThread.Start();
        }
        public void GetConfidence()
        {
            TesseractService tess = new TesseractService(_lang);
            Confidence = tess.GetConfidenceForOrientation(_img,Angle);
            Finished = true;
        }

    }
}

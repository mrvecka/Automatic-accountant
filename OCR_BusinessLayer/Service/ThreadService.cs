using OCR_BusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Service
{
    public class ThreadService
    {
        private List<FileToProcess> _filesToProcess;
        private List<PreviewObject> _previewObjects;
        private string _lang = "";
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
                TesseractService tess = new TesseractService(_lang);
                var progress = new Progress<int>(percent =>
                {
                    s.progressBar.Value = s.progressBar.Value + percent;
                });
                await Task.Run(() =>
                {
                    PreviewObject p = tess.ProcessImage(s,progress);
                    p.path = s.path;
                    _previewObjects.Add(p);

                });

                s.progressBar.Value = 100;
                s.button.Enabled = true;
            }
        }



    }
}

using System.Windows.Forms;

namespace OCR_BusinessLayer.Classes
{
    public class FileToProcess
    {
        public string Path;
        public Button Button;
        public ProgressBar ProgressBar;
        public Label Coenfidence;

        public FileToProcess(string ppath, ProgressBar pprogress, Button pbutton)
        {
            Path = ppath;
            ProgressBar = pprogress;
            Button = pbutton;
        }
    }
}

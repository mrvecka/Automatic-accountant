using System.Windows.Forms;

namespace OCR_BusinessLayer.Classes
{
    public class FileToProcess
    {
        public string path;
        public Button button;
        public ProgressBar progressBar;
        public Label coenfidence;

        public FileToProcess(string ppath, ProgressBar pprogress, Button pbutton)
        {
            path = ppath;
            progressBar = pprogress;
            button = pbutton;
        }
    }
}

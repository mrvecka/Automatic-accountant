using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace OCR_BusinessLayer.Service
{
    public class FileService
    {


        public static List<string> FindFiles(string path)
        {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                List<string> files = new List<string>();
                var filter = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
                foreach (var f in filter)
                {
                    files.AddRange(Directory.GetFiles(path, String.Format("*.{0}",f), SearchOption.AllDirectories));
                }

                return files;
            }
            return null;
        }

        public static string FindDatabase()
        {
            string s = Environment.CurrentDirectory;
            string[] path = Directory.GetFiles(s, String.Format("???_????.mdb"), SearchOption.TopDirectoryOnly);
            if (path.Length == 0)
            {
                MessageBox.Show("Database not found!");
                return "";
            }
            else
            {
                return path[0];
            }


        }
    }


   


}

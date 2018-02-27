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
            var filter = new string[] {"jpg", "jpeg", "png", "gif", "tiff", "bmp", "pdf" };
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                List<string> files = new List<string>();
                foreach (var f in filter)
                {
                    files.AddRange(Directory.GetFiles(path, String.Format("*.{0}", f), SearchOption.AllDirectories));
                }

                return files;
            }
            if (File.Exists(path))
            {
                List<string> files = new List<string>();

                var p = path.Substring(path.LastIndexOf('.')+1);
                foreach (var s in filter)
                {
                    if (s.Equals(p))
                    {
                        files.Add(path);
                        return files;
                    }
                }

                return null;
            }
            return null;
        }

    }





}

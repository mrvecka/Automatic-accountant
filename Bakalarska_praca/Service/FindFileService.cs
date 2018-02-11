using System;
using System.Collections.Generic;
using System.IO;

namespace Bakalarska_praca.Service
{
    public class FindFileService
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
    }


   


}

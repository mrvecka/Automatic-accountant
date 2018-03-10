using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace OCR_BusinessLayer.Service
{
    public class FileService
    {


        public static List<string> FindFiles(string path, string[] filter)
        {
            if (CheckForPermission(path))
            {
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

                    var p = path.Substring(path.LastIndexOf('.') + 1);
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
            }
            return null;
        }
        public static List<string> FindTrainedData(string path, string[] filter)
        {
            var data = FindFiles(path, filter);
            
            List<string> final = new List<string>();
            if (data == null)
                return final;

            foreach (string lang in data)
            {
                var s = lang.Substring(lang.LastIndexOf('\\') + 1);
                if (s.Length == 15)
                {
                    s = s.Substring(0, 3);
                    if (!final.Contains(s))
                        final.Add(s);
                }
            }
            return final;
        }
        private static bool CheckForPermission(string path)
        {
            try
            {
                using (FileStream fstream = new FileStream(path + ".txt", FileMode.Create))
                using (TextWriter writer = new StreamWriter(fstream))
                {
                    writer.WriteLine("sometext");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return false;
            }
            File.Delete(path + ".txt");
            return true;
        }

    }





}

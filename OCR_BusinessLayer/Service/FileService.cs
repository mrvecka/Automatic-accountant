using OCR_BusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        public static bool GenerateTxtFiles(List<PreviewObject> files)
        {
            foreach (PreviewObject path in files)
            {
                GenerateTxtFile(path);
            }
            return true;
        }

        public static bool GenerateTxtFile(PreviewObject item)
        {
            string tab = "\t";
            string newLine = "\n";
            Client dod = item.Clients.Where(d => d.ClientID == "Dodávateľ").First();
            Client odb = item.Clients.Where(d => d.ClientID == "Odberateľ").First();
            Client pos = item.Clients.Where(d => (d.ClientID == "Poštová adresa")|| (d.ClientID == "Adresa") || (d.ClientID == "Korešpondenčná adresa")).First();
            Client kon = item.Clients.Where(d => d.ClientID == "Konečný príjemca").First();

            if (dod == null) dod = new Client();
            if (odb == null) odb = new Client();
            if (pos == null) pos = new Client();
            if (kon == null) kon = new Client();

            string date = item.Evidence.DateOfCreate == "" ? item.Evidence.DocumentCreateDate : item.Evidence.DateOfCreate; 
            string textToFile = $"R00{tab}T01{tab}{dod.Name}{tab}{dod.ICO}{tab}{dod.Street}{tab}{dod.PSC}{tab}{dod.City}{newLine}";
            textToFile += $"R01{tab}{item.Evidence.OrderNumber}{tab}{odb.Name}{tab}{odb.ICO}{tab}{date}{tab}{item.Evidence.DateOfPayment}{tab}{item.Evidence.DateOfTax}{tab}";//DUZP
            textToFile += $"{item.Evidence.BaseLower}{tab}{item.Evidence.BaseHeigher}{tab}{item.Evidence.BaseZero}{tab}{item.Evidence.BaseNotContain}{tab}{item.Evidence.RateLower}{tab}{item.Evidence.RateHeigher}{tab}{item.Evidence.AmountLower}{tab}{item.Evidence.AmountHeigher}{tab}{tab}{item.Evidence.Amount}{tab}{tab}{tab}{tab}{tab}{tab}{tab}{tab}";//prevadzka patri pred posledny tab
            textToFile += $"{odb.Street}{tab}{odb.PSC}{tab}{odb.City}{tab}{odb.DIC}{tab}{tab}{tab}{tab}{tab}{tab}{item.Evidence.EvidenceNumber}{tab}";//cislo objednavky
            textToFile += $"{item.Evidence.CreatorName}{tab}{item.Evidence.KonstSymbol}{tab}{item.Evidence.VariabilSymbol}{tab}{item.Evidence.SpecSymbol}{tab}";//specificky symbol
            textToFile += $"{item.Evidence.RefundMethode}{tab}{item.Evidence.Transport}{tab}EUR{tab}1{tab}1{tab}{item.Evidence.Amount}{tab}{odb.State}{tab}{odb.ICDPHStateCode}{tab}{odb.ICDPH}{tab}";//49 IC DPH
            textToFile += $"{dod.AccountNumber}{tab}{dod.Bank}{tab}{odb.State}{tab}{tab}{tab}{dod.SWIFT}{tab}{dod.IBAN}{tab}{dod.ICDPHStateCode}{tab}{dod.ICDPH}{tab}{dod.State}{tab}";//60 dodavatel stat
            textToFile += $"{tab}{tab}{tab}{tab}{tab}{tab}{tab}{tab}{item.Evidence.DateOfCreate}{tab}{tab}{item.Evidence.VariabilSymbol}{tab}";// 71 variabiny symbol
            textToFile += $"{tab}{pos.Name}{tab}{tab}{tab}{pos.Street}{tab}{pos.PSC}{tab}{pos.City}{tab}{tab}{tab}{tab}{item.Evidence.CreatorName}{tab}{dod.Phone}{tab}";// 84 telefon
            textToFile += $"{tab}{tab}{odb.IBAN}{tab}{odb.AccountNumber}{newLine}";

            CreateFile(Common.ModifyPath(item.Path,"txt"),textToFile);

            return true;
        }

        private static void CreateFile(string path,string text)
        {
            System.IO.File.WriteAllText(path, text);
        }


    }





}

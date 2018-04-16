using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Classes.Client;
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
        private static string tab = "\t";
        private static string newLine = Environment.NewLine;
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
            string textToFile = string.Empty;
            int count = 0;
            foreach (PreviewObject item in files)
            {
                Client dod = new Client();
                Client odb = new Client();
                Client pos = new Client();
                Client kon = new Client();

                GetClients(item, ref dod, ref odb, ref pos, ref kon);

                if (count == 0)
                    textToFile = $@"R00{tab}T01{tab}{dod?.Name}{tab}{dod?.ICO}{tab}{dod?.Street}{tab}{dod?.PSC}{tab}{dod?.City}{newLine}";
                count++;
                textToFile += GenerateTxtFile(item,dod,odb,pos,kon);
            }

            CreateFile($@".\..\..\..\Export\invoice{DateTime.Now.ToLongDateString()}.txt", textToFile);
            return true;
        }

        public static string GenerateTxtFile(PreviewObject item, Client dod, Client odb, Client pos, Client kon)
        {            

            string date = item?.Evidence?.DateOfCreate == "" ? item?.Evidence?.DocumentCreateDate : item?.Evidence?.DateOfCreate;
          
            string textToFileR00 = $"R01{tab}{item?.Evidence?.OrderNumber}{tab}{odb?.Name}{tab}{odb?.ICO}{tab}{date}{tab}{item?.Evidence?.DateOfPayment}{tab}{item?.Evidence?.DateOfTax}{tab}";//DUZP
            textToFileR00 += $"{item?.Evidence?.BaseLower}{tab}{item?.Evidence?.BaseHigher}{tab}{item?.Evidence?.BaseZero}{tab}{item?.Evidence?.BaseNotContain}{tab}{item?.Evidence?.RateLower}{tab}{item?.Evidence?.RateHigher}{tab}{item?.Evidence?.AmountLower}{tab}{item?.Evidence?.AmountHigher}{tab}{tab}{item?.Evidence?.Amount}{tab}{tab}{tab}{tab}{tab}{tab}{tab}{tab}";//prevadzka patri pred posledny tab
            textToFileR00 += $"{odb?.Street}{tab}{odb?.PSC}{tab}{odb?.City}{tab}{odb?.DIC}{tab}{tab}{tab}{tab}{tab}{tab}{item?.Evidence?.EvidenceNumber}{tab}";//cislo objednavky
            textToFileR00 += $"{item?.Evidence?.CreatorName}{tab}{item?.Evidence?.KonstSymbol}{tab}{item?.Evidence?.VariabilSymbol}{tab}{item?.Evidence?.SpecSymbol}{tab}";//specificky symbol
            textToFileR00 += $"{item?.Evidence?.RefundMethode}{tab}{item?.Evidence?.Transport}{tab}EUR{tab}1{tab}1{tab}{item?.Evidence?.Amount}{tab}{odb?.State}{tab}{odb?.ICDPHStateCode}{tab}{odb?.ICDPH}{tab}";//49 IC DPH
            textToFileR00 += $"{dod?.AccountNumber}{tab}{dod?.Bank}{tab}{odb?.State}{tab}{tab}{tab}{dod?.SWIFT}{tab}{dod?.IBAN}{tab}{dod?.ICDPHStateCode}{tab}{dod?.ICDPH}{tab}{dod?.State}{tab}";//60 dodavatel stat
            textToFileR00 += $"{tab}{tab}{tab}{tab}{tab}{tab}{tab}{tab}{item?.Evidence?.DateOfCreate}{tab}{tab}{item?.Evidence?.VariabilSymbol}{tab}";// 71 variabiny symbol
            textToFileR00 += $"{tab}{pos?.Name}{tab}{tab}{tab}{pos?.Street}{tab}{pos?.PSC}{tab}{pos?.City}{tab}{tab}{tab}{tab}{item?.Evidence?.CreatorName}{tab}{dod?.Phone}{tab}";// 84 telefon
            textToFileR00 += $@"{tab}{tab}{odb?.IBAN}{tab}{odb?.AccountNumber}{newLine}";

            

            return textToFileR00;
        }

        public static bool GenerateTxtFile(PreviewObject item)
        {
            Client dod = new Client();
            Client odb = new Client();
            Client pos = new Client();
            Client kon = new Client();
            GetClients(item, ref dod, ref odb, ref pos, ref kon);

            string date = item?.Evidence?.DateOfCreate == "" ? item?.Evidence?.DocumentCreateDate : item?.Evidence?.DateOfCreate;

            string textToFileR00 = $@"R00{tab}T01{tab}{dod?.Name}{tab}{dod?.ICO}{tab}{dod?.Street}{tab}{dod?.PSC}{tab}{dod?.City}{newLine}";
            textToFileR00 += GenerateTxtFile(item, dod, odb, pos, kon);
            CreateFile($@".\..\..\..\Export\invoice{DateTime.Now.ToLongDateString()}.txt", textToFileR00);

            return true;


        }

        private static void GetClients(PreviewObject item, ref Client dod, ref Client odb, ref Client pos, ref Client kon)
        {
            foreach (Client c in item.Clients)
            {
                if (Common.RemoveDiacritism(c.ClientID).Equals("Dodavatel"))
                {
                    dod = c;
                    continue;
                }
                else if (Common.RemoveDiacritism(c.ClientID).Equals("Odberatel"))
                {
                    odb = c;
                    continue;
                }
                else if (Common.RemoveDiacritism(c.ClientID).Equals("Postova adresa") || Common.RemoveDiacritism(c.ClientID).Equals("Adresa") || Common.RemoveDiacritism(c.ClientID).Equals("Korespondencna adresa"))
                {
                    pos = c;
                    continue;
                }
                else if (Common.RemoveDiacritism(c.ClientID).Equals("Konecny prijemca"))
                {
                    kon = c;
                    continue;
                }
            }

            if (dod == null) dod = new Client();
            if (odb == null) odb = new Client();
            if (pos == null) pos = new Client();
            if (kon == null) kon = new Client();
        }

        private static void CreateFile(string path,string R00)
        {
            System.IO.File.WriteAllText(path, R00);

        }

        public static bool CheckGoogleJson()
        {
            return File.Exists(Path.GetFullPath(@".\..\..\..\OCR_BusinessLayer\lib\GoogleVision\OCR Api 1-fd24826faf62.json"));                
        }


    }





}

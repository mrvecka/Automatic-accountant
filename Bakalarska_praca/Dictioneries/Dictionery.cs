using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bakalarska_praca.Dictioneries
{
    class Dictionary
    {
        public Dictionary<string,string> header;
        public Dictionary<string, string> columns;
        public Dictionary<string, string> clients;

        public Dictionary()
        {
            Init();
        }


        private void Init()

        {
            InitHeader();
            columns = new Dictionary<string, string>();
            columns.Add("Dodávateľ", "Supplier");
            columns.Add("Odberateľ","Client");
            columns.Add("Konečný príjemca", "Client");
            columns.Add("Poštová adresa", "Client");
            columns.Add("Adresa", "Client");


            clients = new Dictionary<string, string>();
            clients.Add("IČO", "ICO");
            clients.Add("IČ DPH", "ICDPH");
            clients.Add("DIČ", "DIC");
            clients.Add("Mobil", "Phone");
            clients.Add("Telefón", "Phone");
            clients.Add("E-mail", "Email");
            clients.Add("Doprava", "Transport");
            clients.Add("Úhrada", "RefundMethode");
            clients.Add("rada", "RefundMethode");
            clients.Add("Banka", "Bank");
            clients.Add("Číslo účtu", "AccountNumber");
            clients.Add("IBAN", "IBAN");
            clients.Add("SWIFT", "SWIFT");
            clients.Add("Zákaznícke číslo", "ClientNumber");



        }

        private void InitHeader()
        {
            header = new Dictionary<string, string>();
            header.Add("Variabilný symbol(uveďte pri platbe)", "VariabilSymbol");
            header.Add("Var. symbol", "VariabilSymbol");
            header.Add("Konšt.symbol", "KonstSymbol");
            header.Add("Konštantný symbol", "KonstSymbol");
            header.Add("Faktúra č.", "EvidenceNumber");
            header.Add("Faktúra - daňový doklad č.", "EvidenceNumber");
            header.Add("Doklad č.", "EvidenceNumber");
            header.Add("Zákaznícke číslo", "ClientNumber");
            header.Add("Doprava", "Transport");
            header.Add("Úhrada", "RefundMethode");
            header.Add("Banka", "Bank");
            header.Add("Číslo účtu", "AccountNumber");
            header.Add("IBAN", "IBAN");
            header.Add("SWIFT", "SWIFT");





            //header.Add("Odberateľ");
            //header.Add("Odberatel");
            //header.Add("Zákaznícke číslo");
            //header.Add("Zák. číslo");
            //header.Add("Zákazníske č.");
            //header.Add("Dodávateľ");
            //header.Add("Dodávatel");
            //header.Add("Banka");
            //header.Add("Číslo účtu");
            //header.Add("IBAN");
            //header.Add("SWIFT");
            //header.Add("Úhrada");
            //header.Add("rada");
            //header.Add("Doprava");
        }

    }
}

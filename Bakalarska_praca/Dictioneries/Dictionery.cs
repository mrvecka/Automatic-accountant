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
        public List<string> canDeleteKeys;
        public Dictionary()
        {
            Init();
        }


        private void Init()

        {

            InitHeader();
            InitCanDeleteKeys();
            columns = new Dictionary<string, string>();
            columns.Add("Dodávateľ", "Client");
            columns.Add("Odberateľ","Client");
            columns.Add("Konečný príjemca", "Client");
            columns.Add("Poštová adresa", "Client");
            columns.Add("Adresa", "Client");
            columns.Add("Sídlo firmy", "Client");
            columns.Add("Korešpondenčná adresa","Client");


            clients = new Dictionary<string, string>();
            clients.Add("IČO", "ICO");
            clients.Add("ICO", "ICO");
            clients.Add("DIČ", "DIC");
            clients.Add("DIC", "DIC");
            clients.Add("IČ DPH", "ICDPH");
            clients.Add("IC DPH", "ICDPH");
            clients.Add("Telefón", "Phone");
            clients.Add("Mobil", "Phone");
            clients.Add("Tel", "Phone");
            clients.Add("IBAN/SWIFT", "IBAN");
            clients.Add("SWIFT", "SWIFT");
            clients.Add("IBAN", "IBAN");
            clients.Add("E-mail", "Email");
            clients.Add("Fax", "Fax");
            clients.Add("Web", "Web");
            clients.Add("WWW", "Web");
            clients.Add("Peňažný ústav", "Bank");
            clients.Add("kód banky", "BankCode");
            clients.Add("Banka", "Bank");
            clients.Add("Číslo účtu/kód", "AccountNumber");
            clients.Add("Číslo účtu", "AccountNumber");
            clients.Add("Účet", "AccountNumber");
            clients.Add("Zákaznícke číslo", "ClientNumber");

        }

        private void InitHeader()
        {
            header = new Dictionary<string, string>();
            header.Add("Variabilný symbol(uveďte pri platbe)", "VariabilSymbol");
            header.Add("Variabilný symbol", "VariabilSymbol");
            header.Add("VS", "VariabilSymbol");
            header.Add("Konštantný symbol", "KonstSymbol");
            header.Add("Konšt.symbol", "KonstSymbol");
            header.Add("KS", "KonstSymbol");
            header.Add("Faktúra - daňový doklad č.", "EvidenceNumber");
            header.Add("Predfaktura č.", "EvidenceNumber");
            header.Add("Faktúra číslo.", "EvidenceNumber");
            header.Add("Doklad číslo", "EvidenceNumber");
            header.Add("Faktúra č.", "EvidenceNumber");
            header.Add("Faktúra", "EvidenceNumber");
            header.Add("Dodací list číslo", "DeliveryNumber");
            header.Add("Číslo dod. listu", "DeliveryNumber");
            header.Add("Číslo objednávky", "OrderNumber");
            header.Add("Objednávka číslo", "OrderNumber");
            header.Add("Objednávka č.", "OrderNumber");
            header.Add("Miesto plnenia", "PlaceOfSupply");
            header.Add("Miesto určenia", "PlaceOfDelivery");
            header.Add("Miesto dodania", "PlaceOfDelivery");
            header.Add("Dodávateľ", "Client");
            header.Add("Odberateľ", "Client");
            header.Add("Poštová adresa", "Client");
            header.Add("Konečný príjemca", "Client");
            header.Add("Korešpondenčná adresa", "Client");
            header.Add("Sídlo firmy", "Client");
            header.Add("Spôsob úhrady", "RefundMethode");
            header.Add("Spôsob platby", "RefundMethode");
            header.Add("Forma úhrady", "RefundMethode");
            header.Add("Úhrada", "RefundMethode");
            header.Add("rada", "RefundMethode");
            header.Add("Spôsob dopravy", "Transport");
            header.Add("Doprava", "Transport");
            header.Add("Celkom k úhrade EUR", "Amount");
            header.Add("Cena celkom s DPH", "Amount");
            header.Add("Spolu na úhradu", "Amount");
            header.Add("Suma k úhrade", "Amount");
            header.Add("Celková suma", "Amount");
            header.Add("K úhrade", "Amount");
            header.Add("Dátum vystavenia dokladu", "DocumentCreateDate");
            header.Add("Dátum vyhotovenia", "DocumentCreateDate");
            header.Add("zo dňa", "DateOfCreate");
            header.Add("Faktúru vyhotovil", "CreatorName");
            header.Add("Dátum splatnosti", "DateOfPayment");
            header.Add("Dátum vzniku daňovej povinnosti", "DateOfTax");
            header.Add("Dátum zdaniteľného plnenia", "DateOfTax");
            header.Add("Daňová povinnosť", "DateOfTax");
            header.Add("Dátum dodania/pzp", "DateOfDelivery");
            header.Add("Expedicia", "DateOfDelivery");
            header.Add("Dátum objednania", "DateOfOrder");
            header.Add("Objednané dňa", "DateOfOrder");
            header.Add("Objednané", "DateOfOrder");

        }
        private void InitCanDeleteKeys()
        {
            canDeleteKeys = new List<string>();
            canDeleteKeys.Add("VariabilSymbol");
            canDeleteKeys.Add("EvidenceNumber");
            canDeleteKeys.Add("KonstSymbol");
            canDeleteKeys.Add("DeliveryNumber");
            canDeleteKeys.Add("OrderNumber");
            canDeleteKeys.Add("RefundMethode");
            canDeleteKeys.Add("DateOfCreate");
            canDeleteKeys.Add("CreatorName");
            canDeleteKeys.Add("DateOfPayment");
            canDeleteKeys.Add("DateOfTax");
            canDeleteKeys.Add("DocumentCreateDate");
            canDeleteKeys.Add("DateOfDelivery");
            canDeleteKeys.Add("DateOfOrder");


        }

    }
}

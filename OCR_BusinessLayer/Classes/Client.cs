using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Classes
{
    public class Client : ExpandableObjectConverter
    {
        public string ClientID { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string PSCCity { get; set; }
        public string PSC { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ICO { get; set; }
        public string DIC { get; set; }
        public string ICDPH { get; set; }
        public string ICDPHStateCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Transport { get; set; }
        public string Bank { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; }
        public string SWIFT { get; set; }
        public string ClientNumber { get; set; }
        public string Fax { get; set; }
        public string Web { get; set; }
        public string BankCode { get; set; }

    }
}

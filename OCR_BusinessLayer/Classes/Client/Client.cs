using OCR_BusinessLayer.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Classes.Client
{
    [TypeConverter(typeof(ClientConverter))]
    public class Client
    {
        private string clientID = null;
        private string name = null;
        private string street = null;
        private string pscCity = null;
        private string psc = null;
        private string city = null;
        private string state = null;
        private string ico = null;
        private string dic = null;
        private string icdph = null;
        private string icdphStateCode = null;
        private string phone = null;
        private string email = null;
        private string tran = null;
        private string bank = null;
        private string accountNumber = null;
        private string iban = null;
        private string swift = null;
        private string clientNumber = null;
        private string fax = null;
        private string web = null;
        private string bankCode = null;

        public string ClientID
        {
            get => clientID;
            set
            {
                clientID = value;
                clientID = ValidationServiceClient.Validate_ClientID(this);
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                name = ValidationServiceClient.Validate_Name(this);
            }
        }
        public string Street
        {
            get => street;
            set
            {
                street = value;
                street = ValidationServiceClient.Validate_Street(this);
            }
        }
        public string PSCCity
        {
            get => pscCity;
            set
            {
                pscCity = value;
                pscCity = ValidationServiceClient.Validate_PSCCity(this);
            }           
        }
        public string PSC
        {
            get => psc;
            set
            {
                psc = value;
                psc = ValidationServiceClient.Validate_PSC(this);
            }
        }
        public string City
        {
            get => city;
            set
            {
                city = value;
                city = ValidationServiceClient.Validate_City(this);
            }
        }
        public string State
        {
            get => state;
            set
            {
                state = value;
                state = ValidationServiceClient.Validate_State(this);

            }
        }
        public string ICO
        {
            get => ico;
            set
            {
                ico = value;
                ico = ValidationServiceClient.Validate_ICO(this);
            }
        }
        public string DIC
        {
            get => dic;
            set
            {
                dic = value;
                dic = ValidationServiceClient.Validate_DIC(this);
            }
        }
        public string ICDPH
        {
            get => icdph;
            set
            {
                icdph = value;
                icdph = ValidationServiceClient.Validate_ICDPH(this);
            }
        }
        public string ICDPHStateCode
        {
            get => icdphStateCode;
            set
            {
                icdphStateCode = value;
                icdphStateCode = ValidationServiceClient.Validate_ICDPHStateCode(this);
            }
        }
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                phone = ValidationServiceClient.Validate_Phone(this);
            }
        }
        public string Email
        {
            get => email;
            set
            {
                email = value;
                email = ValidationServiceClient.Validate_Email(this);
            }
        }
        public string Transport
        {
            get => tran;
            set
            {
                tran = value;
                tran = ValidationServiceClient.Validate_Transport(this);
            }
        }
        public string Bank
        {
            get => bank;
            set
            {
                bank = value;
                bank = ValidationServiceClient.Validate_Bank(this);
            }
        }
        public string AccountNumber
        {
            get => accountNumber;
            set
            {
                accountNumber = value;
                accountNumber = ValidationServiceClient.Validate_AccountNumber(this);
            }
        }
        public string IBAN
        {
            get => iban;
            set
            {
                iban = value;
                iban = ValidationServiceClient.Validate_IBAN(this);
            }
        }
        public string SWIFT
        {
            get => swift;
            set
            {
                swift = value;
                swift = ValidationServiceClient.Validate_SWIFT(this);
            }
        }
        public string ClientNumber
        {
            get => clientNumber;
            set
            {
                clientNumber = value;
                clientNumber = ValidationServiceClient.Validate_ClientNumber(this);
            }
        }
        public string Fax
        {
            get => fax;
            set
            {
                fax = value;
                fax = ValidationServiceClient.Validate_Fax(this);
            }
        }
        public string Web
        {
            get => web;
            set
            {
                web = value;
                web = ValidationServiceClient.Validate_Web(this);
            }
        }
        public string BankCode
        {
            get => bankCode;
            set
            {
                bankCode = value;
                bankCode = ValidationServiceClient.Validate_BankCode(this);
            }
            
        }

        public Client()
        {
            clientID = "";
            name = "";
            street = "";
            pscCity = "";
            psc = "";
            city = "";
            state = "";
            ico = "";
            dic = "";
            icdph = "";
            icdphStateCode = "";
            phone = "";
            email = "";
            tran = "";
            bank = "";
            accountNumber = "";
            iban = "";
            swift = "";
            clientNumber = "";
            fax = "";
            web = "";
            bankCode = "";
        }


    }
}

using OCR_BusinessLayer.Service;

namespace OCR_BusinessLayer.Classes
{
    public class Evidence
    {
        private string varSymbol = null;
        private string konSymbol = null;
        private string speSymbol = null;
        private string eviNumber = null;
        private string delNumber = null;
        private string ordNumber = null;
        private string tran = null;
        private string clNumber = null;
        private string docCreateDate = null;
        private string dateOfTax = null;
        private string placeOfSupply = null;
        private string placeOfDelivery = null;
        private string dateOfPayment = null;
        private string dateOfCreate = null;
        private string refundMethod = null;
        private string creatorName = null;
        private string dateOfDelivery = null;
        private string dateOfOrder = null;

        private string amount = null;
        private string baseLower = null;
        private string baseHigher = null;
        private string baseZero = null;
        private string baseNotContain = null;
        private string rateLower = null;
        private string rateHigher = null;
        private string amountLower = null;
        private string amountHigher = null;

        public string VariabilSymbol
        {
            get => varSymbol;
            set
            {
                varSymbol = value;
                varSymbol = ValidationServiceEvidence.Validate_VariabilSymbol(this);
            } 
        } 
        public string KonstSymbol
        {
            get => konSymbol;
            set
            {
                konSymbol = value;
                konSymbol = ValidationServiceEvidence.Validate_VariabilSymbol(this);
            } 
        }
        public string SpecSymbol
        {
            get => speSymbol;
            set
            {
                speSymbol = value;
                speSymbol = ValidationServiceEvidence.Validate_SpecSymbol(this);
            }
        }
        public string EvidenceNumber
        {
            get => eviNumber;
            set
            {
                eviNumber = value;
                eviNumber = ValidationServiceEvidence.Validate_EvidenceNumber(this);
            }
        }
        public string DeliveryNumber
        {
            get => delNumber;
            set
            {
                delNumber = value;
                delNumber = ValidationServiceEvidence.Validate_DeliveryNumber(this);
            }
        }
        public string OrderNumber
        {
            get => ordNumber;
            set
            {
                ordNumber = value;
                ordNumber = ValidationServiceEvidence.Validate_OrderNumber(this);
            }
        }
        public string Transport
        {
            get => tran;
            set
            {
                tran = value;
                tran = ValidationServiceEvidence.Validate_Transpport(this);
            }
        } 
        public string ClientNumber
        {
            get => clNumber;
            set
            {
                clNumber = value;
                clNumber = ValidationServiceEvidence.Validate_ClientNumber(this);
            }
        }
        public string DocumentCreateDate
        {
            get => docCreateDate;
            set
            {
                docCreateDate = value;
                docCreateDate = ValidationServiceEvidence.Validate_DocumentCreateDate(this);
            }
        }
        public string DateOfTax
        {
            get => dateOfTax;
            set
            {
                dateOfTax = value;
                dateOfTax = ValidationServiceEvidence.Validate_DateOfTax(this);
            }
        }
        public string PlaceOfSupply
        {
            get => placeOfSupply;
            set
            {
                placeOfSupply = value;
                placeOfSupply = ValidationServiceEvidence.Validate_PlaceOfSupply(this);
            }
        }
        public string PlaceOfDelivery
        {
            get => placeOfDelivery;
            set
            {
                placeOfDelivery = value;
                placeOfDelivery = ValidationServiceEvidence.Validate_PlaceOfDelivery(this);
            }
        }
        public string DateOfPayment
        {
            get => dateOfPayment;
            set
            {
                dateOfPayment = value;
                dateOfPayment = ValidationServiceEvidence.Validate_DateOfPayment(this);
            }
        }
        public string DateOfCreate
        {
            get => dateOfCreate;
            set
            {
                dateOfCreate = value;
                dateOfCreate = ValidationServiceEvidence.Validate_DateOfCreate(this);
            }
        }
        public string RefundMethode
        {
            get => refundMethod;
            set
            {
                refundMethod = value;
                refundMethod = ValidationServiceEvidence.Validate_RefundMethode(this);
            }
        }
        public string CreatorName
        {
            get => creatorName;
            set
            {
                creatorName = value;
                creatorName = ValidationServiceEvidence.Validate_CreatorName(this);
            }
        }
        public string DateOfDelivery
        {
            get => dateOfDelivery;
            set
            {
                dateOfDelivery = value;
                dateOfDelivery = ValidationServiceEvidence.Validate_DateOfDelivery(this);
            }
        }
        public string DateOfOrder
        {
            get => dateOfOrder;
            set
            {
                dateOfOrder = value;
                dateOfOrder = ValidationServiceEvidence.Validate_DateOfOrder(this);
            }
        }


        public string Amount
        {
            get => amount;
            set
            {
                amount = value;
                amount = ValidationServiceEvidence.Validate_Amount(this);
            }
        }
        public string BaseLower
        {
            get => baseLower;
            set
            {
                baseLower = value;
                baseLower = ValidationServiceEvidence.Validate_BaseLower(this);
            }
        }
        public string BaseHigher
        {
            get => baseHigher;
            set
            {
                baseHigher = value;
                baseHigher = ValidationServiceEvidence.Validate_BaseHigher(this);
            }
        }
        public string BaseZero
        {
            get => baseZero;
            set
            {
                baseZero = value;
                baseZero = ValidationServiceEvidence.Validate_BaseZero(this);
            }
        }
        public string BaseNotContain
        {
            get => baseNotContain;
            set
            {
                baseNotContain = value;
                baseNotContain = ValidationServiceEvidence.Validate_BaseNotContain(this);
            }
        }
        public string RateLower
        {
            get => rateLower;
            set
            {
                rateLower = value;
                rateLower = ValidationServiceEvidence.Validate_RateLower(this);
            }
        }
        public string RateHigher
        {
            get => rateHigher;
            set
            {
                rateHigher = value;
                rateHigher = ValidationServiceEvidence.Validate_RateHigher(this);
            }
        }
        public string AmountLower
        {
            get => amountLower;
            set
            {
                amountLower = value;
                amountLower = ValidationServiceEvidence.Validate_AmountLower(this);
            }
        }
        public string AmountHigher
        {
            get => amountHigher;
            set
            {
                amountHigher = value;
                amountHigher = ValidationServiceEvidence.Validate_AmountHigher(this);
            }
        }
    }
}

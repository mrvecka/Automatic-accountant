using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Classes.Client;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OCR_BusinessLayer.Service
{

    public class ValidationServiceEvidence 
    {
        public static void ValidateEvidence(Evidence e)
        {
            Validate_VariabilSymbol(e);
            Validate_KonstSymbol(e);
            Validate_SpecSymbol(e);
            Validate_EvidenceNumber(e);
            Validate_DeliveryNumber(e);
            Validate_OrderNumber(e);
            Validate_Transpport(e);
            Validate_ClientNumber(e);
            Validate_DocumentCreateDate(e);
            Validate_DateOfTax(e);
            Validate_PlaceOfSupply(e);
            Validate_PlaceOfDelivery(e);
            Validate_DateOfPayment(e);
            Validate_DateOfCreate(e);
            Validate_RefundMethode(e);
            Validate_CreatorName(e);
            Validate_DateOfDelivery(e);
            Validate_DateOfOrder(e);

            Validate_Amount(e);
            Validate_BaseLower(e);
            Validate_BaseHigher(e);
            Validate_BaseZero(e);
            Validate_BaseNotContain(e);
            Validate_RateLower(e);
            Validate_RateHigher(e);
            Validate_AmountLower(e);
            Validate_AmountHigher(e);
        }
        public static string Validate_VariabilSymbol(Evidence e)
        {
            string symbol = string.Empty;
            if (e.VariabilSymbol != null)
            {
                symbol = e.VariabilSymbol.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.NumbersOnly(symbol);

            }
            return symbol;
        }


        public static string Validate_KonstSymbol(Evidence e)
        {
            string symbol = string.Empty;
            if (e.KonstSymbol != null)
            {
                symbol = e.KonstSymbol.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.NumbersOnly(symbol);
                if (symbol.Length > 4)
                {
                    symbol = symbol.Substring(0, 4);
                }
            }
            return symbol;

        }

        public static string Validate_SpecSymbol(Evidence e)
        {
            return e.SpecSymbol;
        }
        public static string Validate_EvidenceNumber(Evidence e)
        {
            string symbol = string.Empty;
            if (e.EvidenceNumber != null)
            {
                symbol = e.EvidenceNumber.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrWhiteSpace(symbol))
                {

                    for (int i = 0; i < symbol.Length; i++)
                    {
                        if ((symbol[i] >= 48 && symbol[i] <= 57) || (symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122) || symbol[i] == 45 || symbol[i] == 32)
                        {
                            continue;
                        }
                        else
                        {
                            symbol = symbol.Remove(i, 1);
                            i--;
                        }
                    }
                }
            }
            return symbol.Trim();
        }

        public static string Validate_OrderNumber(Evidence e)
        {
            string symbol = string.Empty;
            if (e.OrderNumber != null)
            {
                symbol = e.OrderNumber.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.NumbersOnly(symbol);
            }
            return symbol;

        }

        public static string Validate_DeliveryNumber(Evidence e)
        {
            string symbol = string.Empty;
            if (e.DeliveryNumber != null)
            {
                symbol = e.DeliveryNumber.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.NumbersOnly(symbol);
            }
            return symbol;

        }

        public static string Validate_ClientNumber(Evidence e)
        {
            string symbol = string.Empty;
            if (e.ClientNumber != null)
            {
                symbol = e.ClientNumber.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.NumbersOnly(symbol);
            }
            return symbol;

        }

        public static string Validate_Transpport(Evidence e)
        {
            string symbol = string.Empty;
            if (e.Transport != null)
            {
                symbol = e.Transport.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesOnly(symbol);
            }
            return symbol;

        }

        public static string Validate_PlaceOfSupply(Evidence e)
        {
            string symbol = string.Empty;
            if (e.PlaceOfSupply != null)
            {
                symbol = e.PlaceOfSupply.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesOnly(symbol);
            }
            return symbol;

        }

        public static string Validate_PlaceOfDelivery(Evidence e)
        {
            string symbol = string.Empty;
            if (e.PlaceOfDelivery != null)
            {
                symbol = e.PlaceOfDelivery.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesOnly(symbol);
            }
            return symbol;

        }

        public static string Validate_RefundMethode(Evidence e)
        {
            string symbol = string.Empty;
            if (e.RefundMethode != null)
            {
                symbol = e.RefundMethode.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesOnly(symbol);
            }
            return symbol;

        }
        public static string Validate_CreatorName(Evidence e)
        {
            string symbol = string.Empty;
            if (e.CreatorName != null)
            {
                symbol = e.CreatorName.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesOnly(symbol);
            }
            return symbol;

        }
        public static string Validate_DocumentCreateDate(Evidence e)
        {
            string symbol = string.Empty;
            if (e.DocumentCreateDate != null)
            {
                symbol = e.DocumentCreateDate.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidationHelper.ValidateDate(symbol);
                }
            }
            return symbol;

        }
        public static string Validate_DateOfTax(Evidence e)
        {
            string symbol = string.Empty;
            if (e.DateOfTax != null)
            {
                symbol = e.DateOfTax.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidationHelper.ValidateDate(symbol);
                }
            }
            return symbol;

        }
        public static string Validate_DateOfPayment(Evidence e)
        {
            string symbol = string.Empty;
            if (e.DateOfPayment != null)
            {
                symbol = e.DateOfPayment.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidationHelper.ValidateDate(symbol);
                }
            }
            return symbol;

        }
        public static string Validate_DateOfCreate(Evidence e)
        {
            string symbol = string.Empty;
            if (e.DateOfCreate != null)
            {
                symbol = e.DateOfCreate.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidationHelper.ValidateDate(symbol);
                }
            }
            return symbol;

        }
        public static string Validate_DateOfDelivery(Evidence e)
        {
            string symbol = string.Empty;
            if (e.DateOfDelivery != null)
            {
                symbol = e.DateOfDelivery.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidationHelper.ValidateDate(symbol);
                }
            }
            return symbol;

        }
        public static string Validate_DateOfOrder(Evidence e)
        {
            string symbol = string.Empty;
            if (e.DateOfOrder != null)
            {
                symbol = e.DateOfOrder.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidationHelper.ValidateDate(symbol);
                }
            }
            return symbol;

        }
        public static string Validate_Amount(Evidence e)
        {
            return e.Amount;
        }

        public static string Validate_BaseLower(Evidence e)
        {
            return e.BaseLower;
        }

        public static string Validate_BaseHigher(Evidence e)
        {
            return e.BaseHigher;
        }

        public static string Validate_BaseZero(Evidence e)
        {
            return e.BaseZero;
        }

        public static string Validate_BaseNotContain(Evidence e)
        {
            return e.BaseNotContain;
        }

        public static string Validate_RateLower(Evidence e)
        {
            return e.RateLower;
        }

        public static string Validate_RateHigher(Evidence e)
        {
            return e.RateHigher;
        }

        public static string Validate_AmountLower(Evidence e)
        {
            return e.AmountLower;
        }

        public static string Validate_AmountHigher(Evidence e)
        {
            return e.AmountHigher;
        }
    }


    public class ValidationServiceClient
    {

        public static void ValidateClient(Client c)
        {
            Validate_ClientID(c);
            Validate_Name( c);
            Validate_Street( c);
            Validate_PSCCity( c);
            Validate_PSC(c);
            Validate_City(c);
            Validate_State( c);
            Validate_ICO(c);
            Validate_DIC( c);
            Validate_ICDPH( c);
            Validate_ICDPHStateCode(c);
            Validate_Phone( c);
            Validate_Email( c);
            Validate_Transport( c);
            Validate_Bank( c);
            Validate_AccountNumber( c);
            Validate_BankCode( c);
            Validate_IBAN( c);
            Validate_SWIFT( c);
            Validate_ClientNumber(c);
            Validate_Fax(c);
            Validate_Web( c);
        }

        #region Validations Client

        public static string Validate_ClientID(Client c)
        {
            return c.ClientID;
        }

        public static string Validate_Name( Client c)
        {
            string symbol = string.Empty;
            if (c.Name != null)
            {
                symbol = c.Name.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesOnly(symbol);
            }
            return symbol;
        }
        public static string Validate_Street( Client c)
        {
            string symbol = string.Empty;
            if (c.Street != null)
            {
                symbol = c.Street.Trim(CONSTANTS.charsToTrimLine);
            }
            return symbol;
        }
        public static string Validate_PSCCity( Client c)
        {
            string symbol = string.Empty;
            if (c.PSCCity != null)
            {
                symbol = c.PSCCity.Trim(CONSTANTS.charsToTrimLine);
            }
            BreakDownPsc(c, symbol);
            return symbol;
        }

        public static string Validate_PSC(Client c)
        {
            return c.PSC;
        }
        public static string Validate_City(Client c)
        {
            return c.City;
        }

        public static string Validate_State( Client c)
        {
            string symbol = string.Empty;
            if (c.State != null)
            {
                symbol = c.State.Trim(CONSTANTS.charsToTrimLine);
            }
            return symbol;
        }

        public static string Validate_ICO(Client c)
        {
            string symbol = string.Empty;
            if (c.ICO != null)
            {
                symbol = c.ICO.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.NumbersOnly(symbol);
            }
            return symbol;
        }

        public static string Validate_DIC( Client c)
        {
            string symbol = string.Empty;
            if (c.DIC != null)
            {
                symbol = c.DIC.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.NumbersOnly(symbol);
            }
            return symbol;
        }

        public static string Validate_ICDPH( Client c)
        {
            string symbol = string.Empty;
            int countOfLetters = 0;
            if (c.ICDPH != null)
            {
                symbol = c.ICDPH.Trim(CONSTANTS.charsToTrimLine);
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (ValidationHelper.ContainOnlyLetters(symbol[i].ToString()))
                    {
                        countOfLetters++;
                    }
                    else if (ValidationHelper.numbersOnly.Contains(symbol[i]))
                    {
                        continue;
                    }
                    else
                    {
                        symbol = symbol.Remove(i, 1);
                    }
                }
                if (countOfLetters > 2)
                {
                    symbol = string.Empty;
                }

            }
            BreakDownICDPH(c,symbol);
            return symbol;
        }

        public static string Validate_ICDPHStateCode(Client c)
        {
            return c.ICDPHStateCode;
        }

        public static string Validate_Phone( Client c)
        {
           
            string symbol = string.Empty;
            if (c.Phone != null)
            {
                symbol = c.Phone.Trim(CONSTANTS.charsToTrimLine);
                const string MatchPhonePattern = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
                Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(symbol);
                foreach (Match match in matches)
                {

                    symbol = match.Value.ToString();
                    break;
                }

            }
            return symbol;
        }

        public static string Validate_Email( Client c)
        {

            string symbol = string.Empty;
            if (c.Email != null)
            {
                symbol = c.Email.Trim(CONSTANTS.charsToTrimLine);
                const string MatchPhonePattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
                Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(symbol);
                foreach (Match match in matches)
                {

                    symbol = match.Value.ToString();
                    break;
                }

            }
            return symbol;
        }

        public static string Validate_Transport( Client c)
        {

            string symbol = string.Empty;
            if (c.Transport != null)
            {
                symbol = c.Transport.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesOnly(symbol);

            }
            return symbol;
        }


        public static string Validate_Bank( Client c)
        {
            string symbol = string.Empty;
            if (c.Bank != null)
            {
                symbol = c.Bank.Trim(CONSTANTS.charsToTrimLine);
                symbol = ValidationHelper.LettersSpacesDotsOnly(symbol);

            }
            return symbol;
        }

        public static string Validate_AccountNumber( Client c)
        {
            string symbol = string.Empty;
            if (c.AccountNumber != null)
            {
                symbol = c.AccountNumber.Trim(CONSTANTS.charsToTrimAccountNumber);
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (ValidationHelper.numbersOnly.Contains(symbol[i]))
                    {
                        continue;
                    }
                    else if (symbol[i] == 47)
                    {
                        c.BankCode = symbol.Substring(i, 5);
                    }
                    else
                    {
                        symbol = symbol.Remove(i, 1);
                        i--;
                    }

                }

            }
            return symbol;
        }
        public static string Validate_BankCode( Client c)
        {
            string symbol = string.Empty;
            if (c.BankCode != null)
            {
                symbol = c.BankCode.Trim(CONSTANTS.charsToTrimAccountNumber);
                symbol = ValidationHelper.NumbersOnly(symbol);

            }
            return symbol;
        }

        public static string Validate_IBAN( Client c)
        {
            string symbol = string.Empty;
            int countOfLetters = 0;
            if (c.IBAN != null)
            {
                symbol = c.IBAN.Trim(CONSTANTS.charsToTrimAccountNumber);
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (ValidationHelper.ContainOnlyLetters(symbol[i].ToString()))
                    {
                        countOfLetters++;
                    }
                    else if (ValidationHelper.numbersOnly.Contains(symbol[i]))
                    {
                        continue;
                    }
                    else
                    {
                        symbol = symbol.Remove(i, 1);
                    }
                }
                if (countOfLetters > 2)
                {
                    symbol = string.Empty;
                }

            }
            return symbol;
        }

        public static string Validate_SWIFT( Client c)
        {
            string symbol = string.Empty;
            if (!string.IsNullOrEmpty(c.SWIFT))
            {
                symbol = c.SWIFT.Trim(CONSTANTS.charsToTrimAccountNumber);
                symbol = ValidationHelper.LettersOnly(symbol);
                symbol = symbol.Substring(0, 8);
            }
            return symbol;
        }
        public static string Validate_ClientNumber(Client c)
        {
            return c.ClientNumber;
        }

        public static string Validate_Fax(Client c)
        {
            return c.Fax;
        }

        public static string Validate_Web( Client c)
        {
            string symbol = string.Empty;
            if (c.Web != null)
            {
                symbol = c.Web.Trim(CONSTANTS.charsToTrimAccountNumber);
                symbol = ValidationHelper.LettersDotsOnly(symbol);
                if (symbol[0] != 'w')
                {
                    symbol = "www" + symbol;
                }
            }
            return symbol;
        }




        #endregion


        #region Help Function

        

        private static void BreakDownPsc(Client c,string psc)
        {

            if (!string.IsNullOrEmpty(psc))
            {

                Regex rgx = new Regex(@"\d{5}", RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(psc);
                if (matches.Count > 0)
                {
                    c.PSC = matches[0].Value;
                    c.City = psc.Replace(c.PSC, "");
                }
                else
                {
                    rgx = new Regex(@"\d{3} ?\d{2}", RegexOptions.IgnoreCase);
                    matches = rgx.Matches(psc);
                    if (matches.Count > 0)
                    {
                        c.PSC = matches[0].Value;
                        c.City = psc.Replace(c.PSC, "");
                    }
                    else
                    {
                        c.PSC = string.Empty;
                        c.City = string.Empty;
                    }
                }

            }

        }
        private static void BreakDownICDPH(Client c,string icdph)
        {
            if (string.IsNullOrEmpty(icdph))
            {

                Regex rgx = new Regex(@"[A-Za-z]{2}", RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(icdph);
                if (matches.Count > 0)
                {
                    c.ICDPHStateCode = matches[0].Value;
                }
                else
                {
                    c.ICDPHStateCode = string.Empty;
                }
            }
        }



        #endregion

    }
}

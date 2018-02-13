using OCR_BusinessLayer.Classes;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OCR_BusinessLayer.Service
{
    public class ValidationService
    {
        private static List<char> numbersOnly;

        public static void Validate(ref Evidence e)
        {
            Initialization();
            ValidateEvidence(ref e);
        }

        public static void Validate(ref Client c)
        {
            Initialization();
            ValidateClient(ref c);
        }

        private static void ValidateEvidence(ref Evidence e)
        {
            Validate_ValiabilSymbol(ref e);
            Validate_KonstSymbol(ref e);
            Validate_EvidenceNumber(ref e);
            Validate_DeliveryNumber(ref e);
            Validate_OrderNumber(ref e);
            Validate_ClientNumber(ref e);
            Validate_CreatorName(ref e);
            Validate_PlaceOfSupply(ref e);
            Validate_PlaceOfDelivery(ref e);
            Validate_RefundMethode(ref e);
            Validate_Transpport(ref e);
            Validate_DocumentCreateDate(ref e);
            Validate_DateOfTax(ref e);
            Validate_DateOfPayment(ref e);
            Validate_DateOfCreate(ref e);
            Validate_DateOfDelivery(ref e);
            Validate_DateOfOrder(ref e);
        }

        private static void ValidateClient(ref Client c)
        {
            Validate_Name(ref c);
            //street
            //psc
            //state
            Validate_ICO(ref c);
            Validate_DIC(ref c);
            Validate_ICDPH(ref c);
            Validate_Phone(ref c);
            Validate_Email(ref c);
            Validate_Transport(ref c);
            Validate_Bank(ref c);
            Validate_AccountNumber(ref c);
            Validate_BankCode(ref c);
            Validate_IBAN(ref c);
            Validate_SWIFT(ref c);
            Validate_Web(ref c);
        }

        #region Initialization

        private static void Initialization()
        {
            numbersOnly = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        }

#endregion


        #region Validations Evidence

        private static void Validate_ValiabilSymbol(ref Evidence e)
        {
            string symbol = "";
            if (e.VariabilSymbol != null)
            {
                symbol = e.VariabilSymbol.Trim(CONSTANTS.charsToTrimLine);
                symbol = NumbersOnly(symbol);
                
            }
            e.VariabilSymbol = symbol;
        }


        private static void Validate_KonstSymbol(ref Evidence e)
        {
            string symbol = "";
            if (e.KonstSymbol != null)
            {
                symbol = e.KonstSymbol.Trim(CONSTANTS.charsToTrimLine);
                symbol = NumbersOnly(symbol);
                    if (symbol.Length > 4)
                    {
                        symbol = symbol.Substring(0, 4);
                    }
            }
            e.KonstSymbol = symbol;

        }
        private static void Validate_EvidenceNumber(ref Evidence e)
        {
            string symbol = "";
            if (e.EvidenceNumber != null)
            {
                symbol = e.EvidenceNumber.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrWhiteSpace(symbol))
                {

                    for (int i = 0; i < symbol.Length; i++)
                    {
                        if ((symbol[i] >= 48 && symbol[i] <= 57) || (symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >=97 && symbol[i] <=122) || symbol[i] == 45 || symbol[i] == 32)
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
            e.EvidenceNumber = symbol.Trim();
        }

        private static void Validate_OrderNumber(ref Evidence e)
        {
            string symbol = "";
            if (e.OrderNumber != null)
            {
                symbol = e.OrderNumber.Trim(CONSTANTS.charsToTrimLine);
                symbol = NumbersOnly(symbol);
            }
            e.OrderNumber = symbol;

        }

        private static void Validate_DeliveryNumber(ref Evidence e)
        {
            string symbol = "";
            if (e.DeliveryNumber != null)
            {
                symbol = e.DeliveryNumber.Trim(CONSTANTS.charsToTrimLine);
                symbol = NumbersOnly(symbol);
            }
            e.DeliveryNumber = symbol;

        }

        private static void Validate_ClientNumber(ref Evidence e)
        {
            string symbol = "";
            if (e.ClientNumber != null)
            {
                symbol = e.ClientNumber.Trim(CONSTANTS.charsToTrimLine);
                symbol = NumbersOnly(symbol);
            }
            e.ClientNumber = symbol;

        }

        private static void Validate_Transpport(ref Evidence e)
        {
            string symbol = "";
            if (e.Transport != null)
            {
                symbol = e.Transport.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesOnly(symbol);
            }
            e.Transport = symbol;

        }

        private static void Validate_PlaceOfSupply(ref Evidence e)
        {
            string symbol = "";
            if (e.PlaceOfSupply != null)
            {
                symbol = e.PlaceOfSupply.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesOnly(symbol);
            }
            e.PlaceOfSupply = symbol;

        }

        private static void Validate_PlaceOfDelivery(ref Evidence e)
        {
            string symbol = "";
            if (e.PlaceOfDelivery != null)
            {
                symbol = e.PlaceOfDelivery.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesOnly(symbol);
            }
            e.PlaceOfDelivery = symbol;

        }

        private static void Validate_RefundMethode(ref Evidence e)
        {
            string symbol = "";
            if (e.RefundMethode != null)
            {
                symbol = e.RefundMethode.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesOnly(symbol);
            }
            e.RefundMethode = symbol;

        }
        private static void Validate_CreatorName(ref Evidence e)
        {
            string symbol = "";
            if (e.CreatorName != null)
            {
                symbol = e.CreatorName.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesOnly(symbol);
            }
            e.CreatorName = symbol;

        }
        private static void Validate_DocumentCreateDate(ref Evidence e)
        {
            string symbol = "";
            if (e.DocumentCreateDate != null)
            {
                symbol = e.DocumentCreateDate.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidateDate(symbol);
                }
            }
            e.DocumentCreateDate = symbol;

        }
        private static void Validate_DateOfTax(ref Evidence e)
        {
            string symbol = "";
            if (e.DateOfTax != null)
            {
                symbol = e.DateOfTax.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidateDate(symbol);
                }
            }
            e.DateOfTax = symbol;

        }
        private static void Validate_DateOfPayment(ref Evidence e)
        {
            string symbol = "";
            if (e.DateOfPayment != null)
            {
                symbol = e.DateOfPayment.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidateDate(symbol);
                }
            }
            e.DateOfPayment = symbol;

        }
        private static void Validate_DateOfCreate(ref Evidence e)
        {
            string symbol = "";
            if (e.DateOfCreate != null)
            {
                symbol = e.DateOfCreate.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidateDate(symbol);
                }
            }
            e.DateOfCreate = symbol;

        }
        private static void Validate_DateOfDelivery(ref Evidence e)
        {
            string symbol = "";
            if (e.DateOfDelivery != null)
            {
                symbol = e.DateOfDelivery.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidateDate(symbol);
                }
            }
            e.DateOfDelivery = symbol;

        }
        private static void Validate_DateOfOrder(ref Evidence e)
        {
            string symbol = "";
            if (e.DateOfOrder != null)
            {
                symbol = e.DateOfOrder.Trim(CONSTANTS.charsToTrimLine);
                if (!string.IsNullOrEmpty(symbol))
                {
                    symbol = ValidateDate(symbol);
                }
            }
            e.DateOfOrder = symbol;

        }



        #endregion

        #region Validations Client

        private static void Validate_Name(ref Client c)
        {
            string symbol = "";
            if (c.Name != null)
            {
                symbol = c.Name.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesOnly(symbol);
            }
            c.Name = symbol;
        }

        private static void Validate_ICO(ref Client c)
        {
            string symbol = "";
            if (c.ICO != null)
            {
                symbol = c.ICO.Trim(CONSTANTS.charsToTrimLine);
                symbol = NumbersOnly(symbol);
            }
            c.ICO = symbol;
        }

        private static void Validate_DIC(ref Client c)
        {
            string symbol = "";
            if (c.DIC != null)
            {
                symbol = c.DIC.Trim(CONSTANTS.charsToTrimLine);
                symbol = NumbersOnly(symbol);
            }
            c.DIC = symbol;
        }

        private static void Validate_ICDPH(ref Client c)
        {
            string symbol = "";
            int countOfLetters = 0;
            if (c.ICDPH != null)
            {
                symbol = c.ICDPH.Trim(CONSTANTS.charsToTrimLine);
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (ContainOnlyLetters(symbol[i].ToString()))
                    {
                        countOfLetters++;
                    }
                    else if (numbersOnly.Contains(symbol[i]))
                    {
                        continue;
                    }
                    else
                    {
                        symbol = symbol.Replace(symbol[i],' ');
                    }
                }
                if (countOfLetters > 2)
                {
                    symbol = "";
                }

            }
            c.ICDPH = symbol;
        }

        private static void Validate_Phone(ref Client c)
        {
           
            string symbol = "";
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
            c.Phone = symbol;
        }

        private static void Validate_Email(ref Client c)
        {

            string symbol = "";
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
            c.Email = symbol;
        }

        private static void Validate_Transport(ref Client c)
        {

            string symbol = "";
            if (c.Transport != null)
            {
                symbol = c.Transport.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesOnly(symbol);

            }
            c.Transport = symbol;
        }


        private static void Validate_Bank(ref Client c)
        {
            string symbol = "";
            if (c.Bank != null)
            {
                symbol = c.Bank.Trim(CONSTANTS.charsToTrimLine);
                symbol = LettersSpacesDotsOnly(symbol);

            }
            c.Bank = symbol;
        }

        private static void Validate_AccountNumber(ref Client c)
        {
            string symbol = "";
            if (c.AccountNumber != null)
            {
                symbol = c.AccountNumber.Trim(CONSTANTS.charsToTrimAccountNumber);
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (numbersOnly.Contains(symbol[i]))
                    {
                        continue;
                    }
                    else if (symbol[i] == 47)
                    {
                        c.BankCode = symbol.Substring(i, 4);
                    }
                    else
                    {
                        symbol = symbol.Remove(i, 1);
                        i--;
                    }

                }

            }
            c.AccountNumber = symbol;
        }
        private static void Validate_BankCode(ref Client c)
        {
            string symbol = "";
            if (c.BankCode != null)
            {
                symbol = c.BankCode.Trim(CONSTANTS.charsToTrimAccountNumber);
                symbol = NumbersOnly(symbol);

            }
            c.BankCode = symbol;
        }

        private static void Validate_IBAN(ref Client c)
        {
            string symbol = "";
            int countOfLetters = 0;
            if (c.IBAN != null)
            {
                symbol = c.IBAN.Trim(CONSTANTS.charsToTrimAccountNumber);
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (ContainOnlyLetters(symbol[i].ToString()))
                    {
                        countOfLetters++;
                    }
                    else if (numbersOnly.Contains(symbol[i]))
                    {
                        continue;
                    }
                    else
                    {
                        symbol = symbol.Replace(symbol[i], ' ');
                    }
                }
                if (countOfLetters > 2)
                {
                    symbol = "";
                }

            }
            c.IBAN = symbol;
        }

        private static void Validate_SWIFT(ref Client c)
        {
            string symbol = "";
            if (c.SWIFT != null)
            {
                symbol = c.SWIFT.Trim(CONSTANTS.charsToTrimAccountNumber);
                symbol = LettersOnly(symbol);
                symbol = symbol.Substring(0, 8);
            }
            c.SWIFT = symbol;
        }

        private static void Validate_Web(ref Client c)
        {
            string symbol = "";
            if (c.Web != null)
            {
                symbol = c.Web.Trim(CONSTANTS.charsToTrimAccountNumber);
                symbol = LettersDotsOnly(symbol);
                if (symbol[0] != 'w')
                {
                    symbol = "www" + symbol;
                }
            }
            c.Web = symbol;
        }




        #endregion


        #region Help Function

        private static string NumbersOnly(string symbol)
        {
            if (!string.IsNullOrWhiteSpace(symbol))
            {
                for (int i = 0; i < symbol.Length; i++)
                {
                    if (numbersOnly.Contains(symbol[i]))
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
            return symbol;
        }

        private static string LettersOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122))
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol;
        }

        private static string LettersSpacesOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122) || symbol[i] == 32)
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol;
        }

        private static string LettersDotsOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122) || symbol[i] == 46)
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol;
        }

        private static string LettersSpacesDotsOnly(string symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                if ((symbol[i] >= 65 && symbol[i] <= 90) || (symbol[i] >= 97 && symbol[i] <= 122) || symbol[i] == 32 || symbol[i] == 46)
                {
                    continue;
                }
                else
                {
                    symbol = symbol.Remove(i, 1);
                    i--;
                }
            }
            return symbol;
        }


        private static string ValidateDate(string symbol)
        {
            string date = "";
            //120314
            // 12032014 or 12.03.14 or 12 03 14
            //12.03. 14
            // 12 03 2014
            // 12.03. 2014

            //day
            int index = symbol.IndexOfAny(numbersOnly.ToArray());
            var s = symbol.Substring(index, 2);
            date += s;
            date += ".";
            symbol = symbol.Replace(s, "");

            // month
            index = symbol.IndexOfAny(numbersOnly.ToArray());
            s = symbol.Substring(index, 2);
            date += s;
            date += ". ";
            symbol = symbol.Replace(s, "");

            //year
            index = symbol.IndexOfAny(numbersOnly.ToArray());
            s = symbol.Substring(index);
            if (s.Length == 2)
            {
                date += "20";
            }
            date += s;


            return date;
        }


        private static bool ContainOnlyLetters(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i] >= 65 && text[i] <= 90) || (text[i] >= 97 && text[i] <= 122))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}

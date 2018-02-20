namespace OCR_BusinessLayer
{
    public class CONSTANTS
    {
        public static int SIMILARITY = 70;
        public static int PROXIMITY = 20;
        public static char[] charsToTrim = { ':', '/',' ' };
        public static char[] charsToTrimLine = { ' ','\'', ';', '/', '\\', '|', '\n', ')', '(', '!', '&', '=', '´', '%', 'ˇ', '+', '“', '$', '?','!'};
        public static char[] charsToTrimAccountNumber = { ' ', '\'', ';', '\\', '|', '\n', ')', '(', '!', '&', '=', '´', '%', 'ˇ', '+', '“', '$', '?' };


        public enum Result
        {
            True = 1,
            False = 2,
            Continue = 3,
            Break = 4,
        }
        public enum Operation
        {
            SELECT = 1,
            INSERT = 2,
            UPDATE =3,
            DELETE = 4,
        }
    }
}

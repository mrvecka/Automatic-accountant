namespace OCR_BusinessLayer
{
    public class CONSTANTS
    {
        public static int SIMILARITY = 70;
        public static int PROXIMITY = 20;
        public static int PATTERN_CHECK_XY_PROXIMITY = 10;
        public static int PATTERN_CHECK_WIDTHHEIGHT_PROXIMITY = 20;
        public static int MAX_LENGTH_OF_ONE_WORD = 300;
        public static int MAX_LENGTH_OF_SQL_STRING = 100;
        public static char[] charsToTrim = { ':', '/',' ' };
        public static char[] charsToTrimLine = { ' ','\'', ';', '/', '\\', '|', '\n', ')', '(', '!', '&', '=', '´', '%', 'ˇ', '+', '“', '$', '?','!'};
        public static char[] charsToTrimAccountNumber = { ' ', '\'', ';', '\\', '|', '\n', ')', '(', '!', '&', '=', '´', '%', 'ˇ', '+', '“', '$', '?' };
        public static string[] filter = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "pdf" };
        public static string[] trainedData = new string[] { "traineddata"};




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

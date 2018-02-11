using Bakalarska_praca.Classes;
using Bakalarska_praca.Dictioneries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Bakalarska_praca.Service
{
    class DictionaryService : IDisposable
    {
        private int keysInRow = 0;
        private bool nextLineIsColumn = false;
        private int columnsCount = 0;
        private List<Column> listOfColumns;
        private List<Column> TempListOfColumn;
        private List<Client> listOfClients;
        private List<string> keysToDelete;
        private Dictionary dic;
        private KeyValuePair<string, string> pair;
        private Evidence eud;
        private char[] charsToTrim = { ' ', ':', ';', '/', '\\', '|' };
        private int pictureWidth;
        public void Dispose()
        {

        }
        /// <summary>
        /// Iterate trought lines
        /// Try to find relevant data and store them
        /// </summary>
        /// <param name="lines">List of lines</param>
        /// <param name="width">Width of document</param>
        /// <returns></returns>
        public void MakeObjectsFromLines(PreviewObject p, FileToProcess file,IProgress<int> progress)
        {
            int Step = p.Lines.Count / 60;
            pictureWidth = p.img.Width;
            dic = new Dictionary();
            eud = new Evidence();
            Type type = eud.GetType();
            listOfColumns = new List<Column>();
            TempListOfColumn = new List<Column>();
            listOfClients = new List<Client>();
            keysToDelete = new List<string>();
            foreach (TextLine line in p.Lines)
            {
                keysInRow = 0;
                listOfColumns.AddRange(TempListOfColumn);
                TempListOfColumn.Clear();
                foreach (var item in dic.header.Where(c => keysToDelete.Contains(c.Value)).ToList())
                {
                    dic.header.Remove(item.Key);
                }
                keysToDelete.Clear();

                if (GoLikeColumn())
                {
                    FillColumns(line);
                    continue;
                }
                string t = line.text;
                GetDataFromLine(line, ref t, dic.header, type, eud);



                progress.Report(Step);
            }
        }

        private bool GoLikeColumn()
        {
            var col = listOfColumns.Where(c => c.Completed == false).FirstOrDefault();
            return col == null ? false : true;
        }

        /// <summary>
        /// Methode gets a text from line and try to get relevant data based on dictionary
        /// Methode is called recursively, if key is found methode is called for the rest of the text if there are any keys
        /// if yes methode is called again
        /// if not then text is value for key in previous call and is saved to object
        /// </summary>
        /// <param name="line">Object Of TextLine</param>
        /// <param name="dic">Dictionary where is methode looking for keys</param>
        /// <param name="type">Type of object where the found data will be stored</param>
        /// <param name="data">Object for data</param>
        /// <returns></returns>
        private bool GetDataFromLine(TextLine line, ref string lineText, Dictionary<string, string> dictionary, Type type, Object data, bool isColumn = false, Column col = null, bool lookingForRight = false)
        {
            int firstCharindex;
            int keyLength;
            bool keyFound = false;
            int similarity = 0;
            bool IndexIsNull = false;
            string stringKey = "";
            CONSTANTS.Result res = CONSTANTS.Result.Continue;
            foreach (KeyValuePair<string, string> key in dictionary)
            {
                firstCharindex = lineText.ToLower().IndexOf(key.Key.Substring(0, 1).ToLower()); // index prveho vyskytu prveho znaku z kluca 
                keyLength = key.Key.Length; // dlzka kluca
                while ((keyLength + firstCharindex) <= lineText.Length && firstCharindex != -1) // ak je kluc vacsi ako text nie je to on a idem prec
                {

                    stringKey = lineText.Substring(firstCharindex, keyLength); // toto by mal byt kluc z textu ktory som rozpoznal
                    similarity = SimilarityService.GetSimilarity(key.Key.ToLower(), stringKey.ToLower());
                    if (similarity > CONSTANTS.SIMILARITY)
                    {
                        if (col != null && type == eud.GetType())
                        {
                            // aktualny stlpec skoncil
                            col.Completed = true;
                            col.Bottom = line.Words[0].Bounds.Top;
                        }
                        res = jahoda(line, key, stringKey, ref lineText, firstCharindex, dictionary, type, data, ref keyFound, lookingForRight,ref isColumn,col);
                        if (res == CONSTANTS.Result.Continue) { break; }
                        else if (res == CONSTANTS.Result.True) { return true; }
                        else if (res == CONSTANTS.Result.False) { return false; }
                        else if (res == CONSTANTS.Result.Break) { break; }
                    }
                    else
                    {
                        string s = lineText.Substring(lineText.IndexOf(stringKey) + 1).ToLower();
                        int index = s.IndexOf(key.Key.Substring(0, 1).ToLower());
                        if (index == 0 && IndexIsNull || index == -1)
                        {
                            // zacyklil som sa alebo som nic nenasiel tak idem na dalsi kluc
                            break;
                        }
                        if (index == 0)
                            IndexIsNull = true;

                        firstCharindex += index+1;
                    }



                }
                if (res == CONSTANTS.Result.Break && lineText.Length < 10) { break; }
                else if (res == CONSTANTS.Result.Continue) { continue; }
                else { continue; }

            }

            if (isColumn && !keyFound && col != null && col.FirstLineInColumn > 4)
            {
                // skus iny slovnik
                if (type == eud.GetType())
                {
                    foreach (Column c in listOfColumns)
                        GetDataFromLine(line, ref lineText, dic.clients, c.GetType(), listOfClients[c.Id - 1], false, col, false);
                }
                else
                {
                    GetDataFromLine(line, ref lineText, dic.header, eud.GetType(), eud, false, col, false);
                }

            }

            if (isColumn && col != null && !keyFound)
            {
                GetDataFromLine(line, ref lineText, dic.header, eud.GetType(), eud, false, col, false);
                if (keysInRow == 0)
                {
                    Client client = (Client)data;
                    string s = lineText.Trim(charsToTrim);
                    if (!string.IsNullOrEmpty(s) && s.Length >= 5)
                    {
                        switch (col.FirstLineInColumn)
                        {
                            case 1:                                                        
                                client.Name = lineText;
                                col.FirstLineInColumn++;
                                break;
                            case 2:
                                client.Street = lineText;
                                col.FirstLineInColumn++;
                                break;
                            case 3:
                                client.PSCCity = lineText;
                                col.FirstLineInColumn++;
                                break;
                            case 4:
                                client.State = lineText;
                                col.FirstLineInColumn++;
                                break;

                        }
                    }
                }
            }


            if (keysInRow >= 2)
                return false;

            return keyFound;
        }


        private CONSTANTS.Result jahoda(TextLine line, KeyValuePair<string, string> key, string stringKey, ref string lineText, int firstCharIndex,
                                        Dictionary<string, string> dictionary, Type type, Object data, ref bool keyFound, bool lookingForRight,
                                        ref bool isColumn,Column col)
        {
            string stringKeyValue = "";
            if (lookingForRight)
            {
                
                pair = key;
                
            }
            if (dic.columns.ContainsKey(key.Key))
            {
                // je to stlpec
                nextLineIsColumn = true;
                columnsCount++;
                Column column = GetColumnParam(columnsCount, stringKey, line);
                column.Text = key.Key;
                if (ColumnAlreadyExists(column))
                {
                    return CONSTANTS.Result.Continue;
                }
                Client n = new Client();
                EndRelativeColumn(column);
                var s = line.text;
                TryGetRightXOfColumn(column, line,ref s, stringKey, n);
                TempListOfColumn.Add(column);
                lineText = lineText.Replace(key.Key, "");
                listOfClients.Add(n);
                s = s.Trim(charsToTrim);
                if (!(string.IsNullOrEmpty(s) || s.Length < 5)) // uz aktualny riadok moze byt stlpec tak to tu poriesim
                {
                    s = s.Replace(stringKey, "");
                    GetDataFromLine(line, ref s, dictionary, type, n,true,column,false); // ak mam za klucovym slovom (Odberatel) nejaky text a nie su tam ine klucove slova
                }

                return CONSTANTS.Result.Break;
            }

            keyFound = true;
            if (isColumn && type == eud.GetType())
            {
                stringKeyValue = line.text.Substring(firstCharIndex + stringKey.Length);
            }
            else
            {
                stringKeyValue = lineText.Substring(firstCharIndex + stringKey.Length);
            }
            if (!isColumn && listOfClients.Count > 0 && type == listOfClients[0]?.GetType())
            {
                isColumn = true;
            }

            keysInRow++;
            // nasiel som nejaky kluc v riadku, toto by mala byt jeho hodnota ale moze obsahovat este nejaky iny kluc tak sa radsej pozriem                                                       
            // pozriem sa ci je este nejaky kluc za nim, ak ano tak opakujem ak nie tak dany string je hodnota
            var dict = dictionary.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            dict.Remove(key.Key);
            if (!GetDataFromLine(line, ref stringKeyValue, dict, type, data,isColumn,col))
            {
                //keyFound = false; // ak mam v riadku dve a viac klucovych slov tak to nastavim na false aby mi to v predchadzajucom volani vbehlo sem a nastavila sa hodnota 
                if (dic.canDeleteKeys.Contains(key.Value))
                {
                    keysToDelete.Add(key.Value);
                }
                SaveData(ref keyFound, isColumn, type, key, data, stringKeyValue, ref lineText, firstCharIndex);

                if (pair.Value != "" && lookingForRight)
                    return CONSTANTS.Result.True;

                if (lineText.Length < 5)
                    return CONSTANTS.Result.Break;
            }
            if (pair.Value != "" && lookingForRight)
                return CONSTANTS.Result.True;

            return CONSTANTS.Result.Break;
        }

        /// <summary>
        /// Methode save found data to object
        /// </summary>
        /// <param name="keyFound">Set to true if isColumn is true and type is Evidence</param>
        /// <param name="isColumn">True if looking in column</param>
        /// <param name="type">Type of object using to store data</param>
        /// <param name="key">KeyPair found in dictionary</param>
        /// <param name="data">Object to store data</param>
        /// <param name="stringKeyValue">Value to key</param>
        /// <param name="lineText">Text in line, after save key and stringKeyValue is removed from lineText</param>
        /// <param name="firstCharIndex">Start of replacing</param>
        private void SaveData(ref bool keyFound, bool isColumn, Type type, KeyValuePair<string, string> key,
                                Object data, string stringKeyValue, ref string lineText, int firstCharIndex)
        {
            PropertyInfo prop;
            if (isColumn && type == eud.GetType())
            {
                keyFound = true;
            }
            prop = type.GetProperty(key.Value);
            prop.SetValue(data, stringKeyValue, null);
            lineText = lineText.Replace(lineText.Substring(firstCharIndex), "");
        }

        /// <summary>
        /// Methode try to determine if column already exists, true if exists
        /// </summary>
        /// <param name="col">Object of Column</param>
        /// <returns></returns>
        private bool ColumnAlreadyExists(Column col)
        {
            if (listOfColumns.Count == 0 && TempListOfColumn.Count == 0)
                return false;
            else
            {
                var match = listOfColumns.Where(c => c.Text.Equals(col.Text)).FirstOrDefault();
                if (match == null) { match = TempListOfColumn.Where(c => c.Text.Equals(col.Text)).FirstOrDefault(); }
                return match != null ? true : false;
            }

        }

        private void EndRelativeColumn(Column col)
        {
            foreach (Column c in listOfColumns)
            {
                if (c.Top < col.Top && Math.Abs(c.Left - col.Left) < 50)
                {
                    c.Completed = true;
                    c.Blocked = true;
                }
            }
        }

        /// <summary>
        /// Methode try to get right X position of Column
        /// if there is text in line after column methode GetDataFromLine is called on dictionary of general info
        /// if found left X position of found key is returned 
        /// else look for text for client and width of paper is returned 
        /// </summary>
        /// <param name="a">Object of TextLine</param>
        /// <param name="line">Text in current line</param>
        /// <param name="stringKey">Found key</param>
        /// <param name="n">Object of Client</param>
        /// <returns></returns>
        private void TryGetRightXOfColumn(Column col, TextLine a,ref string line, string stringKey, Client n)
        {

            if (!SetRightXByExistingColumn(col))
            {
                line = line.Substring(line.IndexOf(stringKey) + stringKey.Length);
                if (GetDataFromLine(a, ref line, dic.header, eud.GetType(), eud, false, null, true))
                {
                    int len = pair.Key.IndexOf(" ");
                    if (len == -1)
                        len = pair.Key.Length;

                    string word = pair.Key.Substring(0, len);
                    if (!string.IsNullOrEmpty(word))
                    {
                        foreach (Word w in a.Words)
                        {
                            int sim = SimilarityService.GetSimilarity(word, w.Text);
                            if (sim > CONSTANTS.SIMILARITY)
                            {
                                col.Right = w.Bounds.Left - CONSTANTS.PROXIMITY;
                                break;
                            }

                        }

                    }

                }
                else
                {
                    GetDataFromLine(a, ref line, dic.clients, n.GetType(), n, false, null, true); // pozri ci dany text patri klientovi
                    col.Right = pictureWidth;
                }
            }


        }

        private bool SetRightXByExistingColumn(Column col)
        {
            if (listOfColumns.Count > 0)
            {
                foreach (Column c in listOfColumns)
                {
                    if (!c.Completed)
                    {
                        if (c.Right > col.Left && (!c.Completed || (c.Bottom != 0 && c.Bottom < col.Top)))
                        {
                            col.Right = c.Left;
                            return true;
                        }
                    }
                }

            }
            return false;

        }


        /// <summary>
        /// return first n characters to " " from the start
        /// </summary>
        /// <param name="s">Input text</param>
        /// <returns></returns>
        private string getFirstNNumberAsString(string s)
        {
            return s.Substring(0, s.IndexOf(' '));
        }
        /// <summary>
        /// Methode set general info to Column
        /// </summary>
        /// <param name="id">ID of Column</param>
        /// <param name="text">For who is the column. etc. Odberatel</param>
        /// <param name="line">Line where the Column was found</param>
        /// <returns></returns>
        private Column GetColumnParam(int id, string text, TextLine line)
        {
            Column c = new Column();
            c.Id = id;
            GetFirstWordOfPhrase(ref text);
            foreach (Word w in line.Words)
            {
                w.Text = (w.Text.Trim(charsToTrim));
                c.Text = w.Text;
                if (w.Text.Equals(text.Trim(charsToTrim)))
                {
                    c.Left = w.Bounds.Left;
                    c.Top = w.Bounds.Top;
                    break;
                }
            }
            return c;

        }

        private void GetFirstWordOfPhrase(ref string text)
        {
            text = text.Trim();
            if (text.Contains(" "))
            {
                text = text.Substring(0, text.IndexOf(" "));
            }
        }

        /// <summary>
        /// If current line should be Column methode extract data
        /// </summary>
        /// <param name="line">Currently processed line</param>
        private void FillColumns(TextLine line)
        {
            string otherText = line.text;
            string colText = "";
            Client client;
            bool stillColumn = false;
            foreach (Column col in listOfColumns)
            {
                if (!col.Blocked)
                {
                    client = listOfClients[col.Id - 1];
                    string text = GetWordsForColumn(col, line).Trim(charsToTrim);
                    if (!col.Completed)
                    {
                        keysInRow = 0;
                        stillColumn = true;
                        colText += text; // najskor si tu dam to co som uz pouzil

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            Type type = client.GetType();
                            GetDataFromLine(line, ref text, dic.clients, type, client, true, col);

                        }
                    }
                    else
                    {
                        Type type = client.GetType();
                        GetDataFromLine(line, ref otherText, dic.clients, type, client, true, col);
                    }
                    if (!string.IsNullOrEmpty(colText))
                    {
                        otherText = otherText.Replace(colText, "");
                        colText = "";
                    }
                }
            }
            otherText = otherText.Trim(charsToTrim);
            if (!string.IsNullOrEmpty(otherText) || otherText.Length > 3)
                GetDataFromLine(line, ref otherText, dic.header, eud.GetType(), eud, false, null);


            if (!stillColumn && TempListOfColumn.Count > 0)
                nextLineIsColumn = true;

        }


        private void ColumnFinished(Column col, TextLine line)
        {
            bool unfinishedColumn = false;
            foreach (var column in listOfColumns)
            {
                if (!column.Completed)
                    unfinishedColumn = true;
            }

            //stlpec je ukonceny ale nejaky iny este nie tak pozriem ci sa na tomto mieste nachadzaju nejake udaje
            if (unfinishedColumn)
            {
                string text = GetWordsForColumn(col, line);

                if (!string.IsNullOrWhiteSpace(text))
                {

                    if (text.Contains(":"))
                    {
                        string[] lineText = text.Split(':');
                        if (lineText.Length % 2 != 0)
                        {
                            lineText = RepairLineText(lineText, dic.clients);
                        }
                        int index = 0;
                        Type type = eud.GetType();
                        while (index < lineText.Length)
                        {

                            SearchInDictionary(lineText, ref index, dic.header, type, eud);
                            index++;
                        }

                    }
                    else
                    {

                    }

                }
            }
        }

        /// <summary>
        /// Methode return text for Column according to position of words 
        /// </summary>
        /// <param name="col">Colum for which I m looking for words</param>
        /// <param name="line">Line where I m looking</param>
        /// <returns></returns>
        private string GetWordsForColumn(Column col, TextLine line)
        {
            string a = "";

            foreach (Word w in line.Words)
            {
                if (w.Bounds.Left < col.Right)
                {
                    if (((w.Bounds.Left <= col.Left && w.Bounds.Right > col.Left) || w.Bounds.Left >= col.Left) && w.Bounds.Right < col.Right)
                    {
                        a += w.Text + " ";

                    }
                }
            }
            return a.Trim();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineText"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private string[] RepairLineText(string[] lineText, Dictionary<string, string> dic)
        {
            //string[] line = new string[lineText.Length + 1];
            List<string> line = new List<string>();
            bool found = false;
            string value = "";
            int index = 1;
            int lineIndex = index;
            line.Add(lineText[0]);
            while (index < lineText.Length)
            {
                foreach (KeyValuePair<string, string> key in dic)
                {
                    if (lineText[index].Contains(key.Key))
                    {
                        found = true;
                        value = key.Key;
                    }

                }
                if (found)
                {

                    line.Add(lineText[index].Replace(value, ""));
                    line.Add(value);
                }
                else
                {
                    line.Add(lineText[index]);
                }

                index++;
                found = false;
            }

            return line.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineText"></param>
        /// <param name="index"></param>
        /// <param name="dic"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        private void SearchInDictionary(string[] lineText, ref int index, Dictionary<string, string> dic, Type type, Object data)
        {
            PropertyInfo prop;
            int similarity = 0;
            foreach (KeyValuePair<string, string> key in dic)
            {
                similarity = SimilarityService.GetSimilarity(key.Key, lineText[index]);
                if (similarity > CONSTANTS.SIMILARITY)
                {
                    index++;
                    lineText[index] = lineText[index].Trim();
                    prop = type.GetProperty(key.Value);
                    prop.SetValue(data, lineText[index], null);
                    break;
                }
            }
        }
    }
}

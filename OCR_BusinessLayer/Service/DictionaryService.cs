using Bakalarska_praca.Dictioneries;
using OCR_BusinessLayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OCR_BusinessLayer.Service
{
    class DictionaryService : IDisposable
    {
        private int _keysInRow = 0;
        private int _columnsCount = 0;
        private List<Column> _listOfColumns;
        private List<Column> _TempListOfColumn;
        private List<Client> _listOfClients;
        private List<string> _keysToDelete;
        private Dictionary _dic;
        private PreviewObject _p;
        private KeyValuePair<string, string> _pair;
        private Evidence _eud;

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
        public void MakeObjectsFromLines(PreviewObject p, FileToProcess file, IProgress<int> progress)
        {
            _p = p;
            int Step = p.Lines.Count / 60;
            _dic = new Dictionary();
            _eud = new Evidence();
            Type type = _eud.GetType();
            _listOfColumns = new List<Column>();
            _TempListOfColumn = new List<Column>();
            _listOfClients = new List<Client>();
            _keysToDelete = new List<string>();
            _p.ListOfKeyPossitions = new List<PossitionOfWord>();
            foreach (TextLine line in p.Lines)
            {
                line.Text = line.Text.Trim(CONSTANTS.charsToTrimLine);
                _keysInRow = 0;
                _listOfColumns.AddRange(_TempListOfColumn);
                _TempListOfColumn.Clear();
                foreach (var item in _dic.header.Where(c => _keysToDelete.Contains(c.Value)).ToList())
                {
                    _dic.header.Remove(item.Key);
                }
                _keysToDelete.Clear();

                if (GoLikeColumn())
                {
                    FillColumns(line);
                    continue;
                }
                string t = line.Text;
                GetDataFromLine(line, ref t, _dic.header, type, _eud);



                progress.Report(Step);
            }
            ValidationService.Validate(_eud);
            foreach (Client c in _listOfClients)
            {
                ValidationService.Validate(c);
            }
        }

        private bool GoLikeColumn()
        {
            var col = _listOfColumns.Where(c => c.Completed == false).FirstOrDefault();
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
                    if (similarity > CONSTANTS.SIMILARITY && !IsInMiddleOfWord(lineText, firstCharindex, keyLength, key.Key[0], key.Key[key.Key.Length - 1], stringKey))
                    {
                        if (col != null && type == _eud.GetType())
                        {
                            // aktualny stlpec skoncil
                            col.Completed = true;
                            col.Bottom = line.Words[0].Bounds.Top;
                        }
                        res = jahoda(line, key, stringKey, ref lineText, firstCharindex, dictionary, type, data, ref keyFound, lookingForRight, ref isColumn, col);
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

                        firstCharindex += index + 1;
                    }



                }
                if (res == CONSTANTS.Result.Break && lineText.Length < 10) { break; }
                else if (res == CONSTANTS.Result.Continue) { continue; }
                else { continue; }

            }

            if (isColumn && !keyFound && col != null && col.FirstLineInColumn > 4)
            {
                // skus iny slovnik
                if (type == _eud.GetType())
                {
                    foreach (Column c in _listOfColumns)
                        GetDataFromLine(line, ref lineText, _dic.clients, c.GetType(), _listOfClients[c.Id - 1], false, col, false);
                }
                else
                {
                    GetDataFromLine(line, ref lineText, _dic.header, _eud.GetType(), _eud, false, col, false);
                }

            }

            if (isColumn && col != null && !keyFound)
            {
                GetDataFromLine(line, ref lineText, _dic.header, _eud.GetType(), _eud, false, col, false);
                if (_keysInRow == 0)
                {
                    Client client = (Client)data;
                    string s = lineText.Trim(CONSTANTS.charsToTrim);
                    if (!string.IsNullOrEmpty(s) && s.Length >= 5)
                    {
                        switch (col.FirstLineInColumn)
                        {
                            case 1:
                                client.Name = lineText;
                                col.FirstLineInColumn++;
                                SavePossitionToLists("Name","", lineText, line, line,col.Text + " Meno");
                                break;
                            case 2:
                                client.Street = lineText;
                                col.FirstLineInColumn++;
                                SavePossitionToLists("Street","", lineText, line, line, col.Text + " Ulica");
                                break;
                            case 3:
                                client.PSCCity = lineText;
                                col.FirstLineInColumn++;
                                SavePossitionToLists("PSCCity", "", lineText, line, line, col.Text + " Psč");
                                break;
                            case 4:
                                client.State = lineText;
                                col.FirstLineInColumn++;
                                SavePossitionToLists("State","", lineText, line, line, col.Text + " Štát");
                                break;

                        }
                    }
                }
            }


            if (_keysInRow >= 2)
                return false;

            return keyFound;
        }


        private CONSTANTS.Result jahoda(TextLine line, KeyValuePair<string, string> key, string stringKey, ref string lineText, int firstCharIndex,
                                        Dictionary<string, string> dictionary, Type type, Object data, ref bool keyFound, bool lookingForRight,
                                        ref bool isColumn, Column col)
        {
            string stringKeyValue = "";
            if (lookingForRight)
            {

                _pair = key;

            }
            if (_dic.columns.ContainsKey(key.Key))
            {
                // je to stlpec
                _columnsCount++;
                Column column = GetColumnParam(_columnsCount, stringKey, line);
                column.Text = key.Key;
                if (ColumnAlreadyExists(column))
                {
                    return CONSTANTS.Result.Continue;
                }
                Client n = new Client();
                EndRelativeColumn(column);
                var s = line.Text;
                TryGetRightXOfColumn(column, line, ref s, stringKey, n);
                _TempListOfColumn.Add(column);
                lineText = lineText.Replace(key.Key, "");
                _listOfClients.Add(n);
                s = s.Trim(CONSTANTS.charsToTrim);
                if (!(string.IsNullOrEmpty(s) || s.Length < 5)) // uz aktualny riadok moze byt stlpec tak to tu poriesim
                {
                    s = s.Replace(stringKey, "");
                    GetDataFromLine(line, ref s, dictionary, type, n, true, column, false); // ak mam za klucovym slovom (Odberatel) nejaky text a nie su tam ine klucove slova
                }

                return CONSTANTS.Result.Break;
            }

            keyFound = true;
            if (isColumn && type == _eud.GetType())
            {
                stringKeyValue = line.Text.Substring(firstCharIndex + stringKey.Length);
            }
            else
            {
                stringKeyValue = lineText.Substring(firstCharIndex + stringKey.Length);
            }
            if (!isColumn && _listOfClients.Count > 0 && type == _listOfClients[0]?.GetType())
            {
                isColumn = true;
            }

            _keysInRow++;
            // nasiel som nejaky kluc v riadku, toto by mala byt jeho hodnota ale moze obsahovat este nejaky iny kluc tak sa radsej pozriem                                                       
            // pozriem sa ci je este nejaky kluc za nim, ak ano tak opakujem ak nie tak dany string je hodnota
            var dict = dictionary.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            dict.Remove(key.Key);
            stringKeyValue = stringKeyValue.Trim(CONSTANTS.charsToTrimLine);
            if (!GetDataFromLine(line, ref stringKeyValue, dict, type, data, isColumn, col))
            {
                //keyFound = false; // ak mam v riadku dve a viac klucovych slov tak to nastavim na false aby mi to v predchadzajucom volani vbehlo sem a nastavila sa hodnota 
                if (_dic.canDeleteKeys.Contains(key.Value))
                {
                    _keysToDelete.Add(key.Value);
                }
                bool saved = false;
                stringKeyValue = stringKeyValue.Trim(CONSTANTS.charsToTrimLine);
                if (string.IsNullOrEmpty(stringKeyValue) && SETTINGS.GoInColumnForValue && _dic.valueInColumn.Contains(key.Value))
                    stringKeyValue = FindValueInColumn(line, stringKey, key.Key, ref saved);

                if (!saved)
                    SavePossitionToLists(key.Key, stringKey.Trim(CONSTANTS.charsToTrim), stringKeyValue.Trim(CONSTANTS.charsToTrim), line, line);
                SaveData(ref keyFound, isColumn, type, key, data, stringKeyValue, ref lineText, firstCharIndex);

                if (_pair.Value != "" && lookingForRight)
                    return CONSTANTS.Result.True;

                if (lineText.Length < 5)
                    return CONSTANTS.Result.Break;
            }
            if (_pair.Value != "" && lookingForRight)
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
            if (isColumn && type == _eud.GetType())
            {
                keyFound = true;
            }
            prop = type.GetProperty(key.Value);
            if (prop.GetValue(data) == null)
            {
                prop.SetValue(data, stringKeyValue, null);
            }
            lineText = lineText.Replace(lineText.Substring(firstCharIndex), "");
        }

        private void SavePossitionToLists(string key, string stringkey, string value, TextLine Keyline, TextLine valueLine, string colText = "", int x1 = 0, int x2 = 0)
        {
            List<Word> tmpKeyWords = new List<Word>();
            List<Word> tmpValueWords = new List<Word>();
            string first = GetFirstWordOfPhrase(stringkey);
            string last = GetLastWordOfPhrase(stringkey);
            float conf = 0;
            int count = 0;
            foreach (Word wk in Keyline.Words)
            {
                string s = wk.Text.Trim(CONSTANTS.charsToTrimLine);
                if (s.Contains(first) && !string.IsNullOrWhiteSpace(s))
                {
                    tmpKeyWords.Add(wk);
                    conf += wk.Confidence;
                    count++;
                    if (first.Equals(last))
                        break;
                    else
                        first = GetNextWordOfPhrase(first, stringkey);

                }

            }
            if (valueLine != null)
            {
                foreach (Word wv in valueLine.Words)
                {

                    if (!string.IsNullOrWhiteSpace(value) && (value.Contains(wv.Text.Trim(CONSTANTS.charsToTrimLine)) || wv.Text.Contains(value)))
                    {
                        tmpValueWords.Add(wv);
                        conf += wv.Confidence;
                        count++;
                    }
                }
            }

            //key
            PossitionOfWord pk = new PossitionOfWord();
            if (stringkey != "")
            {
                var v = tmpKeyWords.First<Word>();
                var vl = tmpKeyWords.Last<Word>();
                pk.KeyBounds = new System.Drawing.Rectangle(v.Bounds.X, v.Bounds.Y, tmpKeyWords.Last<Word>().Bounds.Right - v.Bounds.Left, v.Bounds.Height);
            }
            if (key.Equals("Name") || key.Equals("Street") || key.Equals("PSCCity") || key.Equals("State"))
                pk.Key = colText;
            else
                pk.Key = stringkey;

            pk.Value = value;

            //value
            if (value == "")
            {
                pk.ValueBounds = new System.Drawing.Rectangle(pk.KeyBounds.Right, pk.KeyBounds.Y, 100, pk.KeyBounds.Height);

            }
            else
            {
                var w = tmpValueWords.First<Word>();
                if (Keyline == valueLine)
                {
                    //nie je to stlpec
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        pk.ValueBounds = new System.Drawing.Rectangle(w.Bounds.X, w.Bounds.Y, tmpValueWords.Last<Word>().Bounds.Right - w.Bounds.Left, w.Bounds.Height);

                    }
                    else
                    {
                        var x = w.Bounds.Right;
                        var y = w.Bounds.Y;
                        var width = 0;
                        try
                        {
                            if (stringkey == "")
                            {
                                width = tmpValueWords.Last<Word>().Bounds.Right - w.Bounds.Left;
                            }
                            else
                            {
                                var vl = tmpKeyWords.Last<Word>();
                                width = Keyline.Words[Keyline.Words.IndexOf(vl) + 1].Bounds.Left - vl.Bounds.Right;
                            }
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            width = w.Bounds.Right + 50;
                        }
                        pk.ValueBounds = new System.Drawing.Rectangle(x, y, width, w.Bounds.Height);


                    }
                }
                else
                {
                    //hladal som v stlpci
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        pk.ValueBounds = new System.Drawing.Rectangle(w.Bounds.X, w.Bounds.Y, tmpValueWords.Last<Word>().Bounds.Right - w.Bounds.Left, w.Bounds.Height);
                    }
                    else
                    {
                        pk.ValueBounds = new System.Drawing.Rectangle(x1, valueLine.Bounds.Y, x2 - x1, w.Bounds.Height);

                    }
                }
            }


            pk.Confidence = conf / count;
            _p.ListOfKeyPossitions.Add(pk);

        }


        /// <summary>
        /// Methode try to determine if column already exists, true if exists
        /// </summary>
        /// <param name="col">Object of Column</param>
        /// <returns></returns>
        private bool ColumnAlreadyExists(Column col)
        {
            if (_listOfColumns.Count == 0 && _TempListOfColumn.Count == 0)
                return false;
            else
            {
                var match = _listOfColumns.Where(c => c.Text.Equals(col.Text)).FirstOrDefault();
                if (match == null) { match = _TempListOfColumn.Where(c => c.Text.Equals(col.Text)).FirstOrDefault(); }
                return match != null ? true : false;
            }

        }
        /// <summary>
        /// Methode mark column as blocked if current column is under him
        /// </summary>
        /// <param name="col">Current column</param>
        private void EndRelativeColumn(Column col)
        {
            foreach (Column c in _listOfColumns)
            {
                if (c.Top < col.Top && Math.Abs(c.Left - col.Left) < 50)
                {
                    c.Completed = true;
                    c.Blocked = true;
                }
            }
        }

        /// <summary>
        /// Methode determine if the found key is in middle of word or not
        /// if true it's not a key else it's key
        /// </summary>
        /// <param name="text">Text where looking for key</param>
        /// <param name="firstChar">Index of first cahracter of key</param>
        /// <param name="keyLength">Length of key</param>
        /// <param name="ch">First character</param>
        /// <param name="c">Last character</param>
        /// <param name="stringKey">Orginal key</param>
        /// <returns></returns>
        private bool IsInMiddleOfWord(string text, int firstChar, int keyLength, char ch, char c, string stringKey)
        {
            string sub = "";
            int length = 0;
            if (firstChar == 0)
            {
                if (text.Length - firstChar >= keyLength + 1)
                {
                    length = keyLength + 1;
                }
                else
                {
                    length = keyLength;
                }

                sub = text.Substring(firstChar, length);
                if (sub[sub.Length - 1] == ' ' || sub[sub.Length - 1] == '.' || sub[sub.Length - 1] == ',' || sub[sub.Length - 1] == ':' || sub[sub.Length - 1] == ';' || sub[sub.Length - 1] == c)
                    return false;
            }
            else
            {
                if (text.Length - firstChar - 1 >= keyLength + 2)
                {
                    length = keyLength + 2;
                }
                else if (text.Length - firstChar - 1 >= keyLength + 1)
                {
                    length = keyLength + 1;
                }
                else
                {
                    length = keyLength;
                }
                sub = text.Substring(firstChar - 1, length).Trim(CONSTANTS.charsToTrimLine);
                if (sub.Length - stringKey.Length <= 1)
                    return false;

                if ((sub[0] == ' ' || sub[0] == ch) && (sub[sub.Length - 1] == ' ' || sub[sub.Length - 1] == '.' || sub[sub.Length - 1] == ',' || sub[sub.Length - 1] == ':' || sub[sub.Length - 1] == ';' || sub[sub.Length - 1] == c))
                    return false;

            }

            return true;
        }

        /// <summary>
        /// Methode look for value to key in column
        /// </summary>
        /// <param name="line">Current line</param>
        /// <param name="foundKey">Current key</param>
        /// <returns></returns>
        private string FindValueInColumn(TextLine line, string foundKey, string key, ref bool saved)
        {
            int x1 = 0, x2 = 0;
            string firstWord = GetFirstWordOfPhrase(foundKey).Trim();
            string lastWord = GetLastWordOfPhrase(foundKey).Trim();
            string res = "";
            if (firstWord.Equals(lastWord))
            {
                Word w = line.Words.Where(c => c.Text.Trim(CONSTANTS.charsToTrim).Equals(foundKey.Trim())).FirstOrDefault();
                x1 = w.Bounds.Left;
                x2 = w.Bounds.Right;
            }
            else
            {
                Word w = line.Words.Where(c => c.Text.Trim(CONSTANTS.charsToTrim).Equals(lastWord)).FirstOrDefault();
                Word w2 = line.Words[line.Words.IndexOf(w) - 1];
                if (!w2.Text.Trim(CONSTANTS.charsToTrim).Equals(firstWord))
                {
                    w2 = line.Words[line.Words.IndexOf(w2) - 1];
                }

                x1 = w2.Bounds.Left;
                x2 = w.Bounds.Right;
            }

            try
            {
                for (int i = 1; i <= 2; i++)
                {
                    TextLine t = _p.Lines[_p.Lines.IndexOf(line) + i];
                    res = GetWordsForColumn(new Column { Left = x1, Right = x2 }, t);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        SavePossitionToLists(key, foundKey.Trim(CONSTANTS.charsToTrim), res.Trim(CONSTANTS.charsToTrim), line, t,"", x1, x2);
                        saved = true;
                        break;
                    }

                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception(e.Message);
            }

            return res;
        }



        /// <summary>
        /// Methode try to get right X position of Column
        /// if there is text in line after column, methode GetDataFromLine is called on dictionary of general info
        /// if found left X position of found key is returned 
        /// else look for text for client and width of paper is returned 
        /// </summary>
        /// <param name="a">Object of TextLine</param>
        /// <param name="line">Text in current line</param>
        /// <param name="stringKey">Found key</param>
        /// <param name="n">Object of Client</param>
        /// <returns></returns>
        private void TryGetRightXOfColumn(Column col, TextLine a, ref string line, string stringKey, Client n)
        {

            if (!SetRightXByExistingColumn(col))
            {
                line = line.Substring(line.IndexOf(stringKey) + stringKey.Length);
                if (GetDataFromLine(a, ref line, _dic.header, _eud.GetType(), _eud, false, null, true))
                {
                    int len = _pair.Key.IndexOf(" ");
                    if (len == -1)
                        len = _pair.Key.Length;

                    string word = _pair.Key.Substring(0, len);
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
                    GetDataFromLine(a, ref line, _dic.clients, n.GetType(), n, false, null, true); // pozri ci dany text patri klientovi
                    col.Right = _p.Img.Width;
                }
            }


        }

        /// <summary>
        /// Methode set right edge of column by existing colum (if exists)
        /// </summary>
        /// <param name="col">Current column</param>
        /// <returns></returns>
        private bool SetRightXByExistingColumn(Column col)
        {
            if (_listOfColumns.Count > 0)
            {
                foreach (Column c in _listOfColumns)
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
            text = GetFirstWordOfPhrase(text);
            foreach (Word w in line.Words)
            {
                w.Text = (w.Text.Trim(CONSTANTS.charsToTrim));
                c.Text = w.Text;
                if (w.Text.Equals(text.Trim(CONSTANTS.charsToTrim)))
                {
                    c.Left = w.Bounds.Left;
                    c.Top = w.Bounds.Top;
                    break;
                }
            }
            return c;

        }

        /// <summary>
        /// Methode return first word in text
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns></returns>
        private string GetFirstWordOfPhrase(string text)
        {
            text = text.Trim();
            if (text.Contains(" "))
            {
                text = text.Substring(0, text.IndexOf(" "));
            }
            return text.Trim();
        }

        private string GetNextWordOfPhrase(string prev, string text)
        {
            text = text.Remove(0, text.IndexOf(prev));
            text = text.Replace(prev, "");
            text = text.Trim();
            if (text.Contains(" "))
            {
                text = text.Substring(0, text.IndexOf(" "));
            }
            return text.Trim();
        }

        /// <summary>
        /// Methode return last word in text
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns></returns>
        private string GetLastWordOfPhrase(string text)
        {
            text = text.Trim();
            if (text.Contains(" "))
            {
                text = text.Substring(text.LastIndexOf(" "));
            }
            return text.Trim();
        }

        /// <summary>
        /// If current line should be Column methode extract data
        /// </summary>
        /// <param name="line">Currently processed line</param>
        private void FillColumns(TextLine line)
        {
            string otherText = line.Text;
            string colText = "";
            Client client;
            foreach (Column col in _listOfColumns)
            {
                if (!col.Blocked)
                {
                    client = _listOfClients[col.Id - 1];
                    string text = GetWordsForColumn(col, line).Trim(CONSTANTS.charsToTrim);
                    if (!col.Completed)
                    {
                        _keysInRow = 0;
                        colText += text; // najskor si tu dam to co som uz pouzil

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            Type type = client.GetType();
                            GetDataFromLine(line, ref text, _dic.clients, type, client, true, col);

                        }
                    }
                    else
                    {
                        Type type = client.GetType();
                        GetDataFromLine(line, ref otherText, _dic.clients, type, client, true, col);
                    }
                    if (!string.IsNullOrEmpty(colText))
                    {
                        otherText = otherText.Replace(colText, "");
                        colText = "";
                    }
                }
            }
            otherText = otherText.Trim(CONSTANTS.charsToTrim);
            if (!string.IsNullOrEmpty(otherText) || otherText.Length > 3)
                GetDataFromLine(line, ref otherText, _dic.header, _eud.GetType(), _eud, false, null);

        }


        private void ColumnFinished(Column col, TextLine line)
        {
            bool unfinishedColumn = false;
            foreach (var column in _listOfColumns)
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
                            lineText = RepairLineText(lineText, _dic.clients);
                        }
                        int index = 0;
                        Type type = _eud.GetType();
                        while (index < lineText.Length)
                        {

                            SearchInDictionary(lineText, ref index, _dic.header, type, _eud);
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
                if (w.Bounds.Left < col.Right && w.Bounds.Width < CONSTANTS.MAX_LENGTH_OF_ONE_WORD)
                {
                    if (((w.Bounds.Left <= col.Left && w.Bounds.Right > col.Left) || w.Bounds.Left >= col.Left) && ((w.Bounds.Right >= col.Right && w.Bounds.Left < col.Right) || w.Bounds.Right <= col.Right))

                    {
                        if (col.Left > w.Bounds.Right)
                            col.Left = w.Bounds.Right;

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

using Bakalarska_praca.Dictioneries;
using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Classes.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
        public void MakeObjectsFromLines(PreviewObject p,IProgress<int> progress)
        {
            _p = p;
            _dic = (Dictionary)Dictionary.GetInstance().Clone();
            _eud = new Evidence();
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
                GetDataFromLine(line, ref t, _dic.header, _eud);



                progress.Report(1);
            }

            _p.ListOfKeyColumn = new List<Column>(_listOfColumns);
            _p.Evidence = _eud;
            _p.Clients = new ClientCollection(_listOfClients) { Capacity =5};
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
        private bool GetDataFromLine(TextLine line, ref string lineText, Dictionary<string, string> dictionary, Object data, bool isColumn = false, Column col = null, bool lookingForRight = false)
        {
            int firstCharindex;
            int keyLength;
            bool keyFound = false;
            int similarity = 0;
            bool IndexIsNull = false;
            string stringKey = string.Empty;
            CONSTANTS.Result res = CONSTANTS.Result.Continue;
            foreach (KeyValuePair<string, string> key in dictionary)
            {
                firstCharindex = lineText.ToLower().IndexOf(key.Key.Substring(0, 1).ToLower()); //  index of the first occurrence of the first character of the key
                keyLength = key.Key.Length; // length of the key
                while ((keyLength + firstCharindex) <= lineText.Length && firstCharindex != -1) // if the key is longet than found text it si not the text i'm looking for, go to next text
                {

                    stringKey = lineText.Substring(firstCharindex, keyLength); // this is the text i got from OC
                    similarity = SimilarityService.GetSimilarity(key.Key.ToLower(), stringKey.ToLower());
                    if (similarity > CONSTANTS.SIMILARITY && !IsInMiddleOfWord(lineText, firstCharindex, keyLength, key.Key[0], key.Key[key.Key.Length - 1], stringKey))
                    {
                        if (col != null && data.GetType() == _eud.GetType())
                        {
                            // current column ended
                            col.Completed = true;
                            col.Bottom = line.Words[0].Bounds.Top;
                        }
                        res = PrepareToSave(line, key, stringKey, ref lineText, firstCharindex, dictionary, data, ref keyFound, lookingForRight, ref isColumn, col);
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
                            // nothing has been found, move to the next key
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
                // try different dictionary
                if (data.GetType() == _eud.GetType())
                {
                    foreach (Column c in _listOfColumns)
                        GetDataFromLine(line, ref lineText, _dic.clients, _listOfClients[c.Id - 1], false, col, false);
                }
                else
                {
                    GetDataFromLine(line, ref lineText, _dic.header, _eud, false, col, false);
                }

            }

            if (isColumn && col != null && !keyFound)
            {
                GetDataFromLine(line, ref lineText, _dic.header, _eud, false, col, false);
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
                                SavePossitionToLists("Name", string.Empty, client.Name, line, line, col.Text + " Meno");
                                break;
                            case 2:
                                client.Street = lineText;
                                col.FirstLineInColumn++;
                                SavePossitionToLists("Street", string.Empty, client.Street, line, line, col.Text + " Ulica");
                                break;
                            case 3:
                                client.PSCCity = lineText;
                                col.FirstLineInColumn++;
                                SavePossitionToLists("PSCCity", string.Empty, client.PSCCity, line, line, col.Text + " Psč");
                                break;
                            case 4:
                                client.State = lineText;
                                col.FirstLineInColumn++;
                                SavePossitionToLists("State", string.Empty, client.State, line, line, col.Text + " Štát");
                                break;

                        }
                    }
                }
            }


            if (_keysInRow >= 2)
                return false;
            if (lookingForRight && _pair.Key != null)
                return true;


            return keyFound;
        }


        private CONSTANTS.Result PrepareToSave(TextLine line, KeyValuePair<string, string> key, string stringKey, ref string lineText, int firstCharIndex,
                                        Dictionary<string, string> dictionary, Object data, ref bool keyFound, bool lookingForRight,
                                        ref bool isColumn, Column col)
        {
            string stringKeyValue = string.Empty;
            if (lookingForRight)
            {

                _pair = key;

            }
            if (_dic.columns.ContainsKey(key.Key))
            {
                // it is a column
                _columnsCount++;
                Column column = GetColumnParam(_columnsCount, stringKey, line);
                column.Text = key.Key;
                if (ColumnAlreadyExists(column))
                {
                    column = null;
                    _columnsCount--;
                    return CONSTANTS.Result.Continue;
                }
                Client n = new Client();
                n.ClientID = column.Text;
                EndRelativeColumn(column);
                var s = line.Text;
                TryGetRightXOfColumn(column, line, ref s, stringKey, n);
                _TempListOfColumn.Add(column);
                lineText = lineText.Replace(key.Key, string.Empty);
                _listOfClients.Add(n);
                s = s.Trim(CONSTANTS.charsToTrim);
                if (!(string.IsNullOrEmpty(s) || s.Length < 5)) // current column could be a column => check it
                {
                    s = s.Replace(stringKey, string.Empty);
                    GetDataFromLine(line, ref s, dictionary, n, true, column, false); // key word followed by (Odberatel)
                }

                return CONSTANTS.Result.Break;
            }

            keyFound = true;
            if (isColumn && data.GetType() == _eud.GetType())
            {
                stringKeyValue = line.Text.Substring(firstCharIndex + stringKey.Length);
            }
            else
            {
                stringKeyValue = lineText.Substring(firstCharIndex + stringKey.Length);
            }
            if (!isColumn && _listOfClients.Count > 0 && data.GetType() == _listOfClients[0]?.GetType())
            {
                isColumn = true;
            }

            _keysInRow++;
            // there were key in the row, this should be it's value but it could contain more keys => check for another keys
            //if there are keys behind current key, look for more key, othervise this is it's value
            var dict = dictionary.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            dict.Remove(key.Key);
            stringKeyValue = stringKeyValue.Trim(CONSTANTS.charsToTrimLine);
            if (!GetDataFromLine(line, ref stringKeyValue, dict, data, isColumn, col))
            {

                if (_dic.canDeleteKeys.Contains(key.Value))
                {
                    _keysToDelete.Add(key.Value);
                }
                bool saved = false;
                stringKeyValue = stringKeyValue.Trim(CONSTANTS.charsToTrimLine);
                if (string.IsNullOrEmpty(stringKeyValue) &&  _dic.valueInColumn.Contains(key.Value))
                    stringKeyValue = FindValueInColumn(line, stringKey, key.Key, ref saved);

                stringKeyValue = stringKeyValue.Trim(CONSTANTS.charsToTrimLineForpossition);


                stringKeyValue = SaveData(ref keyFound, isColumn, key, data, stringKeyValue, ref lineText, firstCharIndex);

                if (!saved)
                    SavePossitionToLists(key.Value, stringKey.Trim(CONSTANTS.charsToTrimLineForpossition), stringKeyValue, line, line);

                if (data.GetType() == typeof(Evidence))
                {
                    float fakeConf = 0;
                    int fakeCount = 0;
                    EndRelaticeColumnByText(GetWordsForPositionSave(line, stringKeyValue, ref fakeConf, ref fakeCount).FirstOrDefault());
            }
                if (_pair.Value != string.Empty && lookingForRight)
                    return CONSTANTS.Result.True;

                if (lineText.Length < 5)
                    return CONSTANTS.Result.Break;
            }
            if (_pair.Value != string.Empty && lookingForRight)
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
        private string SaveData(ref bool keyFound, bool isColumn, KeyValuePair<string, string> key,
                                Object data, string stringKeyValue, ref string lineText, int firstCharIndex)
        {
            PropertyInfo prop;
            if (isColumn && data.GetType() == _eud.GetType())
            {
                keyFound = true;
            }

            prop = data.GetType().GetProperty(key.Value);

                prop.SetValue(data, stringKeyValue, null);
            
            lineText = lineText.Replace(lineText.Substring(firstCharIndex), string.Empty);
            return (string)prop.GetValue(data);
        }

        private void SavePossitionToLists(string key, string stringkey, string value, TextLine Keyline, TextLine valueLine, string colText = "", int x1 = 0, int x2 = 0)
        {
            if (!_p.ListOfKeyPossitions.Any(c => c.Key.Equals(key)))
            {
                List<Word> tmpKeyWords = new List<Word>();
                List<Word> tmpValueWords = new List<Word>();
                float conf = 0;
                int count = 0;
                tmpKeyWords = GetWordsForPositionSave(Keyline, stringkey, ref conf, ref count);

                tmpValueWords = GetWordsForPositionSave(valueLine, value, ref conf, ref count, x1, x2);

                //key
                PossitionOfWord pk = new PossitionOfWord();
                if (stringkey != string.Empty)
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
                if (value == string.Empty || !tmpValueWords.Any())
                {
                    pk.ValueBounds = new System.Drawing.Rectangle(pk.KeyBounds.Right, pk.KeyBounds.Y, 100, pk.KeyBounds.Height);

                }
                else
                {
                    var w = tmpValueWords.First<Word>();
                    if (Keyline == valueLine)
                    {
                        // not a column
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
                                if (stringkey == string.Empty)
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
                        // was looking in column
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

                pk.Confidence = string.Format("{0:N2}%", (conf / count));
                pk.DictionaryKey = key;
                _p.ListOfKeyPossitions.Add(pk);
            }
        }

        private List<Word> GetWordsForPositionSave(TextLine line, string value, ref float conf, ref int count, int x1 = 0, int x2 = 0)
        {
            var tmpWords = new List<Word>();
            var tmp = new List<Word>();
            string first, sFirst = GetFirstWordOfPhrase(value);
            string last = GetLastWordOfPhrase(value);
            if (line != null)
            {
                first = GetFirstWordOfPhrase(value);
                last = GetLastWordOfPhrase(value);
                foreach (Word wv in line.Words)
                {
                    string s = wv.Text.Trim(CONSTANTS.charsToTrimLineForpossition);
                    if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(s) && s.Contains(first))
                    {
                        tmp.Clear();
                        if (x1 != 0 && x2 != 0)
                        {
                            if ((wv.Bounds.Left > x1 && wv.Bounds.Right < x2) || (wv.Bounds.Left < x1 && wv.Bounds.Right > x1) ||
                                (wv.Bounds.Left < x2 && wv.Bounds.Right > x2) || (wv.Bounds.Left < x1 && wv.Bounds.Right > x2))
                            {
                                tmpWords.Add(wv);
                                conf += wv.Confidence;
                                count++;
                                if (first.Equals(last))
                                    break;
                                else
                                    first = GetNextWordOfPhrase(first, value);
                            }
                            continue;
                        }
                        tmpWords.Add(wv);
                        conf += wv.Confidence;
                        count++;
                        if (first.Equals(last))
                            break;
                        else
                            first = GetNextWordOfPhrase(first, value);
                    }
                    else
                    {
                        tmp.AddRange(tmpWords);
                        tmpWords.Clear();
                        first = sFirst;
                    }

                }
            }

            if (tmpWords.Count == 0)
                tmpWords = tmp;
            return tmpWords;

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

        private void EndRelaticeColumnByText(Word w)
        {
            if (w != null)
                foreach (Column c in _listOfColumns)
                {
                    if (w.Bounds.Right < c.Right)
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
            string sub = string.Empty;
            int length = 0;
            if (text.Length == stringKey.Length)
                return false;

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
            string res = string.Empty;
            if (firstWord.Equals(lastWord))
            {
                Word w = line.Words.Where(c => c.Text.Trim(CONSTANTS.charsToTrim).Equals(foundKey.Trim())).FirstOrDefault();
                x1 = w.Bounds.Left;
                x2 = w.Bounds.Right;
            }
            else
            {
                bool found = false;
                var tmp = line.Words.Where(c => c.Text.Trim(CONSTANTS.charsToTrim).Equals(lastWord)).ToList();
                Word w = tmp.FirstOrDefault();
                Word w2 = line.Words[line.Words.IndexOf(w)];
                var index = line.Words.IndexOf(w);
                if (index != 0)
                    while (index >= 0)
                    {
                        w2 = line.Words[line.Words.IndexOf(w2) - 1];
                        index--;
                        if (w2.Text.Trim(CONSTANTS.charsToTrim).Equals(lastWord))
                        {
                            if (tmp.IndexOf(w) + 1 < tmp.Count())
                            {
                                w = tmp[tmp.IndexOf(w) + 1];
                                w2 = line.Words[line.Words.IndexOf(w)];
                                index = line.Words.IndexOf(w);
                                continue;
                            }
                            else
                                break;
                        }
                        if (w2.Text.Trim(CONSTANTS.charsToTrim).Equals(firstWord))
                        {
                            found = true;
                            break;
                        }
                        if (!found && index == 0)
                        {
                            if (tmp.IndexOf(w) + 1 < tmp.Count())
                            {
                                w = tmp[tmp.IndexOf(w) + 1];
                                w2 = line.Words[line.Words.IndexOf(w)];
                                index = line.Words.IndexOf(w);
                            }
                            else
                                break;
                        }
                    }

                x1 = w2.Bounds.Left;
                x2 = w.Bounds.Right;
            }

            try
            {
                for (int i = 1; i <= 2; i++)
                {
                    TextLine t = _p.Lines[_p.Lines.IndexOf(line) + i];
                    res = Common.GetWordsForColumn(new Column { Left = x1, Right = x2 }, t);
                    if (!string.IsNullOrWhiteSpace(res) || i == 2)
                    {
                        SavePossitionToLists(key, foundKey.Trim(CONSTANTS.charsToTrimLineForpossition), res.Trim(CONSTANTS.charsToTrimLineForpossition), line, t, string.Empty, x1, x2);
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
                if (GetDataFromLine(a, ref line, _dic.header, _eud, false, null, true))
                {
                    int len = _pair.Key.IndexOf(" ");
                    if (len == -1)
                        len = _pair.Key.Length;

                    string word = col.Text.Substring(0, len);
                    if (!string.IsNullOrEmpty(word))
                    {
                        foreach (Word w in a.Words)
                        {
                            int sim = SimilarityService.GetSimilarity(word.ToLower(), w.Text.ToLower());
                            if (sim > CONSTANTS.SIMILARITY)
                            {
                                if (w.Bounds.Right > _p.Img.Width / 2)
                                {
                                    col.Right = _p.Img.Width;
                                }
                                else
                                {
                                    col.Right = _p.Img.Width / 2;
                                }
                                break;
                            }

                        }

                    }

                }
                else
                {
                    GetDataFromLine(a, ref line, _dic.clients, n, false, null, true); // check if current text belongs to client
                    if (col.Left < _p.Img.Width / 2)
                    {
                        col.Right = _p.Img.Width/2;
                    }
                    else
                    {
                        col.Right = _p.Img.Width;
                    }
                }
            }
            if (col.Right == 0)
                col.Right = _p.Img.Width;


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
            text = text.Trim(CONSTANTS.charsToTrimLineForpossition);
            if (text.Contains(" "))
            {
                text = text.Substring(0, text.IndexOf(" "));
            }
            return text.Trim(CONSTANTS.charsToTrimLineForpossition);
        }

        /// <summary>
        /// Methode return next word in text, depends on prev parameter
        /// </summary>
        /// <param name="prev">Indicates word befor word which shoul by returned</param>
        /// <param name="text">Text where looking for words</param>
        /// <returns></returns>
        private string GetNextWordOfPhrase(string prev, string text)
        {
            text = text.Remove(0, text.IndexOf(prev));
            if (text.Contains(prev) && !string.IsNullOrEmpty(prev))
                text = text.Replace(prev, string.Empty);
            text = text.Trim();
            if (text.Contains(" "))
            {
                text = text.Substring(0, text.IndexOf(" "));
            }
            return text.Trim(CONSTANTS.charsToTrimLineForpossition);
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
            return text.Trim(CONSTANTS.charsToTrimLineForpossition);
        }

        /// <summary>
        /// If current line should be Column methode extract data
        /// </summary>
        /// <param name="line">Currently processed line</param>
        private void FillColumns(TextLine line)
        {
            string otherText = line.Text;
            string colText = string.Empty;
            Client client;
            foreach (Column col in _listOfColumns)
            {
                if (!col.Blocked)
                {
                    client = _listOfClients[col.Id - 1];
                    string text = Common.GetWordsForColumn(col, line).Trim(CONSTANTS.charsToTrim);
                    _keysInRow = 0;
                    colText += text; // najskor si tu dam to co som uz pouzil

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        GetDataFromLine(line, ref text, _dic.clients, client, true, col);
                    }

                    if (!string.IsNullOrEmpty(colText))
                    {
                        otherText = otherText.Replace(colText, string.Empty);
                        colText = string.Empty;
                    }
                }
            }
            otherText = otherText.Trim(CONSTANTS.charsToTrim);
            if (!string.IsNullOrEmpty(otherText) || otherText.Length > 3)
                GetDataFromLine(line, ref otherText, _dic.header, _eud, false, null);

        }
    }
}

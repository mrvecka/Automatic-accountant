using Bakalarska_praca.Classes;
using Bakalarska_praca.Dictioneries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bakalarska_praca.Service
{
    class DictionaryService : IDisposable
    {
        private bool isCommonInfo = true;
        private bool columns = false;
        private bool nextLineIsColumn = false;
        private int columnsCount = 0;
        private bool firstLineInColumn = true;
        private List<Column> listOfColumns;
        private List<Client> listOfClients;
        private Dictionary dic;
        private Evidence eud;
        private static int SIMILARITY = 70;
        public void Dispose()
        {

        }
        /// <summary>
        /// prechadza riadky rozpoznane kniznicou tesseract
        /// snazi sa z textu zistit potrebne udaje a naplnit objekty
        /// </summary>
        /// <param name="lines">zoznam riadkov</param>
        /// <param name="width">sirka dokumentu</param>
        /// <returns></returns>
        public bool MakeObjectsFromLines(List<TextLine> lines, int width)
        {
            dic = new Dictionary();
            string[] lineText;
            int similarity;
            eud = new Evidence();
            Type type = eud.GetType();
            PropertyInfo prop;
            listOfColumns = new List<Column>();
            bool found = false;
            listOfClients = new List<Client>();
            foreach (TextLine line in lines)
            {
                if (nextLineIsColumn)
                {
                    FillColumns(line);
                    continue;
                }

                GetDataFromLine(line, ref line.text, dic.header, type, eud);

                    if (nextLineIsColumn)
                    {
                        listOfColumns[listOfColumns.Count - 1].Right = width;
                    }                

            }
            return true;
        }
        /// <summary>
        /// metoda dostane text v riadku a z tohoto textu sa snazi dostat data na zaklade slovnika 'dic' v ktorom su ulozene klucove slova
        /// metoda sa vola rekurzivne na cely riadok, ak najde nejaky kluc tak sa pozrie zbytok textu ci sa tam nahoudou nenachadza iny kluc
        /// ak ano tak sa metoda opakuje
        /// ak nie tak dany text je hodnotou ku klucu z predchadzajuceho volania a ulozi sa do Property objektu
        /// ak metoda nasla aspon nieco tak vrati true inak false
        /// </summary>
        /// <param name="line">text v riadku</param>
        /// <param name="dic">slovnik nad ktorym hladam udaje</param>
        /// <param name="type">typ objektu do ktoreho idem ukladat</param>
        /// <param name="data">objekt do ktoreho ukladam</param>
        /// <returns></returns>
        private bool GetDataFromLine(TextLine line, ref string lineText, Dictionary<string, string> dictionary, Type type, Object data, bool isColumn = false, Column col = null)
        {
            int firstCharindex;
            int keyLength;
            bool keyFound = false;
            int similarity = 0;
            string stringKey = "";
            string stringKeyValue = "";
            PropertyInfo prop;
            foreach (KeyValuePair<string, string> key in dictionary)
            {
                firstCharindex = lineText.IndexOf(key.Key.Substring(0, 1)); // index prveho vyskytu prveho znaku z kluca 
                keyLength = key.Key.Length; // dlzka kluca
                if (firstCharindex >= 0) // tu idem len ak som nasiel dany znak s texte
                {
                    if ((keyLength + firstCharindex) < lineText.Length) // ak je kluc vacsi ako text nie je to on a idem prec
                    {
                        stringKey = lineText.Substring(firstCharindex, keyLength + 1); // toto by mal byt kluc z textu ktory som rozpoznal
                        similarity = SimilarityService.GetSimilarity(key.Key, stringKey);
                        if (similarity > 70)
                        {
                            if (key.Key.Equals("Odberateľ") || key.Key.Equals("Dodávateľ") || key.Key.Equals("Poštová adresa"))
                            {
                                // je to stlpec
                                nextLineIsColumn = true;
                                listOfClients.Add(new Client());
                                columnsCount++;
                                Column column = GetColumnParam(columnsCount, stringKey, line);
                                listOfColumns.Add(column);

                                if (listOfColumns.Count > 1)
                                {
                                    Column prev = listOfColumns[listOfColumns.IndexOf(column) - 1];
                                    prev.Right = column.Left;
                                }
                            }

                            keyFound = true;
                            stringKeyValue = lineText.Substring(firstCharindex + stringKey.Length + 1); // nasiel som nejaky kluc v riadku, toto by mala byt jeho hodnota ale moze obsahovat este nejaky iny kluc tak sa radsej pozriem                                                       
                            // pozriem sa ci je este nejaky kluc za nim, ak ano tak opakujem ak nie tak dany string je hodnota                        
                            if (!GetDataFromLine(line, ref stringKeyValue, dictionary, type, data))
                            {
                                prop = type.GetProperty(key.Value);
                                prop.SetValue(data, stringKeyValue, null);
                                lineText = lineText.Replace(lineText.Substring(firstCharindex), "");
                                if (lineText.Length < 5)
                                    break;
                            }
                        }


                    }
                }
            }



            if (isColumn && col != null && !keyFound)
            {
                Client client = (Client)data;
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

            if (isColumn && !keyFound)
            {
                // skus iny slovnik
                if (GetDataFromLine(line, ref stringKeyValue, dic.header, eud.GetType(), eud))
                {
                    // aktualny stlpec skoncil
                    col.Completed = true;
                    col.Bottom = line.Words[0].Bounds.Top;
                }
            }

            return keyFound;
        }


        /// <summary>
        /// vrati suvisly sled cisel zo zaciatku stringu
        /// </summary>
        /// <param name="s">vstupny text</param>
        /// <returns></returns>
        private string getFirstNNumberAsString(string s)
        {
            return s.Substring(0, s.IndexOf(' '));
        }
        /// <summary>
        /// funkcia nastavi zakladne info o stlpci
        /// </summary>
        /// <param name="id">cislo stlpca</param>
        /// <param name="text">urcuje pre koho je stlpec urceny napr. Odberatel</param>
        /// <param name="line">riadok v ktorom sa stlpec nachadza</param>
        /// <returns></returns>
        private Column GetColumnParam(int id, string text, TextLine line)
        {
            Column c = new Column();
            c.Id = id;
            foreach (Word w in line.Words)
            {
                w.Text = (w.Text.Trim(':')).Trim();
                c.Text = w.Text;
                if (w.Text.Equals(text.Trim()))
                {
                    c.Left = w.Bounds.Left;
                    c.Top = w.Bounds.Top;
                    break;
                }
            }
            return c;

        }
        /// <summary>
        /// prechadza riadky a podla pozici slov zistuje text pre jednotlive stlpce
        /// </summary>
        /// <param name="line">aktualne spracovavany riadok</param>
        private void FillColumns(TextLine line)
        {
            bool found = false;
            Client client;
            bool stillColumn = false;
            foreach (Column col in listOfColumns)
            {
                if (!col.Completed)
                {
                    found = false;
                    stillColumn = true;
                    client = listOfClients[col.Id - 1];
                    string text = GetWordsForColumn(col, line);


                    if (!string.IsNullOrWhiteSpace(text))
                    {
                    Type type = client.GetType();
                    GetDataFromLine(line, ref text, dic.clients, type, client, true, col);
                        
                    }
                }
                else
                {
                    ColumnFinished(col, line, ref found);
                }
            }

            if (!stillColumn)
                nextLineIsColumn = false;

        }


        private void ColumnFinished(Column col, TextLine line, ref bool found)
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

                            SearchInDictionary(lineText, ref index, ref found, dic.header, type, eud);
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
        /// funkcia vrati text pre dany stlpec podla slov v nom
        /// </summary>
        /// <param name="col">stlpec pre ktory hladam slova</param>
        /// <param name="line">riadok v ktorom hladam slova</param>
        /// <returns></returns>
        private string GetWordsForColumn(Column col, TextLine line)
        {
            string a = "";

            foreach (Word w in line.Words)
            {
                if (w.Bounds.Left < col.Right && w.Bounds.Left > col.Left - 20)
                    a += w.Text + " ";
            }
            return a.Trim();

        }

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

        private void SearchInDictionary(string[] lineText, ref int index, ref bool found, Dictionary<string, string> dic, Type type, Object data)
        {
            PropertyInfo prop;
            int similarity = 0;
            foreach (KeyValuePair<string, string> key in dic)
            {
                similarity = SimilarityService.GetSimilarity(key.Key, lineText[index]);
                if (similarity > SIMILARITY)
                {
                    index++;
                    lineText[index] = lineText[index].Trim();
                    prop = type.GetProperty(key.Value);
                    prop.SetValue(data, lineText[index], null);
                    found = true;
                    break;
                }
            }
        }
    }
}

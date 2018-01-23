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
        public void Dispose()
        {

        }

        public bool MakeObjectsFromLines(List<TextLine> lines, int width)
        {
            dic = new Dictionary();
            string[] lineText;
            int similarity;
            Evidence eud = new Evidence();
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
                if (line.text.Contains(":") && !columns)
                {

                    lineText = line.text.Split(':');

                    int index = 0;
                    while (index < lineText.Length)
                    {
                        found = false;
                        foreach (KeyValuePair<string, string> key1 in dic.columns)
                        {
                            similarity = SimilarityService.GetSimilarity(key1.Key, lineText[index]);
                            if (similarity > 70)
                            {
                                columns = true;
                                nextLineIsColumn = true;
                                listOfClients.Add(new Client());
                                columnsCount++;
                                Column col = GetColumnParam(columnsCount, lineText[index], line);
                                listOfColumns.Add(col);

                                if (listOfColumns.Count > 1)
                                {
                                    Column prev = listOfColumns[listOfColumns.IndexOf(col) - 1];
                                    prev.Right = col.Left;
                                }
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            index++;
                            continue;
                        }

                        foreach (KeyValuePair<string, string> key in dic.header)
                        {
                            similarity = SimilarityService.GetSimilarity(key.Key, lineText[index]);
                            if (similarity > 70)
                            {
                                index++;
                                lineText[index] = lineText[index].Trim();
                                if (key.Value == "KonstSymbol")
                                {
                                    lineText[index] = getFirstNNumberAsString(lineText[index]);
                                }
                                prop = type.GetProperty(key.Value);
                                prop.SetValue(eud, lineText[index], null);

                                found = true;
                                break;
                            }
                        }


                        index++;
                    }

                }

                if (columns)
                {
                    listOfColumns[listOfColumns.Count - 1].Right = width;
                }
            }
            return true;
        }


        private string getFirstNNumberAsString(string s)
        {
            return s.Substring(0, s.IndexOf(' '));
        }

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

        private void FillColumns(TextLine line)
        {
            bool found = false;
            Client client;
            bool stillColumn = false;
            foreach (Column col in listOfColumns)
            {
                if (!col.Completed)
                {
                    stillColumn = true;
                    client = listOfClients[col.Id - 1];
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
                            int similarity;
                            Type type = client.GetType();
                            PropertyInfo prop;
                            while (index < lineText.Length)
                            {
                                foreach (KeyValuePair<string, string> key in dic.clients)
                                {
                                    similarity = SimilarityService.GetSimilarity(key.Key, lineText[index]);
                                    if (similarity > 70)
                                    {
                                        index++;
                                        lineText[index] = lineText[index].Trim();
                                        prop = type.GetProperty(key.Value);
                                        prop.SetValue(client, lineText[index], null);
                                        found = true;
                                        break;
                                    }
                                }


                                index++;
                            }
                            if (!found)
                            {
                                col.Completed = true;
                            }
                        }
                        else
                        {

                            switch (col.FirstLineInColumn)
                            {
                                case 1:
                                    client.Name = text;
                                    col.FirstLineInColumn++;
                                    break;
                                case 2:
                                    client.Street = text;
                                    col.FirstLineInColumn++;
                                    break;
                                case 3:
                                    client.PSCCity = text;
                                    col.FirstLineInColumn++;
                                    break;
                                case 4:
                                    client.State = text;
                                    col.FirstLineInColumn++;
                                    break;

                            }

                        }

                    }
                }
            }

            if (!stillColumn)
                nextLineIsColumn = false;



        }
        private string GetWordsForColumn(Column col, TextLine line)
        {
            string a = "";

            foreach (Word w in line.Words)
            {
                if (w.Bounds.Left < col.Right && w.Bounds.Left > col.Left - 10)
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
    }
}

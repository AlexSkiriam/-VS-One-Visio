using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Visio = Microsoft.Office.Interop.Visio;

namespace _VS_One__Visio
{
    class PhrasesFunctions
    {
        public List<PhrasesArray> getAllPhrases(Visio.Shapes shapes, bool validationNeeded = true, bool onlyElaboratedPhrases = false)
        {
            if (validationNeeded) return withValidate(shapes, onlyElaboratedPhrases);
            else return withoutValidate(shapes, onlyElaboratedPhrases);
        }

        public List<PhrasesArray> withValidate(Visio.Shapes shapes, bool onlyElaboratedPhrases)
        {
            List<PhrasesArray> outputPhrase = new List<PhrasesArray>();

            Dictionary<string, Visio.Shape> dict = new Dictionary<string, Visio.Shape>();
            Dictionary<string, Visio.Shape> newDict = new Dictionary<string, Visio.Shape>();

            List<string> textInShapes = new List<string>();
            List<string> newTextInShapes = new List<string>();

            foreach (Visio.Shape shp in shapes)
            {
                if (shp.NameU.IndexOf("Process") > -1 && (onlyElaboratedPhrases ? shp.CellsU["FillForegnd"].Formula == "THEMEGUARD(RGB(146;208;80))" : true))
                {
                    string text = getOnlyStringNumber(shp.Text);
                    if (!dict.ContainsKey(text) && text != "А")
                    {
                        addInListWithNoDuplicates(textInShapes, text);
                        dict.Add(text, shp);
                    }
                }
            }

            textInShapes.Sort();

            foreach (string str in textInShapes)
            {
                Visio.Shape shape = dict[str];

                string phrasesText = getOnlyPhraseText(shape.Text);
                if (!newDict.ContainsKey(phrasesText))
                {
                    addInListWithNoDuplicates(newTextInShapes, phrasesText);
                    newDict.Add(phrasesText, shape);
                }
            }

            int i = 0;
            foreach (string str in newTextInShapes)
            {
                Visio.Shape shape = newDict[str];

                PhrasesArray phrasesArray = new PhrasesArray();

                string phrasesText = getOnlyPhraseText(shape.Text);
                if (!String.IsNullOrEmpty(phrasesText))
                {
                    checkingForAnAdditionalsPharases(i, phrasesText, shapes, shape, outputPhrase);

                    i++;
                }
            }

            return outputPhrase;
        }

        public List<string> variableText(Visio.Shapes shapes)
        {
            List<string> newList = new List<string>();

            foreach (Visio.Shape shp in shapes)
            {
                foreach (Match match in Regex.Matches(shp.Text, @"\[[^\]]+\]"))
                {
                    if(!newList.Contains(match.Value)) newList.Add(match.Value);
                }
            }

            return newList;
        }

        public List<PhrasesArray> withoutValidate(Visio.Shapes shapes, bool onlyElaboratedPhrases)
        {
            List<PhrasesArray> outputPhrase = new List<PhrasesArray>();

            Dictionary<string, Visio.Shape> dict = new Dictionary<string, Visio.Shape>();

            List<string> textInShapes = new List<string>();

            foreach (Visio.Shape shp in shapes)
            {
                if (shp.NameU.IndexOf("Process") > -1 && (onlyElaboratedPhrases ? shp.CellsU["FillForegnd"].Formula == "THEMEGUARD(RGB(146;208;80))" : true))
                {
                    if (!dict.ContainsKey(shp.Text))
                    {
                        addInListWithNoDuplicates(textInShapes, shp.Text);
                        dict.Add(shp.Text, shp);
                    }
                }
            }

            textInShapes.Sort();

            int i = 0;
            foreach (string str in textInShapes)
            {
                Visio.Shape shape = dict[str];

                PhrasesArray phrasesArray = new PhrasesArray();

                string phrasesText = getOnlyPhraseText(shape.Text);
                if (!String.IsNullOrEmpty(phrasesText))
                {
                    checkingForAnAdditionalsPharases(i, phrasesText, shapes, shape, outputPhrase);

                    i++;
                }
            }

            return outputPhrase;
        }

        public List<string> getPhrases(Visio.Shapes shapes)
        {
            List<string> textInShapes = new List<string>();

            foreach (Visio.Shape shp in shapes)
            {
                if (shp.NameU.IndexOf("Process") > -1)
                {
                    string text = getInitialText(shp.Text);
                    addInListWithNoDuplicates(textInShapes, text);
                }
            }

            textInShapes.Sort();
            textInShapes = newList(textInShapes);
            return textInShapes;
        }

        public void addInListWithNoDuplicates(List<string> sourceList, string sourceString)
        {
            if (!sourceList.Contains(sourceString)) sourceList.Add(sourceString);
        }

        //Функция преобразует изначальную строку в элемент listView (с порядковым номером)
        public string getInitialText(string sourceString)
        {
            string returnString = "";
            string newString = removeSurplusEnters(sourceString);

            string[] lines = Regex.Split(newString, @"\n");
            foreach (string line in lines)
            {
                string stringEnd = (Array.IndexOf(lines, line) == lines.Length - 1) ? "" : "\n";
                
                string newLine = cleanSurplusSpaces(line);
                if (Array.IndexOf(lines, line) == 0 && checkNumberContains(line)) newLine = blockNumberParser(line) + " ";
                returnString += newLine + stringEnd;
            }
            return returnString;
        }

        public string getOnlyStringNumber(string sourceString)
        {
            string newString = removeSurplusEnters(sourceString);

            string[] lines = Regex.Split(newString, @"\n");

            return (checkNumberContains(lines[0])) ? blockNumberParser(lines[0]) : "А";
        }

        public string getOnlyPhraseText(string sourceString)
        {
            string returnString = "";
            string newString = removeSurplusEnters(sourceString);

            string[] lines = Regex.Split(newString, @"\n");

            foreach (string line in lines)
            {
                string stringEnd = (Array.IndexOf(lines, line) == lines.Length - 1) ? "" : "\n";
                if (Array.IndexOf(lines, line) > 0) returnString += line + stringEnd;
            }
            return returnString;
        }

        //Функция формирует номер строки
        public string blockNumberParser(string sourceString)
        {
            string additional = "";
            string number = "";

            Match match = Regex.Match(sourceString, @"(\w*\.\s*)?(\w+)(\s*№\s*)((\d+\.?)+)");
            if(match.Success)
            {
                additional = match.Groups[1].Value;
                number = match.Groups[4].Value;
            }
            return additional + number;
        }

        public List<string> newList(List<string> sourceList)
        {
            List<string> returnList = new List<string>();

            foreach (string str in sourceList)
            {
                string newStr = Regex.Replace(str, @"^[^((\d+.?)+)]*((\d+.?)+)\n", "");
                string[] phrsArr = Regex.Split(str, @"Фраза для повтора[^:]*:");
                foreach (string line in phrsArr)
                {
                    returnList.Add(Regex.Replace(line, @"(Основная фраза|Фраза для повтора)[^:]*:", ""));
                }
            }
            return returnList;
        }

        //Функция проверяет наличие номера блока в передаваемой строке
        public bool checkNumberContains(string sourceString)
        {
            Match match = Regex.Match(sourceString, @"(\w*\.\s*)?(\w+)(\s*№\s*)((\d+\.?)+)");
            return match.Success;
        }

        //Функция удаляет лишние переносы строки (начало и конец строки)
        public string removeSurplusEnters(string sourceString)
        {
            string returnString = Regex.Replace(sourceString, @"(^\n+|\n+$)", "");
            return returnString;
        }

        //Функция удаляет лишние пробелы в строке (начало, середина и конец строки)
        public string cleanSurplusSpaces(string sourceString)
        {
            string returnString = Regex.Replace(sourceString, @"(^\s+|\s+$)", "");
            returnString = Regex.Replace(returnString, @"(\s+)", " ");
            return returnString;
        }

        //Функция получает интенты
        public string getConnecterText(Visio.Shape sourceShp, Visio.Shapes shapes, string additionalText)
        {
            var incomingConnect = Visio.VisConnectedShapesFlags.visConnectedShapesIncomingNodes;
            var incomingGlued = Visio.VisGluedShapesFlags.visGluedShapesIncoming1D;

            var stringToReturn = String.Format("[\"{0}\"]", additionalText);

            var shpIds = sourceShp.ConnectedShapes(incomingConnect, "");
            if (shpIds.GetUpperBound(0) == 0)
            {
                var element = Convert.ToInt32(shpIds.GetValue(0));
                Visio.Shape newShape = shapes.ItemFromID[element];
                if (newShape.NameU.IndexOf("Subprocess") > -1)
                {
                    var newShpIds = sourceShp.GluedShapes(incomingGlued, "", newShape);
                    if (newShpIds.GetUpperBound(0) == 0)
                    {
                        var subElement = Convert.ToInt32(newShpIds.GetValue(0));

                        var connector = shapes.ItemFromID[subElement];
                        var oneIntent = getOneIntent(connector.Text);
                        var withOutSurplusSpaces = cleanSurplusSpaces(oneIntent);

                        stringToReturn = String.Format("[{0}\"{1}\"]", additionalText, withOutSurplusSpaces);
                    }
                    else
                    {
                        stringToReturn = String.Format("[\"{0}\"]", additionalText);
                    }
                }
                else
                {
                    stringToReturn = getConnecterText(newShape, shapes, additionalText);
                }
            }
            else
            {
                stringToReturn = "[\"\"]";
            }
            return stringToReturn;
        }

        //Функция получает только один интент из всех полученных (переданных)
        public string getOneIntent(string sourceString)
        {
            string strToReturn = "";
            string[] strArr = splitByUpperCase(sourceString);

            if (strArr.Length > 1) strToReturn = strArr[0]; else strToReturn = sourceString;

            return strToReturn;
        }

        //Функция получает разбивает строку интентов по enter`у
        public string[] splitByUpperCase(string sourceString)
        {
            return Regex.Split(sourceString, @"(\\|\n)");
        }

        public void checkingForAnAdditionalsPharases(int number, string sourceStr, Visio.Shapes shapes, Visio.Shape shp, List<PhrasesArray> list)
        {
            string[] phrsArr = Regex.Split(sourceStr, @"Фраза для повтора[^:]*:");

            for (int i = 0; i < phrsArr.Length; i++)
            {
                var itemText = phrsArr[i];

                itemText = Regex.Replace(itemText, @"(Основная фраза|Фраза для повтора)[^:]*:", "");

                var additionalText = "";

                if (i == 1)
                {
                    additionalText = "Снова ";
                }
                else if (i > 1)
                {
                    additionalText = String.Format("Снова_{0} ", i);
                }

                list.Add(new PhrasesArray
                {
                    phraseNumber = number,
                    phraseIntent = getConnecterText(shp, shapes, additionalText),
                    phrase = cleanSurplusSpaces(itemText),
                    nameSpace = getNameSpace(shp, (i < 1)),
                    phraseShape = shp
                });
            }
        }

        public List<string> additionalPharsesList(string sourceString)
        {
            List<string> returnList = new List<string>();
            string pattern = @"((?<=Основная фраза[^:]*:\s*).*(?=Фраза для повтора)|(?<=Фраза для повтора[^:]*:\s*).*(?=Фраза для повтора[^\d]*\d*[^:]*:\s*)|(?<=Фраза для повтора[^\d]*\d*[^:]*:\s*).*(?=$))";
            MatchCollection matchCollection = Regex.Matches(sourceString.Replace(Environment.NewLine, " "), pattern);
            foreach (Match match in matchCollection) returnList.Add(match.Value);
            return returnList;
        }

        public string getNameSpace(Visio.Shape shape, bool mainPhrase)
        {
            return (shape.Comments.Count == 1) ? nameSpaceParser(shape.Comments[1].Text, mainPhrase) : "";
        }

        public string nameSpaceParser(string commentText, bool mainPharse)
        {
            string returnString = "";

            if (mainPharse)
            {
                Match match = Regex.Match(commentText, @"^\s*\w+(\([^\)]*\));");
                returnString = (match.Success) ? match.Value : "";
            }
            else
            {
                Match match = Regex.Match(commentText, @"\s*\w+(\([^\)]*\));$");
                returnString = (match.Success) ? match.Value : "";
            }
            return returnString;
        }

        //СТАРЫЙ КОД!!!
        public string getIntentsFromShapes(Visio.Shape sourceShp, Visio.Shapes shapes)
        {
            var incomingConnect = Visio.VisConnectedShapesFlags.visConnectedShapesIncomingNodes;
            var incomingGlued = Visio.VisGluedShapesFlags.visGluedShapesIncoming1D;

            var stringToReturn = "";

            var shpIds = sourceShp.ConnectedShapes(incomingConnect, "");
            if (shpIds.GetUpperBound(0) == 0)
            {
                var element = Convert.ToInt32(shpIds.GetValue(0));
                Visio.Shape newShape = shapes.ItemFromID[element];
                if (newShape.NameU.IndexOf("Subprocess") > -1)
                {
                    var newShpIds = sourceShp.GluedShapes(incomingGlued, "", newShape);
                    if (newShpIds.GetUpperBound(0) == 0)
                    {
                        var subElement = Convert.ToInt32(newShpIds.GetValue(0));

                        var connector = shapes.ItemFromID[subElement];

                        stringToReturn = connector.Text;
                    }
                    else
                    {
                        stringToReturn = "";
                    }
                }
                else
                {
                    stringToReturn = getIntentsFromShapes(newShape, shapes);
                }
            }
            else
            {
                stringToReturn = "";
            }
            return stringToReturn;
        }

        public List<String> removeDuplicates(List<string> sourceArray)
        {
            List<String> returnArray = new List<string>();

            foreach (string s in sourceArray)
            {
                if (!returnArray.Contains(s))
                {
                    returnArray.Add(s);
                }
            }

            return returnArray;
        }
        //СТАРЫЙ КОД!!!
    }

    //Класс для хранения информации о фразах
    class PhrasesArray
    {
        public int phraseNumber { get; set; }
        public string phraseIntent { get; set; }
        public string phrase { get; set; }
        public string nameSpace { get; set; }
        public Visio.Shape phraseShape { get; set; }
    }
}

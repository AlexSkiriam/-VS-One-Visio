using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Visio = Microsoft.Office.Interop.Visio;
using Excel = Microsoft.Office.Interop.Excel;

namespace _VS_One__Visio
{
    class TextAnalysis
    {
        PhrasesFunctions functions = new PhrasesFunctions();

        public int phrasesCount(Visio.Shapes allShapes) 
        {
            int counter = 0;
            foreach (Visio.Shape shape in allShapes)
            {
                if (shape.NameU.IndexOf("Process") > -1) counter++;
            }
            return counter;
        }

        public List<Visio.Shape> getShapesWithoutNumber(Visio.Shapes allShapes)
        {
            List<Visio.Shape> shapes = new List<Visio.Shape>();

            foreach (Visio.Shape shape in allShapes)
            {
                if (shape.NameU.IndexOf("Process") > -1)
                {
                    if (functions.getOnlyStringNumber(shape.Text) == "А") shapes.Add(shape);
                }
            }
            return shapes;
        }

        public string getErrorText(string sourceText)
        {
            string error = "";

            //Регулярка: @"(\w*\.\s*)?(\w+)(\s*№\s*)((\d+\.?)+)" 
            //Условия:
            Match matchOnlyText = Regex.Match(sourceText, @"^(\s*[^\d]+\s*){2,5}");
            Match matchOnlyNumbers = Regex.Match(sourceText, @"(^\s*№\s*((\d+\.?)+)|^\s*((\d+\.?)+))");
            Match matchNumbersNotNumSymbol = Regex.Match(sourceText, @"^\w+\s*(\w+\s*)?((\d+\.?)+)");

            if (matchOnlyNumbers.Success) error = "В блоке отсутствует конструкция \"Блок №\" перед номером блока";
            else if (matchNumbersNotNumSymbol.Success) error = "В блоке отсутствует \"№\" после \"Блок\"";
            else if (matchOnlyText.Success) error = "В блоке отсутствует номер блока";

            return error;
        }

        public bool containsBlock(string sourceString)
        {
            Match match = Regex.Match(sourceString, @"^(Блок)\s*(\w+){2,5}");
            return match.Success;
        }

        public string withoutSurplusSymbols(string sourceString)
        {
            string returnString = "";

            returnString = Regex.Replace(sourceString, @"\([^ \)]+\)", "");
            returnString = Regex.Replace(returnString, @"[^ \w \d]", @"\s");
            returnString = Regex.Replace(returnString, @"\s+", @"\s");

            return returnString;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace _VS_One__Visio
{
    class OwnSorter
    {
        Dictionary<string, string> sourceNum = new Dictionary<string, string>();
        Dictionary<string, string> sourceBlock = new Dictionary<string, string>();

        public List<string> sort(List<string> sourceList)
        {
            List<string> num = new List<string>();
            List<string> block = new List<string>();

            int maxPointNumber = 0;

            foreach (string str in sourceList)
            {
                if (Regex.IsMatch(str, @"[А-я]")) block.Add(str);
                else
                {
                    if (Regex.Matches(str, @"\.").Count > maxPointNumber) maxPointNumber = Regex.Matches(str, @"\.").Count;
                    num.Add(str);
                }
            }

            List<string> returnList = new List<string>();

            returnList.AddRange(onlyNumbers(maxPointNumber, num));
            returnList.AddRange(numbersAndBlocks(maxPointNumber, block));

            return returnList;
        }

        public int getMaxPointNumber(List<string> sourceList, bool is_only_number = false)
        {
            int maxPointNumber = 0;

            foreach (string str in sourceList)
            {
                if (Regex.Matches(str, @"\.").Count > maxPointNumber) maxPointNumber = Regex.Matches(str, @"\.").Count - ((is_only_number) ? 0 : 1);
            }

            return maxPointNumber;
        }

        public List<string> numbersAndBlocks(int number, List<string> list)
        {
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                string block = Regex.Match(s, @"^\w+\.").Value;
                string numberS = s.Replace(block, "").Replace(" ", "");
                newList.Add($"{block} {numberBuilder(number, numberS)}");
                sourceBlock.Add($"{block} {numberBuilder(number, numberS)}", s);
            }

            List<string> reList = sortByNumbersAndBlocks(number, newList);
            return replaceList(reList, sourceBlock);
        }

        public List<string> onlyNumbers(int number, List<string> list)
        {
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                newList.Add(numberBuilder(number, s));
                sourceNum.Add(numberBuilder(number, s), s);
            }
            List<string> reList = sortByNumbers(number, newList);
            return replaceList(reList, sourceNum);

        }

        private List<string> replaceList(List<string> source, Dictionary<string, string> dict)
        {
            List<string> returnList = new List<string>();
            foreach (string str in source)
            {
                returnList.Add(dict[str]);
            }
            return returnList;
        }

        private List<string> sortByNumbers(int number, List<string> list)
        {
            var sorted = list.OrderBy(x => int.Parse(x.Split('.')[0]));
            for (int i = 0; i < number; i++)
            {
                i++;
                sorted.ThenBy(x => int.Parse(x.Split('.')[i]));
            }
            return sorted.ToList();
        }

        private List<string> sortByNumbersAndBlocks(int number, List<string> list)
        {
            var sorted = list.OrderBy(x => x.Split('.')[0]);
            sorted = list.OrderBy(x => int.Parse(x.Split('.')[1]));
            for (int i = 1; i < number; i++)
            {
                i++;
                sorted.ThenBy(x => int.Parse(x.Split('.')[i]));
            }
            return sorted.ToList();
        }

        private string numberBuilder(int number, string str)
        {
            int points = Regex.Matches(str, @"\.").Count;
            string t = str;
            while (points < number)
            {
                t += ".0";
                points = Regex.Matches(t, @"\.").Count;
            }
            return t;
        }
    }
}

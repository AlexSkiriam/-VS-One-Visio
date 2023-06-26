using System;
using System.Collections.Generic;
using System.Linq;

namespace _VS_One__Visio
{
    class LevinsteinDistanceClass
    {
        public int findWithLevinstainInTextMeta(string source, Dictionary<string, int> textMeta)
        {
            Dictionary<int, int> postions = new Dictionary<int, int>();
            foreach (KeyValuePair<string, int> keyValue in textMeta)
            {
                if (!postions.ContainsKey(keyValue.Value)) postions.Add(keyValue.Value, LevenshteinDistance(source, keyValue.Key));
            }
            var keyOfMinValue = postions.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
            return keyOfMinValue;
        }

        public string findWithLevinstainInList(string sourceElement, List<string> sourceList, int MaxDifference = 10)
        {
            string returnString = $"Похожие правила для {sourceElement}:\n";
            foreach (string str in sourceList)
            {
                if (LevenshteinDistance(sourceElement, str) <= MaxDifference)
                {
                    returnString += str + " ";
                }
            }
            return (returnString == $"Похожие правила для {sourceElement}:\n") ? "" : returnString + "\n";
        }

        public int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                if (string.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (string.IsNullOrEmpty(target)) return source.Length;

            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];
            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                                distance[previousRow, j] + 1,
                                distance[currentRow, j - 1] + 1),
                                distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }
    }
}

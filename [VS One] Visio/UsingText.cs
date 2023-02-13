using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace _VS_One__Visio
{
    class UsingText
    {
		public string defaultStateText(string prefix, string stateName, string nextStateName, string resultStateName, string mainText, string additionalText, string fisrtTab, string tabs)
        {
			return fisrtTab + "{" +
			"\n" + tabs + "\t\"name\": \"" + prefix + stateName + "_conditions\"," +
			"\n" + tabs + "\t\"conditions\": [" +
			"\n" + tabs + "\t\t{" +
			"\n" + tabs + "\t\t\t\"dialog.visited_state_times\": { \"" + prefix + stateName + "_conditions\": 3 }," +
			"\n" + tabs + "\t\t\t\"next_state\": \"" + resultStateName + "\"" +
			"\n" + tabs + "\t\t}," +
			"\n" + tabs + "\t\t{" +
			"\n" + tabs + "\t\t\t\"dialog.visited_state_times\": { \"" + prefix + stateName + "_conditions\": 2 }," +
			"\n" + tabs + "\t\t\t\"next_state\": {" +
			"\n" + tabs + "\t\t\t\t\"name\": \"" + prefix + stateName + "_speech_2\"," +
			"\n" + tabs + "\t\t\t\t\"speech\": [" +
			"\n" + splittedText(additionalText, tabs + "\t\t\t\t\t") +
			"\n" + tabs + "\t\t\t\t]," +
			"\n" + tabs + "\t\t\t\t\"next_state\": \"" + nextStateName + "\"" +
			"\n" + tabs + "\t\t\t}," +
			"\n" + tabs + "\t\t}," +
			"\n" + tabs + "\t\t{" +
			"\n" + tabs + "\t\t\t\"always_true\": {}," +
			"\n" + tabs + "\t\t\t\"next_state\": {" +
			"\n" + tabs + "\t\t\t\t\"name\": \"" + prefix + stateName + "_speech\"," +
			"\n" + tabs + "\t\t\t\t\"speech\": [" +
			"\n" + splittedText(mainText, tabs + "\t\t\t\t\t") +
			"\n" + tabs + "\t\t\t\t]," +
			"\n" + tabs + "\t\t\t\t\"next_state\": \"" + nextStateName + "\"" +
			"\n" + tabs + "\t\t\t}," +
			"\n" + tabs + "\t\t}," +
			"\n" + tabs + "\t]" +
			"\n" + tabs + "},";
		}

		public string vsIceStateText(string prefix, string stateName, string nextStateName, string resultStateName, string mainText, string additionalText, string fisrtTab, string tabs)
		{
			return fisrtTab + "{" +
			"\n" + tabs + "\t\"name\": \"" + prefix + stateName + "_conditions\"," +
			"\n" + tabs + "\t\"conditions\": [" +
			"\n" + tabs + "\t\t{" +
			"\n" + tabs + "\t\t\t\"dialog.visited_state_times\": {" +
			"\n" + tabs + "\t\t\t\t\"" + prefix + stateName + "_conditions\": {" +
			"\n" + tabs + "\t\t\t\t\t\"eq\": 3" +
			"\n" + tabs + "\t\t\t\t}" +
			"\n" + tabs + "\t\t\t}," +
			"\n" + tabs + "\t\t\t\"display_name\": \"Неконструктивный диалог\"," +
			"\n" + tabs + "\t\t\t\"next_state\": \"" + resultStateName + "\"" +
			"\n" + tabs + "\t\t}," +
			"\n" + tabs + "\t\t{" +
			"\n" + tabs + "\t\t\t\"dialog.visited_state_times\": {" +
			"\n" + tabs + "\t\t\t\t\"" + prefix + stateName + "_conditions\": {" +
			"\n" + tabs + "\t\t\t\t\t\"eq\": 2" +
			"\n" + tabs + "\t\t\t\t}" +
			"\n" + tabs + "\t\t\t}," +
			"\n" + tabs + "\t\t\t\"display_name\": \"Фраза для повтора\"," +
			"\n" + tabs + "\t\t\t\"next_state\": \"" + prefix + stateName + "_speech_2\"" +
			"\n" + tabs + "\t\t}," +
			"\n" + tabs + "\t\t{" +
			"\n" + tabs + "\t\t\t\"always_true\": { }," +
			"\n" + tabs + "\t\t\t\"display_name\": \"Основная фраза\"," +
			"\n" + tabs + "\t\t\t\"next_state\": \"" + prefix + stateName + "_speech\"" +
			"\n" + tabs + "\t\t}," +
			"\n" + tabs + "\t]," +
			"\n" + tabs + "\t\"display_name\": \"Ответ на возражение\"" +
			"\n" + tabs + "}," +
			"\n" + tabs + "{" +
			"\n" + tabs + "\t\"name\": \"" + prefix + stateName + "_speech\"," +
			"\n" + tabs + "\t\"speech\": [" +
			"\n" + splittedText(mainText, tabs + "\t\t") +
			"\n" + tabs + "\t]," +
			"\n" + tabs + "\t\"next_state\": \"" + nextStateName + "\"" +
			"\n" + tabs + "}," +
			"\n" + tabs + "{" +
			"\n" + tabs + "\t\"name\": \"" + prefix + stateName + "_speech_2\"," +
			"\n" + tabs + "\t\"speech\": [" +
			"\n" + splittedText(additionalText, tabs + "\t\t") +
			"\n" + tabs + "\t]," +
			"\n" + tabs + "\t\"next_state\": \"" + nextStateName + "\"" +
			"\n" + tabs + "},";
		}

		public string resultStateText(string stateName, string displayName, string mainText, string fisrtTab, string tabs)
		{
			return fisrtTab + "{" +
			"\n" + tabs +"\t\"name\": \"" + stateName + "_result\"," +
			"\n" + tabs + "\t\"display_name\": \"" + displayName + "\"," +
			"\n" + tabs + "\t\"type_result\": \"" + stateName + "\"," +
			"\n" + tabs + "\t\"next_state\": \"" + stateName + "_speech\"" +
			"\n" + tabs + "}," +
			"\n" + tabs + "{" +
			"\n" + tabs + "\t\"name\": \"" + stateName + "_speech\"," +
			"\n" + tabs + "\t\"speech\": [" +
			"\n" + tabs + "\t\t{" +
			"\n" + splittedText(mainText, tabs + "\t\t\t") +
			"\n" + tabs + "\t\t}," +
			"\n" + tabs + "\t]," +
			"\n" + tabs + "\t\"next_state\": \"end\"" +
			"\n" + tabs + "},";
		}

		public string tabsToReturn(int i, bool main = true)
		{
			string returnString = "";

			for (int j = 0; j < (main ? i : i++); j++)
			{
				returnString += "\t";
			}

			return returnString;
		}

		public string splittedText(string sourceString, string tabs)
        {
			string returnString = "";

			string[] strings = Regex.Split(sourceString, @"(\s\([^)]+\)\s|\[[^]]+\][\. \, \s \; \:]*)");

			foreach (string str in strings)
			{
				Match matchDeleted = Regex.Match(str, @"^\([^)]+\)$");

				if (!matchDeleted.Success)
				{
					string endOfString = (Array.IndexOf(strings, str) == strings.Length - 1) ? "" : "\n";

					Match matchComments = Regex.Match(str, @".\([^)]+\)[\. \, \s \; \:]");
					Match matchTts = Regex.Match(str, @"\[[^]]+\]");

					if (matchComments.Success)
					{
						returnString += tabs + "{\n"
							+ tabs + "\t\"text_male\"  : \"" + Regex.Replace(str, @"(^\s+|\s+$)", "") + "\",\n"
							+ tabs + "\t\"text_female\": \"" + Regex.Replace(str, @"(^\s+|\s+$)", "") + "\"\n"
							+ tabs + "}," + endOfString;
					}
					else if (matchTts.Success)
					{
						returnString += tabs + "/*{\n"
							+ tabs + "\t\"runtime\": \"" + Regex.Replace(str, @"(^\s+|\s+$)", "") + "\",\n"
							+ tabs + "},*/" + endOfString;
					}
					else
					{
						returnString += tabs + "{\n"
							+ tabs + "\t\"text\": \"" + Regex.Replace(str, @"(^\s+|\s+$)", "") + "\",\n"
							+ tabs + "}," + endOfString;
					}
				}
			}
			return returnString;
		}
	}
}

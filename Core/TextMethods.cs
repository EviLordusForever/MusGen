using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Core
{
	class TextMethods
	{
		public static string RemoveTags(string str)
		{
			int startIndex = 0;
			int endIndex = 0;
			string[] splited = str.Split('>');
			string res = "";
			for (int i = 0; i < splited.Length; i++)
			{
				if (splited[i].Contains("<"))
					splited[i] = splited[i].Remove(splited[i].IndexOf('<'));
				res += splited[i] + " ";
			}
			return res.Remove(res.Length - 1);
		}

		public static string[] StringSuperSplit(string str, string after, string before)
		{
			//blablabla(this)blabla(this)bla(this)blablabla
			string res = "";

			for (; str.Contains(after);)
			{
				str = StringAfter(str, after);
				if (!str.Contains(before)) break;
				res += StringBefore(str, before);
				if (str.Contains(after)) res += "~";
			}

			return res.Split('~');
		}

		public static string BeforeClosing(string str, char opener, char closer)
		{
			int level = 1;
			int i = 0;
			for (; i < str.Length && level > 0; i++)
			{
				if (str[i] == opener) level++;
				if (str[i] == closer) level--;
			}
			return str.Remove(i - 1);
		}

		public static string StringInsert(string str, string after, string what)
		{
			string strAfter = StringAfter(str, after);
			return StringBefore(str, strAfter) + what + strAfter;
		}

		public static string StringAfter(string str, string cutter)
		{
			return str.Substring(str.IndexOf(cutter) + cutter.Length);
		}

		public static string StringAfterLast(string str, string cutter)
		{
			return str.Substring(str.LastIndexOf(cutter) + cutter.Length);
		}

		public static string StringBefore(string str, string cutter)
		{
			return str.Remove(str.IndexOf(cutter));
		}

		public static string StringBeforeLast(string str, string cutter)
		{
			return str.Remove(str.LastIndexOf(cutter));
		}

		public static string StringInsideFirst(string str, string after, string before)
		{
			str = StringAfter(str, after);
			return StringBefore(str, before);
		}

		public static string StringInsideWhole(string str, string after, string before)
		{
			str = StringAfter(str, after);
			return StringBeforeLast(str, before);
		}

		public static string StringInsideLast(string str, string after, string before)
		{
			str = StringAfterLast(str, after);
			return StringBefore(str, before);
		}

		public static bool IsLetter(char symbol)
		{
			if (char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.UppercaseLetter || char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.LowercaseLetter)
				return true;
			else
				return false;
		}

		public static bool IsLowcaseLetter(char symbol)
		{
			if (char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.LowercaseLetter)
				return true;
			else
				return false;
		}

		public static bool IsNumber(char symbol)
		{
			if (symbol == '0' || symbol == '1' || symbol == '2' || symbol == '3' || symbol == '4' || symbol == '5' || symbol == '6' || symbol == '7' || symbol == '8' || symbol == '9')
				return true;
			else
				return false;
		}

		public static bool CanBeInLink(char symbol)
		{
			if (symbol == '#' || IsNumber(symbol) || char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.DashPunctuation || char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.UppercaseLetter || char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.LowercaseLetter)
				return true;
			else
				return false;
		}

		public static string ModifyText(string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				if (char.GetUnicodeCategory(text[i]) == System.Globalization.UnicodeCategory.UppercaseLetter && char.GetUnicodeCategory(text[i + 1]) == System.Globalization.UnicodeCategory.UppercaseLetter)
					return "КАПС";
				if (text[i] == '\n' && text[i + 1] == '\n' && text[i + 2] == '\n')
				{
					return "ENTERS";
				}
				if (text[i] == '\n' && text[i + 1] == '\n')
				{
					text = text.Remove(i, 1);
				}
				if (text[i] == '\n')
					text = text.Remove(i + 1) + "     " + text.Substring(i + 1);
				if (text[i] == ' ' && text[i + 1] == ' ')
					text = text.Remove(i, 1);
			}
			return text;
		} //Удаляет двойные пробелы, отсеивает тексты с капсом и сбольшим количеством ентеров, добавляет отступ после ентера

		public static string RemoveNotLettersAndNotSpaces(string str)
		{
			for (int i = str.Length - 1; i >= 0; i--)
			{
				if (!(IsLowcaseLetter(str[i]) || str[i] == ' '))
					str = str.Remove(i, 1);
			}
			return str;
		}

		public static string RemoveSymbols(string str, char symbol)
		{
			string[] strParts = str.Split(symbol);
			int len = str.Length;
			str = string.Concat(strParts);
			/*
            int i = 0;
            foreach (string strPart in strParts)
            {
                i++;
                str += strPart;

                if (i % 20000 == 0)
                    View.ShoutConsole("TextMethods", "Removing Symbols", "TM.RS", $"Удаление символов. Символы удалены. Соединение частей строки: {str.Length} из {len}");
            }
            */

			return str;
		}

		public static string RemoveDoubles(string text, char symbol)
		{
			for (int i = 0; i < text.Length - 1; i++)
			{
				if (text[i] == symbol && text[i + 1] == symbol)
				{
					text = text.Remove(i, 1);
					i--;
				}
			}
			return text;
		}

		public static int SubstringsCount(string str, string str0)
		{
			int count = (str.Length - str.Replace(str0, "").Length) / str0.Length;
			return count;
		}

		public static int CharsCount(string str, char c)
		{
			int count = 0;

			foreach (char s in str)
				if (c == s)
					count++;

			return count;
		}

		public static List<string> StringSplitLast(string str, char c, int count)
		{
			List<string> res = new List<string>(0);

			for (int i = 0; i < count; i++)
			{
				if (!str.Contains(c))
				{
					res.Add(str);
					break;
				}

				res.Add(StringAfterLast(str, c.ToString()));
				str = StringBeforeLast(str, c.ToString());
			}

			return res;
		}

		public static string ModifyStringAsList(string str)
		{
			//Disk.ErrorLog(str);
			//Удаляет двойные энтеры, двойные пробелы, табы, пустые строки, пробелы в начале и конце строк, ненужные энтеры в начале и конце строки
			for (int i = str.Length - 1; i > 0; i--)
			{
				if (str[i] == ' ' && str[i - 1] == ' ')
					str = str.Remove(i, 1);
				else if (str[i] == '\t')
					str = str.Remove(i, 1);
				else if ((str[i] == '\n' || str[i] == '\r') && str[i - 1] == ' ')
					str = str.Remove(i - 1, 1);
				else if (str[i] == ' ' && (str[i - 1] == '\n' || str[i - 1] == '\r'))
					str = str.Remove(i, 1);
				else if ((str[i] == '\n' || str[i] == '\r') && (str[i - 1] == '\n' || str[i - 1] == '\r'))
					str = str.Remove(i, 1);
			}

			if (str.Length == 0)
				return str;

			if (str[str.Length - 1] == '\n' || str[str.Length - 1] == '\r')
				str = str.Remove(str.Length - 1);

			if (str[0] == '\n' || str[0] == '\r')
				str = str.Remove(0, 1);

			for (int i = str.Length - 1; i > 0; i--)
				if (str[i] == '\r' || str[i] == '\n')
					str = str.Remove(i) + "\r\n" + str.Substring(i + 1);

			return str;
		}

		public static string ToPositiveIntegerNumberOrZero(string str)
		{
			for (int i = str.Length - 1; i >= 0; i--)
				if (!IsNumber(str[i]))
					str = str.Remove(i, 1);

			while (str.Length > 1 && str[0] == '0')
				str = str.Substring(1);

			return str;
		}

		public static string ToIntegerNumber(string str)
		{
			if (str.Length > 0)
			{
				if (!IsNumber(str[0]) && str[0] != '-')
					str = str.Remove(0, 1);

				for (int i = str.Length - 1; i >= 1; i--)
					if (i > 0 && !IsNumber(str[i]))
						str = str.Remove(i, 1);

				while (str.Length > 1 && str[0] == '0')
					str = str.Substring(1);
			}

			return str;
		}

		public static string[] ProSplit(string str, string separator)
		{
			//Это как обычный Split, но работает с разделением строками
			str = str.Replace(separator, "~");

			return str.Split('~');
		}

		public static string[] OmegaSplit(string str, params char[] cs)
		{
			//Это как обычный Split, но при этом он не удаляет разделяющие символы
			for (int i = 0; i < str.Length; i++)
			{
				foreach (char c in cs)
				{
					if (str[i] == c)
					{
						str = str.Insert(i, "~");
						i++;
						break;
					}
				}
			}

			return str.Split('~');
		}

		public static string RemoveOneNotNumber(string str)
		{
			for (int i = 0; i < str.Length; i++)
				if (!IsNumber(str[i]))
					return str.Remove(i, 1);

			return str;
		}
	}
}

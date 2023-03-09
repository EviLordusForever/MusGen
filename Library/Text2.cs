namespace Library
{
	public static class Text2
	{
		public static string[] StringSuperSplit(string str, string after, string before)
		{
			//blablabla(this)blabla(this)bla(this)blablabla
			string res = "";

			for (; str.Contains(after);)
			{
				str = StringAfter(str, after);
				if (!str.Contains(before))
					break;
				res += StringBefore(str, before);
				if (str.Contains(after))
					res += "~";
			}

			return res.Split('~');
		}

		public static string[] StringUltraSuperSplit(string str, string opener, string closer, int maximumSymbols)
		{
			//ffffffff {tttt{ttt{tt}tt{tt}tt}t} ffff {tt} ff {tt}

			string res = "";
			str += "    ";

			int level = 0;
			int startIndex = 0;
			bool weAreIn = false;
			string structureMap = "";

			for (int i = 0; i < str.Length; i++)
			{
				try
				{
					if (str.Substring(i).Remove(opener.Length) == opener)
					{
						level++;
						structureMap += "<";
					}
					if (str.Substring(i).Remove(closer.Length) == closer)
					{
						level--;
						structureMap += ">";
					}
				}
				catch
				{
					break;
				}
				if (!weAreIn && level > 0)
				{
					startIndex = i;
					weAreIn = true;
				}
				if (weAreIn && level <= 0)
				{
					weAreIn = false;
					res += str.Substring(startIndex, i - startIndex + closer.Length) + "~";

					if (i > maximumSymbols)
						break;
				}
			}
			return res.Remove(res.Length - 1).Split('~');
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
			return Char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.UppercaseLetter
				|| Char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.LowercaseLetter;
		}

		public static bool IsLowcaseLetter(char symbol)
		{
			return Char.GetUnicodeCategory(symbol) == System.Globalization.UnicodeCategory.LowercaseLetter;
		}

		public static bool IsNumber(char symbol)
		{
			return symbol == '0' || symbol == '1' || symbol == '2' || symbol == '3' || symbol == '4' || symbol == '5' || symbol == '6' || symbol == '7' || symbol == '8' || symbol == '9';
		}

		public static string ModifyText(string text)
		{
			//Deleting double spaces, removing textes with CAPSLOCK
			//and big enters count, adding offset after enters
			for (int i = 0; i < text.Length; i++)
			{
				if (Char.GetUnicodeCategory(text[i]) == System.Globalization.UnicodeCategory.UppercaseLetter && Char.GetUnicodeCategory(text[i + 1]) == System.Globalization.UnicodeCategory.UppercaseLetter)
					throw new Exception("CAPSLOCK");
				if (text[i] == '\n' && text[i + 1] == '\n' && text[i + 2] == '\n')
					throw new Exception("ENTERS");
				if (text[i] == '\n' && text[i + 1] == '\n')
					text = text.Remove(i, 1);
				if (text[i] == '\n')
					text = text.Remove(i + 1) + "     " + text.Substring(i + 1);
				if (text[i] == ' ' && text[i + 1] == ' ')
					text = text.Remove(i, 1);
			}
			return text;
		}

		public static string RemoveNotLettersAndNotSpaces(string str)
		{
			for (int i = str.Length - 1; i >= 0; i--)
				if (!(IsLowcaseLetter(str[i]) || str[i] == ' '))
					str = str.Remove(i, 1);

			return str;
		}

		public static string RemoveSymbols(string str, char symbol)
		{
			return String.Concat(str.Split(symbol));
		}

		public static string RemoveDoubles(string text, char symbol)
		{
			for (int i = 0; i < text.Length - 1;)
				if (text[i] == symbol && text[i + 1] == symbol)
					text = text.Remove(i, 1);
				else
					i++;

			return text;
		}

		public static int SubstringsCount(string str, string str0)
		{
			return (str.Length - str.Replace(str0, "").Length) / str0.Length;
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

				res.Add(Text2.StringAfterLast(str, c.ToString()));
				str = Text2.StringBeforeLast(str, c.ToString());
			}

			return res;
		}

		public static string ModifyStringAsList(string str)
		{
			//Deleting double enters, spaces, tabs,
			//empty strings,
			//spaces and unused enters in begining and ending of strings

			for (int i = str.Length - 1; i > 0; i--)
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
					if ((i > 0 && !IsNumber(str[i])))
						str = str.Remove(i, 1);

				while (str.Length > 1 && str[0] == '0')
					str = str.Substring(1);
			}

			return str;
		}

		public static string[] ProSplit(string str, string separator)
		{
			//Like ususal spit, but with string separators

			return str.Split(str.Replace(separator, "~"));
		}

		public static string[] OmegaSplit(string str, params char[] cs)
		{
			//Like usual split, but it is not deleting separatos
			//And more separators
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

		public static string ToClearUrl(string str)
		{
			try
			{
				str = RemoveSymbols(str, ' ');

				if (str.Remove(8).ToLower() == "https://")
					str = str.Substring(8);
				if (str.Remove(7).ToLower() == "https:/")
					str = str.Substring(7);
				if (str.Remove(7).ToLower() == "http://")
					str = str.Substring(7);
				if (str.Remove(6).ToLower() == "http:/")
					str = str.Substring(6);

				if (str.Remove(6).ToLower() == "https:")
					str = str.Substring(6);
				if (str.Remove(5).ToLower() == "https")
					str = str.Substring(5);
				if (str.Remove(5).ToLower() == "http:")
					str = str.Substring(5);

				if (str.Remove(4).ToLower() == "http")
					str = str.Substring(4);
				if (str.Remove(3).ToLower() == "htt")
					str = str.Substring(3);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

			return str;
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

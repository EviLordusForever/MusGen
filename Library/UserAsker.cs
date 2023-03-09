using Microsoft.VisualBasic;

namespace Library
{
	public static class UserAsker
	{
		public static bool Ask(string q)
		{
			return MessageBox.Show(q, "Hey", MessageBoxButtons.YesNo) == DialogResult.Yes;
		}

		public static string AskFolder(Environment.SpecialFolder rootFolder, string description)
		{
			string res = "";
			Application.OpenForms[0].Invoke(new Action(() =>
			{
				FolderBrowserDialog fbd;
				fbd = new FolderBrowserDialog();
				fbd.RootFolder = rootFolder;
				fbd.Description = description;
				fbd.ShowDialog();
				res = fbd.SelectedPath;
			}));
			return res;
		}

		public static string AskValue(string prompt, string title, string defaultValue)
		{
			return Interaction.InputBox(prompt, title, defaultValue);
		}

		public static void SayWait(string q)
		{
			MessageBox.Show(q);
		}
	}
}

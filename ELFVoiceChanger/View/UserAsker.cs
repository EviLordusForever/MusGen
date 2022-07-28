using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.View
{
	public static class UserAsker
	{
		public static bool Ask(string q)
		{
			var confirmResult = MessageBox.Show(q, "Hey", MessageBoxButtons.YesNo);
			if (confirmResult == DialogResult.Yes)
				return true;
			else
				return false;
		}

		public static void ShowProgress(string text)
		{
			FormsManager.OpenProgressForm();
			FormsManager.progressForm.Text = text;
		}

		public static void SetProgress(int value)
		{
			FormsManager.progressForm.progress.Value = value;
		}

		public static void CloseProgressForm()
		{
			FormsManager.CloseProgressForm();
		}
	}
}

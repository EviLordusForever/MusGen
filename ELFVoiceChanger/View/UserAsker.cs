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
			FormsManager.mainForm.Invoke(new Action(() =>
			{
				FormsManager.OpenProgressForm();
				FormsManager.progressForm.Text = text;
				FormsManager.progressForm.progress.Maximum = 1000;
				//FormsManager.progressForm.progress.Minimum = 0;
			}));
		}

		public static void SetProgress(double value)
		{
			FormsManager.mainForm.Invoke(new Action(() =>
			{
				FormsManager.progressForm.progress.Value = (int)(value * 1000);
			}));
		}

		public static void CloseProgressForm()
		{
			FormsManager.mainForm.Invoke(new Action(() =>
			{
				FormsManager.CloseProgressForm();
			}));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Forms;

namespace MusGen
{
	internal class ProgressShower
	{
		public static void ShowProgress(string text)
		{
			FormsManager._mainForm.Invoke(new Action(() =>
			{
				FormsManager.OpenProgressForm();
				FormsManager._progressForm.Text = text;
				FormsManager._progressForm.progress.Maximum = 1000;
				//FormsManager.progressForm.progress.Minimum = 0;
			}));
		}

		public static void SetProgress(double value)
		{
			FormsManager._mainForm.Invoke(new Action(() =>
			{
				FormsManager._progressForm.progress.Value = (int)(value * 1000);
			}));
		}

		public static void CloseProgressForm()
		{
			FormsManager._mainForm.Invoke(new Action(() =>
			{
				FormsManager.CloseProgressForm();
			}));
		}
	}
}

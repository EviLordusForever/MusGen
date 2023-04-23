using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

namespace MusGen
{
	internal class ProgressShower
	{
		public static void ShowProgress(string text)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				WindowsManager.OpenProgressWindow(text);
			});
		}

		public static void SetProgress(double value)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				WindowsManager._progressWindow.PB.Value = value * 100;
			});
		}

		public static void CloseProgressForm()
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				WindowsManager._progressWindow.Close();
			});
		}
	}
}

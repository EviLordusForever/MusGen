using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

namespace MusGen
{
	public static class ProgressShower
	{
		private static string _text;

		public static void Show(string text)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				WindowsManager.OpenProgressWindow(text);
				Set(0);
				WindowsManager._progressWindow.UpdateLayout();
			});
			_text = text;
			Logger.Log($"Started: {text}");
		}

		public static void Set(double value)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				WindowsManager._progressWindow.PB.Value = value * 100;
			});
		}

		public static void Close()
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				WindowsManager._progressWindow.Visibility = Visibility.Collapsed;
			});
			Logger.Log($"Done: {_text}");
		}
	}
}

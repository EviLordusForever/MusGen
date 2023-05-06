using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using MusGen.View.Windows;

namespace MusGen
{
	public static class WindowsManager
	{
		public static LoggerWindow _logWindow;
		public static ProgressWindow _progressWindow;
		public static RealtimeFFTWindow _realtimeFFTWindow;
		public static MainWindow _mainWindow;
		public static AboutWindow _aboutWindow;

		public static void Open(UIElement el)
		{
			_mainWindow.place.Child = el;
		}

		public static void OpenRealtimeFFTWindow()
		{
			if (_realtimeFFTWindow == null || !_realtimeFFTWindow.IsVisible)
			{
				_realtimeFFTWindow = new RealtimeFFTWindow();
				_realtimeFFTWindow.Show();
			}

			_realtimeFFTWindow.Activate();
			_realtimeFFTWindow.WindowState = WindowState.Maximized;
		}

		public static void OpenAboutWindow()
		{
			if (_aboutWindow == null || !_aboutWindow.IsVisible)
			{
				_aboutWindow = new AboutWindow();
				_aboutWindow.Show();
			}

			_aboutWindow.Activate();
			_aboutWindow.WindowState = WindowState.Normal;
		}

		public static void OpenProgressWindow(string text)
		{
			if (_progressWindow == null || !_progressWindow.IsVisible)
			{
				_progressWindow = new ProgressWindow();
				_progressWindow.Show();
			}

			_progressWindow.Title = text;
		}

		public static void OpenLogWindow()
		{
			if (_logWindow == null || !_logWindow.IsVisible)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					_logWindow = new LoggerWindow();
					_logWindow.Show();
				});
			}
		}
	}
}
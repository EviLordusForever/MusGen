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
		public static AudioRecreationWindow _audioRecreationWindow;
		public static ProgressWindow _progressWindow;

		public static void OpenProgressWindow(string text)
		{
			if (_progressWindow == null)
			{
				_progressWindow = new ProgressWindow();
				_progressWindow.Show();
			}
			else if (!_progressWindow.IsVisible)
			{
				_progressWindow = new ProgressWindow();
				_progressWindow.Show();
			}

			_progressWindow.Title = text;
		}

		public static void OpenLogWindow()
		{
			if (_logWindow == null)
			{
				_logWindow = new LoggerWindow();
				_logWindow.Show();
			}
			else if (!_logWindow.IsVisible)
			{
				_logWindow = new LoggerWindow();
				_logWindow.Show();
			}
		}

		public static void OpenAudioRecreationWindow()
		{
			if (_audioRecreationWindow == null)
			{
				_audioRecreationWindow = new AudioRecreationWindow();
				_audioRecreationWindow.Show();
			}
			else if (!_audioRecreationWindow.IsVisible)
			{
				_audioRecreationWindow = new AudioRecreationWindow();
				_audioRecreationWindow.Show();
			}
			else if (_audioRecreationWindow.WindowState == WindowState.Minimized)
			{
				_audioRecreationWindow.WindowState = WindowState.Normal;
			}

			_audioRecreationWindow.Activate();
		}
	}
}
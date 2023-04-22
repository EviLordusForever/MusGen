using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using MusGen.View.Windows;

namespace MusGen.Forms
{
	public static class FormsManager
	{
		public static LoggerWindow logWindow;

		public static void OpenLogWindow()
		{
			if (logWindow == null)
			{
				logWindow = new LoggerWindow();
				logWindow.Show();
			}
			else if (!logWindow.IsVisible)
			{
				logWindow = new LoggerWindow();
				logWindow.Show();
			}
		}
	}
}
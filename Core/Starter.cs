using System.Threading;
using System;
using System.Windows;
using System.Runtime.InteropServices;

namespace MusGen
{
	public static class Starter
	{
		public static void OnStart()
		{
			Logger.Log("Hello. App started. Good luck!");

			Thread tr = new(Tr);
			tr.Name = "Starter";
			tr.Start();

			void Tr()
			{
				Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					WindowsManager._mainWindow.WindowState = WindowState.Minimized;
				}));

				SpectrumFinder.Init();
				SpectrumDrawer.Init();

				Logger.Log("Tests are started...");

				Tests.HungarianAlgorithm();
				Tests.GradientDithering();
				Tests.GraphDrawerGradient();
				Tests.SPL();
				Tests.ArrayToLog();

				Logger.Log("Tests are completed.");

				while (Logger._updated)
					Thread.Sleep(100);

				Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					WindowsManager._mainWindow.WindowState = WindowState.Normal;
					WindowsManager._mainWindow.Activate();
					WindowsManager._mainWindow.Focus();
					WindowsManager._mainWindow.BringIntoView();
				}));
			}
		}
	}
}

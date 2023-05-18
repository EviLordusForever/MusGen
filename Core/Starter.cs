using System.Threading;
using System;
using System.Windows;
using System.Runtime.InteropServices;
using Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace MusGen
{
	public static class Starter
	{
		public static void OnStart()
		{
			Logger.Log("▶ Hello. App started. Good luck!");

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
				FftRecognitionModel.Init(AP.FftSize, (int)AP.SampleRate, AP._lc);

				/*				Logger.Log("Tests are started...");

								Tests.HungarianAlgorithm();
								Tests.GradientDithering();
								Tests.GraphDrawerGradient();
								Tests.SPL();
								Tests.ArrayToLog();
								Tests.SoftOctaveReverser();
								Tests.FromLogTest();
								Tests.Ceiling();
								Tests.Smoothing();
								Tests.FrequenciesResolution();	
								Tests.FftRecognitionModelTest();

								Logger.Log("Tests are completed.");
				*/

				//PeaksFinding.Workflow.Make();

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

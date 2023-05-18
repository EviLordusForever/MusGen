using System.Threading;
using System;
using System.Windows;
using System.Runtime.InteropServices;
using Extensions;

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
/*				string py1 = $"{DiskE._programFiles}Python\\38\\python38.dll";
				string py2 = $"{DiskE._programFiles}Python\\38\\";
				Environment.SetEnvironmentVariable("PATH", py2, EnvironmentVariableTarget.Process);
				Environment.SetEnvironmentVariable("PYTHONHOME", py2, EnvironmentVariableTarget.Process);
				Environment.SetEnvironmentVariable("PYTHONPATH", py2, EnvironmentVariableTarget.Process);
				Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", py1);*/

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

				PeaksFinding.Workflow.Make();

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

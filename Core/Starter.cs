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
				//SMM.Init(); ////////////////////////////

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
								Tests.Alg();
								//Tests.NLS();
								Logger.Log("Tests are completed.");
				*/

				//Tests.Curve();

				//Change("TEST");
				//Change("TreeSong");

/*				ushort samplesCount = 10;
				float duration = 2;
				ushort heigh = 1024;
				ushort cs = 1;
				Nad grad = new Nad(samplesCount, duration, cs, heigh);
				for (int s = 0; s < samplesCount; s++)
				{
					grad._samples[s] = new NadSample(heigh);
					for (ushort c = 0; c < heigh; c++)
					{
						grad._samples[s]._amplitudes[c] = 1;
						grad._samples[s]._indexes[c] = c;
					}
				}

				grad.Export("Grad");*/

				void Change(string name)
				{
					string path = $"{DiskE._programFiles}Export\\Nads\\{name}.nad";
					Nad nad = new Nad(0, 0, 0, 0);
					nad.Read(path);
					nad.Export(name + "_compressed");
				}

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

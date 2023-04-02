using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Core;
using MusGen.Voice.Models;
using MusGen.View;
using System.Threading;
using Library;

namespace MusGen
{
	public static class EffectsCreator
	{
		public static Wav wavIn;
		public static Wav wavOut;
		public static string waveType = "sin";
		public static float adaptiveCeiling = 0;

		public static void Startup(string originPath)
		{
			wavIn = new Wav();
			wavIn.Read(originPath, 0);

			wavOut = new Wav();
			wavOut.Read(originPath, 0);

			wavOut.L = new float[wavIn.L.Length];
			if (wavOut.channels == 2)
				wavOut.R = new float[wavIn.R.Length];
		}

		public static void Startup(string originPath, int limitSec)
		{
			wavIn = new Wav();
			wavIn.Read(originPath, 0);

			wavOut = new Wav();
			wavOut.Read(originPath, 0);

			wavOut.L = new float[Math.Min(limitSec * wavIn.sampleRate, wavIn.L.Length)];
			if (wavOut.channels == 2)
				wavOut.R = new float[Math.Min(limitSec * wavIn.sampleRate, wavIn.R.Length)];
		}

		public static void Export(string name)
		{
			wavOut.Export(name);
		}

		public static void EffectPanWaving(string originPath, string outName)
		{
			Startup(originPath);

			double t = 0;
			for (int i = 0; t < wavIn.L.Length; i++)
			{
				wavOut.L[i] = wavIn.L[(int)t];
				t += Math.Sin(i / 8096.0) + 1;
			}

			t = 0;
			for (int i = 0; t < wavIn.R.Length; i++)
			{
				wavOut.R[i] = wavIn.R[(int)t];
				t += Math.Sin(i / 8080.0) + 1;
			}

			Export(outName);
		}

		public static void EffectSqrt(string originPath, string outName)
		{
			Startup(originPath);

			for (int i = 0; i < wavIn.L.Length; i++)
				if (wavIn.L[(int)i] >= 0)
					wavOut.L[i] = (float)Math.Pow(wavIn.L[(int)i], 0.5);
				else
					wavOut.L[i] = -(float)Math.Pow(-wavIn.L[(int)i], 0.5);

			for (int i = 0; i < wavIn.R.Length; i++)
				if (wavIn.R[(int)i] >= 0)
					wavOut.R[i] = (float)Math.Pow(wavIn.R[(int)i], 0.5);
				else
					wavOut.R[i] = -(float)Math.Pow(-wavIn.R[(int)i], 0.5);

			Export(outName);
		}

		public static void EffectDftMulti(string originPath, string outName, int limitSec)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				Startup(originPath);

				float pi2 = MathF.PI * 2;

				float signal = 0;
				float A = 1;
				double AA = 1;

				double i2 = 0;

				double mismatch = 1;
				double mismatchLimit = 1;

				int channels = 5;

				float[] periods1 = new float[channels];
				float[] periods2 = new float[channels];
				float[] amps1 = new float[channels];
				float[] amps2 = new float[channels];
				float[] t = new float[channels];

				uint graphStep = wavIn.sampleRate / 60;



				ProgressShower.ShowProgress("Effect7 making");
				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);

				UserAsker.Ask($"Name: {outName}\nWave type: {waveType}\nRecreation hannels: {channels}\nSamples: {limit}\nSample rate: {wavOut.sampleRate}\nSeconds: {(int)(limit / wavOut.sampleRate)}");
				
				for (int i = 0; i < limit; i++)
				{
					if (i % 250 == 0)
					{
						PeriodFinder.FP_DFT_MULTI(ref periods1, ref amps1, wavIn, i, 4000, 20, 15, $"", adaptiveCeiling);
						adaptiveCeiling *= 0.99f;
						//SortByFrequency();
						ProgressShower.SetProgress(1.0 * i / limit);
					}

					if (i % graphStep == 0)
					{
						GraphDrawer.DrawGraphPlus($"{i}", PeriodFinder.dft, PeriodFinder.leadIndexes, amps1, adaptiveCeiling);
						PeriodFinder.FP_DFT_MULTI(ref periods1, ref amps1, wavIn, i, 4000, 20, 15, $"", adaptiveCeiling);
						adaptiveCeiling *= 0.99f;						
					}

					signal = 0;

					for (int c = 0; c < channels; c++)
					{
						amps2[c] = amps2[c] * 0.8f + amps1[c] * 0.2f;
						periods2[c] = periods2[c] * 0.8f + periods1[c] * 0.2f;

						t[c] += pi2 * 0.001f / periods2[c];

						signal += (float)(F(t[c]) * amps2[c]);
					}

					wavOut.L[i] = signal / channels;
					if (wavIn.channels == 2)
						wavOut.R[i] = wavOut.L[i];
				}

				Export(outName);

				ProgressShower.CloseProgressForm();

				void SortByFrequency()
				{
					float[] periodsSorted = new float[periods1.Length];
					float[] ampsSorted = new float[amps1.Length];

					for (int i = 0; i < periods1.Length; i++)
					{
						int id = Math2.IndexOfMax(periods1);
							
						periodsSorted[i] = periods1[id];
						ampsSorted[i] = amps1[id];

						periods1[id] = 0;
					}

					periods1 = periodsSorted;
					amps1 = ampsSorted;
				}
			}
		}


		public static float F(float t)
		{
			if (waveType == "sin")
				return MathF.Sin(t);
			else if (waveType == "sqaure")
				return Math.Sign(Math.Sin(t));
			else
				return MathF.Sin(t);
		}
	}
}
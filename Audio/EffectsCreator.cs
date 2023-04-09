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
		public static string waveForm = "sin";
		public static float adaptiveCeiling = 3;

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

		public static void EffectFFTMulti(string originPath, string outName)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				//PARAMS:

				int limitSec = 25;
				int channels = 5;
				float smooth = 0.98f;
				bool drawGraph = true;
				int FFTsize = 512;
				float trashSize = 15;

				Startup(originPath);

				float signal = 0;
				float antismooth = 1 - smooth;

				float pi2 = MathF.PI * 2;
				float buf = pi2 / wavIn.sampleRate;

				float[] periods1 = new float[channels];
				float[] periods2 = new float[channels];
				float[] amps1 = new float[channels];
				float[] amps2 = new float[channels];
				float[] t = new float[channels];

				outName += $" (FFT {FFTsize} ch {channels} tr {trashSize})";

				uint graphStep = wavIn.sampleRate / 60;

				ProgressShower.ShowProgress("Effect FFT multi audio recreation...");
				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);

				UserAsker.Ask($"Name: {outName}\nWave type: {waveForm}\nRecreation channels: {channels}\nSmooth: {smooth}\nFFT size: {FFTsize}\nTrash size: {trashSize}\nDraw graph: {drawGraph}\nSamples: {limit}\nSample rate: {wavOut.sampleRate}\nSeconds: {(int)(limit / wavOut.sampleRate)}");
				
				for (int i = 0; i < limit; i++)
				{
					if (i % 250 == 0)
					{
						PeriodFinder.FFT_MULTI(ref periods1, ref amps1, wavIn, i, FFTsize, trashSize, ref adaptiveCeiling);
						//SortByFrequency();
					}

					if (i % graphStep == 0)
					{
						if (drawGraph)
						{
							float[] amps0 = new float[channels];
							for (int c = 0; c < channels; c++)
								amps0[c] = amps1[c] * PeriodFinder.amplitudeOverflow;

							GraphDrawer.Draw($"{i}", PeriodFinder.dft, PeriodFinder.leadIndexes, amps0, adaptiveCeiling);
							adaptiveCeiling *= 0.99f;
						}

						ProgressShower.SetProgress(1.0 * i / limit);
					}

					signal = 0;

					for (int c = 0; c < channels; c++)
					{
						amps2[c] = amps2[c] * smooth + amps1[c] * antismooth;
						if (periods1[c] < 0.05f)
							periods2[c] = periods2[c] * smooth + periods1[c] * antismooth;

						t[c] += buf / periods2[c];

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
			if (waveForm == "sin")
				return MathF.Sin(t);
			else if (waveForm == "sqaure")
				return Math.Sign(Math.Sin(t));
			else
				return MathF.Sin(t);
		}
	}
}
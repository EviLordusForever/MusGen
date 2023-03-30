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

		public static void Effect3(string originPath, string outName, int limitSec)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				Startup(originPath);

				double pi2 = Math.PI * 2;
				double period = 0.01;
				double periodNew = 0.01;
				double t = 0;
				float sint = 0;
				float A = 1;
				double AA = 1;

				double mismatch = 1;
				double mismatchLimit = 0.45;

				ProgressShower.ShowProgress("Effect3 making");

				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);
				for (int i = 0; i < limit; i++)
				{
					if (i % 500 == 0)
						if (i < wavIn.L.Length)
						{
							periodNew = PeriodFinder.FindPeriod(wavIn, i, 8000, out mismatch) / 2;

							if (mismatch < mismatchLimit)
								period = period * 0.75 + periodNew * 0.25; //

							if (i < limit - 500)
								A = FindA(i, i + 500);

							ProgressShower.SetProgress(1.0 * i / limit);
						}

					AA = AA * 0.98 + A * 0.02;

					t += pi2 / period;
					sint = (float)(Math.Sin(t) * 0.99 * AA);

					wavOut.L[i] = sint;
					if (wavIn.channels == 2)
						wavOut.R[i] = sint;
				}

				Export(outName);

				ProgressShower.CloseProgressForm();

				float FindA(int from, int to)
				{
					float A = 0;
					for (int i = from; i < to; i++)
						if (Math.Abs(wavIn.L[i]) > A)
							A = Math.Abs(wavIn.L[i]);
					return A;
				}
			}
		}

		public static void Effect4(string originPath, string outName, int limitSec)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				Startup(originPath);

				double pi2 = Math.PI * 2;
				double period = 0.01;
				double periodNew = 0.01;
				double t = 0;
				float sint = 0;
				float A = 1;
				double AA = 1;

				double i2 = 0;

				double mismatch = 1;
				double mismatchLimit = 0.45;

				ProgressShower.ShowProgress("Effect4 making");
				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);
				for (int i = 0; i < limit; i++)
				{
					if (i % 500 == 0)
						if (i < wavIn.L.Length)
						{
							if (i > i2)
							{
								i2 += (wavIn.sampleRate / 60.0);

								periodNew = PeriodFinder.FindPeriod_WithAnimation(wavIn, i, 8000, out mismatch, mismatchLimit, period, i) / 2;
							}
							else
								periodNew = PeriodFinder.FindPeriod(wavIn, i, 8000, out mismatch) / 2;

							if (mismatch < mismatchLimit)
								period = period * 0.75 + periodNew * 0.25; //

							if (i < limit - 500)
								A = FindA(i, i + 500);

							ProgressShower.SetProgress(1.0 * i / limit);
						}

					AA = AA * 0.98 + A * 0.02;

					t += pi2 / period;
					sint = (float)(Math.Sin(t) * 0.99 * AA);

					wavOut.L[i] = sint;
					if (wavIn.channels == 2)
						wavOut.R[i] = sint;
				}

				Export(outName);

				ProgressShower.CloseProgressForm();

				float FindA(int from, int to)
				{
					float A = 0;
					for (int i = from; i < to; i++)
						if (Math.Abs(wavIn.L[i]) > A)
							A = Math.Abs(wavIn.L[i]);
					return A;
				}
			}
		}

		public static void Effect5(string originPath, string outName, int limitSec)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				Startup(originPath);

				double pi2 = Math.PI * 2;
				double period = 0.01;
				double t = 0;
				float sint = 0;
				float A = 1;
				double AA = 1;

				double i2 = 0;

				double mismatch = 1;
				double mismatchLimit = 1;

				ProgressShower.ShowProgress("Effect5 making");
				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);
				for (int i = 0; i < limit; i++)
				{
					if (i % 500 == 0)
					{
						if (i > i2)
						{
							i2 += (wavIn.sampleRate / 60.0);

							period = PeriodFinder.FP_DFT_ANI(wavIn, i, 2000, 10, out mismatch, mismatchLimit, period, i) / 2;
						}
						else
							period = PeriodFinder.FP_DFT(wavIn, i, 2000, 10, out mismatch) / 2;

						if (i < limit - 500)
							A = FindA(i, i + 500);

						ProgressShower.SetProgress(1.0 * i / limit);
					}

					AA = AA * 0.98 + A * 0.02;

					t += pi2 / period;
					sint = (float)(Math.Sin(t) * 0.99 * AA);

					wavOut.L[i] = sint;
					if (wavIn.channels == 2)
						wavOut.R[i] = sint;
				}

				Export(outName);

				ProgressShower.CloseProgressForm();

				float FindA(int from, int to)
				{
					float A = 0;
					for (int i = from; i < to; i++)
						if (Math.Abs(wavIn.L[i]) > A)
							A = Math.Abs(wavIn.L[i]);
					return A;
				}
			}
		}

		public static void EffectDftSingle(string originPath, string outName, int limitSec)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				Startup(originPath);

				double pi2 = Math.PI * 2;
				double period_actual = 40;
				double t = 0;
				float signal = 0;

				double a_smooth = 0;

				double a_new = 0;
				double a_actual = 0;

				double period_new = period_actual;

				ProgressShower.ShowProgress("Effect6 making");
				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);
				for (int i = 0; i < limit; i++)
				{
					if (i % 500 == 0)
						if (i < wavIn.L.Length)
						{
							period_actual = PeriodFinder.FP_DFT(wavIn, i, 2000, 10, out a_new);

							ProgressShower.SetProgress(1.0 * i / limit);
						}

					if (Math.Abs(a_actual) > 1)
						a_actual *= 1 / Math.Abs(a_actual);

					a_smooth = a_smooth * 0.98 + a_actual * 0.02;

					t += pi2 / period_actual;
					signal = (float)(F((float)t) * 0.99 * a_smooth);

					wavOut.L[i] = signal;
					if (wavIn.channels == 2)
						wavOut.R[i] = signal;
				}

				Export(outName);

				ProgressShower.CloseProgressForm();

				float FindA(int from, int to)
				{
					float A = 0;
					for (int i = from; i < to; i++)
						if (Math.Abs(wavIn.L[i]) > A)
							A = Math.Abs(wavIn.L[i]);
					return A;
				}
			}
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
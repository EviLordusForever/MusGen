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

		public static void Effect1(string originPath, string outName)
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

		public static void Effect2(string originPath, string outName)
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
						if (i < wavIn.L.Length)
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

		public static void Effect6(string originPath, string outName, int limitSec)
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

				double i2 = 0;

				double a_new = 0;
				double a_actual = 0;
				double mismatchLimit = 1;

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
					signal = (float)(F(t) * 0.99 * a_smooth);

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

		public static void Effect7(string originPath, string outName, int limitSec)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				Startup(originPath);

				double pi2 = Math.PI * 2;

				float sint = 0;
				float A = 1;
				double AA = 1;

				double i2 = 0;

				double mismatch = 1;
				double mismatchLimit = 1;

				int C = 10;

				float[] periods1 = new float[C];
				float[] periods2 = new float[C];
				float[] amps1 = new float[C];
				float[] amps2 = new float[C];
				double[] t = new double[C];


				ProgressShower.ShowProgress("Effect7 making");
				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);
				for (int i = 0; i < limit; i++)
				{
					if (i % 500 == 0)
						if (i < wavIn.L.Length)
						{
							PeriodFinder.FP_DFT_MULTI(ref periods1, ref amps1, wavIn, i, 2000, 10);
							ProgressShower.SetProgress(1.0 * i / limit);
							//GraficsMaker.MakeGraficPlus(i.ToString(), PeriodFinder.dft, -5, -5, -5, -5, -5);
						}

					sint = 0;

					for (int j = 0; j < C; j++)
					{
						amps2[j] = amps2[j] * 0.98f + amps1[j] * 0.02f;
						//periods2[j] = periods2[j] * 0.9 + periods1[j] * 0.1;

						t[j] += pi2 / periods1[j];
						sint += (float)(F(t[j]) * 0.99 * amps2[j]);
					}

					wavOut.L[i] = sint / C;
					if (wavIn.channels == 2)
						wavOut.R[i] = sint / C;
				}

				Export(outName);

				ProgressShower.CloseProgressForm();
			}
		}


		public static double F(double t)
		{
			return Math.Sign(Math.Sin(t));
		}
	}
}
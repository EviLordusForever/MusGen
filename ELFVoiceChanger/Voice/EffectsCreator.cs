using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.Core;
using ELFVoiceChanger.Voice.Models;
using ELFVoiceChanger.View;
using System.Threading;

namespace ELFVoiceChanger.Voice
{
	public static class EffectsCreator
	{
		public static Wav wavIn;
		public static Wav wavOut;

		public static void Startup(string originPath)
		{
			wavIn = new Wav();
			wavIn.ReadWav(originPath, 0);

			wavOut = new Wav();
			wavOut.ReadWav(originPath, 0);

			wavOut.L = new float[wavIn.L.Length];
			if (wavOut.channels == 2)
				wavOut.R = new float[wavIn.R.Length];
		}

		public static void Startup(string originPath, int limitSec)
		{
			wavIn = new Wav();
			wavIn.ReadWav(originPath, 0);

			wavOut = new Wav();
			wavOut.ReadWav(originPath, 0);

			wavOut.L = new float[limitSec * wavIn.sampleRate];
			if (wavOut.channels == 2)
				wavOut.R = new float[limitSec * wavIn.sampleRate];
		}

		public static void Save(string outName)
		{
			wavOut.SaveWav(Disk.programFiles + "Export\\" + outName + ".wav");
		}

		public static void StartEffectAsync(string originPath, string outName, int effectNUmber)
		{
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

			Save(outName);
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

			Save(outName);
		}

		public static void Effect3(string originPath, string outName)
		{
			Startup(originPath);

			double pi2 = Math.PI * 2;
			double period = 0;
			double mismatch = 0;
			double t = 0;
			float sint = 0;
			float A = 1;
			double AA = 1;

			for (int i = 0; i < wavIn.L.Length; i++)
			{
				if (i % 500 == 0)
					if (i < wavIn.L.Length - PeriodFinder.maxPeriod - 2)
					{
						period = PeriodFinder.FindPeriod(wavIn, i, i + PeriodFinder.maxPeriod, out mismatch) / 2;
						A = FindA(i, i + 500);
					}

				AA = AA * 0.98 + A * 0.02;

				t += pi2 / period;
				sint = (float)(Math.Sin(t) * 0.99 * AA);

				wavOut.L[i] = sint;
				if (wavIn.channels == 2)
					wavOut.R[i] = sint;
			}

			Save(outName);

			float FindA(int from, int to)
			{
				float A = 0;
				for (int i = from; i < to; i++)
					if (Math.Abs(wavIn.L[i]) > A)
						A = Math.Abs(wavIn.L[i]);
				return A;
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
				double mismatchLimit = 0.35;

				UserAsker.ShowProgress("Effect4 making");

				for (int i = 0; i < wavIn.sampleRate * limitSec; i++)
				{
					if (i % 100 == 0)
						if (i < wavIn.L.Length)
						{
							if (i > i2)
							{
								i2 += (wavIn.sampleRate / 60.0);

								periodNew = PeriodFinder.FindPeriod_WithAnimation(wavIn, i, 2000, out mismatch, mismatchLimit, period, i) / 2;
							}
							else
								periodNew = PeriodFinder.FindPeriod(wavIn, i, i + 1000, out mismatch) / 2;

							if (mismatch < mismatchLimit)
								period = period * 0.75 + periodNew * 0.25; //

							A = FindA(i, i + 500);

							UserAsker.SetProgress(1.0 * i / (wavIn.sampleRate * limitSec));
						}

					AA = AA * 0.98 + A * 0.02;

					t += pi2 / period;
					sint = (float)(Math.Sin(t) * 0.99 * AA);

					wavOut.L[i] = sint;
					if (wavIn.channels == 2)
						wavOut.R[i] = sint;
				}

				Save(outName);

				UserAsker.CloseProgressForm();

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
	}
}

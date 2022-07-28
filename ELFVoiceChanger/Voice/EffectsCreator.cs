using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.Core;
using ELFVoiceChanger.Voice.Models;

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

		public static void Save(string outName)
		{
			wavOut.SaveWav(Disk.programFiles + "Export\\" + outName + ".wav");
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
			double t = 0;
			float sint = 0;
			float A = 1;
			double AA = 1;

			for (int i = 0; i < wavIn.L.Length; i++)
			{
				if (i % 500 == 0)
					if (i < wavIn.L.Length - 1001)
					{
						period = PerioudFinder.FindPeriod(wavIn, i, i + 1000) / 2;
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

		public static void Effect4(string originPath, string outName)
		{
			Startup(originPath);

			double pi2 = Math.PI * 2;
			double period = 0;
			double t = 0;
			float sint = 0;
			float A = 1;
			double AA = 1;

			for (int i = 0; i < wavIn.L.Length; i++)
			{
				if (i % 500 == 0)
					if (i < wavIn.L.Length - 1001)
					{
						period = PerioudFinder.FindPeriod(wavIn, i, i + 1000) / 2;
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
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Voice.Models;
using System.Threading;
using Extensions;

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
			if (wavOut._channels == 2)
				wavOut.R = new float[wavIn.R.Length];
		}

		public static void Startup(string originPath, int limitSec)
		{
			wavIn = new Wav();
			wavIn.Read(originPath, 0);

			wavOut = new Wav();
			wavOut.Read(originPath, 0);

			wavOut.L = new float[Math.Min(limitSec * wavIn._sampleRate, wavIn.L.Length)];
			if (wavOut._channels == 2)
				wavOut.R = new float[Math.Min(limitSec * wavIn._sampleRate, wavIn.R.Length)];
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

			wavOut.Export(outName, true);
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

			wavOut.Export(outName, true);
		}
	}
}
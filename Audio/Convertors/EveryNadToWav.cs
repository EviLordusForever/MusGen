using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using NAudio.SoundFont;
using OneOf.Types;
using Tensorflow;

namespace MusGen
{
	public static class EveryNadToWav
	{
		public static Wav Make(Nad nad, string wavPath)
		{
			Wav wav = new Wav();
			wav.Read(wavPath);

			return Make(nad, wav);
		}

		public static Wav Make(Nad nad)
		{
			int length = (int)(AP.SampleRate * nad._duration);
			Wav wav = new Wav(length, 1);

			return Make(nad, wav);
		}

		public static Wav Make(Nad nad, Wav wav)
		{
			ushort cs = nad._cs;
			float buf = MathF.PI * 2 / AP.SampleRate;
			int progressStep = (int)(wav.Length / 2000);
			float max = 0;

			var newSamples = new NadSample[nad._samples.Length + cs * 2 + (int)(cs * 10f / 718f * nad.Width)]; ////////
			Array.Copy(nad._samples, 0, newSamples, cs, nad._samples.Length); 

			ProgressShower.Show("NAD to WAV...");

			int startedCount = 0;
			int cores = HardwareParams._coresCount;

			for (int core = 0; core < cores; core++)
			{
				int coreCopy = core;
				Task.Run(() => AsyncTask(coreCopy));
			}

			async Task AsyncTask(int core)
			{
				startedCount++;

				float[] fadeins = new float[cs];
				float[] fadeouts = new float[cs];
				int[] lefts = new int[cs];
				int[] rights = new int[cs];

				for (int s = core; s < wav.Length; s += cores)
				{
					GetValues(s);
					WriteSignal(s);
					Progress(s);
				}
				startedCount--;

				void GetValues(int s)
				{
					for (int idk = 0; idk < cs; idk++)
					{
						double b = 1.0 * s * nad.Width / (wav.Length * cs);
						double state = b + 1.0 * idk / cs + cs;

						int left = (int)Math.Floor(state);
						int right = (int)Math.Ceiling(state);

						if (right == left)
							right += 1;

						fadeouts[idk] = MathE.FadeOut((float)state - left);
						fadeins[idk] = 1 - fadeouts[idk];

						lefts[idk] = (int)left * cs - idk;
						rights[idk] = (int)right * cs - idk;
					}
				}

				void WriteSignal(int s)
				{
					Sines sines = new Sines();

					for (int i = 0; i < cs; i++)
					{
						AddSines(newSamples[lefts[i]], fadeouts[i]);
						AddSines(newSamples[rights[i]], fadeins[i]);
					}

					wav.L[s] = sines.GetSignal(s);
					max = max >= MathF.Abs(wav.L[s]) ? max : MathF.Abs(wav.L[s]);

					void AddSines(NadSample ns, float fade)
					{
						if (ns != null)
							for (int c = 0; c < ns.Height; c++)
							{
								ushort idx = ns._indexes[c];
								float amp = ns._amplitudes[c] * fade;
								sines.Add(amp, idx);
							}
					}
				}
			}

			Thread.Sleep(1000);

			while (startedCount > 0)
				Thread.Sleep(500);

			ProgressShower.Close();

			Normalize();

			return wav;

			void Progress(int s)
			{
				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / wav.Length);
			}

			void Normalize()
			{
				ProgressShower.Show("Normalization");
				for (int s = 0; s < wav.Length; s++)
				{
					float v = wav.L[s];
					wav.L[s] = v / max * 0.9f;
					if (wav._channels == 2)
						wav.R[s] = wav.L[s];

					Progress(s);
				}
				ProgressShower.Close();
			}

			void ClearZone2(float[] zone)
			{
				int window1 = (int)(AP.SampleRate / 300f);
				int window2 = (int)(AP.SampleRate / 300f / 6);
				var abs = ArrayE.AbsArrayCopy(zone);
				var average1 = ArrayE.SmoothArrayCopy_Quality_NonNegative(abs, window1);
				var average2 = ArrayE.SmoothArrayCopy_Quality_NonNegative(abs, window2);
				var stdDev1 = ArrayE.StdDev(average1, abs, window1);
				var stdDev2 = ArrayE.StdDev(average2, abs, window2);


				for (int i = 0; i < zone.Length; i++)
				{
					float d = (average1[i] + stdDev1[i]) / (average2[i] + stdDev2[i]);
					if (d < 1)
						zone[i] *= d;
				}
			}

			void ClearZone(float[] zone)
			{
				int window = (int)(AP.SampleRate / 120f);
				var abs = ArrayE.AbsArrayCopy(zone);
				var average = ArrayE.SmoothArrayCopy_Quality_NonNegative(abs, window);
				var stdDev = new float[zone.Length];

				for (int i = 0; i < zone.Length; i++)
				{
					stdDev[i] = MathF.Pow(average[i] - abs[i], 2);

					if (float.IsNaN(stdDev[i]))
					{
					}
				}

				stdDev = ArrayE.SmoothArrayCopy_Quality_NonNegative(stdDev, window);
				stdDev = ArrayE.Sqrt(stdDev);

				for (int i = 0; i < zone.Length; i++)
				{
					float x = abs[i];
					float av = average[i];
					float std = stdDev[i];
					float h = SoftCut(x, 1, (av + std) * 1.3f);
					zone[i] = h * MathF.Sign(zone[i]);
					if (float.IsNaN(zone[i]))
					{
					}
				}
			}

			void ClearZoneKaiser(float[] zone)
			{
				float cutoff = SpectrumFinder._frequenciesLg[(int)(AP.SpectrumSize * 0.9f)];
				zone = KaiserFilter.Make(zone, AP.SampleRate, cutoff, AP._kaiserFilterLength_ForProcessing, AP._kaiserFilterBeta, true);
			}
		}

		public static float SoftCut(float x, float c, float h)
		{
			if (h == 0)
				return 0;
			if (x < h)
				return x;
			else
				return h * (1 + (1 - 1 / MathF.Pow(x / h, c)) / c);
		}

		public static float HardTanh(float x, float c, float h)
		{
			if (h == 0)
				return 0;
			if (x < h * (c - 1) / c)
				return x;
			else
				return h * (MathF.Tanh((x / h - 1) * c + 1) + c - 1) / c;
		}

		public static double F(double t)
		{
			if (AP._waveForm == "sin")
				return Math.Sin(t);
			else if (AP._waveForm == "sqaure")
				return Math.Sign(Math.Sin(t));
			else
				return Math.Sin(t);
		}

		public static float F(float t)
		{
			if (AP._waveForm == "sin")
				return MathF.Sin(t);
			else if (AP._waveForm == "sqaure")
				return MathF.Sign(MathF.Sin(t));
			else
				return MathF.Sin(t);
		}
	}
}

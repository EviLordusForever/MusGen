using System;
using System.Linq;
using Extensions;
using NAudio.SoundFont;
using OneOf.Types;
using Tensorflow;

namespace MusGen
{
	public static class EveryNadToWav
	{
		public static int zonesCount = 3;
		public static bool _divideZones = false;

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
			int length = wav.Length;
			//double sps = nad.Width / (double)nad._duration;
			float pi2 = MathF.PI * 2;
			float buf = pi2 / AP.SampleRate;
			int progressStep = (int)(length / 2000);
			float max = 0;

			string delme5 = "";

			float[] fadeins = new float[cs];
			float[] fadeouts = new float[cs];
			int[] lefts = new int[cs];
			int[] rights = new int[cs];

			float[][] signals = new float[zonesCount][];

			signals[0] = new float[wav.Length];
			signals[1] = new float[wav.Length];
			signals[2] = new float[wav.Length];

			var newSamples = new NadSample[nad._samples.Length + cs * 2 + (int)(cs * 10f / 718f * nad.Width)]; ////////
			Array.Copy(nad._samples, 0, newSamples, cs, nad._samples.Length); 

			ProgressShower.Show("Nad to Wav...");			

			for (int s = 0; s < length; s++)
			{
				GetValues(s);
				for (int zone = 0; zone < 3; zone++)
					WriteSample(s, zone);
				Progress(s);
			}

			//ClearZone2(signals[1]);

			if (!_divideZones)
				for (int s = 0; s < length; s++)
				{
					float signal = 0;
					for (int zone = 0; zone < 3; zone++)
						signal += signals[zone][s];
					wav.L[s] = signal;

					max = max >= MathF.Abs(signal) ? max : MathF.Abs(signal);

					Progress(s);
				}
			else
			{
				wav.L = signals[0].concat(signals[1]).concat(signals[2]);
				max = MathF.Max(wav.L.Max(), -wav.L.Min());
			}

			ProgressShower.Close();

			Normalize();

			return wav;

			void GetValues(int s)
			{
				for (int idk = 0; idk < cs; idk++)
				{
					double b = 1.0 * s * nad.Width / (length * cs);
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

			void WriteSample(int s, int zone)
			{
				for (int i = 0; i < cs; i++)
					signals[zone][s] += GetSignal(lefts[i], rights[i], fadeins[i], fadeouts[i]);

				float GetSignal(int left, int right, float fadein, float fadeout)
				{
					float leftSignal = 0;
					float rightSignal = 0;

					if (newSamples[left] != null)
						leftSignal = GetSignal2(newSamples[left], fadeout);
					if (newSamples[right] != null)
						rightSignal = GetSignal2(newSamples[right], fadein);

					return leftSignal + rightSignal;
				}

				float GetSignal2(NadSample ns, float fade)
				{
					float signal = 0;

					if (ns.Height > 0)
					{
						int count = 0;
						for (int c = 0; c < ns.Height; c++)
						{
							ushort ind = ns._indexes[c];
							if (GetZone(ind) == zone)
							{
								count++;

								float frq = SpectrumFinder._frequenciesLg[ind];
								float amp = ns._amplitudes[c];

								signal += amp * MathF.Sin(buf * frq * s);
							}
						}

						if (count > 0)
							signal *= fade / MathF.Sqrt(count);
					}

					return signal;
				}
			}

			ushort GetZone(ushort ind)
			{
				if (ind < AP.SpectrumSize * 0.6f)
					return 0;
				else if (ind < AP.SpectrumSize * 0.90f)
					return 1;
				else
					return 2;
			}

			void Progress(int s)
			{
				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / length);
			}

			void Normalize()
			{
				ProgressShower.Show("Normalization");
				for (int s = 0; s < length; s++)
				{
					float v = wav.L[s];
					wav.L[s] = v / max;
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

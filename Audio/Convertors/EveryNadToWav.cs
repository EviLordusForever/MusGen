using System;
using Extensions;
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

			var newSamples = new NadSample[nad._samples.Length + cs * 2 + (int)(cs * 10f / 718f * nad.Width)]; ////////
			Array.Copy(nad._samples, 0, newSamples, cs, nad._samples.Length); 

			ProgressShower.Show("Nad to Wav...");			

			for (int s = 0; s < length; s++)
			{
				GetValues(s);
				WriteSample(s);
				Progress(s);

/*				delme5 += String.Join(";", fadeins) + ";;";
				delme5 += String.Join(";", lefts) + ";;";
				delme5 += String.Join(";", wav.L[s]) + "\n";

				if (s > AP.SampleRate)
				{
					DiskE.WriteToProgramFiles("delme5", "csv", delme5, false);
					Logger.Log("done.");
					break;
				}*/
			}

			ProgressShower.Close();

			Normalize();
			ProgressShower.Close();
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

			void WriteSample(int s)
			{
				float signal = 0;

				for (int i = 0; i < cs; i++)
					signal += GetSignal(lefts[i], rights[i], fadeins[i], fadeouts[i]);

				signal /= cs;
				
				wav.L[s] = signal;

				if (Math.Abs(wav.L[s]) > max)
					max = Math.Abs(wav.L[s]);

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
						for (int c = 0; c < ns.Height; c++)
						{
							float frq = SpectrumFinder._frequenciesLg[ns._indexes[c]];
							float amp = ns._amplitudes[c];

							signal += amp * MathF.Sin(buf * frq * s);
						}

						signal *= fade / MathF.Sqrt(ns.Height);
					}

					return signal;
				}
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
					wav.L[s] /= max;
					if (wav._channels == 2)
						wav.R[s] = wav.L[s];

					Progress(s);
				}
			}
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

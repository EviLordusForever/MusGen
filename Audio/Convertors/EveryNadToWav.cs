using System;
using Extensions;

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
			int length = (int)AP.SampleRate * nad._duration;
			Wav wav = new Wav(length, 1);

			return Make(nad, wav);
		}

		public static Wav Make(Nad nad, Wav wav)
		{
			int length = wav.Length;
			double[] t = new double[AP._peaksLimit];
			double[] tOld = new double[AP._peaksLimit];
			int fadeSamplesLeft = 0;
			int sps = wav.Length / nad.Width;
			int samplesForFade = (int)(AP._fadeTime * AP.SampleRate / sps);
			float pi2 = MathF.PI * 2;
			float buf = pi2 / AP.SampleRate;
			int progressStep = (int)(length / 2000);
			float max = 0;

			ProgressShower.Show("Nad to Wav...");

			int ns = 0;
			for (int s = 0; s < length; s++)
			{
				int newNs = (int)((1f * s / length) * nad._samples.Length);
				if (newNs != ns)
				{
					ns = newNs;
					fadeSamplesLeft = samplesForFade;
					SetTimeOld();
				}
				else
					fadeSamplesLeft--;

				WriteSample(s, ns);
				Progress(s);
			}

			Normalize();
			ProgressShower.Close();
			return wav;

			void WriteSample(int s,int ns)
			{
				float newSignal = 0;
				float oldSignal = 0;
				float fade = 0;
				float antifade = 1;

				if (fadeSamplesLeft > 0)
				{
					float status = 1 - 1f * fadeSamplesLeft / samplesForFade;
					fade = MathE.FadeOut(status);
					antifade = 1 - fade;
				}

				for (int c = 0; c < nad._samples[ns].Width; c++)
				{
					if (nad._samples[ns]._frequencies[c] < 20f)
						nad._samples[ns]._frequencies[c] = 20f;

					t[c] += buf * nad._samples[ns]._frequencies[c];
					newSignal += (float)F(t[c]) * nad._samples[ns]._amplitudes[c];
				}

				newSignal *= antifade / nad._samples[ns].Width;

				if (fadeSamplesLeft > 0)
				{
					for (int c = 0; c < nad._samples[ns - 1].Width; c++)
					{
						tOld[c] += buf * nad._samples[ns - 1]._frequencies[c];
						oldSignal += (float)F(tOld[c]) * nad._samples[ns - 1]._amplitudes[c];
					}

					oldSignal *= fade / nad._samples[ns - 1].Width;
				}

				wav.L[s] = oldSignal + newSignal;

				if (Math.Abs(wav.L[s]) > max)
					max = Math.Abs(wav.L[s]);
			}

			void SetTimeOld()
			{
				for (int c = 0; c < AP._peaksLimit; c++)
					tOld[c] = t[c];
			}

			void Progress(int s)
			{
				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / length);
			}

			void Normalize()
			{
				ProgressShower.Show("Normalization");
				ProgressShower.Set(0);
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

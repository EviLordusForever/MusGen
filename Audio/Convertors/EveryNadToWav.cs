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
			int length = (int)(AP.SampleRate * nad._duration);
			Wav wav = new Wav(length, 1);

			return Make(nad, wav);
		}

		public static Wav Make(Nad nad, Wav wav)
		{
			int length = wav.Length;
			int fadeSamplesLeft = 0;
			float sps = 1f * wav.Length / nad.Width;
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
				}
				else
					fadeSamplesLeft--;

				WriteSample(s, ns);
				Progress(s);
			}

			ProgressShower.Close();

			Normalize();
			ProgressShower.Close();
			return wav;

			void WriteSample(int s, int ns)
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

				if (nad._samples[ns].Height > 0)
				{
					for (int c = 0; c < nad._samples[ns].Height; c++)
					{
						if (nad._samples[ns]._frequencies[c] < 20f)
							nad._samples[ns]._frequencies[c] = 20f;

						float frq = nad._samples[ns]._frequencies[c];
						float amp = nad._samples[ns]._amplitudes[c];
						float value = amp * MathF.Sin(pi2 * frq * s / AP.SampleRate);
						newSignal += value;
					}

					newSignal *= antifade / MathF.Sqrt(nad._samples[ns].Height);
				} //

				if (fadeSamplesLeft > 0)
					if (nad._samples[ns - 1].Height > 0)
					{
						for (int c = 0; c < nad._samples[ns - 1].Height; c++)
						{
							float frq = nad._samples[ns - 1]._frequencies[c];
							float amp = nad._samples[ns - 1]._amplitudes[c];
							float value = amp * MathF.Sin(pi2 * frq * s / AP.SampleRate);
							oldSignal += value;
						}

						oldSignal *= fade / MathF.Sqrt(nad._samples[ns - 1].Height);
					}

				wav.L[s] = oldSignal + newSignal;

				if (Math.Abs(wav.L[s]) > max)
					max = Math.Abs(wav.L[s]);
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

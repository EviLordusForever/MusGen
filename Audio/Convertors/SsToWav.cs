using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class SsToWav
	{
		public static Wav Make(SS ss)
		{
			ss = SsDelogariphmisator.Make(ss);

			int wavLength = (int)(ss._s.Length / ss._sps * AP.SampleRate);
			Wav wav = new Wav(wavLength, 1);
			int fadeSamplesLeft = 0;
			int samplesForFade = (int)(AP._fadeTime * wavLength / ss._s.Length);
			float pi2 = MathF.PI * 2;
			float buf = pi2 / AP.SampleRate;
			int progressStep = (int)(wavLength / 1000);
			float max = 0;
			float[] signal = new float[AP.FftSize];
			float[] signalOld = new float[AP.FftSize];
			int signalPoint = 0;
			int oldSignalPoint = 0;

			ProgressShower.Show("Ss to wav...");

			float[] phases = new float[ss.Height * 2];
			float[] phasesChanges = new float[ss.Height * 2];

			for (int p = 0; p < phasesChanges.Length; p++)
				phasesChanges[p] = MathF.PI * (MathE.rnd.Next() * 2 - 1) * (AP._phasesShift + MathE.rnd.NextSingle() * AP._phasesShiftRandom);

			int ns = 0;
			for (int s = 0; s < wavLength; s++, signalPoint++, oldSignalPoint++)
			{
				int newNs = (int)(1f * s / wavLength * ss._s.Length);
				if (newNs != ns)
				{
					if (newNs >= ss.Width)
						break;

					ns = newNs;
					fadeSamplesLeft = samplesForFade;

					signalOld = signal;
					signal = FFT.Inverse(ss._s[ns], phases);

					Phases();

					oldSignalPoint = signalPoint;
					signalPoint = 0;
				}
				else
					fadeSamplesLeft--;

				WriteSample(s, ns);

				if (Math.Abs(wav.L[s]) > max)
					max = Math.Abs(wav.L[s]);

				Progress(s);
			}

			Normalize();
			ProgressShower.Close();
			return wav;

			void Phases()
			{
				for (int p = 0; p < ss.Height; p++)
				{
					phases[p] += phasesChanges[p];

					if (Math.Abs(phases[p]) > 1000000)
						phases[p] = 0;
				}
			}

			void WriteSample(int s, int ns)
			{
				float fade = 0;
				float antifade = 1;

				if (fadeSamplesLeft > 0)
				{
					float status = 1 - 1f * fadeSamplesLeft / samplesForFade;
					fade = MathE.Fade(status);
					antifade = 1 - fade;
				}

				if (oldSignalPoint < signalOld.Length)
					wav.L[s] = signalOld[oldSignalPoint] * fade + signal[signalPoint] * antifade;
				else
					wav.L[s] = signal[signalPoint];
			}

			void Progress(int s)
			{
				if (s % progressStep == 0)
					ProgressShower.Set(1.0 * s / wavLength);
			}

			void Normalize()
			{
				ProgressShower.Show("Normalization");
				ProgressShower.Set(0);
				for (int s = 0; s < wavLength; s++)
				{
					wav.L[s] /= max;
					if (wav._channels == 2)
						wav.R[s] = wav.L[s];

					Progress(s);
				}
			}
		}
	}
}

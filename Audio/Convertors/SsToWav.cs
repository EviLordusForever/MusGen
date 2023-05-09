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
			int shiftSignalPoint = 0;
			int signalPoint = 0;
			int oldSignalPoint = 0;

			ProgressShower.Show("Ss to wav...");

			float[] phases = new float[ss.Height * 2];
			float phasesMove = 0;

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

					float[] stretched = ArrayE.NormalStretch(ss._s[ns], AP._ifftScale);
					float[] phasesStretched = ArrayE.NormalStretch(phases, AP._ifftScale);

/*					int length = stretched.Length / 4;
					for (int i = 0; i < length; i++)
						stretched[i] *= 1 - MathE.Fade(1f * i / length);*/

					signal = FFT.Inverse(stretched, phasesStretched);
					//signal = FFT.Inverse(ss._s[ns], phases);

/*					int length = signal.Length;
					for (int i = 0; i < length; i++)
						signal[i] *= MathF.Sqrt(i);*/

					if (newNs == 160)
						DiskE.WriteToProgramFiles("delme", "csv", TextE.ToCsvString(signal, signalOld), false);

					Phases();

					oldSignalPoint = signalPoint;


					signalPoint = MathE.rnd.Next((int)(ss.Height * (AP._newSampleShift * 0.75f + 0.25f)));

/*					shiftSignalPoint += 5;
					if (shiftSignalPoint > ss.Height)
						shiftSignalPoint = 0;
					signalPoint = shiftSignalPoint;*/

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
				phasesMove += AP._phasesMove;

				float pi2 = 2 * MathF.PI;
				for (int x = 0; x < ss.Height; x++)
					phases[x] = MathF.Sin((x + ss.Height / AP._phasesFrequency * phasesMove) * pi2 / ss.Height * AP._phasesFrequency) * pi2 * AP._phasesHeight;
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

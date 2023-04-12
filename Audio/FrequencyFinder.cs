using Library;
using static Library.Array2;

namespace MusGen
{
	public static class FrequencyFinder
	{
		public static int size = 300;
		public static float[] spectrum;
		public static float[] spectrumClone;
		public static float[] oldSpectrum;
		public static float[] frequencies;
		public static int[] leadIndexes;
		public static float amplitudeOverdrive;
		public static float amplitudeMax;
		public static float amplitudeMaxForWholeTrack;

		public static void ByDFT(ref float[] periods, ref float[] amplitudes, Wav wav, int start, int L, int step, float trashSize, string graficName, ref float adaptiveCeiling)
		{
			if (start + L >= wav.L.Length - 1)
				return;

			float leadFrequency = 0;
			float leadAmplitude = 0;
			int leadIndex = 0;

			leadIndexes = new int[periods.Length];

			spectrum = new float[size];
			frequencies = new float[size];
			spectrumClone = new float[size];

			int index = 0;

			float buffer = 2 * MathF.PI * step / wav.sampleRate;

			for (float frequency = 20f; index < spectrum.Length; frequency *= 1.023f)
			{
				float re = 0;
				float im = 0;

				float fakePoint = 0;
				float fakeStep = buffer * frequency;

				for (int sample = start; sample < start + L; sample += step)
				{
					re += wav.L[sample] * MathF.Cos(fakePoint);
					im += wav.L[sample] * MathF.Sin(fakePoint);

					fakePoint += fakeStep;
				}

				spectrum[index] = 0.5f * MathF.Sqrt(re * re + im * im);
				spectrumClone[index] = spectrum[index];

				frequencies[index] = frequency;
				index++;
			}

			FindLeadFrequency();
			amplitudeMax = leadAmplitude;
			amplitudeOverdrive = MathF.Max(leadAmplitude, 1);
			adaptiveCeiling = Math.Max(adaptiveCeiling, amplitudeMax / amplitudeOverdrive);

			for (int i = 0; i < periods.Count(); i++)
			{
				FindLeadFrequency();

				periods[i] = 1 / leadFrequency;
				amplitudes[i] = leadAmplitude / amplitudeOverdrive;
				leadIndexes[i] = leadIndex;

				RemoveTrash(leadIndex, trashSize);
			}

			void FindLeadFrequency()
			{
				leadFrequency = 0;
				leadAmplitude = 0;
				leadIndex = 0;
				index = 0;				

				for (; index < spectrumClone.Length; index++)
				{
					if (spectrumClone[index] > leadAmplitude)
					{
						leadAmplitude = spectrumClone[index];
						leadFrequency = frequencies[index];
						leadIndex = index;
					}
				}

				leadAmplitude = spectrum[leadIndex];
			}

			void RemoveTrash(int point, float size)
			{
				for (int i = 0; i < spectrumClone.Length; i++)
					spectrumClone[i] = spectrumClone[i] * MathF.Abs(MathF.Tanh((i - point)/size));
			}
		}

		public static void ByFFT(ref float[] frequencies, ref float[] amplitudes, Wav wav, int start, int FFTsize, float trashSize, float[] smoothMask)
		{
			if (start + FFTsize >= wav.L.Length - 1)
				return;

			float leadFrequency = 0;
			float leadAmplitude = 0;			
			int leadIndex = 0;

			leadIndexes = new int[frequencies.Length];

			Complex[] complex = new Complex[FFTsize];
			for (int i = 0; i < FFTsize; i++)
				complex[i] = new Complex(wav.L[start + i], 0);

			complex = FFT.Forward(complex);

			float[] oldSpectrum = spectrum;

			spectrum = new float[FFTsize / 2];
			spectrumClone = new float[FFTsize / 2];

			for (int i = 0; i < FFTsize / 2; i++)
			{
				float newValue = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
				spectrum[i] = oldSpectrum[i] * smoothMask[i] + newValue * (1 - smoothMask[i]);
				spectrumClone[i] = spectrum[i];
			}

			spectrumClone[0] = 0;
			FindLeader();
			amplitudeMax = leadAmplitude;
			amplitudeOverdrive = MathF.Max(leadAmplitude, amplitudeMaxForWholeTrack);

			frequencies[0] = leadFrequency;
			amplitudes[0] = leadAmplitude / amplitudeOverdrive;
			leadIndexes[0] = leadIndex;

			for (int i = 1; i < frequencies.Count(); i++)
			{
				RemoveLeader(leadIndex, trashSize);

				FindLeader();

				frequencies[i] = leadFrequency;
				amplitudes[i] = leadAmplitude / amplitudeOverdrive;

				leadIndexes[i] = leadIndex;
			}

			void FindLeader()
			{
				leadIndex = 0;
				leadFrequency = 0;
				leadAmplitude = 0;

				for (int i = 0; i < spectrumClone.Length; i++)
					if (spectrumClone[i] > leadAmplitude)
					{
						leadAmplitude = spectrumClone[i];
						leadIndex = i;
					}

				leadAmplitude = spectrum[leadIndex]; //test me

				leadFrequency = (1f * leadIndex / FFTsize) * wav.sampleRate;
				//From frequency formula
			}

			void RemoveLeader(int point, float size)
			{
				for (int i = 0; i < spectrumClone.Length; i++)
					spectrumClone[i] = spectrumClone[i] * MathF.Abs(MathF.Tanh((i - point) / size));
			}
		}
	}
}
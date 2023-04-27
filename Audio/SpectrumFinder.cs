using System;
using Extensions;
using static Extensions.ArrayE;

namespace MusGen
{
	public static class SpectrumFinder
	{
		public static float[] _spectrum;
		public static float[] _oldSpectrum;

		public static float[] _frequencies;
		public static float[] _smoothMask;

		public static float[] Find(Wav wav, int start, int FFTsize)
		{
			Complex[] complex;

			FillComplexFromWav();
			complex = FFT.Forward(complex);
			ProcessComplex();
			return _spectrum;


			void FillComplexFromWav()
			{
				complex = new Complex[FFTsize];
				for (int i = 0; i < FFTsize; i++)
					complex[i] = new Complex(wav.L[start + i], 0);
			}

			void ProcessComplex()
			{
				_oldSpectrum = _spectrum;
				_spectrum = new float[FFTsize / 2];

				for (int i = 0; i < FFTsize / 2; i++)
				{
					float newValue = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
					_spectrum[i] = _oldSpectrum[i] * _smoothMask[i] + newValue * (1 - _smoothMask[i]);
				}
			}
		}

		public static void CalculateSmoothMask(float xScale, float yScale, int FFTsize, uint sampleRate)
		{
			_smoothMask = new float[FFTsize];

			for (int x = 0; x < FFTsize; x++)
				_smoothMask[x] = yScale / MathF.Pow(MathF.E, x * sampleRate / (FFTsize * xScale));
		}

		public static void FillFrqs(int fftSize, uint sampleRate)
		{
			_frequencies = new float[fftSize];

			for (int index = 0; index < fftSize; index++)
				_frequencies[index] = (1f * index / fftSize) * sampleRate;
		}
	}
}
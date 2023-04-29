using System;
using Extensions;
using static Extensions.ArrayE;

namespace MusGen
{
	public static class SpectrumFinder
	{
		public static float[] _spectrum;
		public static float[] _spectrumLogarithmic;
		public static float[] _oldSpectrum;

		public static float[] _frequenciesLinear;
		public static float[] _frequenciesLogarithmic;
		public static float[] _smoothMask;

		static SpectrumFinder()
		{
			_spectrum = new float[AP._fftSize / 2];
			_spectrumLogarithmic = new float[AP._fftSize / 2];
			_oldSpectrum = new float[AP._fftSize / 2];
			InitFrequencies();
			InitFrequenciesLogarithmic();
			CalculateSmoothMask();
			Logger.Log("Spectrum finder was initialized.");

			void InitFrequencies()
			{
				_frequenciesLinear = new float[AP._fftSize / 2];

				for (int index = 0; index < AP._fftSize / 2; index++)
					_frequenciesLinear[index] = (1f * index / AP._fftSize) * AP._sampleRate;
			}

			void InitFrequenciesLogarithmic()
			{
				_frequenciesLogarithmic = ArrayE.RescaleArrayToLog(_frequenciesLinear, AP._fftSize, AP._fftSize / 2);
			}

			void CalculateSmoothMask()
			{
				_smoothMask = new float[AP._fftSize];

				for (int x = 0; x < AP._fftSize; x++)
					_smoothMask[x] = AP._smoothYScale / MathF.Pow(MathF.E, x * AP._sampleRate / (AP._fftSize * AP._smoothXScale));
			}
		}

		public static float[] Find(Wav wav, int start)
		{
			Complex[] complex;

			FillComplexFromWav();
			complex = FFT.Forward(complex);
			ProcessComplex();
			return _spectrum;


			void FillComplexFromWav()
			{
				complex = new Complex[AP._fftSize];
				for (int i = 0; i < AP._fftSize; i++)
				{
					float wf = WindowFunction.F(i);
					complex[i] = new Complex(wav.L[start + i] * wf, 0);
				}
			}

			void ProcessComplex()
			{
				_oldSpectrum = _spectrum;
				_spectrum = new float[AP._fftSize / 2];

				for (int i = 1; i < AP._fftSize / 2; i++)
				{
					float newValue = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
					_spectrum[i] = _oldSpectrum[i] * _smoothMask[i] + newValue * (1 - _smoothMask[i]);
				}
			}
		}

		public static float[] Logarithmise()
		{
			_spectrumLogarithmic = ArrayE.RescaleArrayToLog(_spectrum, AP._fftSize, AP._fftSize / 2);
			return _spectrumLogarithmic;
		}
	}
}
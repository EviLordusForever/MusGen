using System;
using Extensions;
using static Extensions.ArrayE;

namespace MusGen
{
	public static class SpectrumFinder
	{
		private static int _fftSize;
		public static float[] _spectrum;
		public static float[] _spectrumLogarithmic;
		public static float[] _oldSpectrum;

		public static float[] _frequenciesLinear;
		public static float[] _frequenciesLogarithmic;
		public static float[] _smoothMask;

		public static void Init(int fftSize, uint sampleRate, float smoothXScale, float smoothYScale)
		{
			_fftSize = fftSize;
			_spectrum = new float[fftSize / 2];
			_spectrumLogarithmic = new float[fftSize / 2];
			_oldSpectrum = new float[fftSize / 2];
			InitFrequencies();
			InitFrequenciesLogarithmic();
			CalculateSmoothMask();
			Logger.Log("Spectrum finder was initialized.");

			void InitFrequencies()
			{
				_frequenciesLinear = new float[_fftSize / 2];

				for (int index = 0; index < _fftSize / 2; index++)
					_frequenciesLinear[index] = (1f * index / fftSize) * sampleRate;
			}

			void InitFrequenciesLogarithmic()
			{
				_frequenciesLogarithmic = ArrayE.RescaleArrayToLog(_frequenciesLinear, _fftSize, _fftSize / 2);
			}

			void CalculateSmoothMask()
			{
				_smoothMask = new float[fftSize];

				for (int x = 0; x < fftSize; x++)
					_smoothMask[x] = smoothYScale / MathF.Pow(MathF.E, x * sampleRate / (fftSize * smoothXScale));
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
				complex = new Complex[_fftSize];
				for (int i = 0; i < _fftSize; i++)
					complex[i] = new Complex(wav.L[start + i], 0);
			}

			void ProcessComplex()
			{
				_oldSpectrum = _spectrum;
				_spectrum = new float[_fftSize / 2];

				for (int i = 0; i < _fftSize / 2; i++)
				{
					float newValue = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
					_spectrum[i] = _oldSpectrum[i] * _smoothMask[i] + newValue * (1 - _smoothMask[i]);
				}
			}
		}

		public static float[] Logarithmise()
		{
			_spectrumLogarithmic = ArrayE.RescaleArrayToLog(_spectrum, _fftSize, _fftSize / 2);
			return _spectrumLogarithmic;
		}
	}
}
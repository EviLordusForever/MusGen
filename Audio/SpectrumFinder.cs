﻿using System;
using Extensions;
using static Extensions.ArrayE;

namespace MusGen
{
	public static class SpectrumFinder
	{
		public static int gg;

		public static float[] _spectrum;
		public static float[] _spectrumLogarithmic;
		public static float[] _oldSpectrum;

		public static float[] _spectrumLow;
		public static float[] _spectrumLowLogarithmic;
		public static float[] _oldSpectrumLow;

		public static float[] _frequenciesLinear;
		public static float[] _frequenciesLogarithmic;
		public static float[] _smoothMask;

		private static int _spectrumLowEndIndex;
		private static float[] _fadeLowMask;
		private static float[] _antifadeLowMask;

		static SpectrumFinder()
		{
			_spectrum = new float[AP._fftSize / 2];
			_spectrumLogarithmic = new float[AP._fftSize / 2];
			_oldSpectrum = new float[AP._fftSize / 2];

			_spectrumLow = new float[AP._fftSize / 2];
			_spectrumLowLogarithmic = new float[AP._fftSize / 2];
			_oldSpectrumLow = new float[AP._fftSize / 2];

			InitFrequencies();
			InitFrequenciesLogarithmic();
			CalculateSmoothMask();
			FillFadeLowMask();
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
					_smoothMask[x] =  AP._smoothYScale / MathF.Pow(MathF.E, x * AP._sampleRate / (AP._fftSize * AP._smoothXScale));
			}

			void FillFadeLowMask()
			{
				float lastIndex = AP._fftSize / 2f / AP._lc;
				int L0 = AP._fftSize;
				int L = AP._fftSize / 2;
				_spectrumLowEndIndex = (int)(MathE.ToLogScale(lastIndex / L, L0) * L);

				float start = _spectrumLowEndIndex / 2f;
				float count = _spectrumLowEndIndex - start;

				_fadeLowMask = new float[_spectrumLowEndIndex];
				_antifadeLowMask = new float[_spectrumLowEndIndex];

				for (int i = 0; i < _spectrumLowEndIndex; i++)
				{
					if (i < (int)start)
					{
						_fadeLowMask[i] = 0;
						_antifadeLowMask[i] = 1;
					}
					else
					{
						_fadeLowMask[i] = 1 - MathE.Fade((i - start) / count);
						_antifadeLowMask[i] = 1 - _fadeLowMask[i];
					}
				}
			}
		}

		public static float[] Find(Wav wav, int start)
		{
			Complex[] complex;

			FillComplexFromWav();
			complex = FFT.Forward(complex);
			ProcessComplex();

			FillComplexFromWavLow();
			complex = FFT.Forward(complex);
			ProcessComplexLow();

			return _spectrum;


			void FillComplexFromWav()
			{
				complex = new Complex[AP._fftSize];
				int offset = AP._fftSize * (AP._lc - 1);
				for (int i = 0; i < AP._fftSize; i++)
				{
					float wf = WindowFunction.F(i);					
					complex[i] = new Complex(wav.L[start + offset + i] * wf, 0);
				}
			}

			void FillComplexFromWavLow()
			{
				complex = new Complex[AP._fftSize];
				complex[0] = new Complex(0, 0);
				for (int i = 1; i < AP._fftSize; i++)
				{
					float wf = WindowFunction.F((int)(i));

					float v = wav.L[start + i * AP._lc];

					complex[i] = new Complex(v * wf, 0);
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

			void ProcessComplexLow()
			{
				_oldSpectrumLow = _spectrumLow;
				_spectrumLow = new float[AP._fftSize / 2];

				for (int i = 1; i < AP._fftSize / 2; i++)
				{
					float newValue = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
					_spectrumLow[i] = _oldSpectrumLow[i] * _smoothMask[i] + newValue * (1 - _smoothMask[i]);
				}
			}
		}

		public static void Logarithmise()
		{
			_spectrumLogarithmic = ArrayE.RescaleArrayToLog(_spectrum, AP._fftSize, AP._fftSize / 2);

			_spectrumLowLogarithmic = ArrayE.RescaleArrayToLog(_spectrumLow, AP._fftSize / AP._lc, (int)_spectrumLowEndIndex);

			for (int i = 0; i < _spectrumLowLogarithmic.Length; i++)
				_spectrumLogarithmic[i] = _spectrumLogarithmic[i] * _fadeLowMask[i] + _spectrumLowLogarithmic[i] * _antifadeLowMask[i];
		}
	}
}
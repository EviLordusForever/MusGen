﻿using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Extensions;

namespace MusGen
{
	public static class SpectrumFinder
	{
		public static float[] _spectrumLinear;
		public static float[] _spectrumLogarithmic;

		public static float[] _spectrumLowLinear;
		public static float[] _spectrumLowLogarithmic;

		public static float[] _frequenciesLinear;
		public static float[] _frequenciesLogarithmic;

		public static float[] _frequenciesLowLinear;
		public static float[] _frequenciesLowLogarithmic;

		public static float[] _octaves;
		public static float[] _octavesIndexes;

		private static int _spectrumLowEndIndex;
		private static float[] _fadeLowMask;
		private static float[] _antifadeLowMask;

		static SpectrumFinder()
		{	
		}	

		public static void Init()
		{
			_spectrumLinear = new float[AP._fftSize / 2];
			_spectrumLogarithmic = new float[AP._fftSize / 2];

			_spectrumLowLinear = new float[AP._fftSize / 2];
			_spectrumLowLogarithmic = new float[AP._fftSize / 2];

			FindFrequencies();
			SetOctaves();
			FindLowIndex();
			FillFadeLowMask();
			Logger.Log("Spectrum Finder was initialized.");

			void FindLowIndex()
			{
				float lastIndex = AP._fftSize / 2f / AP._lc;
				int L0 = AP._fftSize;
				int L = AP._fftSize / 2;
				_spectrumLowEndIndex = (int)(MathE.ToLogScale(lastIndex / L, L0) * L) - 1;
				_spectrumLowEndIndex -= 1;
			}

			void FillFadeLowMask()
			{
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

			void FindFrequencies()
			{
				_frequenciesLinear = new float[AP._fftSize / 2];
				_frequenciesLowLinear = new float[AP._fftSize / 2];

				for (int index = 0; index < AP._fftSize / 2; index++)
					_frequenciesLinear[index] = (1f * index / AP._fftSize) * AP.SampleRate;

				for (int index = 0; index < AP._fftSize / 2; index++)
					_frequenciesLowLinear[index] = (1f * index / AP._fftSize) * AP.SampleRate / AP._lc;

				_frequenciesLogarithmic = ArrayE.RescaleArrayToLog(_frequenciesLinear, AP._fftSize, AP._fftSize / 2);
				_frequenciesLowLogarithmic = ArrayE.RescaleArrayToLog(_frequenciesLowLinear, AP._fftSize / AP._lc, (int)_spectrumLowEndIndex);

				DiskE.WriteToProgramFiles("frqs", "csv", TextE.ToCsvString(_frequenciesLinear, _frequenciesLogarithmic, _frequenciesLowLinear, _frequenciesLowLogarithmic), false);

				for (int i = 0; i < _frequenciesLowLogarithmic.Length; i++)
					_frequenciesLogarithmic[i] = _frequenciesLowLogarithmic[i];
			}

			void SetOctaves()
			{
				_octaves = new float[10];
				_octavesIndexes = new float[10];

				float[] b = new float[11];
				float[] c = new float[12];

				b[0] = 30.87f;
				c[1] = 32.70f;
				b[1] = 61.74f;
				c[2] = 65.41f;
				b[2] = 123.47f;
				c[3] = 130.81f;
				b[3] = 246.94f;
				c[4] = 261.63f;
				b[4] = 493.88f;
				c[5] = 523.25f;
				b[5] = 987.77f;
				c[6] = 1046.50f;
				b[6] = 1975.53f;
				c[7] = 2093.00f;
				b[7] = 3951.07f;
				c[8] = 4186.01f;
				b[8] = 7902.13f;
				c[9] = 8372.02f;
				b[9] = 15728f;
				c[10] = c[9] * 2;
				b[10] = b[9] * 2;
				c[11] = c[10] * 2;

				for (int i = 0; i < 10; i++)
					_octaves[i] = (c[i + 1] + b[i]) / 2;

				for (int i = 0; i < 10; i++)
					_octavesIndexes[i] = FindIndex(_octaves[i]);

				float FindIndex(float f)
				{
					int i = 0;
					while (_frequenciesLogarithmic[i] < f)
						i++;

					float less = _frequenciesLogarithmic[i - 1];
					float more = _frequenciesLogarithmic[i];
					float power = MathE.Interpolate(less, more, f);

					float index = i - 1 + power;

					return index;
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

			Logarithmise();
			return _spectrumLogarithmic;

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
				for (int i = 1; i < AP._fftSize / 2; i++)
					_spectrumLinear[i] = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
			}

			void ProcessComplexLow()
			{
				for (int i = 1; i < AP._fftSize / 2; i++)
					_spectrumLowLinear[i] = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
			}

			void Logarithmise()
			{
				_spectrumLogarithmic = ArrayE.RescaleArrayToLog(_spectrumLinear, AP._fftSize, AP._fftSize / 2);

				_spectrumLowLogarithmic = ArrayE.RescaleArrayToLog(_spectrumLowLinear, AP._fftSize / AP._lc, (int)_spectrumLowEndIndex);

				for (int i = 0; i < _spectrumLowLogarithmic.Length; i++)
					_spectrumLogarithmic[i] = _spectrumLogarithmic[i] * _fadeLowMask[i] + _spectrumLowLogarithmic[i] * _antifadeLowMask[i];
			}
		}
	}
}
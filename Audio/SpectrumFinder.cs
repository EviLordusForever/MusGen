using System;
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

		public static float[] _spectrumMixed;

		public static float[] _frequenciesLinear;
		public static float[] _frequenciesLg;

		public static float[] _frequenciesLowLinear;
		public static float[] _frequenciesLowLg;

		public static float[] _octaves;
		public static float[] _octavesIndexes;
		public static float _octaveSize;

		private static int _spectrumLowEndIndex;
		public static float[] _fadeInLowMask;
		public static float[] _fadeOutLowMask;

		public static float[,] _fftRecognitionModel;

		public static float _max;

		public static void Init()
		{
			_spectrumLinear = new float[AP.SpectrumSizeNoGG];
			_spectrumLogarithmic = new float[AP.SpectrumSizeGG];

			_spectrumLowLinear = new float[AP.SpectrumSizeNoGG];
			_spectrumLowLogarithmic = new float[AP.SpectrumSizeGG];

			_spectrumMixed = new float[AP.SpectrumSizeGG];

			FindFrequenciesLinear();
			FindLowIndex();
			FindFrequenciesLogarithmic();
			SetOctaves();
			FillFadeLowMask();
			Logger.Log("Spectrum Finder was initialized.");

			void FindLowIndex()
			{
				float lastIndex = AP.SpectrumSize / AP._lc;
				int L0 = AP.FftSize;
				int L = AP.SpectrumSize;
				_spectrumLowEndIndex = (int)(MathE.ToLogScale(lastIndex / L, L0) * L) - 1;
				//
			}

			void FillFadeLowMask()
			{
				float start = _spectrumLowEndIndex * AP._lcFadeStart;
				float end = _spectrumLowEndIndex * AP._lcFadeEnd;
				float count = end - start;

				_fadeInLowMask = new float[AP.SpectrumSizeGG];
				_fadeOutLowMask = new float[AP.SpectrumSizeGG];

				for (int i = 0; i < AP.SpectrumSizeGG; i++)
				{
					if (i < (int)start)
					{
						_fadeOutLowMask[i] = 1;
						_fadeInLowMask[i] = 0;						
					}
					else if (i < (int)end)
					{
						_fadeOutLowMask[i] = MathE.FadeOut((i - start) / count);
						_fadeInLowMask[i] = 1 - _fadeOutLowMask[i];
					}
					else
					{
						_fadeOutLowMask[i] = 0;
						_fadeInLowMask[i] = 1;					
					}
				}

				DiskE.WriteToProgramFiles("fades", "csv", TextE.ToCsvString(_fadeInLowMask, _fadeOutLowMask), false);
			}

			void FindFrequenciesLinear()
			{
				_frequenciesLinear = new float[AP.SpectrumSize];
				_frequenciesLowLinear = new float[AP.SpectrumSize];

				for (int index = 0; index < _frequenciesLinear.Length; index++)
					_frequenciesLinear[index] = (1f * (index / AP._gg) / AP.FftSize) * AP.SampleRate;

				for (int index = 0; index < _frequenciesLowLinear.Length; index++)
					_frequenciesLowLinear[index] = (1f * (index / AP._gg) / AP.FftSize) * AP.SampleRate / AP._lc;
			}

			void FindFrequenciesLogarithmic()
			{
				_frequenciesLg = ArrayE.RescaleArrayToLog(_frequenciesLinear, AP.FftSize, AP.SpectrumSizeGG, false);
				_frequenciesLowLg = ArrayE.RescaleArrayToLog(_frequenciesLowLinear, AP.FftSize / AP._lc, (int)_spectrumLowEndIndex, false);
				float[] copy = new float[_frequenciesLg.Length];
				_frequenciesLg.CopyTo(copy, 0);				

				for (int i = 0; i < _frequenciesLowLg.Length; i++)
					_frequenciesLg[i] = _frequenciesLowLg[i];

				float[] empty = new float[0];

				DiskE.WriteToProgramFiles("frqs", "csv", TextE.ToCsvString(_frequenciesLinear, copy, _frequenciesLowLinear, _frequenciesLowLg, empty, copy, _frequenciesLg), false);
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

				_octaveSize = (_octavesIndexes[9] - _octavesIndexes[0]) / 9;

				float FindIndex(float f)
				{
					int i = 0;
					while (_frequenciesLg[i] < f)
						i++;

					float less = _frequenciesLg[i - 1];
					float more = _frequenciesLg[i];
					float power = MathE.Interpolate(less, more, f);

					float index = i - 1 + power;

					return index;
				}
			}
		}

		public static float[] Find(float[] signal, float[] signalLow, bool useMiddleSmooth)
		{
			Wav wav = new Wav(signal.Length);
			wav.L = signal;
			Wav wavLow = new Wav(signalLow.Length);
			wavLow.L = signalLow;
			return Find(wav, wavLow, 0, useMiddleSmooth);
		}

		public static float[] FindN(Wav wav, Wav wavLow, int start, int x, int step, bool useMiddleSmooth)
		{
			float[] res = new float[AP.SpectrumSize];

			for (int i = 0; i < x; i++)
			{
				float[] subres = Find(wav, wavLow, start + step * i, useMiddleSmooth);

				for (int j = 0; j < res.Length; j++)
					res[j] += subres[j];
			}

			for (int i = 0; i < res.Length; i++)
				res[i] /= x;

			return res;
		}

		public static float[] Find(Wav wav, Wav wavLow, int start, bool useMiddleSmooth)
		{
			Complex[] complex;

			FillComplexFromWav();
			complex = FFT.Forward(complex);
			ProcessComplex();

			FillComplexFromWavLow();
			complex = FFT.Forward(complex);
			ProcessComplexLow();

			LogarithmiseAndMix();
			return _spectrumMixed;

			void FillComplexFromWav()
			{
				complex = new Complex[AP.FftSize];
				complex[0] = new Complex(0, 0);
				int offset = AP.FftSize * ((AP._lc - 1) / 2);
				for (int i = 1; i < AP.FftSize; i++)
				{
					float wf = WindowFunction.F(i);					
					complex[i] = new Complex(wav.L[start + offset + i] * wf, 0);
				}
			}

			void FillComplexFromWavLow()
			{
				complex = new Complex[AP.FftSize];
				complex[0] = new Complex(0, 0);
				complex[complex.Length - 1] = new Complex(0, 0);

				for (int i = 1; i < AP.FftSize; i++)
				{
					float wf = WindowFunction.F(i);

					int newIndex = start + i * AP._lc; //

					complex[i] = new Complex(wavLow.L[newIndex] * wf, 0);
				}
			}

			void ProcessComplex()
			{
				for (int i = 1; i < AP.FftSize / 2; i++)
					_spectrumLinear[i] = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
			}

			void ProcessComplexLow()
			{
				for (int i = 1; i < AP.FftSize / 2; i++)
					_spectrumLowLinear[i] = (float)(Math.Sqrt(Math.Pow(complex[i].Real, 2) + Math.Pow(complex[i].Imaginary, 2)));
			}

			void LogarithmiseAndMix()
			{
				float[] newSpectrumLg = ArrayE.RescaleArrayToLog(_spectrumLinear, AP.FftSize, AP.SpectrumSizeGG, false);

				if (useMiddleSmooth)
					for (int i = 0; i < newSpectrumLg.Length; i++)
					{
						float frq = _frequenciesLg[i];
						float x = AP._spectrumMiddleSmoother * AP._sps * AP._cs;
						float influence = x / (frq + x);
						_spectrumLogarithmic[i] = _spectrumLogarithmic[i] * influence + newSpectrumLg[i] * (1 - influence);
					}
				else
					_spectrumLogarithmic = newSpectrumLg;

				_spectrumLowLogarithmic = ArrayE.RescaleArrayToLog(_spectrumLowLinear, AP.FftSize / AP._lc, _spectrumLowEndIndex, false);

				_spectrumMixed = new float[_spectrumMixed.Length];

				for (int i = 0; i < _spectrumMixed.Length; i++)
				{
					if (i < _spectrumLowLogarithmic.Length)
						_spectrumMixed[i] = _spectrumLogarithmic[i] * _fadeInLowMask[i] + _spectrumLowLogarithmic[i] * _fadeOutLowMask[i];
					else
						_spectrumMixed[i] = _spectrumLogarithmic[i];

					if (_max < _spectrumMixed[i])
						_max = _spectrumMixed[i];
				}
			}
		}
	}
}
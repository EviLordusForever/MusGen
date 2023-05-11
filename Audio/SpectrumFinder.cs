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

		public static float[] _frequenciesLinear;
		public static float[] _frequenciesLogarithmic;
		public static float[] _logarithmicDeltas;

		public static float[] _frequenciesLowLinear;
		public static float[] _frequenciesLowLogarithmic;

		public static float[] _octaves;
		public static float[] _octavesIndexes;
		public static float _octaveSize;

		private static int _spectrumLowEndIndex;
		public static float[] _fadeInLowMask;
		public static float[] _fadeOutLowMask;

		public static float _max;

		public static void Init()
		{
			_spectrumLinear = new float[AP.FftSize / 2];
			_spectrumLogarithmic = new float[AP.FftSize / 2];

			_spectrumLowLinear = new float[AP.FftSize / 2];
			_spectrumLowLogarithmic = new float[AP.FftSize / 2];

			FindFrequenciesLinear();
			FindLowIndex();
			FindFrequenciesLogarithmic();
			FindLogDeltas();
			SetOctaves();
			FillFadeLowMask();
			Logger.Log("Spectrum Finder was initialized.");

			void FindLowIndex()
			{
				float lastIndex = AP.FftSize / 2f / AP._lc;
				int L0 = AP.FftSize;
				int L = AP.FftSize / 2;
				_spectrumLowEndIndex = (int)(MathE.ToLogScale(lastIndex / L, L0) * L) - 1;
				//
			}

			void FillFadeLowMask()
			{
				float start = _spectrumLowEndIndex / 2f;
				float end = _spectrumLowEndIndex;
				float count = end - start;

				_fadeInLowMask = new float[AP.FftSize / 2];
				_fadeOutLowMask = new float[AP.FftSize / 2];

				for (int i = 0; i < _fadeInLowMask.Length; i++)
				{
					if (i < (int)start)
					{
						_fadeInLowMask[i] = 0;
						_fadeOutLowMask[i] = 1;
					}
					else if (i < (int)end)
					{
						_fadeInLowMask[i] = 1 - MathE.FadeOut((i - start) / count);
						_fadeOutLowMask[i] = 1 - _fadeInLowMask[i];
					}
					else
					{
						_fadeInLowMask[i] = 1;
						_fadeOutLowMask[i] = 0;
					}
				}
			}

			void FindFrequenciesLinear()
			{
				_frequenciesLinear = new float[AP.FftSize / 2];
				_frequenciesLowLinear = new float[AP.FftSize / 2];

				for (int index = 0; index < AP.FftSize / 2; index++)
					_frequenciesLinear[index] = (1f * index / AP.FftSize) * AP.SampleRate;

				for (int index = 0; index < AP.FftSize / 2; index++)
					_frequenciesLowLinear[index] = (1f * index / AP.FftSize) * AP.SampleRate / AP._lc;
			}

			void FindFrequenciesLogarithmic()
			{
				_frequenciesLogarithmic = ArrayE.RescaleArrayToLog(_frequenciesLinear, AP.FftSize, AP.FftSize / 2, false);
				_frequenciesLowLogarithmic = ArrayE.RescaleArrayToLog(_frequenciesLowLinear, AP.FftSize / AP._lc, (int)_spectrumLowEndIndex, false);
				float[] copy = new float[_frequenciesLogarithmic.Length];
				_frequenciesLogarithmic.CopyTo(copy, 0);				

				for (int i = 0; i < _frequenciesLowLogarithmic.Length; i++)
					_frequenciesLogarithmic[i] = _frequenciesLowLogarithmic[i];

				float[] empty = new float[0];

				DiskE.WriteToProgramFiles("frqs", "csv", TextE.ToCsvString(_frequenciesLinear, copy, _frequenciesLowLinear, _frequenciesLowLogarithmic, empty, copy, _frequenciesLogarithmic), false);
			}

			void FindLogDeltas()
			{
				_frequenciesLogarithmic[0] = _frequenciesLogarithmic[1] - (_frequenciesLogarithmic[2] - _frequenciesLogarithmic[1]);

				_logarithmicDeltas = new float[_frequenciesLogarithmic.Length];
				for (int i = 0; i < _logarithmicDeltas.Length; i++)
				{
					int left = Math.Max(0, i - 3);
					int right = Math.Min(_frequenciesLogarithmic.Length - 1, i + 3);
					_logarithmicDeltas[i] = _frequenciesLogarithmic[right] - _frequenciesLogarithmic[left];
					_logarithmicDeltas[i] /= right - left + 1;
				}

				DiskE.WriteToProgramFiles("deltas", "csv", TextE.ToCsvString(_frequenciesLogarithmic, _logarithmicDeltas), false);
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

		public static float[] Find(float[] signal, float[] signalLow)
		{
			Wav wav = new Wav(signal.Length);
			wav.L = signal;
			Wav wavLow = new Wav(signalLow.Length);
			wavLow.L = signalLow;
			return Find(wav, wavLow, 0);
		}

		public static float[] Find(Wav wav, Wav wavLow, int start)
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
				complex = new Complex[AP.FftSize];
				complex[0] = new Complex(0, 0);
				int offset = AP.FftSize * (AP._lc - 1);
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
					float wf = WindowFunction.F((int)(i));

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

			void Logarithmise()
			{
				_spectrumLogarithmic = ArrayE.RescaleArrayToLog(_spectrumLinear, AP.FftSize, AP.FftSize / 2, true);

				_spectrumLowLogarithmic = ArrayE.RescaleArrayToLog(_spectrumLowLinear, AP.FftSize / AP._lc, (int)_spectrumLowEndIndex, true);

				for (int i = 0; i < _spectrumLogarithmic.Length; i++)
				{
					if (i < _spectrumLowLogarithmic.Length)
						_spectrumLogarithmic[i] = _spectrumLogarithmic[i] * _fadeInLowMask[i] + _spectrumLowLogarithmic[i] * _fadeOutLowMask[i];

					if (_max < _spectrumLogarithmic[i])
						_max = _spectrumLogarithmic[i];
				}
			}
		}
	}
}
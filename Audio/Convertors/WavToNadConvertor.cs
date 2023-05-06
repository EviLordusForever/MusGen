using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class WavToNadConvertor
	{
		private static float _lastSample;
		private static double _step;

		public static Nad Make(Wav wav)
		{
			_lastSample = wav.Length;
			Array.Resize(ref wav.L, wav.Length + AP._fftSize * AP._lc);
			_step = wav._sampleRate / AP._sps;
			int samplesCount = (int)Math.Ceiling(_lastSample / _step);
			int duration = (int)Math.Ceiling(1f * wav.Length / wav._sampleRate);

			Nad nad = new Nad(AP._channels, samplesCount, duration);

			ProgressShower.Show("Making nad from wav...");
			int progressStep = nad._samples.Length / 1000;

			int ns = 0;
			for (double s = 0; s < _lastSample; s += _step, ns++)
			{
				nad._samples[ns] = MakeSample(wav, (int)s);

				if (ns > 0)
					nad._samples[ns] = Compare(nad._samples[ns - 1], nad._samples[ns]);

				if (ns % progressStep == 0)
					ProgressShower.Set(1.0 * ns / nad._samples.Length);
			}

			ProgressShower.Close();
			return nad;
		}

		public static NadSample MakeSample(Wav wav, int s)
		{
			NadSample ns = new NadSample(AP._channels);

			SpectrumFinder.Find(wav, s);

			ns._indexes = PeaksFinder.Find(SpectrumFinder._spectrumLogarithmic, AP._channels, AP._peakSize);
			ns._amplitudes = MathE.GetValues(SpectrumFinder._spectrumLogarithmic, ns._indexes);
			ns._frequencies = MathE.GetValues(SpectrumFinder._frequenciesLogarithmic, ns._indexes);

			return ns;
		}

		public static NadSample Compare(NadSample oldNad, NadSample newNad)
		{
			int c = AP._channels;
			int[,] distances = new int[c, c];
			float x = 0;
			float y = 0;

			for (int idNew = 0; idNew < c; idNew++)
				for (int idOld = 0; idOld < c; idOld++)
				{
					x = oldNad._indexes[idOld] - newNad._indexes[idNew];
					x /= (SpectrumFinder._spectrumLinear.Length / 2);
					x *= 4; //check

					y = oldNad._amplitudes[idOld] - newNad._amplitudes[idNew];

					distances[idOld, idNew] = (int)(1000 * Math.Sqrt(x * x + y * y));
				}

			int[] compares = HungarianAlgorithm.Run(distances);

			int[] _indexesCompared = new int[AP._channels];
			float[] _amplitudesCompared = new float[AP._channels];
			float[] _frequenciesCompared = new float[AP._channels];

			for (int id = 0; id < c; id++)
			{
				int idCompared = compares[id];

				_indexesCompared[id] = newNad._indexes[idCompared];
				_frequenciesCompared[id] = newNad._frequencies[idCompared];
				_amplitudesCompared[id] = newNad._amplitudes[idCompared];
			}

			newNad._indexes = _indexesCompared;
			newNad._amplitudes = _amplitudesCompared;
			newNad._frequencies = _frequenciesCompared;

			return newNad;
		}
	}
}

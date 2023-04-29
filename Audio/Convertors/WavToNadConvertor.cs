using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public class WavToNadConvertor
	{
		private int _fftSize;
		private float _nadSamplesPerSecond;
		private int _channels;
		private float _peakSize;
		private bool _logarithmic;

		private float _lastSample;
		private float _sampleRate;
		private double _step;

		public WavToNadConvertor(int fftSize, float nadSamplesPerSecond, int channels, float peakSize, bool logarithmic)
		{
			_fftSize = fftSize;
			_nadSamplesPerSecond = nadSamplesPerSecond;
			_channels = channels;
			_peakSize = peakSize;
			_logarithmic = logarithmic;

			Logger.Log("Wav to nad convertor was initialized.");
		}

		public Nad Make(Wav wav)
		{
			_lastSample = wav.Length - _fftSize;
			_sampleRate = wav._sampleRate;
			_step = _sampleRate / _nadSamplesPerSecond;
			int samplesCount = (int)Math.Ceiling(_lastSample / _step);
			int duration = (int)Math.Ceiling(wav.Length / _sampleRate);

			Nad nad = new Nad(_channels, samplesCount, duration);

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

		public NadSample MakeSample(Wav wav, int s)
		{
			NadSample ns = new NadSample(_channels);

			SpectrumFinder.Find(wav, s);
			SpectrumFinder.Logarithmise();

			if (!_logarithmic)
			{
				ns._indexes = PeaksFinder.Find(SpectrumFinder._spectrum, _channels, _peakSize);
				ns._amplitudes = MathE.GetValues(SpectrumFinder._spectrum, ns._indexes);
				ns._frequencies = MathE.GetValues(SpectrumFinder._frequenciesLinear, ns._indexes);
			}
			else
			{
				ns._indexes = PeaksFinder.Find(SpectrumFinder._spectrumLogarithmic, _channels, _peakSize);
				ns._amplitudes = MathE.GetValues(SpectrumFinder._spectrumLogarithmic, ns._indexes);
				ns._frequencies = MathE.GetValues(SpectrumFinder._frequenciesLogarithmic, ns._indexes);
			}

			return ns;
		}

		public NadSample Compare(NadSample oldNad, NadSample newNad)
		{
			int c = _channels;
			int[,] distances = new int[c, c];
			float x = 0;
			float y = 0;

			for (int idNew = 0; idNew < c; idNew++)
				for (int idOld = 0; idOld < c; idOld++)
				{
					x = oldNad._indexes[idOld] - newNad._indexes[idNew];
					x /= (SpectrumFinder._spectrum.Length / 2);
					x *= 4; //check

					y = oldNad._amplitudes[idOld] - newNad._amplitudes[idNew];

					distances[idOld, idNew] = (int)(1000 * Math.Sqrt(x * x + y * y));
				}

			int[] compares = HungarianAlgorithm.Run(distances);

			int[] _indexesCompared = new int[_channels];
			float[] _amplitudesCompared = new float[_channels];
			float[] _frequenciesCompared = new float[_channels];

			for (int id = 0; id < c; id++)
			{
				int idCompared = compares[id];

				_indexesCompared[id] = newNad._indexes[idCompared];
				_frequenciesCompared[id] = newNad._indexes[idCompared];
				_amplitudesCompared[id] = newNad._amplitudes[idCompared];
			}

			newNad._indexes = _indexesCompared;
			newNad._amplitudes = _amplitudesCompared;
			newNad._frequencies = _frequenciesCompared;

			return newNad;
		}
	}
}

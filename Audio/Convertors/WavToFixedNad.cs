using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class WavToFixedNad
	{
		private static float _lastSample;
		private static double _step;

		public static Nad Make(Wav wav)
		{
			SpectrumFinder._max = 0;
			_lastSample = wav.Length;
			Array.Resize(ref wav.L, wav.Length + AP.FftSize * AP._lc);
			_step = wav._sampleRate / AP._sps;
			int samplesCount = (int)Math.Ceiling(_lastSample / _step);
			float duration = MathF.Ceiling(1f * wav.Length / wav._sampleRate);

			Nad nad = new Nad(AP._channels, samplesCount, duration, AP._cs, (ushort)AP.SpectrumSize);

			ProgressShower.Show("Making nad from wav...");
			int progressStep = nad._samples.Length / 1000;

			Wav wavLow = WavLowPass.FillWavLow(wav, AP._kaiserFilterLength_ForProcessing, true);

			int ns = 0;
			for (double s = 0; s < _lastSample; s += _step, ns++)
			{
				nad._samples[ns] = MakeSample(wav, wavLow, (int)s);

				if (ns > 0)
					nad._samples[ns] = Nad.Compare(nad._samples[ns - 1], nad._samples[ns]);

				if (ns % progressStep == 0)
					ProgressShower.Set(1.0 * ns / nad._samples.Length);
			}

			ProgressShower.Close();
			return nad;
		}

		public static NadSample MakeSample(Wav wav, Wav wavLow, int s)
		{
			NadSample ns = new NadSample(AP._channels);

			SpectrumFinder.Find(wav, wavLow, s);

			ns._indexes = PeaksFinding.PeaksFinder.Find_FixedCount(SpectrumFinder._spectrumLogarithmic, AP._channels, AP._peakWidth_ForFixedNad);
			ns._amplitudes = MathE.GetValues(SpectrumFinder._spectrumLogarithmic, ns._indexes);

			return ns;
		}
	}
}

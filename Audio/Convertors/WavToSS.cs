using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class WavToSS
	{
		public static SS Make(Wav wav)
		{
			AP.SampleRate = (uint)wav._sampleRate;
			int _lastSample = wav.Length;
			Array.Resize(ref wav.L, wav.Length + AP.FftSize * AP._lc);
			double _step = 1.0 * AP.SampleRate / AP._sps;
			int samplesCount = (int)Math.Ceiling(_lastSample / _step);
			int duration = (int)Math.Ceiling(1f * wav.Length / AP.SampleRate);

			SS ss = new SS(samplesCount, AP._sps);

			SpectrumFinder._max = 0;

			Wav wavLow = WavLowPass.FillWavLow(wav, AP._kaiserFilterLength_ForProcessing, true);

			ProgressShower.Show("Making ss from wav...");
			int progressStep = (int)MathF.Ceiling(ss._s.Length / 1000f);


			int ns = 0;
			for (double s = 0; s < _lastSample; s += _step, ns++)
			{
				ss._s[ns] = SpectrumFinder.Find(wav, wavLow, (int)s);

				if (ns % progressStep == 0)
					ProgressShower.Set(1.0 * ns / ss.Width);
			}

			ProgressShower.Close();
			ProgressShower.Show("Ss normalisation...");

			float max = SpectrumFinder._max;

			for (int s = 0; s < ss._s.Length; s++)
			{
				for (int c = 0; c < ss._s[0].Length; c++)
					ss._s[s][c] /= max;

				ProgressShower.Set(1.0 * ns / ss._s.Length);
			}

			ProgressShower.Close();

			return ss;
		}
	}
}

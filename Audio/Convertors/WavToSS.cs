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

			EnlargeWav();

			double _step = 1.0 * AP.SampleRate / (AP._sps * AP._cs);
			int samplesCount = (int)Math.Ceiling(_lastSample / _step);
			int duration = (int)Math.Ceiling(1f * wav.Length / AP.SampleRate);

			SS ss = new SS(samplesCount, (ushort)(AP._sps * AP._cs), AP._cs); ///

			SpectrumFinder._max = 0;

			Wav wavLow = WavLowPass.FillWavLow(wav, AP._kaiserFilterLength_ForProcessing, true);

			ProgressShower.Show($"WAV to SS... (sps = {AP._sps}, cs = {AP._cs})");
			int progressStep = (int)MathF.Ceiling(ss._s.Length / 1000f);

			int ns = 0;
			for (double s = 0; s < _lastSample; s += _step, ns++)
			{
				ss._s[ns] = SpectrumFinder.Find(wav, wavLow, (int)s, true);

				if (ns % progressStep == 0)
					ProgressShower.Set(1.0 * ns / ss.Width);
			}

			ProgressShower.Close();
			ProgressShower.Show("SS normalisation...");

			float max = SpectrumFinder._max;

			for (int s = 0; s < ss.Width; s++)
			{
				for (int c = 0; c < ss.Height; c++)
					ss._s[s][c] /= max;

				ProgressShower.Set(1.0 * s / ss.Width);
			}

			ProgressShower.Close();

			return ss;

			void EnlargeWav()
			{
				int n = AP.FftSize * AP._lc;
				float[] newArray = new float[wav.Length + 2 * n];
				Array.Copy(wav.L, 0, newArray, n, wav.L.Length);
				wav.L = newArray;
			}
		}
	}
}

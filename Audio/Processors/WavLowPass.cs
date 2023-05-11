using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace MusGen
{
	public static class WavLowPass
	{
		public static Wav FillWavLow(Wav wav, int filterLength)
		{
			Wav wavLow = new Wav(wav.Length);
			wav.L.CopyTo(wavLow.L, 0);

			float cutOff = 0.5f * (wav._sampleRate / AP._lc);

			wavLow.L = KaiserFilter.Make(wav.L, wav._sampleRate, cutOff, filterLength, AP._kaiserFilterBeta);

			return wavLow;
		}
	}
}

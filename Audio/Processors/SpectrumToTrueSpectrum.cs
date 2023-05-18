using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SpectrumToTrueSpectrum
	{
		public static float[] Make(float[] spectrum)
		{
			return PeaksFinding.PeaksFinder.ProcessSpectrum(spectrum);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class NadSample
	{
		public float[] frequencies;
		public float[] amplitudes;

		public NadSample(int channelsCount)
		{
			frequencies = new float[channelsCount];
			amplitudes = new float[channelsCount];
		}
	}
}

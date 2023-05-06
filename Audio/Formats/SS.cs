using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen.Audio.Formats
{
	public class SS
	{
		public float[][] _s;
		public int _sps;

		public SS(int count, int spectrumsPerSecond)
		{
			_sps = spectrumsPerSecond;
			_s = new float[count][];
		}
	}
}

using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class SS
	{
		public float[][] _s;
		public ushort _sps;
		public ushort _cs;

		public SS(int count, ushort sps, ushort cs)
		{
			_sps = sps;
			_s = new float[count][];
			_cs = cs;
		}

		public string ToCsv()
		{
			return TextE.ToCsv(_s);
		}

		public int Width
		{
			get
			{
				return _s.Length;
			}
		}

		public int Height
		{
			get
			{
				return _s[0].Length;
			}
		}
	}
}

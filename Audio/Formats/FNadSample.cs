using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	[Serializable]
	public class FNadSample
	{
		public float _index; //in 0-2000
		public float _amplitude; //in 0-2000
		public float _deltaTime; //in ticks / 2000
		public float _duration; //in ticks / 2000

		public float _absoluteTime; //in ticks / 2000
	}
}

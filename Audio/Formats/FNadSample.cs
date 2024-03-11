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
		public int _deltaTime; //in ticks		
		public int _duration; //in ticks
		public float _index; //in 0-1
		public float _amplitude; //in 0-1

		public int _absoluteTime; //in ticks
	}
}

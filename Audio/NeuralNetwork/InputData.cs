using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class InputData
	{
		public float[][][] _data;

		//[q/a][number][values]

		public InputData()
		{
			_data = new float[2][][];
		}

		public float[][] questions
		{
			get
			{
				return _data[0];
			}
			set
			{
				_data[0] = value;
			}
		}

		public float[][] answers
		{
			get
			{
				return _data[1];
			}
			set
			{
				_data[1] = value;
			}
		}
	}
}

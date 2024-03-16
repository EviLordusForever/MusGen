using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class InputDataRNN
	{
		public float[][][] _data;

		//[q/a][testNumber][noteNumber][values]
		//values: 0001000

		public InputDataRNN()
		{
			_data = new float[2][][];
		}

		public int MaxSequenceLength
		{
			get
			{
				return _data[0][0].Length;
			}
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

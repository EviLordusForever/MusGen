using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeaksFinding
{
	public class InputData
	{
		public float[][][] _data;

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

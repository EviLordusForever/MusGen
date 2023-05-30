using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public class SpectrumModel
	{
		public float[,] _model;
		public float[,] _modelN; //normalized
		public double[][] _modelN2; //normalized
		public float[][] _modelN3; //normalized [eq][coef]
		public int[][] _mask;
		public float _max;
		public float[] _maxesForColumns;
		public float[] _curve;
		public int _size;

		public float[] _power;

		//for mask
		//[frequency index][spectrum points]

		//for model
		//[f, s]
		//one column - one coefficient
		//one row - one equation
		//so spectrum is answers and it is vertical
	}
}

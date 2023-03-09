using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFMusGen
{
	public class LeakyReLU : ActivationFunction
	{
		private float _p;

		public override float f(float x)
		{
			if (x > 0)
				return x;
			else
				return x * _p;
		}

		public override float df(float x)
		{
			if (x > 0)
				return 1;
			else
				return _p;
		}

		public LeakyReLU(float p)
		{
			_p = p;
			_type = "LeakyReLU";
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFMusGen
{
	public class Avocado : ActivationFunction
	{
		public override float f(float x)
		{
			if (x > 2)
				return 0.5f + x * 0.25f;
			else if (x > -2)
				return x * 0.5f;
			else
				return -0.5f + x * 0.25f;
		}

		public override float df(float x)
		{
			if (x > 2)
				return 0.25f;
			else if (x > -2)
				return 0.5f;
			else
				return 0.25f;
		}

		public Avocado()
		{
			_type = "Avocado";
		}
	}
}

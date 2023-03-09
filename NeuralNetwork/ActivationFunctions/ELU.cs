using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFMusGen
{
	public class ELU : ActivationFunction
	{
		public override float f(float x)
		{
			if (x > 0)
				return x;
			else
				return MathF.Pow(MathF.E, x) - 1;
		}

		public override float df(float x)
		{
			if (x > 0)
				return 1;
			else
				return MathF.Pow(MathF.E, x);
		}

		public ELU()
		{
			_type = "ELU";
		}
	}
}

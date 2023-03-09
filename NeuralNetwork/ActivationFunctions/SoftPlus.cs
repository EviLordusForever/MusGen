using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFMusGen
{
	public class SoftPlus : ActivationFunction
	{
		public override float f(float x)
		{
			return MathF.Log(1 + MathF.Pow(MathF.E, x));
		}

		public override float df(float x)
		{
			return 1 / (1 + MathF.Pow(MathF.E, -x));
		}

		public SoftPlus()
		{
			_type = "SoftPlus";
		}
	}
}

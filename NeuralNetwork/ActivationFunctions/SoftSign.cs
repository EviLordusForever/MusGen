using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFMusGen
{
	public class SoftSign : ActivationFunction
	{
		public override float f(float x)
		{
			return x / (1 + MathF.Abs(x));
		}

		public override float df(float x)
		{
			return 1 / (2 * MathF.Abs(x) + x * x + 1);
		}

		public SoftSign()
		{
			_type = "SoftSign";
		}
	}
}

namespace ELFMusGen
{
	public class ReLU : ActivationFunction
	{
		public override float f(float x)
		{
			if (x > 0)
				return x;
			else
				return 0;
		}

		public override float df(float x)
		{
			if (x > 0)
				return 1;
			else
				return 0;
		}

		public ReLU()
		{
			_type = "ReLU";
		}
	}
}

namespace ELFMusGen
{
	public class Sigmoid : ActivationFunction
	{
		public override float f(float x)
		{
			return 1f / (1 + MathF.Pow(MathF.E, -x));
		}

		public override float df(float x)
		{
			float epx = MathF.Pow(MathF.E, -x);
			//return epx / (1 + epx)^2
			return epx / ((epx * epx + epx * 2 + 1));
		}

		public Sigmoid()
		{
			_type = "Sigmoid";
		}
	}
}

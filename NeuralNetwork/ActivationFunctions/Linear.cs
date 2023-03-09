namespace ELFMusGen
{
	public class Linear : ActivationFunction
	{
		public override float f(float x)
		{
			return x;
		}

		public override float df(float x)
		{
			return 1;
		}

		public Linear()
		{
			_type = "Linear";
		}
	}
}

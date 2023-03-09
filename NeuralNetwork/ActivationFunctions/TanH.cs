namespace ELFMusGen
{
	public class TanH : ActivationFunction
	{
		public override float f(float x)
		{
			return 2f / (1 + MathF.Pow(1.1f, -x)) - 1;
		}

		public override float df(float x)
		{
			if (MathF.Abs(x) > 900)
				throw new VanishedGradientException();

			float v = 0.19062f * MathF.Pow(1.1f, -x) / MathF.Pow(MathF.Pow(1.1f, -x) + 1, 2);
			return v;
		}

		public TanH()
		{
			_type = "TanH";
		}
	}
}

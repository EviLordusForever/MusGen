namespace ELFMusGen
{
	public abstract class ActivationFunction
	{
		public string _type;

		public abstract float f(float x);

		public abstract float df(float x);
	}
}

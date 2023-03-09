using Newtonsoft.Json;

namespace ELFMusGen
{
	public abstract class Layer
	{
		[JsonIgnore]
		internal NN _ownerNN { get; set; }

		[JsonIgnore]
		public float[][][] _values; //[test][sub][value]

		public ActivationFunction _af;

		public float _dropoutProbability;

		public string _type;

		public abstract void FillWeightsRandomly();

		public abstract void Calculate(int test, float[][] input, bool withDropout);

		public abstract void Calculate(int test, float[] input, bool withDropout);

		public abstract void Dropout();

		public abstract void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights);

		public abstract void FindBPGradient(int test, float desiredValue);

		public abstract void UseMomentumForGradient(int test);

		public abstract void CorrectWeightsByBP(int test, float[][] input);

		public abstract float[][] GetValues(int test);

		public abstract float GetAnswer(int test);

		public abstract float[] AllBPGradients(int test);

		[JsonIgnore]
		public abstract float[][] AllWeights { get; }

		public abstract int WeightsCount { get; }

		public abstract void InitValues(int testsCount);

		public abstract void InitLinksToOwnerNN(NN ownerNN);
	}
}

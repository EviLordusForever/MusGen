using Newtonsoft.Json;
using Library;

namespace ELFMusGen
{
	public class LayerMegatron : Layer
	{
		[JsonIgnore]
		public float[][][] _unnormalizedValues;

		[JsonIgnore]
		public bool[] _dropoutLayer;

		[JsonIgnore]
		public bool[] _falseLayer;

		public Node[] _subs;
		public int _step;
		public int _weightsPerSubCount;
		public int _outsPerSubCount;
		public int _lastMutatedSub;

		public override void FillWeightsRandomly()
		{
			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub].FillRandomly();
		}

		public override void Calculate(int test, float[][] input, bool withDropout)
		{
			for (int sub = 0; sub < _subs.Length; sub++)
				CalculateOneSub(test, input, sub, withDropout);
		}

		private void CalculateOneSub(int test, float[][] input, int sub, bool withDropout)
		{
			for (int v = 0; v < _values[test][sub].Length; v++)
			{
				if (withDropout)
					_unnormalizedValues[test][sub][v] = _subs[sub].Calculate(test, input[0], v * _step, _dropoutLayer) / (1 - _dropoutProbability);
				else

				_values[test][sub][v] = _af.f(_unnormalizedValues[test][sub][v]);
			}
		}

		public override void Dropout()
		{
			//How better to dropout megatron??
			if (_dropoutProbability > 0)
				for (int i = 0; i < _dropoutLayer.Length; i++)
					_dropoutLayer[i] = Math2.rnd.NextSingle() <= _dropoutProbability;
		}

		public override void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights)
		{
			int gradientsPerSubCount = innerBPGradients.Count() / _subs.Count();
			for (int sub = 0; sub < _subs.Count(); sub++)
				FindBPGradientOneSub(test, sub, Array2.SubArray(innerBPGradients, sub * gradientsPerSubCount, gradientsPerSubCount), Array2.SubArray(innerWeights, sub * _outsPerSubCount, _outsPerSubCount));
		}

		private void FindBPGradientOneSub(int test, int sub, float[] innerBPGradients, float[][] innerWeights)
		{
			float buffer = 0;
			for (int n = 0; n < _outsPerSubCount; n++)
			{
				float gwsumm = Node.FindSummOfBPGradientsPerWeights(innerBPGradients, innerWeights[n]);
				buffer += gwsumm * _af.df(_unnormalizedValues[test][sub][n]);
			}
			//buffer /= valuesPerSubCount;
			_subs[sub]._BPgradient[test] += buffer;
			_subs[sub]._BPgradient[test] = _ownerNN.CutGradient(_subs[sub]._BPgradient[test]);
		}

		public override void UseMomentumForGradient(int test)
		{
			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub].UseInertionForGradient(test);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int sub = 0; sub < _subs.Length; sub++)
				CorrectWeightsByBPOneSub(test, sub, input[0]);
		}

		private void CorrectWeightsByBPOneSub(int test, int sub, float[] input)
		{
			for (int v = 0; v < _values[test][sub].Length; v++)
				_subs[sub].CorrectWeightsByBP(test, input, v * _step);
		}

		public override float[][] GetValues(int test)
		{
			return _values[test];
		}

		public override int WeightsCount
		{
			get
			{
				return _subs.Count() * _subs[0]._weights.Count();
			}
		}

		public LayerMegatron(NN ownerNN, int subsCount, int outsPerSubCount, int weightsPerSubCount, int step, float dropoutProbability, ActivationFunction af)
		{
			_type = "megatron";
			_ownerNN = ownerNN;
			_af = af;
			_step = step;
			_outsPerSubCount = outsPerSubCount;
			_weightsPerSubCount = weightsPerSubCount;
			_dropoutProbability = dropoutProbability;

			int testsCount = ownerNN._testerT._testsCount;

			_subs = new Node[subsCount];
			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub] = new Node(ownerNN, testsCount, weightsPerSubCount);

			InitValues(testsCount);
		}

		public LayerMegatron()
		{
			_type = "megatron";
		}

		public override void InitValues(int testsCount)
		{
			_values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				_values[test] = new float[_subs.Count()][];
				for (int sub = 0; sub < _subs.Count(); sub++)
					_values[test][sub] = new float[_outsPerSubCount];
			}

			_unnormalizedValues = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				_unnormalizedValues[test] = new float[_subs.Count()][];
				for (int sub = 0; sub < _subs.Count(); sub++)
					_unnormalizedValues[test][sub] = new float[_outsPerSubCount];
			}

			for (int s = 0; s < _subs.Count(); s++)
				_subs[s].InitValues(testsCount); //does we need you? // ??? //

			_dropoutLayer = new bool[_weightsPerSubCount];
			_falseLayer = new bool[_weightsPerSubCount];

			for (int f = 0; f < _weightsPerSubCount; f++)
				_falseLayer[f] = false;
		}

		public override void InitLinksToOwnerNN(NN ownerNN)
		{
			_ownerNN = ownerNN;

			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub].SetOwnerNN(ownerNN);
		}

		public override float[][] AllWeights
		{
			get
			{
				throw new NotImplementedException();
				//maybe somewhen it will be implemented...
				//but probably no.
			}
		}

		public override float[] AllBPGradients(int test)
		{
			throw new NotImplementedException();
		}

		public override void Calculate(int test, float[] input, bool withDropout)
		{
			throw new NotImplementedException();
		}

		public override float GetAnswer(int test)
		{
			throw new NotImplementedException();
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			throw new NotImplementedException();
		}
	}
}
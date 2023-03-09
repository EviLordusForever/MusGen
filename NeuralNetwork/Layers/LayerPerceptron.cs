using Library;

namespace ELFMusGen
{
	public class LayerPerceptron : Layer
	{
		public Node[] _nodes;
		public int _lastMutatedNode;

		public override void FillWeightsRandomly()
		{
			for (int i = 0; i < _nodes.Length; i++)
				_nodes[i].FillRandomly();
		}

		public override void Calculate(int test, float[][] input, bool withDropout)
		{
			for (int node = 0; node < _nodes.Length; node++)
				if (withDropout)
					if (!_nodes[node]._droppedOut)
						_values[test][0][node] = _af.f(_nodes[node].Calculate(test, input[0], 0)) / (1 - _dropoutProbability);
					else
						_values[test][0][node] = 0;
				else
					_values[test][0][node] = _af.f(_nodes[node].Calculate(test, input[0], 0));
		}

		public override void Calculate(int test, float[] input, bool withDropout)
		{
			for (int node = 0; node < _nodes.Length; node++)
				if (withDropout)
					if (!_nodes[node]._droppedOut)
						_values[test][0][node] = _af.f(_nodes[node].Calculate(test, input, 0)) / (1 - _dropoutProbability);
					else
						_values[test][0][node] = 0;
				else
					_values[test][0][node] = _af.f(_nodes[node].Calculate(test, input, 0));
		}

		public override void Dropout()
		{
			if (_dropoutProbability > 0)
				for (int n = 0; n < _nodes.Count(); n++)
					_nodes[n].Dropout(_dropoutProbability);
		}

		public override void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights)
		{
			for (int n = 0; n < _nodes.Count(); n++)
				_nodes[n].FindBPGradient(test, _af, innerBPGradients, innerWeights[n]);
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			_nodes[0].FindBPGradient(test, _af, desiredValue);
		}

		public override void UseMomentumForGradient(int test)
		{
			for (int n = 0; n < _nodes.Count(); n++)
				_nodes[n].UseInertionForGradient(test);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int node = 0; node < _nodes.Length; node++)
				_nodes[node].CorrectWeightsByBP(test, input[0], 0);
		}

		public void CorrectWeightsByBP(int test, float[] input)
		{
			for (int node = 0; node < _nodes.Length; node++)
				_nodes[node].CorrectWeightsByBP(test, input, 0);
		}

		public override float[][] GetValues(int test)
		{
			return _values[test];
		}

		public override float GetAnswer(int test)
		{
			return _af.f(_nodes[0]._summ[test]);
		}

		public override int WeightsCount
		{
			get
			{
				return _nodes.Count() * _nodes[0]._weights.Count();
			}
		}

		public override float[][] AllWeights
		{
			get
			{
				float[][] allWeights = new float[_nodes[0]._weights.Length][];
				for (int weight = 0; weight < _nodes[0]._weights.Length; weight++)
				{
					allWeights[weight] = new float[_nodes.Length];
					for (int node = 0; node < _nodes.Length; node++)
						allWeights[weight][node] = _nodes[node]._weights[weight];
				}
				return allWeights;
			}
		}

		public override float[] AllBPGradients(int test)
		{
			float[] BPGradients = new float[_nodes.Length];
			for (int node = 0; node < _nodes.Length; node++)
				BPGradients[node] = _nodes[node]._BPgradient[test];
			return BPGradients;
		}

		public LayerPerceptron(NN ownerNN, int nodesCount, int weightsCount, float dropoutProbability, ActivationFunction af)
		{
			_type = "perceptron";
			_ownerNN = ownerNN;
			_af = af;
			_dropoutProbability = dropoutProbability;

			int testsCount = ownerNN._testerT._testsCount;

			_nodes = new Node[nodesCount];
			for (int i = 0; i < _nodes.Count(); i++)
				_nodes[i] = new Node(ownerNN, testsCount, weightsCount);

			InitValues(testsCount);
		}

		public LayerPerceptron()
		{
			_type = "perceptron";
		}

		public override void InitValues(int testsCount)
		{
			_values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				_values[test] = new float[1][];
				_values[test][0] = new float[_nodes.Count()];
			}

			for (int n = 0; n < _nodes.Count(); n++)
				_nodes[n].InitValues(testsCount);
		}

		public override void InitLinksToOwnerNN(NN ownerNN)
		{
			_ownerNN = ownerNN;

			for (int i = 0; i < _nodes.Count(); i++)
				_nodes[i].SetOwnerNN(ownerNN);
		}
	}
}
using static ELFMusGen.Logger;
using Newtonsoft.Json;
using Library;

namespace ELFMusGen
{
	public class Tester
	{
		public int _testsCount;
		public int _batchSize;

		public int _moveAnswersOverZero;
		public int _moveInputsOverZero;
		public int _graphLoadingType;

		public string _graphPath;
		public string _reason;

		[JsonIgnore] private float[] _originalGraph;
		[JsonIgnore] private float[] _derivativeOfGraph;
		[JsonIgnore] private float[] _horizonGraph;

		[JsonIgnore] public List<int> _availableGraphPoints;
		[JsonIgnore] public List<int> _availableGraphPointsForHorizonGraph;
		[JsonIgnore] public float[][] _tests;
		[JsonIgnore] public float[] _answers;
		[JsonIgnore] public bool[] _batch;
		[JsonIgnore] private bool _filledFullBatch;

		[JsonIgnore] private NN _ownerNN { get; set; }

		private void LoadGraph(string path, string reason)
		{
			LoadOriginalGraph(path, reason);
			FillDerivativeOfGraph();
			FillHorizonOfGraph();
		}

		private void LoadOriginalGraph(string graphFolder, string reason)
		{
			var files = Directory.GetFiles(Disk2._programFiles + graphFolder);
			var graphL = new List<float>();
			_availableGraphPoints = new List<int>();
			_availableGraphPointsForHorizonGraph = new List<int>();

			int g = 0;

			for (int f = 0; f < files.Length; f++)
			{
				string[] lines = File.ReadAllLines(files[f]);

				int l = 0;
				while (l < lines.Length)
				{
					graphL.Add(Convert.ToSingle(lines[l]));

					if (l < lines.Length - _ownerNN._inputWindow - _ownerNN._horizon - 2)
					{
						_availableGraphPoints.Add(g);

						if (l > _ownerNN._horizon)
							_availableGraphPointsForHorizonGraph.Add(g);
					}

					l++; g++;
				}

				Log($"Loaded graph: \"{Text2.StringBeforeLast(Text2.StringAfterLast(files[f], "\\"), ".")}\"");
			}

			_originalGraph = graphL.ToArray();
			Log($"Original (and discrete) graph for {reason} loaded.");
			Log("Also available graph points (x2) are loaded.");
			Log("Graph length: " + _originalGraph.Length + ".");
		}

		private void FillDerivativeOfGraph()
		{
			_derivativeOfGraph = new float[_originalGraph.Length];
			for (int i = 1; i < _originalGraph.Length; i++)
				_derivativeOfGraph[i] = _originalGraph[i] - _originalGraph[i - 1];
			Log("Derivative of graph is filled.");
		}

		private void FillHorizonOfGraph()
		{
			_horizonGraph = new float[_originalGraph.Length];
			for (int i = _ownerNN._horizon; i < _originalGraph.Length; i++)
				_horizonGraph[i] = _originalGraph[i] - _originalGraph[i - _ownerNN._horizon];
			Log("HorizonGraph is filled.");
		}

		public void FillTests(int graficLoadingType)
		{
			_graphLoadingType = graficLoadingType;
			FillTests();
		}

		public void FillTests()
		{
			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			if (_graphLoadingType == 0)
				FillTestsFromOriginalGraph();
			else if (_graphLoadingType == 1)
				FillTestsFromDerivativeGraph();
			else if (_graphLoadingType == 2)
				FillTestsFromHorizonGraph();
			else
				throw new Exception();

			FillFullBatch();
		}

		private void FillTestsFromDerivativeGraph()
		{
			int maximalDelta = _availableGraphPoints.Count();
			double deltaOfDelta = 0.990 * maximalDelta / _testsCount;

			int test = 0;
			for (double delta = 0; delta < maximalDelta && test < _testsCount; delta += deltaOfDelta, test++)
			{
				int offset = _availableGraphPoints[Convert.ToInt32(delta)];

				_tests[test] = Array2.SubArray(_derivativeOfGraph, offset, _ownerNN._inputWindow);

				float standartDeviation = Math2.FindStandartDeviation(_tests[test]);
				_tests[test] = Normalize(_tests[test], standartDeviation, _ownerNN._inputAF, _moveInputsOverZero);

				float[] ar = Array2.SubArray(_derivativeOfGraph, offset + _ownerNN._inputWindow, _ownerNN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = _ownerNN._answersAF.f(_answers[test] / standartDeviation) + _moveAnswersOverZero;
			}

			Log($"Tests and answers for NN were filled and normalized from DERIVATIVE graph. ({_tests.Length})");
		}

		private void FillTestsFromOriginalGraph()
		{
			int maximalDelta = _availableGraphPoints.Count();
			double deltaOfDelta = 0.990 * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (double delta = 0; delta < maximalDelta && test < _testsCount; delta += deltaOfDelta, test++)
			{
				int offset = _availableGraphPoints[Convert.ToInt32(delta)];

				_tests[test] = Array2.SubArray(_originalGraph, offset, _ownerNN._inputWindow);

				float final = _tests[test].Last();

				for (int i = 0; i < _tests[test].Length; i++)
					_tests[test][i] = _tests[test][i] - final;

				float standartDeviation = Math2.FindStandartDeviation(_tests[test]);

				Normalize(_tests[test], standartDeviation, _ownerNN._inputAF, _moveInputsOverZero);

				float[] ar = Array2.SubArray(_derivativeOfGraph, offset + _ownerNN._inputWindow, _ownerNN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = _ownerNN._answersAF.f(_answers[test] / standartDeviation) + _moveAnswersOverZero;
			}

			Log($"Tests and answers for NN were filled and normalized from ORIGINAL graph. ({_tests.Length})");
		}

		private void FillTestsFromHorizonGraph()
		{
			int maximalDelta = _availableGraphPointsForHorizonGraph.Count();
			double DeltaOfDelta = 0.990 * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (double delta = 0; delta < maximalDelta && test < _testsCount; delta += DeltaOfDelta, test++)
			{
				int offset = _availableGraphPointsForHorizonGraph[Convert.ToInt32(delta)];

				_tests[test] = Array2.SubArray(_horizonGraph, offset, _ownerNN._inputWindow);
				float standartDeviation = Math2.FindStandartDeviation(_tests[test]);
				_tests[test] = Normalize(_tests[test], standartDeviation, _ownerNN._inputAF, _moveInputsOverZero);

				_answers[test] = _horizonGraph[offset + _ownerNN._inputWindow + _ownerNN._horizon];
				_answers[test] = _ownerNN._answersAF.f(_answers[test] / standartDeviation) + _moveAnswersOverZero;
			}

			Log($"Tests and answers for NN were filled and normalized from HORIZON(!!!) graph. ({_tests.Length})");
		}

		public static float[] Normalize(float[] input, float standartDeviation, ActivationFunction af, float move)
		{
			for (int i = 0; i < input.Length; i++)
				input[i] = af.f(input[i] / standartDeviation) + move;

			return input;
		}

		public void FillBatch()
		{
			FillBatchBy(_batchSize);
		}

		public void FillBatchBy(int count)
		{
			if (count == _testsCount)
				FillFullBatch();
			else
			{
				_batch = new bool[_testsCount];

				int i = 0;
				while (i < count)
				{
					int n = Math2.rnd.Next(_testsCount);
					if (!_batch[n])
					{
						_batch[n] = true;
						i++;
					}
				}

				Log($"Batch refilled ({count})");
				_filledFullBatch = false;
			}
		}

		public void FillFullBatch()
		{
			if (!_filledFullBatch || _batch.Length != _testsCount)
			{
				_batch = new bool[_testsCount];
				for (int i = 0; i < _testsCount; i++)
					_batch[i] = true;

				Log($"Filled full batch ({_testsCount})");
				_filledFullBatch = true;
			}
		}

		[JsonIgnore]
		public float[] OriginalGraph
		{
			get
			{
				return _originalGraph;	
			}
		}

		public void Init(NN ownerNN, string graphPath, string reason)
		{
			_ownerNN = ownerNN;

			if (graphPath != null)
			{
				LoadGraph(graphPath, reason);
				FillTests();
			}
		}

		public Tester(NN ownerNN, int testsCount, string graphPath, string reason, int graphLoadingType, int moveInputsOverZero, int moveAnswersOverZero)
		{
			_testsCount = testsCount;
			_batchSize = testsCount;

			_moveAnswersOverZero = moveAnswersOverZero;
			_moveInputsOverZero = moveInputsOverZero;
			_graphLoadingType = graphLoadingType;

			_graphPath = graphPath;
			_reason = reason;

			Init(ownerNN, _graphPath, _reason);
		}
	}
}
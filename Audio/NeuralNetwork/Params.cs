using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class Params
	{
		public static string _model1Path = $"{DiskE._programFiles}NNModel1";
		public static string _model2Path = $"{DiskE._programFiles}NNModel2";
		public static string _rnn1Path = $"{DiskE._programFiles}RNNModel1";
		public static string _transformerPath = $"{DiskE._programFiles}Transformer";
		public static string _hugeModelPath = $"{DiskE._programFiles}HugeModel";
		public static int _testsCount = -1;
		public static int _batchSize = 3000;
		public static float _minAverageDeltaTime = 0.85f;

		public static int _savingEvery = 30;

		public static float _accordMaxTime = 0.05f;
		public static float _accordThreshold = 0.75f;
	}
}

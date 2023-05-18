using Extensions;

namespace PeaksFinding
{
	public static class Params
	{
		public static string _modelPath = $"{DiskE._programFiles}PeaksFindingModel";
		public static int _batchSize = 3000;
		public static int _epochs = 1000;
		public static int _savingEvery = 10;
		public static int _testsCount = 30000;

		public static int _maxSinusoidsCount = 100;
		public static float _sinusoidsCountP = 0.1f;
	}
}

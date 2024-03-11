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
		public static string _modelPath = $"{DiskE._programFiles}TheModel";
		public static int _testsCount = 30000;
		public static int _batchSize = 3000;

		public static int _savingEvery = 30;
	}
}

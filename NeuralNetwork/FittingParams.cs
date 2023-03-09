using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFMusGen
{
	public class FittingParams
	{
		public float _LEARINING_RATE;
		public float _MOMENTUM;
		public int _trainingTestsCount;
		public int _validationTestsCount;
		public int _batchSize;
		public int _validationRecalculatePeriod;
		public int _statisticsRecalculatePeriod;
		public bool _useDropout;
	}
}

using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeaksFinding
{
	public class OutputData
	{
		[VectorType(512)]
		public float[] amplitudes { get; set; }
	}
}

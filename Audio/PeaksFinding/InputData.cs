using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeaksFinding
{
	public class InputData
	{
		[LoadColumn(0), VectorType(512)]
		public float[] spectrum { get; set; }

		[LoadColumn(1), ColumnName("Label"), VectorType(512)]
		public float[] amplitudes { get; set; }
	}
}

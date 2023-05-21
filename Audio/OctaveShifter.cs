using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusGen
{
	public static class OctaveShifter
	{
		public static float FindShift(SS ss)
		{
			SsSoftOctaveReverser.Init(ss.Width, ss.Height);

			float difference = SpectrumFinder._octavesIndexes[9] - SpectrumFinder._octavesIndexes[0];
			float octaveSize = difference / 9;

			bool[] octaves = new bool[] { false, true, true, true, true, true, true, true, false };

			float[] summ = SuperEqualiser.MakeModel(ss, 0);

			float[] errors = new float[(int)octaveSize];

			float[][] eqs = new float[errors.Length][];

			for (int er = 0; er < errors.Length; er++)
			{
				float shift = -octaveSize / 2 + er;

				float[] reverse = SsSoftOctaveReverser.MakeOne(summ, shift, octaves);

				eqs[er] = new float[summ.Length];

				for (int j = 0; j < summ.Length; j++)
					if (reverse[j] > 0)
						eqs[er][j] = MathF.Abs(summ[j] - reverse[j]);

				for (int j = 0; j < eqs[er].Length; j++)
					errors[er] += MathF.Pow(1 - eqs[er][j], 2);
			}

			float min = errors[0];
			int minIndex = 0;

			for (int i = 0; i < errors.Length; i++)
				if (errors[i] <= min)
				{
					min = errors[i];
					minIndex = i;
				}

			float octaveShift = (int)(-octaveSize / 2 + minIndex);

			Logger.Log($"Best octave shift: {octaveShift} ({octaveShift / octaveSize} of octave)", Brushes.Cyan);
			DiskE.WriteToProgramFiles("OctaveShiftEqs", "csv", TextE.ToCsv(eqs), false);
			DiskE.WriteToProgramFiles("OctaveShiftErrors", "csv", TextE.ToCsvString(errors), false);

			return octaveShift;
		}
	}
}

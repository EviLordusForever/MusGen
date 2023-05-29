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
			return FindShift(SuperEqualiser.MakeModel(ss, 0), ss.Width);
		}

		public static float FindShift(Nad nad)
		{
			return FindShift(SuperEqualiser.MakeModel(nad, 0), nad.Width);
		}

		private static float FindShift(float[] model, int width)
		{
			SsSoftOctaveReverser.Init(width, model.Length);

			float difference = SpectrumFinder._octavesIndexes[9] - SpectrumFinder._octavesIndexes[0];
			float octaveSize = difference / 9;

			bool[] octaves = new bool[] { false, true, true, true, true, true, true, true, false };
			bool[] notOctaves = new bool[] { false, false, false, false, false, false, false, false, false };

			float[] summ = model;

			float[] errors = new float[(int)octaveSize];

			float[][] eqs = new float[errors.Length][];

			for (int er = 0; er < errors.Length; er++)
			{
				float shift = -octaveSize / 2 + er;

				float[] reverse = SsSoftOctaveReverser.MakeOne(summ, shift, octaves, true);
				float[] notReverse = SsSoftOctaveReverser.MakeOne(summ, shift, notOctaves, true);

				eqs[er] = new float[summ.Length];

				/*				for (int j = 0; j < summ.Length; j++)
									eqs[er][j] = MathF.Abs(notReverse[j] - reverse[j]);

								for (int j = 0; j < eqs[er].Length; j++)
									errors[er] += eqs[er][j];*/

				for (int j = 0; j < summ.Length; j++)
					eqs[er][j] = Math.Abs(1 - (1 + notReverse[j]) / (1 + reverse[j]));

				for (int j = 0; j < eqs[er].Length; j++)
					errors[er] += eqs[er][j];
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

			Logger.Log($"Best octave shift: {octaveShift} ({octaveShift / (octaveSize / 2)} of a half-octave)", Brushes.Cyan);
			DiskE.WriteToProgramFiles("OctaveShiftEqs", "csv", TextE.ToCsv(eqs), false);
			DiskE.WriteToProgramFiles("OctaveShiftErrors", "csv", TextE.ToCsvString(errors), false);

			return octaveShift;
		}
	}
}

using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class SuperEqualiser
	{
		private static float[] _modelEx;
		private static float[] _modelTo;
		public static float[] _model;

		public static void MakeModelsOnly(SS ssEx, SS ss)
		{
			_modelEx = MakeModel(ssEx, 1);
			_modelTo = MakeModel(ss, 2);
			_model = new float[ss.Height];

			ProgressShower.Show($"Super equaliser making model part 3");
			float step = (int)Math.Max(1, ss.Height / 500f);

			for (int c = 0; c < ss.Height; c++)
			{
				if (_modelTo[c] != 0)
					_model[c] = _modelEx[c] / _modelTo[c];

				if (c % step == 0)
					ProgressShower.Set(1.0 * c / ss.Height);
			}

			DiskE.WriteToProgramFiles("eqTest", "csv", TextE.ToCsvString(_modelEx, _modelTo, _model), false);

			ProgressShower.Close();
		}

		private static float[] MakeModel(SS ss, int n)
		{
			ProgressShower.Show($"Super equaliser making model part {n}");
			float step = (int)Math.Max(1, ss.Height / 500f);

			float[] model = new float[ss.Height];
			for (int c = 0; c < ss.Height; c++)
			{
				for (int s = 0; s < ss.Width; s++)
					model[c] += ss._s[s][c];

				model[c] /= ss.Width;

				if (c % step == 0)
					ProgressShower.Set(1.0 * c / ss.Height);
			}

			model = ArrayE.SmoothArray(model, ss.Height / 12);

			ProgressShower.Close();

			return model;
		}

		public static Nad Make(Nad nad, SS ssEx, SS ss)
		{
			MakeModelsOnly(ssEx, ss);

			ProgressShower.Show($"Applying super equaliser to NAD...");
			float step = (int)Math.Max(1, nad.Width / 500f);

			for (int s = 0; s < nad.Width; s++)				
			{
				for (int c = 0; c < nad._samples[s].Height; c++)
				{
					int index = nad._samples[s]._indexes[c];
					nad._samples[s]._amplitudes[c] *= _model[index];
				}

				if (s % step == 0)
					ProgressShower.Set(1.0 * s / ss.Height);
			}

			ProgressShower.Close();

			return nad;
		}

		public static SS Make(SS ssEx, SS ss)
		{
			MakeModelsOnly(ssEx, ss);

			ProgressShower.Show($"Applying super equaliser to SS...");
			float step = (int)Math.Max(1, ss.Height / 500f);

			for (int c = 0; c < ss.Height; c++)
			{
				for (int s = 0; s < ss.Width; s++)
					ss._s[s][c] *= _model[c];

				if (c % step == 0)
					ProgressShower.Set(1.0 * c / ss.Height);
			}

			ProgressShower.Close();

			return ss;
		}
	}
}

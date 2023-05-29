using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusGen
{
	public static class OmegaEqualiser
	{
		public static int _sx;
		public static int _sy;

		public static float[][] MakeModel(Nad nad)
		{
			float[][] model = new float[nad.Width][];

			for (int x = 0; x < nad.Width; x++)
			{
				model[x] = new float[nad._specturmSize];

				for (int id = 0; id < nad._samples[x].Height; id++)
				{
					int y = nad._samples[x]._indexes[id];
					model[x][y] = nad._samples[x]._amplitudes[id];
				}
			}

			return Smooth(model, nad.Width, nad._specturmSize);
		}

		public static float[][] MakeModel(SS ss)
		{
			return Smooth(ss._s, ss.Width, ss.Height);
		}

		public static float[][] MakeModel(float[][] a, float[][] b)
		{
			int width = b.Length;
			int height = b[0].Length;

			float summ = 0;
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
				{
					//b[x][y] = b[x][y] >= 0 ? MathF.Sqrt(b[x][y]) : 0;
					//a[x][y] = a[x][y] >= 0 ? MathF.Sqrt(a[x][y]) : 0;

					summ += b[x][y];
				}

			float average = summ / (width * height);
			float d = average / 10;

			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					a[x][y] = (a[x][y] + d) / (b[x][y] + d);

			return a;
		}

		public static float[][] Smooth(float[][] model, int width, int heigh)
		{
			for (int x = 0; x < width; x++)
				model[x] = ArrayE.SmoothArrayCopy(model[x], _sy);

			float[] submodel = new float[heigh];
			float[][] newModel = new float[width][];
			 
			int halfsmooth = _sx / 2;

			for (int x = 0; x < _sx; x++)
				ArrayE.Add(submodel, model[0]);

			for (int x = -halfsmooth; x < width; x++)
			{
				int left = x - halfsmooth;
				int right = x + halfsmooth;
				left = left >= 0 ? left : 0;
				right = right >= 0 ? right : 0;
				right = right < width - 1 ? right : width - 1;

				submodel = ArrayE.Add(submodel, model[right]);
				submodel = ArrayE.Substract(submodel, model[left]);

				if (x >= 0)
					newModel[x] = ArrayE.Divide(submodel, _sx);
			}

			return newModel;
		}

		public static Nad Make(Nad nad, SS ss)
		{
			_sx = (int)((AP._omegaEqSmoothX / nad._duration) * nad.Width);
			_sy = (int)(AP._omegaEqSmoothY * nad._specturmSize);

			Logger.Log($"Started omega equaliser... (sx {_sx}, sy {_sy})");

			float[][] to = MakeModel(nad);
			float[][] from = MakeModel(ss);
			float[][] model = MakeModel(from, to);

			for (int s = 0; s < nad.Width; s++)
				for (int c = 0; c < nad._samples[s].Height; c++)
				{
					int y = nad._samples[s]._indexes[c];
					nad._samples[s]._amplitudes[c] *= model[s][y];
				}

			Logger.Log($"Omega equaliser done.");

			DiskE.WriteToProgramFiles("delme4", "csv", TextE.ToCsv(model), false);

			return nad;
		}
	}
}

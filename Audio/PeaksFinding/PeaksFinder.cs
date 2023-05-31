using System;
using Extensions;
using System.Collections.Generic;
using System.Linq;
using Tensorflow.Keras.Engine;
using Tensorflow;
using MusGen;
using MathNet.Numerics.Statistics;
using Newtonsoft.Json;

namespace PeaksFinding
{
	public static class PeaksFinder
	{
		public static float _average;
		public static float _max;
		public static float _minFromAverage;
		public static float _minFromMaximum;
		public static IModel _model;

		private static float[] fadeLow;
		private static float[] fadeHight;

		static PeaksFinder()
		{
			_model = ModelManager.GetModel();
			FillFades();
		}

		private static void FillFades()
		{
			fadeLow = new float[AP.SpectrumSizeGG];
			fadeHight = new float[AP.SpectrumSizeGG];

			int[] points = ArrayE.Multiply(AP._smootherpoints, AP.SpectrumSizeGG);
			points[points.Length - 1]--; //

			fadeLow = ArrayE.CreateInterpolatedArray(AP.SpectrumSizeGG, points, AP._smoothervalues);

			for (int i = 0; i < fadeLow.Length; i++)
				fadeHight[i] = 1 - fadeLow[i];

			DiskE.WriteToProgramFiles("fades2", "csv", TextE.ToCsvString(fadeLow, fadeHight), false);
		}

		public static ushort[] Find_FixedCount(float[] array, int count, float peakSize)
		{
			float[] array2 = (float[])array.Clone();

		    ushort[] peakIndexes = new ushort[count];
			peakIndexes[0] = FindPeak();
			for (int i = 1; i < count; i++)
			{
				RemovePeak(peakIndexes[i - 1]);
				peakIndexes[i] = FindPeak();
			}
			return peakIndexes;

			ushort FindPeak()
			{
				return MathE.IndexOfMax_short(array2);
			}

			void RemovePeak(int point)
			{
				for (int i = 0; i < array2.Length; i++)
					array2[i] = array2[i] * MathF.Abs(MathF.Tanh((i - point) / peakSize));
			}
		}

		public static List<ushort> FindEvery_By_Solver(float[] array1, out List<float> amps)
		{
			float[] array2 = ArrayE.SmoothArrayCopy(array1, AP._smootherL);
			/////

/*			for (int i = array2.Length / 2; i < array2.Length; i++)
				array2[i] = array1[i];*/

			amps = new List<float>();
			List<ushort> ids;

			ids = MathE.StupiedFilterMask(array2, true);

			for (int i = 0; i < ids.Count; i++)
				amps.Add(array1[ids[i]]);

			int equationsCount = ids.Count();
			int coefficientsCount = ids.Count();

			float[][] eqs = new float[equationsCount][];

			for (int equation = 0; equation < equationsCount; equation++)
			{
				eqs[equation] = new float[coefficientsCount];
				for (int coefficient = 0; coefficient < coefficientsCount; coefficient++)
				{
					int in_spm = ids[equation];
					int in_frq = ids[coefficient];
					eqs[equation][coefficient] = SMM._model._modelN3[in_spm][in_frq];
				}
			}

			float[] b = amps.ToArray();

			var ampsSolved = Solver.Solve(eqs, b, AP._iters, AP._speed);

			for (int i = 0; i < amps.Count; i++)
				amps[i] = ampsSolved[i];

			return ids;
		}

		public static List<ushort> FindEvery_By_Stupied(float[] array1, out List<float> amps)
		{
			amps = new List<float>();

			var arrayCopyH = ArrayE.SmoothArrayCopy(array1, AP._smootherH);
			var arrayCopyL = ArrayE.SmoothArrayCopy(array1, AP._smootherL);

			var arrayMix = ArrayE.MixArrays(arrayCopyL, arrayCopyH, fadeLow, fadeHight);

			List<ushort> peakIndexes = MathE.StupiedFilterMask(arrayMix, true);

			for (int i = 0; i < peakIndexes.Count; i++)
				amps.Add(array1[peakIndexes[i]]);

			return peakIndexes;
		}

		public static List<int> FindEvery_By_Gauss(float[] array1, out List<float> amps)
		{
			List<int> peakIndexes = new List<int>();
			amps = new List<float>();

			float[] array2 = new float[array1.Length];
			for (int i = 0; i < array1.Length; i++)
				array2[i] = array1[i];

			float amp = 0;
			int index = 0;

			_max = array1.Max();
			_average = array1.Average();

			_minFromMaximum = _max * AP._lowestPeak_FromMaximum_Gauss;
			_minFromAverage = _average * AP._lowestPeak_FromAverage_Gauss;

			float minimum = Math.Min(_minFromMaximum, _minFromAverage);

			FindPeak();

			for (int i = 0; i < AP._peaksLimit_Gauss && amp > minimum; i++)
			{
				FindPeak();
				peakIndexes.Add(index);
				amps.Add(amp);
				RemovePeak();
			}

			if (peakIndexes.Count == 0)
			{
				peakIndexes.Add(array1.Length / 2);
				amps.Add(0);
			}

			return peakIndexes;

			void RemovePeak()
			{
				float widthN = AP._peakWidth_ForMultiNad_Gauss;
				widthN *= 1 / SpectrumFinder._frequenciesLg[index];
				float heighN = amp * SpectrumFinder._fadeInLowMask[index];

				float widthL = AP._peakWidth_ForMultiNad_Gauss / AP._lc;
				widthL *= 1 / SpectrumFinder._frequenciesLg[index];
				float heighL = amp * SpectrumFinder._fadeOutLowMask[index];

				for (int x = 0; x < array2.Length; x++)
				{
					array2[x] -= MathE.Gauss(x, index, widthL, heighL);
					array2[x] -= MathE.Gauss(x, index, widthN, heighN);
				}
			}

			void FindPeak()
			{
				amp = 0;
				index = 0;
				for (int i = 0; i < array2.Length; i++)
					if (array2[i] > amp)
					{
						amp = array2[i];
						index = i;
					}
			}
		}

		public static List<int> FindEvery_By_FftRecModel(float[] array1, out List<float> amps)
		{
			List<int> peakIndexes = new List<int>();
			amps = new List<float>();

			float[] array2 = new float[array1.Length];
			for (int i = 0; i < array1.Length; i++)
				array2[i] = array1[i];

			float amp = 0;
			int index = 0;

			_max = array1.Max();
			_average = array1.Average();

			_minFromMaximum = _max * AP._lowestPeak_FromMaximum_FRM;
			_minFromAverage = _average * AP._lowestPeak_FromAverage_FRM;

			float minimum = Math.Min(_minFromMaximum, _minFromAverage);

			FindPeak();

			for (int i = 0; i < AP._peaksLimit_FRM && amp > minimum; i++)
			{
				FindPeak();
				peakIndexes.Add(index);
				amps.Add(amp);
				RemovePeak();
			}

			if (peakIndexes.Count == 0)
			{
				peakIndexes.Add(array1.Length / 2);
				amps.Add(0);
			}

			return peakIndexes;

			void RemovePeak()
			{
				for (int x = 0; x < array2.Length; x++)
					array2[x] -= SMM._model._modelN[x, index] * amp * AP._peakBig;
			}

			void FindPeak()
			{
				amp = 0;
				index = 0;
				for (int i = 0; i < array2.Length; i++)
					if (array2[i] > amp)
					{
						amp = array2[i];
						index = i;
					}
			}
		}

		public static List<int> FindEvery_By_Keras(float[] array, out List<float> amps)
		{
			List<int> ids = new List<int>();
			amps = new List<float>();

			float[] answer = ProcessSpectrum(array);

			float average = answer.Average();

			for (int i = 0; i < AP.SpectrumSize; i++)
				if (answer[i] > average)
				{
					ids.Add(i);
					amps.Add(answer[i]);
				}

			return ids;
		}

		public static List<int> FindEvery_By_SMM(float[] array, out List<float> amps)
		{
			int cycles = 10;
			float[] farray = new float[AP.SpectrumSize];

			for (int cycle = 0; cycle < 10; cycle++)
				for (int si = 0; si < AP.SpectrumSize; si++)
					Optimise(si);

			amps = null;
			return null;

			void Optimise(int si)
			{

			}
		}

		public static float[] ProcessSpectrum(float[] array)
		{
			Shape shape = new Shape(1, array.Length);
			Tensor tensor = constant_op.constant(array, shape: shape);
			tensor.shape = shape;

			Tensor ts = _model.predict(tensor);
			float[] answer = ts.ToArray<float>();

			for (int i = 0; i < answer.Length; i++)
			{
				answer[i] = Math.Min(answer[i], 1);
				answer[i] = Math.Max(answer[i], 0);
			}

			return answer;
		}

		public static float[] ProcessSpectrumCorr(float[] array)
		{
			double[] question = Array.ConvertAll(array, i => (double)i);
			float[] answer = new float[array.Length];

			for (int i = 0; i < array.Length; i++)
				answer[i] = MathF.Max(0, (float)Correlation.Pearson(question, SMM._model._modelN2[i]));
			
			return answer;
			///CHEEEEECK
		}

		public static float[] ProcessSpectrumCorr2(float[] array)
		{
			int count = array.Length;

			float[] answer = new float[count];

			for (int belli = 0; belli < count; belli++)
			{
				float[] difs = new float[count];
				float dif = 0;

				float[] bell = new float[count];
				float[] arrbell = new float[count];
				float arrbellvol = 0;
				float bellvol = 0;

				for (int i = 0; i < count; i++)
				{
					bell[i] = SMM._model._modelN[belli, i];
					arrbell[i] = array[i] * bell[i];

					bellvol += bell[i];
					arrbellvol += arrbell[i];
				}

				float coefficient = bellvol / arrbellvol;

				for (int i = 0; i < count; i++)
				{
					arrbell[i] *= coefficient;

					difs[i] = MathF.Pow(arrbell[i] - bell[i], 2);
					dif += difs[i];
				}

				dif /= bellvol;
				dif = MathF.Pow(dif, 0.5f);
				float cor = 1 - MathF.Min(dif, 1);
				cor = MathF.Pow(cor, 4);
				answer[belli] = cor;
			}

			return answer;
		}

		public static float[] ProcessSpectrumStupied(float[] array)
		{
			array = ArrayE.SmoothArray(array, 5);
			array = MathE.StupiedFilter(array, true);
			return array;
		}

		public static float[] ProcessSpectrumGauss(float[] array)
		{
			float[] res = new float[array.Length];
			List<float> amps = new List<float>();
			List<int> ids = FindEvery_By_Gauss(array, out amps);

			for (int i = 0; i < ids.Count; i++)
				res[ids[i]] = amps[i];

			return res;
		}

		public static float[] ProcessSpectrumSolver(float[] array)
		{
			List<float> amps = new List<float>();
			List<ushort> ids;

			ids = MathE.StupiedFilterMask(array, true);
			//ids = new List<int>();
			//ids.AddRange(ArrayE.OneToN(array.Length - 1));

			for (int i = 0; i < ids.Count; i++)
				amps.Add(array[ids[i]]);

			int equationsCount = ids.Count();
			int coefficientsCount = ids.Count();

			float[][] eqs = new float[equationsCount][];

			for (int equation = 0; equation < equationsCount; equation++)
			{
				eqs[equation] = new float[coefficientsCount];
				for (int coefficient = 0; coefficient < coefficientsCount; coefficient++)
				{
					int in_spm = ids[equation];
					int in_frq = ids[coefficient];
					eqs[equation][coefficient] = SMM._model._modelN3[in_spm][in_frq];
				}
			}

			float[] b = amps.ToArray();

			var ampsSolved = Solver.Solve(eqs, b, AP._iters);

			float[] answer = new float[array.Length];
			for (int i = 0; i < ampsSolved.Length; i++)
				answer[ids[i]] = ampsSolved[i];

			return answer;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Voice;
using Extensions;
using Clr = System.Windows.Media.Color;
using Clr0 = System.Drawing.Color;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MusGen
{
	public static class Tests
	{
		public static void SPL()
		{
			string str = "";
			for (float f = 20; f < 20000; f *= 1.2f)
				str += $"{f};{SoundPressureModel.GetSoundPressureLevel(f)}\n";

			DiskE.WriteToProgramFiles("SPL", "csv", str, false);
		}

		public static void HungarianAlgorithm()
		{
			int[,] costMatrix = new int[,] {
			{  1,  2,  3,  4,  5 },
			{  6,  7,  8,  9, 10 },
			{ 11, 12, 13, 14, 15 },
			{ 16, 17, 18, 19, 20 },
			{ 21, 22, 23, 24, 25 }};

			int[] res = Extensions.HungarianAlgorithm.Run(costMatrix);
			Logger.Log(string.Join(' ', res));
		}

		public static void GradientDithering()
		{
			int size = 1000;

			List<Clr> gradient = new List<Clr>();

			gradient.AddRange(GraphicsE.GetColorGradient(Colors.Black, Colors.White, size));

			WriteableBitmap testBmp = WBMP.Create(size, 255);

			for (int i = 0; i < size; i++)
				testBmp.DrawLine(i, 0, i, 255, gradient[i]);

			DiskE.SaveImagePng(testBmp, $"{DiskE._programFiles}\\Grafics\\GraidentDitheting.bmp");
		}

		public static void GraphDrawerGradient()
		{
			WriteableBitmap testBmp = WBMP.Create(SpectrumDrawer._gradient.Count(), 255);

			for (int i = 0; i < SpectrumDrawer._gradient.Count(); i++)
				testBmp.DrawLine(i, 0, i, 255, SpectrumDrawer._gradient[i]);

			DiskE.SaveImagePng(testBmp, $"{DiskE._programFiles}\\Grafics\\SpectrumGraident.bmp");
		}

		public static void Ceiling()
		{
			Check(59, 120);
			Check(59.9f, 120);
			Check(60, 120);
			Check(61, 120);
			Check(90, 120);
			Check(89, 120);
			Check(91, 120);
			Check(119, 120);
			Check(120, 120.1f);
			Check(119.999f, 120);
			Check(120, 120);

			void Check(float a, float b)
			{
				Logger.Log($"{b} / {a} = {MathF.Ceiling(b / a)}");
			}
		}

		public static void ArrayToLog()
		{
			float[] array1 = new float[1024];
			for (int i = 0; i < 1024; i++)
				array1[i] = i;
			array1 = ArrayE.RescaleArrayToLog(array1, 1024, 2048);


			float[] array2 = new float[1024];
			for (int i = 0; i < 1024; i++)
				array2[i] = i;
			array2 = ArrayE.RescaleArrayToLog(array2, 2, 2048);

			float[] array3 = new float[1024];
			for (int i = 0; i < 1024; i++)
				array3[i] = (i % 32 > 16 ? 1 : -1);
			float[] array3_2 = ArrayE.RescaleArrayToLog(array3, 1024, 2048);

			float[] array4 = new float[1024];
			int m = 1;
			for (int i = 0; i < 1024; i++)
			{
				if (i >= m * 2)
				{
					array4[i] = 1;
					m = i;
				}
			}
			float[] array4_2 = ArrayE.RescaleArrayToLog(array4, 1024, 1024);

			float[] array5 = new float[1024];
			m = 1;
			for (int i = 0; i < 1024; i++)
			{
				if (i >= m * 1.5)
				{
					array5[i] = 1;
					m = i;
				}
			}
			float[] array5_2 = ArrayE.RescaleArrayToLog(array5, 1024, 1024);

			string csv = "";
			for (int i = 0; i < 2048; i++)
			{
				csv += $"{(i < array1.Length ? array1[i] : "")};" +
					$"{(i < array2.Length ? array2[i] : "")};" +
					$"{(i < array3.Length ? array3[i] : "")};" +
					$"{(i < array3_2.Length ? array3_2[i] : "")};" +
					$"{(i < array4.Length ? array4[i] : "")};" +
					$"{(i < array4_2.Length ? array4_2[i] : "")};" +
					$"{(i < array5.Length ? array5[i] : "")};" +
					$"{(i < array5_2.Length ? array5_2[i] : "")}\n";
			}

			DiskE.WriteToProgramFiles("LogArrays", "csv", csv, false);
		}

		public static void SoftOctaveReverser()
		{
			SpectrumFinder.Init();
			SpectrumDrawer.Init();
			SsSoftOctaveReverser.Init(0, AP.FftSize / 2);

			float[] array = ArrayE.OneToNFloat(AP.FftSize / 2);

			for (int i = 0; i < array.Length; i++)
			{
				if (i < array.Length / 3)
					array[i] = MathF.Sin(i / 2f);
				else if (i < array.Length * 2 / 3f)
					array[i] = MathF.Sin(i / 4f);
				else
					array[i] = MathF.Sin(i / 6f);
			}

			array = SsSoftOctaveReverser.MakeOne(array);
			DiskE.WriteToProgramFiles("reverse", "csv", TextE.ToCsvString(array), false);
		}

		public static void FromLogTest()
		{
			float[] array = ArrayE.OneToNFloat(AP.FftSize / 2);

			float[] array2 = ArrayE.RescaleArrayToLog(array, AP.FftSize, AP.FftSize / 2);

			float[] array3 = ArrayE.RescaleArrayFromLog(array2, AP.FftSize, AP.FftSize / 2);

			DiskE.WriteToProgramFiles("FromLogTest", "csv", TextE.ToCsvString(array, array2, array3), false);
		}

		public static void Logging()
		{
			var chars = new char[] { 'g', 'H', 'a', 'I', '2', 's', 'f', '7', '0', ' ' };

			string[] strs = new string[7];

			for (int s = 0; s < strs.Count(); s++)
			{
				strs[s] = "";

				int r = MathE.rnd.Next(500);

				for (int i = 0; i < r; i++)
					strs[s] += chars[MathE.rnd.Next(10)];
			}

			Logger.Log("Logger test started...");

			for (int s = 0; s < strs.Count(); s++)
				Logger.Log(strs[s]);

			Logger.Log("Logger test ended...");
		}
	}
}

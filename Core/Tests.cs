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
			SpectrumDrawer.Init(1, 1, 10);
			WriteableBitmap testBmp = WBMP.Create(SpectrumDrawer.gradient.Count(), 255);

			for (int i = 0; i < SpectrumDrawer.gradient.Count(); i++)
				testBmp.DrawLine(i, 0, i, 255, SpectrumDrawer.gradient[i]);

			DiskE.SaveImagePng(testBmp, $"{DiskE._programFiles}\\Grafics\\SpectrumGraident.bmp");
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

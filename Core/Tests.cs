using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Voice;
using Library;
using Clr = System.Windows.Media.Color;
using Clr0 = System.Drawing.Color;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MusGen
{
	public static class Tests
	{
		public static void TestHungarianAlgorithm()
		{
			int[,] costMatrix = new int[,] {
			{  1,  2,  3,  4,  5 },
			{  6,  7,  8,  9, 10 },
			{ 11, 12, 13, 14, 15 },
			{ 16, 17, 18, 19, 20 },
			{ 21, 22, 23, 24, 25 }};

			int[] res = HungarianAlgorithm.Run(costMatrix);
			Logger.Log(string.Join(' ', res));
		}

		public static void GradientDithering()
		{
			int size = 1000;

			List<Clr> gradient = new List<Clr>();

			gradient.AddRange(Graphics2.GetColorGradientM(Colors.Black, Colors.White, size));

			WriteableBitmap testBmp = WBMP.Create(size, 255);

			for (int i = 0; i < size; i++)
				testBmp.DrawLine(i, 0, i, 255, gradient[i]);

			Disk2.SaveImagePng(testBmp, $"{Disk2._programFiles}\\Grafics\\GraidentDitheting.bmp");
		}

		public static void GraphDrawerGradient()
		{
			GraphDrawer.Init(1, 1, 10);
			WriteableBitmap testBmp = WBMP.Create(GraphDrawer.gradient.Count(), 255);

			for (int i = 0; i < GraphDrawer.gradient.Count(); i++)
				testBmp.DrawLine(i, 0, i, 255, GraphDrawer.gradient[i]);

			Disk2.SaveImagePng(testBmp, $"{Disk2._programFiles}\\Grafics\\SpectrumGraident.bmp");
		}
	}
}

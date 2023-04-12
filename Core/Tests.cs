using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Voice;
using Library;

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

			List<Color> gradient = new List<Color>();

			gradient.AddRange(Graphics2.GetColorGradient(Color.Black, Color.White, size));

			Bitmap testBmp = new Bitmap(size, 255);
			Graphics tgr = Graphics.FromImage(testBmp);
			for (int i = 0; i < size; i++)
				tgr.DrawLine(new Pen(gradient[i]), i, 0, i, 255);

			testBmp.Save($"{Disk2._programFiles}\\Grafics\\GraidentDitheting.bmp");
		}

		public static void GraphDrawerGradient()
		{
			GraphDrawer.Init(1, 1);
			Bitmap testBmp = new Bitmap(GraphDrawer.gradient.Count(), 255);
			Graphics tgr = Graphics.FromImage(testBmp);
			for (int i = 0; i < GraphDrawer.gradient.Count(); i++)
				tgr.DrawLine(new Pen(GraphDrawer.gradient[i]), i, 0, i, 255);

			testBmp.Save($"{Disk2._programFiles}\\Grafics\\SpectrumGraident.bmp");
		}
	}
}

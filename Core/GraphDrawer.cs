using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace MusGen.Core
{
	public static class GraphDrawer
	{
		public static int resX = 1920 / 4;
		public static int resY = 1080 / 4;
		public static Bitmap bmp;
		public static Graphics gr;
		public static List<Color> gradient;

		public static void Draw(string name, float[] input_array)
		{
			Draw(name, input_array, new int[0], new float[0], input_array.Max());
		}

		public static void Draw(string name, float[] input_array, int[] verticalLines, float[] theirSizes, float ceiling)
		{
			float yScale;
			float xScale;
			float powerScale;
			Pen blackPen;

			int yHalf = resY / 2;

			MoveDown();
			Scales();
			DrawPro();
			Pens();
			DrawArray();
			DrawVerticalLines();
			Ending();

			void DrawVerticalLines()
			{
				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					int x = Convert.ToInt32(verticalLines[i] * xScale);
					int y = Convert.ToInt32(theirSizes[i] * yScale);

					Color clr = Graphics2.Rainbow(i * 1f / verticalLines.Length);
					int w = (int)((10f / 1920f) * resX);
					Pen pen = new Pen(clr, w);

					gr.DrawLine(pen, x, yHalf - y, x, yHalf);
				}
			}

			void DrawArray()
			{
				for (int i = 0; i < input_array.Length; i++)
				{
					int y = 0;
					if (input_array[i] > 0)
						y = Convert.ToInt32(input_array[i] * yScale);

					int x = Convert.ToInt32(i * xScale);

					gr.DrawLine(blackPen, x, yHalf - y, x, yHalf);
				}
			}

			void DrawPro()
			{
				int penWidth = Convert.ToInt32(xScale) + 1;

				for (int i = 0; i < input_array.Length; i++)
				{
					Byte power = (byte)(255 * input_array[i] * powerScale);

					int x = Convert.ToInt32(i * xScale);

					Pen pen = new Pen(gradient[power], penWidth);

					gr.DrawLine(pen, x, yHalf + 1, x, yHalf + 2);
				}
			}

			void Scales()
			{
				input_array[0] = 0.0001f;
				yScale = 1f * resY / Math.Max(input_array.Max(), ceiling);
				yScale /= 2;
				powerScale = 1f / theirSizes.Max();
				xScale = 1f * resX / input_array.Length;
			}

			void Pens()
			{
				blackPen = new Pen(Color.Black, Convert.ToInt32(xScale) + 1);
			}

			void MoveDown()
			{
				Rectangle dest = new Rectangle(0, yHalf + 1, resX, yHalf - 1);
				Rectangle from = new Rectangle(0, yHalf + 0, resX, yHalf - 1);
				Bitmap buffer = new Bitmap(resX, yHalf - 1);
				buffer = bmp.Clone(from, bmp.PixelFormat);
				gr.DrawImage(buffer, dest);
				gr.FillRectangle(Brushes.White, 0, 0, resX, yHalf);
			}

			void Ending()
			{
				string path = $"{Disk2._programFiles}Grafics\\g\\g_{name}.jpg";
				//Disk2.SaveImage(bmp, path);
				Graphics2.SaveJPG100(bmp, path); 
				//bmp.Dispose(); //
			}
		}

		public static void Init(int resX, int resY)
		{
			GraphDrawer.resX = resX;
			GraphDrawer.resY = resY;
			bmp = new Bitmap(resX, resY);
			gr = Graphics.FromImage(bmp);
			gr.Clear(Color.Black);

			Color[] clrs = new Color[6];

			clrs[0] = Color.FromArgb(255, 0, 0, 0);
			clrs[1] = Color.FromArgb(255, 75, 0, 255);
			clrs[2] = Color.FromArgb(255, 255, 0, 255);
			clrs[3] = Color.FromArgb(255, 255, 30, 0);
			clrs[4] = Color.FromArgb(255, 255, 195, 0);
			clrs[5] = Color.FromArgb(255, 255, 255, 255);

			int[] sizes = new int[5];

			sizes[0] = 25;
			sizes[1] = 34; //59
			sizes[2] = 61; //120
			sizes[3] = 65; //185
			sizes[4] = 71; //256

			gradient = new List<Color>();

			for (int i = 0; i < 5; i++)
				gradient.AddRange(Graphics2.GetColorGradient(clrs[i], clrs[i + 1], sizes[i]));

			/*Bitmap testBmp = new Bitmap(255, 255);
			Graphics tgr = Graphics.FromImage(testBmp);
			for (int i = 0; i < 255; i++)
				tgr.DrawLine(new Pen(gradient[i]), i, 0, i, 255);

			testBmp.Save($"{Disk2._programFiles}\\Grafics\\graident.bmp");*/
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace MusGen
{
	public static class GraphDrawer
	{
		public static int resX = 1920 / 4;
		public static int resY = 1080 / 4;
		public static Bitmap bmpL1;
		public static Graphics grL1;
		public static Bitmap bmpL2;
		public static Graphics grL2;
		public static Bitmap bmp;
		public static Graphics gr;
		public static List<Color> gradient;
		public static int[] oldXs;
		public static int[] oldYs;

		public static void Draw(string name, float[] input_array)
		{
			Draw(name, input_array, new int[0], new float[0], input_array.Max(), input_array.Max());
		}

		public static void Draw(string name, float[] input_array, int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScale;
			float xScale;
			float powerScale;
			Pen whitePen;

			int yHalf = resY / 2;

			MoveDown();
			grL1.FillRectangle(Brushes.Black, 0, 0, resX, yHalf + 1);
			Scales();
			DrawSpectrum();
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

					grL1.DrawLine(pen, x, yHalf - y, x, yHalf);
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

					grL1.DrawLine(whitePen, x, yHalf - y, x, yHalf);
				}
			}

			void DrawSpectrum()
			{
				int penWidth = Convert.ToInt32(xScale) + 1;

				for (int i = 0; i < input_array.Length; i++)
				{
					int power = Math.Min((int)(2559 * input_array[i] * powerScale), 2559);

					int x = Convert.ToInt32(i * xScale);

					Pen pen = new Pen(gradient[power], penWidth);

					grL1.DrawLine(pen, x, yHalf + 1, x, yHalf + 2);
				}
			}

			void Scales()
			{
				input_array[0] = 0.0001f;
				yScale = 1f * resY / Math.Max(input_array.Max(), adaptiveCeiling);
				yScale /= 2;
				powerScale = 1f / maxCeiling;
				xScale = 1f * resX / input_array.Length;
			}

			void Pens()
			{
				whitePen = new Pen(Color.FromArgb(255, 14, 14, 14), Convert.ToInt32(xScale) + 1);
			}

			void MoveDown()
			{
				Rectangle dest = new Rectangle(0, yHalf + 1, resX, yHalf - 1);
				Rectangle from = new Rectangle(0, yHalf + 0, resX, yHalf - 1);
				Bitmap buffer = new Bitmap(resX, yHalf - 1);
				buffer = bmpL1.Clone(from, bmpL1.PixelFormat);
				grL1.DrawImage(buffer, dest);
			}

			void Ending()
			{
				string path = $"{Disk2._programFiles}Grafics\\g\\g_{name}.jpg";
				//Disk2.SaveImage(bmp, path);
				Graphics2.SaveJPG100(bmpL1, path); 
			}
		}

		public static void DrawType2(string name, int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScaleUp;
			float xScale;
			float yScaleDown;
			Pen whitePen;

			int yHalf = resY / 2;

			MoveDown();
			grL1.FillRectangle(Brushes.Black, 0, 0, resX, yHalf + 1);
			Scales();
			DrawPartUp();
			DrawPartDown();
			Ending();

			void DrawPartDown()
			{
				int penWidth = Convert.ToInt32(xScale) + 1;

				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					int x = Convert.ToInt32(verticalLines[i] * xScale);

					float power0_1 = theirSizes[i];
					//power0_1 = Math2.ToLogScale(power0_1, 10);
					float power0_2559 = (2559 * power0_1);
					int power = Math.Min((int)power0_2559, 2559);
					Color clr = gradient[power];
					//Color clr = Color.FromArgb(power0_255, 255, 255, 255);
					Pen pen = new Pen(clr, penWidth);

					grL1.DrawLine(pen, x, yHalf + 1, x, yHalf + 2);
				}
			}

			void DrawPartUp()
			{
				int penWidth = Convert.ToInt32(xScale) + 1;

				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					float power0_1 = theirSizes[i];
					//power0_1 = Math2.ToLogScale(power0_1, 10);

					int x = Convert.ToInt32(verticalLines[i] * xScale);
					int y = Convert.ToInt32(power0_1 * yScaleUp * 0.95);

					Color clr = Graphics2.Rainbow(i * 1f / verticalLines.Length);
					Pen pen = new Pen(clr, penWidth);

					grL1.DrawLine(pen, x, yHalf / 2 - y, x, yHalf / 2 + y);
				}
			}

			void Scales()
			{
				yScaleUp = 1f * resY / adaptiveCeiling;
				yScaleUp /= 4;
				xScale = 1f * resX / resX;
			}

			void MoveDown()
			{
				int size = yHalf;
				Rectangle dest = new Rectangle(0, yHalf + 1, resX, size);
				Rectangle from = new Rectangle(0, yHalf + 0, resX, size);
				Bitmap buffer = new Bitmap(resX, yHalf - 1);
				buffer = bmpL1.Clone(from, bmpL1.PixelFormat);
				grL1.DrawImage(buffer, dest);
			}

			void Ending()
			{
				string path = $"{Disk2._programFiles}Grafics\\g\\g_{name}.jpg";
				Graphics2.SaveJPG100(bmpL1, path);
			}
		}

		public static void DrawType3(string name, int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScaleUp;
			float xScale;
			float yScaleDown;

			int yHalf = resY / 2;

			gr.Clear(Color.Black);
			grL1.FillRectangle(new SolidBrush(Color.FromArgb(125, 0, 0, 0)), 0, 0, resX, yHalf + 1);
			grL2.Clear(Color.Empty);

			MoveDown();	
			Scales();
			DrawPartUp();
			DrawPartDown();
			Ending();

			void DrawPartDown()
			{
				int penWidth = Convert.ToInt32(xScale) + 1;

				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					int x = Convert.ToInt32(verticalLines[i] * xScale);

					float power0_1 = theirSizes[i];
					power0_1 = Math2.ToLogScale(power0_1, 10);
					float power0_255 = 255 * MathF.Sqrt(power0_1 * yScaleDown);
					int power = Math.Min((int)power0_255, 255);
					Color clr = Color.FromArgb(255, power, power, power);
					Pen pen = new Pen(clr, penWidth);

					grL1.DrawLine(pen, x, yHalf + 1, x, yHalf + 2);
				}
			}

			void DrawPartUp()
			{
				int penWidth = Convert.ToInt32(xScale) + 1;

				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					float power0_1 = theirSizes[i];
					power0_1 = Math2.ToLogScale(power0_1, 10);
					float power0_255 = 255 * power0_1;
					byte power = (byte)Math.Min(power0_255, 255);

					int x = Convert.ToInt32(verticalLines[i] * xScale);
					int y = yHalf - penWidth - Convert.ToInt32(power0_1 * yScaleUp * 0.95);

					Color clr = Graphics2.Rainbow(i * 1f / verticalLines.Length);
					Pen pen = new Pen(clr, penWidth);

					grL1.DrawLine(pen, oldXs[i], oldYs[i], x, y);

					int R = 3;
					grL2.DrawEllipse(pen, x - R, y + 1 - R, 2 * R, 2 * R);

					oldXs[i] = x;
					oldYs[i] = y;
				}
			}

			void Scales()
			{
				yScaleUp = 1f * resY / 1;
				yScaleUp /= 2;
				yScaleDown = 1f / 1;
				xScale = 1f * resX / resX;
			}

			void MoveDown()
			{
				int size = (int)(Math2.rnd.NextSingle() * (yHalf - 1) + 1);
				Rectangle dest = new Rectangle(0, yHalf + 1, resX, size);
				Rectangle from = new Rectangle(0, yHalf, resX, size);
				Bitmap buffer = bmpL1.Clone(from, bmpL1.PixelFormat);
				buffer = (Bitmap)buffer.GetThumbnailImage(resX, yHalf - 1, null, IntPtr.Zero);
				grL1.DrawImage(buffer, dest);
			}

			void Ending()
			{
				gr.DrawImage(bmpL1, 0, 0);
				gr.DrawImage(bmpL2, 0, 0);
				string path = $"{Disk2._programFiles}Grafics\\g\\g_{name}.jpg";
				Graphics2.SaveJPG100(bmp, path);
			}
		}

		public static void Init(int resX, int resY, int channels)
		{
			GraphDrawer.resX = resX;
			GraphDrawer.resY = resY;
			oldXs = new int[channels];
			oldYs = new int[channels];
			for (int i = 0; i < channels; i++)
				oldYs[i] = resY / 2 - 5;
			bmp = new Bitmap(resX, resY);
			gr = Graphics.FromImage(bmp);
			bmpL1 = new Bitmap(resX, resY);
			grL1 = Graphics.FromImage(bmpL1);
			bmpL2 = new Bitmap(resX, resY);
			grL2 = Graphics.FromImage(bmpL2);

			Color[] clrs = new Color[7];
			int[] sizes = new int[6];

			clrs[0] = Color.FromArgb(255, 0, 0, 0);
			clrs[1] = Color.FromArgb(255, 25, 0, 255);
			clrs[2] = Color.FromArgb(255, 215, 0, 255);
			clrs[3] = Color.FromArgb(255, 185, 25, 0);
			clrs[4] = Color.FromArgb(255, 255, 100, 0);
			clrs[5] = Color.FromArgb(255, 255, 255, 0);
			clrs[6] = Color.FromArgb(255, 255, 255, 255);

			sizes[0] = 80; //to blue
			sizes[1] = 120; //to purple 200
			sizes[2] = 300; //to red 500
			sizes[3] = 150; //to orange 650
			sizes[4] = 800; //to yellow 1450
			sizes[5] = 1110; //to white 2560

			gradient = new List<Color>();

			for (int i = 0; i < sizes.Length; i++)
				gradient.AddRange(Graphics2.GetColorGradient(clrs[i], clrs[i + 1], sizes[i]));
		}
	}
}

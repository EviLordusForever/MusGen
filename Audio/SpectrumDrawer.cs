using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using Library;
using Clr = System.Windows.Media.Color;
using static MusGen.HardwareParams;

namespace MusGen
{
	public static class SpectrumDrawer
	{
		public static int resX;
		public static int resY;
		public static int yHalf;
		public static WriteableBitmap wbmpL1;
		public static WriteableBitmap wbmpL2;
		public static WriteableBitmap wbmp;
		public static List<Clr> gradient;
		public static int[] oldXs;
		public static int[] oldYs;

		public static WriteableBitmap DrawType1(float[] input_array, int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScale;
			float xScale;
			float powerScale;

			MoveDown();
			FillBlack();
			Scales();
			DrawSpectrum();
			DrawArray();
			DrawVerticalLines();
			return wbmpL1;

			void FillBlack()
			{
				Graphics2.FillRectangle(wbmpL1, Clr.FromRgb(0, 0, 0), new System.Windows.Int32Rect(0, 0, resX, yHalf + 1));
			}

			void DrawVerticalLines()
			{
				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					int x = Convert.ToInt32(verticalLines[i] * xScale);
					int y = Convert.ToInt32(theirSizes[i] * yScale);

					Clr clr = Graphics2.RainbowM(i * 1f / verticalLines.Length);
					int w = (int)((10f / 1920f) * resX);

					wbmpL1.DrawLineAa(x, yHalf - y, x, yHalf, clr, w);
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

					wbmpL1.DrawLineAa(x, yHalf - y, x, yHalf, Clr.FromRgb(255, 255, 255), 1); //
				}
			}

			void DrawSpectrum()
			{
				int penWidth = Convert.ToInt32(xScale) + 1;

				for (int i = 0; i < input_array.Length; i++)
				{
					int power = Math.Min((int)(2559 * input_array[i] * powerScale), 2559);

					int x = Convert.ToInt32(i * xScale);

					wbmpL1.DrawLineAa(x, yHalf + 1, x, yHalf + 2, gradient[power], penWidth);
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
		}

		public static WriteableBitmap DrawType2(int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScaleUp;
			float xScale;
			float yScaleDown;

			MoveDown();
			FillBlack();
			Scales();
			DrawPartUp();
			DrawPartDown();
			return wbmpL1;

			void FillBlack()
			{
				Graphics2.FillRectangle(wbmpL1, Clr.FromRgb(0, 0, 0), new System.Windows.Int32Rect(0, 0, resX, yHalf + 1));
			}

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
					Clr clr = gradient[power];

					wbmpL1.DrawLineAa(x, yHalf + 1, x, yHalf + 2, clr, penWidth);
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

					Clr clr = Graphics2.RainbowM(i * 1f / verticalLines.Length);

					wbmpL1.DrawLineAa(x, yHalf / 2 - y, x, yHalf / 2 + y, clr, penWidth);
				}
			}

			void Scales()
			{
				yScaleUp = 1f * resY / adaptiveCeiling;
				yScaleUp /= 4;
				xScale = 1f * resX / resX;
			}
		}

		public static WriteableBitmap DrawType3(int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScaleUp;
			float xScale;
			float yScaleDown;

			wbmp.Clear(Clr.FromRgb(0, 0, 0));
			wbmpL1.FillRectangle(0, 0, resX, yHalf + 1, Clr.FromArgb(125, 0, 0, 0));
			wbmp.Clear(Clr.FromArgb(0, 255, 0, 255));

			MoveDown();	
			Scales();
			DrawPartUp();
			DrawPartDown();
			return Ending();

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
					byte power = Math.Min((byte)power0_255, (byte)255);
					Clr clr = Clr.FromArgb(255, power, power, power);

					wbmpL1.DrawLineAa(x, yHalf + 1, x, yHalf + 2, clr, penWidth);
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

					Clr clr = Graphics2.RainbowM(i * 1f / verticalLines.Length);

					wbmpL1.DrawLineAa(oldXs[i], oldYs[i], x, y, clr, penWidth);

					int R = 3;
					wbmpL2.DrawEllipse(x - R, y + 1 - R, 2 * R, 2 * R, clr);

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

			WriteableBitmap Ending()
			{
				wbmp = wbmpL1;
				WBMP.CopyPixels(wbmpL2, wbmp, 0, 0, 0, 0, resX, resY);
				return wbmp;
			}
		}

		public static void MoveDown()
		{
			WBMP.CopyPixels(wbmpL1, wbmpL1, 0, yHalf, 0, yHalf + 1, resX, yHalf - 1);
		}

		public static void Init(int resX, int resY, int channels)
		{
			SpectrumDrawer.resX = resX;
			SpectrumDrawer.resY = resY;
			yHalf = resY / 2;
			oldXs = new int[channels];
			oldYs = new int[channels];
			for (int i = 0; i < channels; i++)
				oldYs[i] = resY / 2 - 5;
			wbmp = WBMP.Create(resX, resY);

			wbmpL1 = WBMP.Create(resX, resY);

			wbmpL2 = WBMP.Create(resX, resY);


			Clr[] clrs = new Clr[7];
			int[] sizes = new int[6];

			clrs[0] = Clr.FromArgb(255, 0, 0, 0);
			clrs[1] = Clr.FromArgb(255, 25, 0, 255);
			clrs[2] = Clr.FromArgb(255, 215, 0, 255);
			clrs[3] = Clr.FromArgb(255, 185, 25, 0);
			clrs[4] = Clr.FromArgb(255, 255, 100, 0);
			clrs[5] = Clr.FromArgb(255, 255, 255, 0);
			clrs[6] = Clr.FromArgb(255, 255, 255, 255);

			sizes[0] = 80; //to blue
			sizes[1] = 120; //to purple 200
			sizes[2] = 300; //to red 500
			sizes[3] = 150; //to orange 650
			sizes[4] = 800; //to yellow 1450
			sizes[5] = 1110; //to white 2560

			gradient = new List<Clr>();

			for (int i = 0; i < sizes.Length; i++)
				gradient.AddRange(Graphics2.GetColorGradientM(clrs[i], clrs[i + 1], sizes[i]));
		}
	}
}

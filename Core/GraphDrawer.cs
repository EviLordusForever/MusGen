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
		public static void Draw(string name, float[] input_array)
		{
			Draw(name, input_array, new int[0], new float[0], input_array.Max());
		}

		public static void Draw(string name, float[] input_array, int[] verticalLines, float[] theirSizes, float ceiling)
		{
			Bitmap bmp;
			Graphics gr;
			double yScale;
			double xScale;
			Pen blackPen;

			BaseStart();
			Scales();
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
					Pen pen = new Pen(clr, 6);

					gr.DrawLine(pen, x, 0, x, y);
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

					gr.DrawLine(blackPen, x, 0, x, y);
				}
			}

			void Scales()
			{
				input_array[0] = 0.0001f;
				yScale = 1080 / Math.Max(input_array.Max(), ceiling);
				xScale = 1920.0 / input_array.Length;
			}

			void Pens()
			{
				blackPen = new Pen(Color.Black, Convert.ToInt32(xScale) + 1);
			}

			void BaseStart()
			{
				bmp = new Bitmap(1920, 1080);
				gr = Graphics.FromImage(bmp);
				gr.Clear(Color.White);
			}

			void Ending()
			{
				Disk2.SaveImage(bmp, Disk2._programFiles + "Grafics\\g\\g_" + name + ".bmp");
				bmp.Dispose(); //
			}
		}
	}
}

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
			float yScale;
			float xScale;
			Pen blackPen;
			int resX = 1920 / 4;
			int resY = 1080 / 4;
			int yHalf = resY / 2;

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
					int w = (int)((16f / 1920f) * resX);
					Pen pen = new Pen(clr, w);

					gr.DrawLine(pen, x, yHalf - y, x, yHalf + y);
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

					gr.DrawLine(blackPen, x, yHalf - y, x, yHalf + y);
				}
			}

			void Scales()
			{
				input_array[0] = 0.0001f;
				yScale = 1f * resY / Math.Max(input_array.Max(), ceiling);
				yScale /= 2;
				xScale = 1f * resX / input_array.Length;
			}

			void Pens()
			{
				blackPen = new Pen(Color.Black, Convert.ToInt32(xScale) + 1);
			}

			void BaseStart()
			{
				bmp = new Bitmap(resX, resY);
				gr = Graphics.FromImage(bmp);
				gr.Clear(Color.White);
			}

			void Ending()
			{
				string path = $"{Disk2._programFiles}Grafics\\g\\g_{name}.jpg";
				//Disk2.SaveImage(bmp, path);
				Graphics2.SaveJPG100(bmp, path); 
				bmp.Dispose(); //
			}
		}
	}
}

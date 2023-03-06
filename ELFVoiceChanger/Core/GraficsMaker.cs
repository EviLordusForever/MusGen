using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.Core
{
	public static class GraficsMaker
	{
		public static void MakeGraficLite(float[] a, float yScale)
		{
			Bitmap bmp = new Bitmap(a.Length, 1080);
			Graphics gr = Graphics.FromImage(bmp);
			gr.Clear(Color.White);
			Pen pen = Pens.Black;

			for (int i = 0; i < a.Length; i++)
				gr.DrawLine(pen, i, 0, i, a[i] * yScale);

			Disk.SaveImage(bmp, Disk.programFiles + "Grafics\\grafic.bmp");
		}

		public static void MakeGraficPlus(string name, float[] a, int redLine1, int redLine2, int greenLine, double limit, double periodShow)
		{
			Bitmap bmp = new Bitmap(1920, 1080);
			Graphics gr = Graphics.FromImage(bmp);
			gr.Clear(Color.White);

			a[0] = 0.0001f;
			double yScale = 1080 / a.Max();
			double xScale = 1920.0 / Math.Max(a.Length, redLine2);

			Pen pen = new Pen(Color.Black, Convert.ToInt32(xScale) + 1);
			Pen redPen = new Pen(Color.FromArgb(255, 0, 0), 4);
			Pen greenPen = new Pen(Color.Green, 7);
			Pen bluePen = new Pen(Color.FromArgb(0, 0, 255), 7);
			int buffer = 0;

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] > 0)
					buffer = Convert.ToInt32(a[i] * yScale);

				gr.DrawLine(pen, Convert.ToInt32(i * xScale), 0, Convert.ToInt32(i * xScale), buffer);
			}

			gr.DrawLine(redPen, Convert.ToInt32(redLine1 * xScale), 0, Convert.ToInt32(redLine1 * xScale), 1080);
			gr.DrawLine(redPen, Convert.ToInt32(redLine2 * xScale), 0, Convert.ToInt32(redLine2 * xScale), 1080);
			
			gr.DrawLine(greenPen, Convert.ToInt32(greenLine * xScale), 0, Convert.ToInt32(greenLine * xScale), 1080);
			gr.DrawLine(bluePen, Convert.ToInt32(periodShow * xScale), 0, Convert.ToInt32(periodShow * xScale), 1080);

			gr.DrawLine(greenPen, 0, Convert.ToInt32(limit * yScale), 1920, Convert.ToInt32(limit * yScale));


			gr.DrawLine(redPen, Convert.ToInt32(1.05 * greenLine / 2 * xScale), 0, Convert.ToInt32(1.05 * greenLine / 2 * xScale), 1080);
			gr.DrawLine(redPen, Convert.ToInt32(0.95 * greenLine / 2 * xScale), 0, Convert.ToInt32(0.95 * greenLine / 2 * xScale), 1080);

			gr.DrawLine(redPen, Convert.ToInt32(1.05 * greenLine / 3 * xScale), 0, Convert.ToInt32(1.05 * greenLine / 3 * xScale), 1080);
			gr.DrawLine(redPen, Convert.ToInt32(0.95 * greenLine / 3 * xScale), 0, Convert.ToInt32(0.95 * greenLine / 3 * xScale), 1080);

			gr.DrawLine(redPen, Convert.ToInt32(1.05 * greenLine / 4 * xScale), 0, Convert.ToInt32(1.05 * greenLine / 4 * xScale), 1080);
			gr.DrawLine(redPen, Convert.ToInt32(0.95 * greenLine / 4 * xScale), 0, Convert.ToInt32(0.95 * greenLine / 4 * xScale), 1080);

			Disk.SaveImage(bmp, Disk.programFiles + "Grafics\\g\\g_" + name + ".bmp");
			bmp.Dispose(); //
		}
	}
}

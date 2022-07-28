using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.Core
{
	public static class GraficsMaker
	{
		public static void MakeGrafic(float[] a, float yScale)
		{
			Bitmap bmp = new Bitmap(a.Length, 1080);
			Graphics gr = Graphics.FromImage(bmp);
			gr.Clear(Color.White);
			Pen pen = Pens.Black;

			for (int i = 0; i < a.Length; i++)
				gr.DrawLine(pen, i, 0, i, a[i] * yScale);

			Disk.SaveImage(bmp, Disk.programFiles + "grafic");
		}

		public static void MakeGraficPlus(string name, float[] a, float yScale, int redLine1, int redLine2, int greenLine)
		{
			Bitmap bmp = new Bitmap(a.Length, 1080);
			Graphics gr = Graphics.FromImage(bmp);
			gr.Clear(Color.White);
			Pen pen = Pens.Black;
			Pen redPen = new Pen(Color.Red, 3);
			Pen greenPen = new Pen(Color.Green, 3);

			for (int i = 0; i < a.Length; i++)
				gr.DrawLine(pen, i, 0, i, a[i] * yScale);

			gr.DrawLine(redPen, redLine1, 0, redLine1, 1080);
			gr.DrawLine(redPen, redLine2, 0, redLine2, 1080);
			gr.DrawLine(redPen, greenLine, 0, greenLine, 1080);

			Disk.SaveImage(bmp, Disk.programFiles + "g_" + name + ".bmp");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace MusGen.Core
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

			Disk2.SaveImage(bmp, Disk2._programFiles + "grafic");
		}
	}
}

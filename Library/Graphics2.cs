using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
	public static class Graphics2
	{
		public static Bitmap RescaleBitmap(Bitmap bmp0, int width, int height)
		{
			Bitmap bmp = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.DrawImage(bmp0, new Rectangle(0, 0, bmp.Width, bmp.Height));
			}
			return bmp;
		}

		public static RotateFlipType RotateFlipTypeRandom
		{
			get
			{
				int n = Math2.rnd.Next(16);
				switch (n)
				{
					case 0:
						return RotateFlipType.Rotate180FlipNone;
					case 1:
						return RotateFlipType.Rotate180FlipX;
					case 2:
						return RotateFlipType.Rotate180FlipXY;
					case 3:
						return RotateFlipType.Rotate180FlipY;

					case 4:
						return RotateFlipType.Rotate270FlipNone;
					case 5:
						return RotateFlipType.Rotate270FlipX;
					case 6:
						return RotateFlipType.Rotate270FlipXY;
					case 7:
						return RotateFlipType.Rotate270FlipY;

					case 8:
						return RotateFlipType.Rotate90FlipNone;
					case 9:
						return RotateFlipType.Rotate90FlipX;
					case 10:
						return RotateFlipType.Rotate90FlipXY;
					case 11:
						return RotateFlipType.Rotate90FlipY;

					case 12:
						return RotateFlipType.RotateNoneFlipNone;
					case 13:
						return RotateFlipType.RotateNoneFlipX;
					case 14:
						return RotateFlipType.RotateNoneFlipXY;
					case 15:
						return RotateFlipType.RotateNoneFlipY;

					default:
						throw new Exception();
				}
			}
		}

		public static Bitmap ToBlackWhite(Bitmap bmp)
		{
			for (int x = 0; x < bmp.Width; x++)
				for (int y = 0; y < bmp.Height; y++)
					bmp.SetPixel(x, y, TBW(bmp.GetPixel(x, y)));
			return bmp;

			Color TBW(Color c)
			{
				byte min = Math.Min(Math.Min(c.R, c.G), c.B);
				return Color.FromArgb(min, min, min);
			}
		}

		public static Bitmap MaximizeContrastAndNegate(Bitmap bmp)
		{
			for (int x = 0; x < bmp.Width; x++)
				for (int y = 0; y < bmp.Height; y++)
					bmp.SetPixel(x, y, TBW(bmp.GetPixel(x, y)));
			return bmp;

			Color TBW(Color c)
			{
				float br = 255 - c.GetBrightness() * 255;
				if (br < 113)
					return Color.Black;
				else
					return Color.White;
			}
		}

		public static Bitmap Negate(Bitmap bmp)
		{
			for (int x = 0; x < bmp.Width; x++)
				for (int y = 0; y < bmp.Height; y++)
					bmp.SetPixel(x, y, Negate(bmp.GetPixel(x, y)));
			return bmp;

			Color Negate(Color c)
			{
				return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
			}
		}

		public static Bitmap TakeScreen2()
		{
			int w = Screen.PrimaryScreen.Bounds.Width;
			int h = Screen.PrimaryScreen.Bounds.Height;
			Bitmap bmp = new Bitmap(w, h);
			Graphics gr = Graphics.FromImage(bmp);
			gr.CopyFromScreen(0, 0, 0, 0, new Size(w, h));
			return bmp;
		}

		public static Bitmap TakeScreen()
		{
			Bitmap bmp = new Bitmap(1, 1);
			SendKeys.SendWait("%{PRTSC}");
			Thread thread = new Thread(() =>
			{
				while (true)
				{
					while (!Clipboard.ContainsImage())
					{
						Thread.Sleep(10);
						SendKeys.SendWait("%{PRTSC}");
					}
					try
					{
						bmp = new Bitmap(Clipboard.GetImage());
						return;
					}
					catch (Exception ex)
					{
						continue;
					}
				}
			});
			thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
			thread.Start();
			thread.Join();

			return bmp;
		}

	}
}

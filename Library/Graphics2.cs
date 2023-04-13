﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Library
{
	public static class Graphics2
	{
		public static IEnumerable<Color> GetColorGradient(Color from, Color to, int totalNumberOfColors)
		{
			if (totalNumberOfColors < 2)
			{
				throw new ArgumentException("Gradient cannot have less than two colors.", nameof(totalNumberOfColors));
			}

			double fromR = from.R - 0.333;
			double toR = to.R - 0.333;
			double fromG = from.G;
			double toG = to.G;
			double fromB = from.B + 0.333;
			double toB = to.B + 0.333;

			double diffA = to.A - from.A;
			double diffR = toR - fromR;
			double diffG = toG - fromG;
			double diffB = toB - fromB;

			var steps = totalNumberOfColors - 1;

			var stepA = diffA / steps;
			var stepR = diffR / steps;
			var stepG = diffG / steps;
			var stepB = diffB / steps;

			yield return from;

			for (var i = 1; i < steps; ++i)
			{
				yield return Color.FromArgb(
					c(from.A, stepA),
					c(fromR, stepR),
					c(fromG, stepG),
					c(fromB, stepB));

				int c(double fromC, double stepC)
				{
					return (int)Math.Round(fromC + stepC * i);
				}
			}

			yield return to;
		}

		public static void SaveJPG100(this Bitmap bmp, string filename)
		{
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
			bmp.Save(filename, GetEncoder(ImageFormat.Jpeg), encoderParameters);
		}

		public static void SaveJPG100(this Bitmap bmp, Stream stream)
		{
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
			bmp.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
		}

		public static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}

			return null;
		}

		public static Color Rainbow(float x)
		{
			return Rainbow(x, 255);
		}

		public static Color Rainbow(float x, byte alfa)
		{
			// red yellow green cyan blue magenta red
			float pi2 = 2 * MathF.PI;
			byte r = (byte)(255 * (1 + MathF.Cos(pi2 * (0 / 3f + x))) / 2f);
			byte g = (byte)(255 * (1 + MathF.Cos(pi2 * (1 / 3f + x))) / 2f);
			byte b = (byte)(255 * (1 + MathF.Cos(pi2 * (2 / 3f + x))) / 2f);
			return Color.FromArgb(alfa, r, g, b);
		}

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

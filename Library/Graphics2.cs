using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Clr0 = System.Drawing.Color;
using Clr = System.Windows.Media.Color;
using System.Runtime.InteropServices;

namespace Library
{
	public static class Graphics2
	{
		public static IEnumerable<Clr0> GetColorGradient(Clr0 from, Clr0 to, int totalNumberOfColors)
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
				yield return Clr0.FromArgb(
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

		public static IEnumerable<Clr> GetColorGradientM(Clr from, Clr to, int totalNumberOfColors)
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
				yield return Clr.FromArgb(
					c(from.A, stepA),
					c(fromR, stepR),
					c(fromG, stepG),
					c(fromB, stepB));

				byte c(double fromC, double stepC)
				{
					return (byte)Math.Round(fromC + stepC * i);
				}
			}

			yield return to;
		}

		public static void FillRectangle(WriteableBitmap bitmap, Clr color, Int32Rect rect)
		{
			int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
			byte[] colorBytes = { color.B, color.G, color.R, color.A };
			int stride = bitmap.BackBufferStride;
			IntPtr buffer = bitmap.BackBuffer + rect.Y * stride + rect.X * bytesPerPixel;

			for (int y = 0; y < rect.Height; y++)
			{
				Marshal.Copy(colorBytes, 0, buffer, colorBytes.Length);
				buffer += stride;
			}

			bitmap.AddDirtyRect(rect);
		} //Check me

		public static void SaveJPG100(this WriteableBitmap bitmap, string fileName)
		{
			JpegBitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.QualityLevel = 100;
			encoder.Frames.Add(BitmapFrame.Create(bitmap));
			using (var fileStream = new FileStream(fileName, FileMode.Create))
			{
				encoder.Save(fileStream);
			}
		}

		public static Clr0 Rainbow(float x)
		{
			return Rainbow(x, 255);
		}

		public static Clr RainbowM(float x)
		{
			// red yellow green cyan blue magenta red
			float pi2 = 2 * MathF.PI;
			byte r = (byte)(255 * (1 + MathF.Cos(pi2 * (0 / 3f + x))) / 2f);
			byte g = (byte)(255 * (1 + MathF.Cos(pi2 * (1 / 3f + x))) / 2f);
			byte b = (byte)(255 * (1 + MathF.Cos(pi2 * (2 / 3f + x))) / 2f);
			return Clr.FromRgb(r, g, b);
		}

		public static Clr0 Rainbow(float x, byte alfa)
		{
			// red yellow green cyan blue magenta red
			float pi2 = 2 * MathF.PI;
			byte r = (byte)(255 * (1 + MathF.Cos(pi2 * (0 / 3f + x))) / 2f);
			byte g = (byte)(255 * (1 + MathF.Cos(pi2 * (1 / 3f + x))) / 2f);
			byte b = (byte)(255 * (1 + MathF.Cos(pi2 * (2 / 3f + x))) / 2f);
			return Clr0.FromArgb(alfa, r, g, b);
		}

		public static WriteableBitmap RescaleBitmap(WriteableBitmap bitmap, int width, int height)
		{
			WriteableBitmap resizedBitmap = new WriteableBitmap(width, height, bitmap.DpiX, bitmap.DpiY, bitmap.Format, null);
			//resizedBitmap.Lock();

			bitmap.CopyPixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), resizedBitmap.BackBuffer, resizedBitmap.BackBufferStride * resizedBitmap.PixelHeight, resizedBitmap.BackBufferStride);
			resizedBitmap.WritePixels(new Int32Rect(0, 0, width, height), resizedBitmap.BackBuffer, resizedBitmap.BackBufferStride * height, resizedBitmap.BackBufferStride);

			//resizedBitmap.Unlock();
			return resizedBitmap;
		}   //Check me

		public static WriteableBitmap ToBlackWhite(WriteableBitmap bitmap)
		{
			byte[] pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
			bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0);

			for (int i = 0; i < pixels.Length; i += 4)
			{
				byte min = Math.Min(Math.Min(pixels[i + 2], pixels[i + 1]), pixels[i]);
				pixels[i + 2] = min;
				pixels[i + 1] = min;
				pixels[i] = min;
			}

			WriteableBitmap bwBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, PixelFormats.Bgr32, null);
			bwBitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels, bitmap.PixelWidth * 4, 0);

			return bwBitmap;
		}

		public static WriteableBitmap MaximizeContrastAndNegate(WriteableBitmap bitmap)
		{
			byte[] pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
			bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0);

			for (int i = 0; i < pixels.Length; i += 4)
			{
				byte gray = (byte)(0.299 * pixels[i + 2] + 0.587 * pixels[i + 1] + 0.114 * pixels[i]);
				byte newPixel = (byte)(gray < 128 ? 255 : 0);
				byte contrastedPixel = (byte)(((newPixel - 128) * 1.8) + 128);
				byte invertedPixel = (byte)(255 - contrastedPixel);
				pixels[i + 2] = invertedPixel;
				pixels[i + 1] = invertedPixel;
				pixels[i] = invertedPixel;
			}

			WriteableBitmap contrastedBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, PixelFormats.Bgr32, null);
			contrastedBitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels, bitmap.PixelWidth * 4, 0);

			return contrastedBitmap;
		}

		public static WriteableBitmap Negate(WriteableBitmap bitmap)
		{
			int width = bitmap.PixelWidth;
			int height = bitmap.PixelHeight;
			int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
			int stride = width * bytesPerPixel;

			byte[] pixelData = new byte[height * stride];
			bitmap.CopyPixels(pixelData, stride, 0);

			for (int y = 0; y < height; y++)
			{
				int rowOffset = y * stride;
				for (int x = 0; x < width; x++)
				{
					int pixelOffset = rowOffset + x * bytesPerPixel;

					// Получаем значения компонент цвета
					byte b = pixelData[pixelOffset];
					byte g = pixelData[pixelOffset + 1];
					byte r = pixelData[pixelOffset + 2];

					// Инвертируем значения компонент цвета
					pixelData[pixelOffset] = (byte)(255 - b);
					pixelData[pixelOffset + 1] = (byte)(255 - g);
					pixelData[pixelOffset + 2] = (byte)(255 - r);
				}
			}

			bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);
			return bitmap;
		}

		/*		public static Bitmap TakeScreen2()
				{
					int w = Screen.PrimaryScreen.Bounds.Width;
					int h = Screen.PrimaryScreen.Bounds.Height;
					Bitmap bmp = new Bitmap(w, h);
					Graphics gr = Graphics.FromImage(bmp);
					gr.CopyFromScreen(0, 0, 0, 0, new Size(w, h));
					return bmp;
				}*/ //FIX

		/*		public static Bitmap TakeScreen()
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
				}*/ //FIX

	}
}

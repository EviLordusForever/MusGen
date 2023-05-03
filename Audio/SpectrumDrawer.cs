﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using Extensions;
using Clr = System.Windows.Media.Color;
using static MusGen.HardwareParams;
using System.Windows;

namespace MusGen
{
	public static class SpectrumDrawer
	{
		public static int _resX;
		public static int _resY;
		public static int _yHalf;
		public static WriteableBitmap _wbmpL1;
		public static WriteableBitmap _wbmpL2;
		public static WriteableBitmap _wbmp;
		public static List<Clr> _gradient;
		public static int[] _oldXs;
		public static int[] _oldYs;
		public static float _indexPianoStart;
		public static float _indexPianoEnd;

		public static WriteableBitmap DrawType1(float[] input_array, int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScale;
			float xScale;
			float powerScale;

			MoveDown();
			FillBlack();
			Scales();
			DrawPartDown();
			DrawPartUp();
			DrawVerticalLines();
			return _wbmpL1;

			void FillBlack()
			{
				GraphicsE.FillRectangle(_wbmpL1, Clr.FromArgb(0, 0, 0, 0), new System.Windows.Int32Rect(0, 0, _resX, _yHalf + 1));
				_wbmpL1.DrawLine(0, _yHalf, _resX, _yHalf, Clr.FromRgb(25, 25, 25));
				_wbmpL1.DrawLine(0, _yHalf+1, _resX, _yHalf+1, Clr.FromArgb(0, 0, 0, 0));
			}

			void DrawVerticalLines()
			{
				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					int x = Convert.ToInt32(verticalLines[i] * xScale);
					int y = Convert.ToInt32(theirSizes[i] * yScale);

					Clr clr = GraphicsE.RainbowM(i * 1f / verticalLines.Length);
					int w = (int)((5f / 1920f) * _resX);

					if (y >= 1)
						_wbmpL1.DrawLineAa(x, _yHalf - 1, x, _yHalf - y, clr, w);
				}
			}

			void DrawPartUp()
			{
				for (int i = 0; i < input_array.Length; i++)
				{
					int y = 0;
					if (input_array[i] > 0)
						y = Convert.ToInt32(input_array[i] * yScale);

					int x = Convert.ToInt32(i * xScale);

					if (y >= 1)
						_wbmpL1.DrawLineAa(x, _yHalf - 1, x, _yHalf - y, Clr.FromRgb(255, 255, 255), 1); //
				}
			}

			void DrawPartDown()
			{
				for (int i = 0; i < input_array.Length; i++)
				{
					int lim = _gradient.Count - 1;

					int power = Math.Min((int)(lim * input_array[i] * powerScale), lim);

					int x = Convert.ToInt32(i * xScale);

					_wbmpL1.SetPixel(x, _yHalf + 1, _gradient[power]);
				}
			}

			void Scales()
			{
				input_array[0] = 0.0001f;
				yScale = 1f * _resY / Math.Max(input_array.Max(), adaptiveCeiling);
				yScale /= 2;
				powerScale = 1f / maxCeiling;
				xScale = 1f * _resX / input_array.Length;
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
			int penWidth = Convert.ToInt32(xScale)/2 + 1;
			DrawPartUp();
			DrawPartDown();
			return _wbmpL1;

			void FillBlack()
			{
				GraphicsE.FillRectangle(_wbmpL1, Clr.FromArgb(0, 0, 0, 0), new System.Windows.Int32Rect(0, 0, _resX, _yHalf + 1));
				_wbmpL1.DrawLine(0, _yHalf, _resX, _yHalf, Clr.FromRgb(25, 25, 25));
				_wbmpL1.DrawLine(0, _yHalf + 1, _resX, _yHalf + 1, Clr.FromArgb(0, 0, 0, 0));
			}

			void DrawPartDown()
			{
				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					int x = Convert.ToInt32(verticalLines[i] * xScale);

					float power0_1 = theirSizes[i] / adaptiveCeiling;
					float powerSqrt = MathF.Sqrt(power0_1);
					byte power0_255 = (byte)(powerSqrt * 255);

					Clr clr = Clr.FromArgb(power0_255, 0, 255, 200);

					_wbmpL1.DrawLine(x - penWidth, _yHalf + 1, x + penWidth, _yHalf + 1, clr);
				}
			}

			void DrawPartUp()
			{
				for (int i = 0; i < verticalLines.Length; i++)
				{
					if (theirSizes[i] > Int32.MaxValue)
						continue;

					float power0_1 = theirSizes[i];

					int x = Convert.ToInt32(verticalLines[i] * xScale);
					int y = Convert.ToInt32(power0_1 * yScaleUp * 0.95);

					Clr clr = GraphicsE.RainbowM(i * 1f / verticalLines.Length);

					_wbmpL1.DrawLineAa(x, _yHalf / 2 - y, x, _yHalf / 2 + y, clr, penWidth);
				}
			}

			void Scales()
			{
				yScaleUp = 1f * _resY / adaptiveCeiling;
				yScaleUp /= 4;
				xScale = 1f * _resX / _resX;
			}
		}

		public static WriteableBitmap DrawType3(int[] verticalLines, float[] theirSizes, float adaptiveCeiling, float maxCeiling)
		{
			float yScaleUp;
			float xScale;
			float yScaleDown;

			_wbmp.Clear(Clr.FromRgb(0, 0, 0));
			_wbmpL1.FillRectangle(0, 0, _resX, _yHalf + 1, Clr.FromArgb(125, 0, 0, 0));
			_wbmp.Clear(Clr.FromArgb(0, 255, 0, 255));

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
					power0_1 = MathE.ToLogScale(power0_1, 10);
					float power0_255 = 255 * MathF.Sqrt(power0_1 * yScaleDown);
					byte power = Math.Min((byte)power0_255, (byte)255);
					Clr clr = Clr.FromArgb(255, power, power, power);

					_wbmpL1.DrawLineAa(x, _yHalf + 1, x, _yHalf + 2, clr, penWidth);
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
					power0_1 = MathE.ToLogScale(power0_1, 10);
					float power0_255 = 255 * power0_1;
					byte power = (byte)Math.Min(power0_255, 255);

					int x = Convert.ToInt32(verticalLines[i] * xScale);
					int y = _yHalf - penWidth - Convert.ToInt32(power0_1 * yScaleUp * 0.95);

					Clr clr = GraphicsE.RainbowM(i * 1f / verticalLines.Length);

					_wbmpL1.DrawLineAa(_oldXs[i], _oldYs[i], x, y, clr, penWidth);

					int R = 3;
					_wbmpL2.DrawEllipse(x - R, y + 1 - R, 2 * R, 2 * R, clr);

					_oldXs[i] = x;
					_oldYs[i] = y;
				}
			}

			void Scales()
			{
				yScaleUp = 1f * _resY / 1;
				yScaleUp /= 2;
				yScaleDown = 1f / 1;
				xScale = 1f * _resX / _resX;
			}

			WriteableBitmap Ending()
			{
				_wbmp = _wbmpL1;
				WBMP.CopyPixels(_wbmpL2, _wbmp, 0, 0, 0, 0, _resX, _resY);
				return _wbmp;
			}
		}

		public static void MoveDown()
		{
			WBMP.CopyPixels(_wbmpL1, _wbmpL1, 0, _yHalf + 1, 0, _yHalf + 2, _resX, _yHalf - 2);
		}

		public static void SetOctaves()
		{
			float spectrumSize = AP._fftSize / 2;

			float b0 = 30.87f;
			float c1 = 32.7f;
			float b9 = 15728f;
			float c10 = 8372.02f * 2;

			float start = (b0 + c1) / 2;
			float end = (b9 + c10) / 2;

			_indexPianoStart = Find(start);
			_indexPianoEnd = Find(end);

			double l = _indexPianoStart;
			double r = spectrumSize - _indexPianoEnd;
			double w = _indexPianoEnd - _indexPianoStart;

			SetPianoRoll();

			float Find(float f)
			{
				int i = 0;

				while (SpectrumFinder._frequenciesLogarithmic[i] < f)
					i++;

				float fmore = SpectrumFinder._frequenciesLogarithmic[i];
				float fless = SpectrumFinder._frequenciesLogarithmic[i - 1];

				float value = (f - fless) / (fmore - fless);
				float indexIdeal = i - 1 + value;
				return indexIdeal;
			}

			void SetPianoRoll()
			{
				if (WindowsManager._realtimeFFTWindow != null)
				{
					WindowsManager._realtimeFFTWindow.Dispatcher.Invoke(() =>
					{
						BitmapImage bitmap = new BitmapImage();
						bitmap.BeginInit();
						bitmap.UriSource = new Uri($"{DiskE._programFiles}\\Images\\PianoRoll.png");
						bitmap.EndInit();
						WriteableBitmap bitmap9 = WBMP.Create((int)bitmap.Width * 9, (int)bitmap.Height);

						for (int i = 0; i < 9; i++)
						{
							int x = i * (int)bitmap.Width;
							WBMP.CopyPixels(bitmap, bitmap9, 0, 0, x, 0, (int)bitmap.Width, (int)bitmap.Height);
							if (i % 2 == 1)
								WBMP.MultiplyAlpha(bitmap9, 0.6f, x, 0, (int)bitmap.Width, (int)bitmap.Height);
						}

						WindowsManager._realtimeFFTWindow.piano.Source = bitmap9;
						WindowsManager._realtimeFFTWindow.piano.Opacity = 0.075;

						WindowsManager._realtimeFFTWindow.piano.Stretch = System.Windows.Media.Stretch.Fill;
						WindowsManager._realtimeFFTWindow.grid.ColumnDefinitions[0].Width = new GridLength(l, GridUnitType.Star);
						WindowsManager._realtimeFFTWindow.grid.ColumnDefinitions[1].Width = new GridLength(w, GridUnitType.Star);
						WindowsManager._realtimeFFTWindow.grid.ColumnDefinitions[2].Width = new GridLength(r, GridUnitType.Star);
					});
				}
			}
		}

		static SpectrumDrawer()
		{
			Init();
		}

		public static void Init()
		{
			_resX = AP._wbmpResX;
			_resY = AP._wbmpResY;
			_yHalf = AP._wbmpResY / 2;
			_oldXs = new int[AP._channels];
			_oldYs = new int[AP._channels];
			for (int i = 0; i < AP._channels; i++)
				_oldYs[i] = _resY / 2 - 5;
			_wbmp = WBMP.Create(_resX, _resY);

			_wbmpL1 = WBMP.Create(_resX, _resY);

			_wbmpL2 = WBMP.Create(_resX, _resY);

			Clr[] clrs = new Clr[7];
			int[] sizes = new int[6];

			clrs[0] = Clr.FromArgb(0, 0, 50, 255);
			clrs[1] = Clr.FromArgb(255, 30, 0, 255);
			clrs[2] = Clr.FromArgb(255, 155, 25, 225);
			clrs[3] = Clr.FromArgb(255, 255, 30, 20);
			clrs[4] = Clr.FromArgb(255, 255, 90, 0);
			clrs[5] = Clr.FromArgb(255, 255, 255, 0);
			clrs[6] = Clr.FromArgb(255, 255, 255, 255);

			sizes[0] = 90; //to blue
			sizes[1] = 80; //to purple
			sizes[2] = 110; //to red
			sizes[3] = 270; //to orange
			sizes[4] = 590; //to yellow
			sizes[5] = 1410; //to white

			_gradient = new List<Clr>();

			for (int i = 0; i < sizes.Length; i++)
				_gradient.AddRange(GraphicsE.GetColorGradient(clrs[i], clrs[i + 1], sizes[i]));

			SetOctaves();
		}
	}
}

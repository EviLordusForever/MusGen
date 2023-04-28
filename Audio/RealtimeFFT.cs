using System;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Extensions;

namespace MusGen
{
	public static class RealtimeFFT
	{
		public static void Start()
		{
			Thread tr = new(Tr);
			tr.Name = "Realtime FFT";
			tr.Start();

			void Tr()
			{
				Logger.Log("Realtime FFT started.");
				WavToNadConvertor wtn = new WavToNadConvertor(AP._fftSize, AP._nadSamplesPerSecond, AP._channels, AP._peakSize, AP._logarithmicNad);

				BitmapSource bitmapSource;

				WriteableBitmap _wbmp = WBMP.Create(AP._wbmpResX, AP._wbmpResY);

				var bytesPerPixel = (_wbmp.Format.BitsPerPixel + 7) / 8;
				var stride = bytesPerPixel * _wbmp.PixelWidth;
				var bufferSize = stride * _wbmp.PixelHeight;
				var pixelWidth = _wbmp.PixelWidth;
				var pixelHeight = _wbmp.PixelHeight;
				var dpiX = _wbmp.DpiX;
				var dpiY = _wbmp.DpiY;
				var format = _wbmp.Format;
				var backBufferStride = _wbmp.BackBufferStride;

				int frames = 0;
				var fps = 0;
				float adaptiveCeiling = 3; //
				NadSample nads = new NadSample(AP._channels);
				NadSample nadsOld = new NadSample(AP._channels);
				var stopwatch = new Stopwatch();
				stopwatch.Start();

				AudioCapturer.Start(AP._sampleRate, 16, 1);
				SpectrumFinder.Init(AP._fftSize, AP._sampleRate, AP._smoothXScale, AP._smoothYScale);
				SpectrumDrawer.Init(AP._wbmpResX, AP._wbmpResY, AP._channels);

				Wav wav = new Wav();
				wav.sampleRate = (int)AP._sampleRate;
				wav.L = new float[AP._fftSize];

				while (true)
				{
					FPS();					

					wav.L = AudioCapturer.GetSamples(AP._fftSize);

					nadsOld = nads;
					nads = wtn.MakeSample(wav, 0);
					AdaptiveCeiling();

					DrawSpectrum();
					ShowImage();
				}

				void FPS()
				{
					if (frames >= 60)
					{
						fps = (int)(60 * 1000 / stopwatch.Elapsed.TotalMilliseconds);
						stopwatch.Restart();
						frames = 0;
					}
					frames++;
					WindowsManager._realtimeFFTWindow.Dispatcher.Invoke(() =>
					{
						WindowsManager._realtimeFFTWindow.fps.Text = fps.ToString();
					});
				}

				void ShowImage()
				{
					byte[] buffer = new byte[bufferSize];
					Marshal.Copy(_wbmp.BackBuffer, buffer, 0, bufferSize);

					WindowsManager._realtimeFFTWindow.img.Dispatcher.Invoke(() =>
					{
						bitmapSource = BitmapSource.Create(
							pixelWidth,
							pixelHeight,
							dpiX,
							dpiY,
							format,
							null,
							buffer,
							backBufferStride);
						WindowsManager._realtimeFFTWindow.img.Source = bitmapSource;
						WindowsManager._realtimeFFTWindow.img.UpdateLayout();
					});
				}

				void DrawSpectrum()
				{
					if (AP._graphType == 1)
					{
						float[] fix = new float[SpectrumFinder._spectrum.Length];
						for (int index = 0; index < SpectrumFinder._spectrum.Length; index++)
						{
							float a = SpectrumFinder._spectrum[index];
							float b = SoundPressureModel.GetSoundPressureLevel(SpectrumFinder._frequenciesLinear[index]);

							fix[index] = a * b;
						} /////////////////////////////////////////////////////////

						_wbmp = SpectrumDrawer.DrawType1(SpectrumFinder._spectrumLogarithmic, nads._indexes, nads._amplitudes, adaptiveCeiling, adaptiveCeiling);
					}
					else if (AP._graphType == 2)
						_wbmp = SpectrumDrawer.DrawType2(nads._indexes, nads._amplitudes, adaptiveCeiling, adaptiveCeiling);
					else if (AP._graphType == 3)
						_wbmp = SpectrumDrawer.DrawType3(nads._indexes, nads._amplitudes, adaptiveCeiling, adaptiveCeiling);
				}

				void AdaptiveCeiling()
				{
					adaptiveCeiling *= 0.99f;
					adaptiveCeiling = Math.Max(adaptiveCeiling, nads._amplitudes[0]);
					adaptiveCeiling = Math.Max(adaptiveCeiling, 0.0001f);
				}
			}
		}
	}
}

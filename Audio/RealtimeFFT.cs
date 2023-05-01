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
		private static Thread _tr;
		private static bool _started;

		public static void Start(string type)
		{
			AP._captureType = type;

			_tr = new(Tr);
			_tr.Name = "Realtime FFT";
			_tr.Start();

			void Tr()
			{
				Logger.Log("Realtime FFT started.");
				
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
				int fps = 0;
				float adaptiveCeiling = 3; //
				NadSample nads = new NadSample(AP._channels);
				NadSample nadsOld = new NadSample(AP._channels);
				var stopwatch = new Stopwatch();
				stopwatch.Start();

				if (AP._captureType == "microphone")
				{
					AP.SampleRate = AP._sampleRateRTFFTMicrophone;
					AudioCapturerMicrophone.Start(AP.SampleRate, 16, 1);
				}
				else if (AP._captureType == "system")
				{
					AP.SampleRate = AP._sampleRateRTFFTSystem;
					AudioCapturerSystem.Start(AP.SampleRate, 16, 1);
				}

				SpectrumDrawer.Init();

				Wav wav = new Wav();
				wav._sampleRate = (int)AP.SampleRate;
				wav.L = new float[AP._fftSize];

				_started = true;

				while (_started)
				{
					FPS();

					if (AP._captureType == "microphone")
						wav.L = AudioCapturerMicrophone.GetSamples(AP._fftSize * AP._lc);
					else if (AP._captureType == "system")
						wav.L = AudioCapturerSystem.GetSamples(AP._fftSize * AP._lc);

					nadsOld = nads;
					nads = WavToNadConvertor.MakeSample(wav, 0);
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
						_wbmp = SpectrumDrawer.DrawType1(SpectrumFinder._spectrumLogarithmic, nads._indexes, nads._amplitudes, adaptiveCeiling, adaptiveCeiling);
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

		public static void Stop()
		{
			if (_started)
			{
				Thread stopThread = new(StopThread);
				stopThread.Name = "Stop Thread";
				stopThread.Start();

				void StopThread()
				{
					_started = false;
					while (_tr.ThreadState != System.Threading.ThreadState.Stopped) { };

					if (AP._captureType == "microphone")
						AudioCapturerMicrophone.Stop();
					else if (AP._captureType == "system")
						AudioCapturerSystem.Stop();

					Logger.Log("Realtime FFT stopped.");
				}
			}
		}
	}
}

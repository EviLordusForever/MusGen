using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Voice.Models;
using System.Threading;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using Library;
using Clr = System.Windows.Media.Color;
using static MusGen.HardwareParams;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace MusGen
{
	public static class EffectsCreator2
	{
		//PARAMS:

		public static string waveForm = "sin";
		public static int limitSec = 2500;
		public static int FFTPerSecond = 100;
		public static int channels = 10;
		public static float fadeTime = 0.99f;
		public static bool drawGraph = true;
		public static int FFTsize = 1024;
		public static float trashSize = 10;
		public static float amplitudeThreshold = 0;
		public static float lfsY = 0.95f;
		public static float lfsX = 4000f;
		public static string pointsConnectingX = "logarithmic";
		public static string pointsConnectingY = "logarithmic";
		public static int graphResX = 256 * 2;
		public static int graphResY = 16 * 9 * 2;
		public static int graphType = 2;
		public static uint sampleRate = 44100;

		//VARIABLES

		public static Wav wavIn;
		public static Wav wavOut;

		public static float adaptiveCeiling = 3;

		public static int fadeSamplesLeft;
		public static float signal;
		public static int FFTstep;
		public static int samplesForFade;
		public static float maxAmplitudeForNormalization;

		public static float pi2;
		public static float buf;

		public static float[] frqsNew;
		public static float[] ampsNew;

		public static float[] frqsOld;
		public static float[] ampsOld;

		public static float[] ampsLg;
		public static int[] idxsLg;

		public static float[] ampsLgOld;
		public static int[] idxsLgOld;

		public static double[] t;
		public static double[] tOld;

		public static string _outName;

		public static uint graphDuration;
		public static long limit;

		public static WriteableBitmap wbmp;
		//public static WriteableBitmap wbmp2;

		//CODE

		public static void Startup(string originPath, string outName3)
		{
			if (!string.IsNullOrEmpty(originPath))
			{
				wavIn = new Wav();
				wavIn.Read(originPath, 0);

				wavOut = new Wav();
				wavOut.Read(originPath, 0);

				wavOut.L = new float[Math.Min(limitSec * sampleRate, wavIn.L.Length)];
				if (wavOut.channels == 2)
					wavOut.R = new float[Math.Min(limitSec * sampleRate, wavIn.R.Length)];

				sampleRate = wavIn.sampleRate;

				fadeSamplesLeft = 0;
				signal = 0;

				FFTstep = (int)(sampleRate / FFTPerSecond);

				samplesForFade = (int)(FFTstep * fadeTime);
				maxAmplitudeForNormalization = 0;

				t = new double[channels];
				tOld = new double[channels];

				_outName = outName3 + $" (FFT {FFTsize} ch {channels} tr {trashSize})";

				graphDuration = sampleRate / 60;
				limit = Math.Min(wavIn.L.Length, sampleRate * limitSec);
			}

			pi2 = MathF.PI * 2;
			buf = pi2 / sampleRate;

			frqsNew = new float[channels];
			ampsNew = new float[channels];

			frqsOld = new float[channels];
			ampsOld = new float[channels];

			ampsLg = new float[channels];
			idxsLg = new int[channels];

			ampsLgOld = new float[channels];
			idxsLgOld = new int[channels];

			FrequencyFinder.spectrum = new float[FFTsize];
			FrequencyFinder.leadIndexes = new int[channels];
			
			FrequencyFinder.FillFrqs(FFTsize, sampleRate);
			FrequencyFinder.amplitudeMaxWholeTrack = 0.1f;
			FrequencyFinder.amplitudeMax = 0;
			FrequencyFinder.CalculateSmoothMask(lfsX, lfsY, FFTsize, sampleRate);
			GraphDrawer.Init(graphResX, graphResY, channels);
		}

		public static void FFT(string originPath, string outName2)
		{
			Thread tr = new(Tr);
			tr.Start();

			void Tr()
			{
				Startup(originPath, outName2);

				if (UserAsker.Ask($"Name: {_outName}\nWave type: {waveForm}\nRecreation channels: {channels}\nFade time: {fadeTime}\nFFT size: {FFTsize}\nFFT per second: {FFTPerSecond}\nTrash size: {trashSize}\nLow frq smoothing size X: {lfsX}\nLow frq smoothing size Y: {lfsY}\nAmplitude threshold: {amplitudeThreshold}\nPoints connecting X: {pointsConnectingX}\nPoints connecting Y: {pointsConnectingY}\nDraw graph: {drawGraph}\nSamples: {limit}\nSample rate: {sampleRate}\nSeconds: {(int)(limit / sampleRate)}"))
					return;

				FindAmplitudeMaxForWholeTrack();

				ProgressShower.ShowProgress($"Effect FFT multi audio recreation... ({_outName})");

				for (int s = 0; s < limit; s++)
				{
					if (s % FFTstep == 0)
					{
						Olds();

						FrequencyFinder.ByFFT(ref frqsNew, ref ampsNew, wavIn, s, FFTsize, trashSize);
						AdaptiveCeiling();

						FindLg();
						ConnectNewPoints();
						DeleteSmallAmplitudes();
						SetTimeOld();

						fadeSamplesLeft = samplesForFade;
					}
					else
						fadeSamplesLeft--;

					WriteSample(s);
					if (drawGraph && s % graphDuration == 0)
					{
						DrawGraph();
						SaveGraph($"{s}");
						ProgressShower.SetProgress(1.0 * s / limit);
					}

					CheckMaxAmplitude(s);
				}

				Normalize();

				ProgressShower.ShowProgress("Saving...");

				wavOut.Export(_outName);

				ProgressShower.CloseProgressForm();

				void SetTimeOld()
				{
					for (int c = 0; c < channels; c++)
						tOld[c] = t[c];
				}

				void WriteSample(int s)
				{
					signal = 0;

					for (int c = 0; c < channels; c++)
					{
						if (frqsNew[c] < 20f)
							frqsNew[c] = 20f;

						if (fadeSamplesLeft > 0)
						{
							float fade = 1 - 1f * fadeSamplesLeft / samplesForFade;
							fade = (MathF.Cos(fade * MathF.PI) + 1) / 2;
							float antifade = 1 - fade;

							float amp = ampsOld[c] * fade;
							tOld[c] += buf * frqsOld[c];
							signal += (float)F(tOld[c]) * amp;

							amp = ampsNew[c] * antifade;
							t[c] += buf * frqsNew[c];
							signal += (float)F(t[c]) * amp;
						}
						else
						{
							t[c] += buf * frqsNew[c];
							signal += (float)F(t[c]) * ampsNew[c] * 1;
						}
					}

					wavOut.L[s] = signal / channels;
				}		

				void SaveGraph(string name)
				{
					string path = $"{Disk2._programFiles}Grafics\\g\\g_{name}.jpg";
					Graphics2.SaveJPG100(wbmp, path);
				}

				void Normalize()
				{
					ProgressShower.ShowProgress($"Normalization... ({maxAmplitudeForNormalization} to 1)");
					ProgressShower.SetProgress(0.0);
					maxAmplitudeForNormalization *= 1.01f;
					for (int i = 0; i < limit; i++)
					{
						wavOut.L[i] /= maxAmplitudeForNormalization;
						if (wavIn.channels == 2)
							wavOut.R[i] = wavOut.L[i];

						if (i % 500 == 0)
							ProgressShower.SetProgress(1.0 * i / limit);
					}
				}

				void FindAmplitudeMaxForWholeTrack()
				{
					ProgressShower.ShowProgress("Finding maximal FFT amplitude...");
					int step = (int)(limit / 26);

					float[] empty = new float[FFTsize];					

					for (int i = 3; i < 23; i++)
					{
						FrequencyFinder.ByFFT(ref frqsNew, ref ampsNew, wavIn, i * step, FFTsize, trashSize);
						ProgressShower.SetProgress((i - 3) / 20.0);
					}

					ProgressShower.SetProgress(0.0);
				}

				void DeleteSmallAmplitudes()
				{
					if (amplitudeThreshold > 0)
						for (int i = 1; i < channels; i++)
							if (ampsNew[i] < ampsNew[0] * amplitudeThreshold)
								ampsNew[i] = 0;
				}

				void CheckMaxAmplitude(int s)
				{
					if (MathF.Abs(wavOut.L[s]) > maxAmplitudeForNormalization)
						maxAmplitudeForNormalization = MathF.Abs(wavOut.L[s]);
				}
			}
		}

		public static void RealtimeFFT()
		{
			Thread tr = new(Tr);
			tr.Name = "Realtime FFT";
			tr.Start();

			void Tr()
			{
				Startup("", "");
				BitmapSource bitmapSource;

				wbmp = WBMP.Create(graphResX, graphResY);

				var bytesPerPixel = (wbmp.Format.BitsPerPixel + 7) / 8;
				var stride = bytesPerPixel * wbmp.PixelWidth;
				var bufferSize = stride * wbmp.PixelHeight;
				var pixelWidth = wbmp.PixelWidth;
				var pixelHeight = wbmp.PixelHeight;
				var dpiX = wbmp.DpiX;
				var dpiY = wbmp.DpiY;
				var format = wbmp.Format;
				var backBufferStride = wbmp.BackBufferStride;

				AudioCapture.Start();

				Wav wav = new Wav();
				wav.sampleRate = sampleRate;
				wav.L = new float[FFTsize];

				while (true)
				{
					Olds();

					wav.L = AudioCapture.GetSamples(FFTsize);

					FrequencyFinder.ByFFT(ref frqsNew, ref ampsNew, wav, 0, FFTsize, trashSize);
					AdaptiveCeiling();

					FindLg();
					ConnectNewPoints();

					FrequencyFinder.amplitudeMaxWholeTrack *= 0.995f;
					DrawGraph();

					byte[] buffer = new byte[bufferSize];
					Marshal.Copy(wbmp.BackBuffer, buffer, 0, bufferSize);					

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

/*					if (Math2.rnd.Next(300) == 0)
					{
						Graphics2.SaveJPG100(wbmp, $"{Disk2._programFiles}GG.bmp");
						string l = string.Join('\n', wav.L);
						Disk2.WriteToProgramFiles("gg", "csv", l, false);

						string p = UserAsker.AskFile("wav");
						wavIn = new Wav();
						wavIn.Read(p, 0);
						wavIn.L = AudioCapture.samples.ToArray();
						wavIn.R = wavIn.L;
						wavIn.SaveTo($"{Disk2._programFiles}gg.wav");
					}*/
				}
			}
		}

		public static float GetSoundPressureLevel(float frequency)
		{
			float[] soundPressureLevels = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.7f, 0.9f, 0.8913f, 0.8913f, 1.1220f, 1.4125f, 1.5849f, 1.4983f, 1.1220f, 1.0f, 0.8913f, 0.8913f, 1.2589f, 1.5849f, 1.4983f, 1.1220f, 0.8913f, 0.7943f, 0.7943f, 1.1220f, 2.2387f, 4.4668f, 6.3096f, 5.6234f, 2.5119f, 1.2589f, 0.8913f, 0.5012f, 0f };

			float[] frequencies = new float[] { 0f, 20, 25, 31.5f, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500, 16000, 20000, 25000 };

			int index = Array.BinarySearch(frequencies, frequency);
			if (index >= 0)
				return soundPressureLevels[index];
			else
			{
				index = ~index;
				int index1 = index - 1;
				if (index1 < 0) index1 = 0;
				int index2 = index;
				if (index2 >= frequencies.Length) index2 = frequencies.Length - 1;

				float x1 = frequencies[index1];
				float x2 = frequencies[index2];
				float y1 = soundPressureLevels[index1];
				float y2 = soundPressureLevels[index2];
				float y = y1 + (y2 - y1) * (frequency - x1) / (x2 - x1);
				return y;
			}
		}

		//STATIC

		public static void Olds()
		{
			frqsOld = frqsNew;
			ampsOld = ampsNew;
			ampsNew = new float[channels];
			frqsNew = new float[channels];
		}

		public static void AdaptiveCeiling()
		{			
			adaptiveCeiling *= 0.99f;
			adaptiveCeiling = Math.Max(adaptiveCeiling, FrequencyFinder.amplitudeMax / FrequencyFinder.amplitudeMaxWholeTrack);
			adaptiveCeiling = Math.Max(adaptiveCeiling, 0.0001f);
		}

		public static void ConnectNewPoints()
		{
			int L = ampsNew.Length;
			int[,] distances = new int[L, L];
			float x = 0;
			float y = 0;

			for (int idNew = 0; idNew < L; idNew++)
				for (int idOld = 0; idOld < L; idOld++)
				{
					if (pointsConnectingX == "logarithmic")
					{
						x = idxsLgOld[idOld] - idxsLg[idNew];
						x /= FrequencyFinder.spectrum.Length;
						x *= 4; //check
					}
					else if (pointsConnectingX == "linear")
					{
						x = frqsOld[idOld] - frqsNew[idNew];
						x /= 20000; //Hz
						x *= 4; //check
					}
					else
						throw new ArgumentException("Smooth type incorrect");

					if (pointsConnectingY == "logarithmic")
						y = ampsLgOld[idOld] - ampsLg[idNew];
					else if (pointsConnectingY == "linear")
						y = ampsOld[idOld] - ampsNew[idNew];

					distances[idOld, idNew] = (int)(1000 * Math.Sqrt(x * x + y * y));
				}

			int[] compares = HungarianAlgorithm.Run(distances);

			float[] ampsCompared = new float[L];
			float[] frqsCompared = new float[L];
			int[] idxsCompared = new int[L];

			float[] ampsLgCompared = new float[L];
			int[] idxsLgCompared = new int[L];

			for (int oldid = 0; oldid < L; oldid++)
			{
				int newid = compares[oldid];

				ampsCompared[oldid] = ampsNew[newid];
				frqsCompared[oldid] = frqsNew[newid];
				idxsCompared[oldid] = FrequencyFinder.leadIndexes[newid];

				ampsLgCompared[oldid] = ampsLg[newid];
				idxsLgCompared[oldid] = idxsLg[newid]; //
			}

			ampsNew = ampsCompared;
			frqsNew = frqsCompared;
			FrequencyFinder.leadIndexes = idxsCompared;
		}

		public static void DrawGraph()
		{
			if (graphType == 1)
			{
				float[] fix = new float[FrequencyFinder.spectrum.Length];
				for (int index = 0; index < FrequencyFinder.spectrum.Length; index++)
				{
					float a = FrequencyFinder.spectrum[index];
					float b = GetSoundPressureLevel(FrequencyFinder.frequencies[index]);

					fix[index] = a * b;

					if (float.IsNaN(fix[index]))
					{
					}
				}

				float[] frqsLg = Array2.RescaleArrayToLog(fix, FFTsize);
				wbmp = GraphDrawer.Draw(frqsLg, idxsLg, ampsNew, adaptiveCeiling, FrequencyFinder.amplitudeMaxWholeTrack);
			}
			else if (graphType == 2)
				wbmp = GraphDrawer.DrawType2(idxsLg, ampsNew, adaptiveCeiling, FrequencyFinder.amplitudeMaxWholeTrack);
			else if (graphType == 3)
			{
			}
		}

		public static void FindLg()
		{
			for (int c = 0; c < channels; c++)
			{
				ampsLgOld[c] = ampsLg[c];
				ampsLg[c] = ampsNew[c];
				ampsLg[c] = Math2.ToLogScale(ampsLg[c], 10);
			}

			idxsLgOld = idxsLg;
			idxsLg = Array2.RescaleIndexesToLog(FrequencyFinder.leadIndexes, FrequencyFinder.spectrum.Length);
		}

		public static double F(double t)
		{
			if (waveForm == "sin")
				return Math.Sin(t);
			else if (waveForm == "sqaure")
				return Math.Sign(Math.Sin(t));
			else
				return Math.Sin(t);
		}

		public static float F(float t)
		{
			if (waveForm == "sin")
				return MathF.Sin(t);
			else if (waveForm == "sqaure")
				return MathF.Sign(MathF.Sin(t));
			else
				return MathF.Sin(t);
		}
	}

	public static class AudioCapturer
	{
		private static int samplesPerBuffer;
		public static readonly List<float> audioBuffer;
		public static WasapiLoopbackCapture loopbackCapture;
		private static bool captureStarted = false;

		static AudioCapturer()
		{
			audioBuffer = new List<float>();
		}

		public static void StartCapture(int samplesPerBuffer)
		{
			AudioCapturer.samplesPerBuffer = samplesPerBuffer;

			if (!captureStarted)
			{
				loopbackCapture = new WasapiLoopbackCapture();
				loopbackCapture.DataAvailable += OnDataAvailable;
				loopbackCapture.StartRecording();

				captureStarted = true;
			}
		}

		public static float[] GetLastSamples()
		{
			lock (audioBuffer)
			{
				int startIndex = audioBuffer.Count - samplesPerBuffer;
				if (startIndex < 0) startIndex = 0;
				float[] samples = new float[samplesPerBuffer];
				if (audioBuffer.Count >= samplesPerBuffer)
					samples = audioBuffer.GetRange(startIndex, samplesPerBuffer).ToArray();
				//audioBuffer.Clear();
				return samples;
			}
		}

		private static void OnDataAvailable(object sender, WaveInEventArgs e)
		{
			byte[] buffer = e.Buffer;
			int bytesRecorded = e.BytesRecorded;

			for (int i = 0; i < bytesRecorded; i += 2)
			{
				short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
				float sample32 = sample / 32768f;
				lock (audioBuffer)
				{
					audioBuffer.Add(sample32);
				}
			}
		}
	}

	public static class AudioCapture
	{
		public static WaveInEvent waveIn;
		public static BufferedWaveProvider bufferedWaveProvider;
		public static List<float> samples = new List<float>();

		public static void Start()
		{
			waveIn = new WaveInEvent();
			waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
			waveIn.BufferMilliseconds = 1000 / 100;
			bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
			waveIn.DataAvailable += (sender, e) =>
			{
				try
				{
					bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
				}
				catch
				{
					bufferedWaveProvider.ClearBuffer();
				}
			};

			waveIn.StartRecording();
		}

		public static float[] GetSamples(int count)
		{
			byte[] buffer = new byte[bufferedWaveProvider.BufferedBytes];
			int byteCount = bufferedWaveProvider.Read(buffer, 0, buffer.Length);
			short[] newSamples = new short[byteCount / 2];
			Buffer.BlockCopy(buffer, 0, newSamples, 0, byteCount);

			for (int i = 0; i < newSamples.Length; i++)
				samples.Add((float)newSamples[i] / (float)short.MaxValue);			

			float[] res = new float[count];

			int startIndex = samples.Count - count;

			if (startIndex >= 0)
				res = samples.GetRange(startIndex, count).ToArray();

			return res;
		}

		public static void Stop()
		{
			waveIn.StopRecording();
			waveIn.Dispose();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.Voice.Models;
using MusGen.View;
using System.Threading;
using Library;

namespace MusGen
{
	public static class EffectsCreator
	{
		public static Wav wavIn;
		public static Wav wavOut;
		public static string waveForm = "sin";
		public static float adaptiveCeiling = 3;

		public static void Startup(string originPath)
		{
			wavIn = new Wav();
			wavIn.Read(originPath, 0);

			wavOut = new Wav();
			wavOut.Read(originPath, 0);

			wavOut.L = new float[wavIn.L.Length];
			if (wavOut.channels == 2)
				wavOut.R = new float[wavIn.R.Length];
		}

		public static void Startup(string originPath, int limitSec)
		{
			wavIn = new Wav();
			wavIn.Read(originPath, 0);

			wavOut = new Wav();
			wavOut.Read(originPath, 0);

			wavOut.L = new float[Math.Min(limitSec * wavIn.sampleRate, wavIn.L.Length)];
			if (wavOut.channels == 2)
				wavOut.R = new float[Math.Min(limitSec * wavIn.sampleRate, wavIn.R.Length)];
		}

		public static void Export(string name)
		{
			wavOut.Export(name);
		}

		public static void EffectPanWaving(string originPath, string outName)
		{
			Startup(originPath);

			double t = 0;
			for (int i = 0; t < wavIn.L.Length; i++)
			{
				wavOut.L[i] = wavIn.L[(int)t];
				t += Math.Sin(i / 8096.0) + 1;
			}

			t = 0;
			for (int i = 0; t < wavIn.R.Length; i++)
			{
				wavOut.R[i] = wavIn.R[(int)t];
				t += Math.Sin(i / 8080.0) + 1;
			}

			Export(outName);
		}

		public static void EffectSqrt(string originPath, string outName)
		{
			Startup(originPath);

			for (int i = 0; i < wavIn.L.Length; i++)
				if (wavIn.L[(int)i] >= 0)
					wavOut.L[i] = (float)Math.Pow(wavIn.L[(int)i], 0.5);
				else
					wavOut.L[i] = -(float)Math.Pow(-wavIn.L[(int)i], 0.5);

			for (int i = 0; i < wavIn.R.Length; i++)
				if (wavIn.R[(int)i] >= 0)
					wavOut.R[i] = (float)Math.Pow(wavIn.R[(int)i], 0.5);
				else
					wavOut.R[i] = -(float)Math.Pow(-wavIn.R[(int)i], 0.5);

			Export(outName);
		}

		public static void EffectFFTMulti(string originPath, string outName)
		{
			Thread tr = new Thread(Tr);
			tr.Start();

			void Tr()
			{
				//PARAMS:

				int limitSec = 2500;
				int channels = 10;
				float smooth = 0.8f;
				bool drawGraph = true;
				int FFTsize = 512;
				float trashSize = 5;
				float amplitudeThreshold = 0;
				float lfsY = 0.95f;
				float lfsX = 2000f;

				Startup(originPath);
				GraphDrawer.Init(256 * 2, 16 * 9 * 2);

				float max = 0;
				float signal = 0;
				float antismooth = 1 - smooth;

				float pi2 = MathF.PI * 2;
				float buf = pi2 / wavIn.sampleRate;

				float[] frqsNew = new float[channels];
				float[] frqsRes = new float[channels];
				float[] ampsNew = new float[channels];
				float[] ampsRes = new float[channels];
				double[] t = new double[channels];
				float[] lowFrequenciesSmoothing = new float[FFTsize];

				outName += $" (FFT {FFTsize} ch {channels} tr {trashSize})";

				uint graphStep = wavIn.sampleRate / 60;

				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);

				bool ok = UserAsker.Ask($"Name: {outName}\nWave type: {waveForm}\nRecreation channels: {channels}\nSmooth: {smooth}\nFFT size: {FFTsize}\nTrash size: {trashSize}\nAmplitude threshold: {amplitudeThreshold}\nDraw graph: {drawGraph}\nSamples: {limit}\nSample rate: {wavOut.sampleRate}\nSeconds: {(int)(limit / wavOut.sampleRate)}");
				if (!ok)
					return;	

				FindAmplitudeMaxForWholeTrack();
				CalculateLowFrequencySmoothing();

				ProgressShower.ShowProgress("Effect FFT multi audio recreation...");

				float maxAmplitudeForNormalization = 0;

				for (int s = 0; s < limit; s++)
				{
					if (s % 441 == 0)
					{
						FrequencyFinder.ByFFT(ref frqsNew, ref ampsNew, wavIn, s, FFTsize, trashSize, lowFrequenciesSmoothing);
						adaptiveCeiling = Math.Max(adaptiveCeiling, FrequencyFinder.amplitudeMax);

						ConnectNewPoints();
						DeleteSmallAmplitudes();
					}

					signal = 0;

					for (int c = 0; c < channels; c++)
					{
						if (frqsNew[c] < 20f)
							frqsNew[c] = 20f;

						ampsRes[c] = ampsRes[c] * smooth + ampsNew[c] * antismooth;
						frqsRes[c] = frqsRes[c] * smooth + frqsNew[c] * antismooth;

						t[c] += (pi2 * frqsRes[c] / wavIn.sampleRate); //check me

						signal += (float)(F(t[c]) * ampsRes[c]);
					}

					DrawGraph(s);

					wavOut.L[s] = signal / channels;

					if (MathF.Abs(wavOut.L[s]) > maxAmplitudeForNormalization)
						maxAmplitudeForNormalization = MathF.Abs(wavOut.L[s]);
				}

				Normalize();

				ProgressShower.ShowProgress("Saving...");

				Export(outName);

				ProgressShower.CloseProgressForm();

				void CalculateLowFrequencySmoothing()
				{
					for (int x = 0; x < FFTsize; x++)
						lowFrequenciesSmoothing[x] = lfsY / MathF.Pow(MathF.E, x * wavOut.sampleRate / (FFTsize * lfsX));
				}

				void DrawGraph(int sampleNumber)
				{
					if (sampleNumber % graphStep == 0)
					{
						if (drawGraph)
						{
							float[] ampsLg = new float[channels];
							for (int c = 0; c < channels; c++)
								ampsLg[c] = ampsNew[c] * FrequencyFinder.amplitudeOverdrive;

							float[] frqsLg = Array2.RescaleArrayToLog2(FrequencyFinder.spectrum);
							int[] idxsLg = Array2.RescaleIndexesToLog2(FrequencyFinder.leadIndexes, frqsLg.Length);

							GraphDrawer.Draw($"{sampleNumber}", frqsLg, idxsLg, ampsLg, adaptiveCeiling, FrequencyFinder.amplitudeMaxForWholeTrack);
							adaptiveCeiling *= 0.99f;
						}

						ProgressShower.SetProgress(1.0 * sampleNumber / limit);
					}
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
					max = 0;

					float[] empty = new float[FFTsize];
					FrequencyFinder.spectrum = new float[FFTsize];

					for (int i = 3; i < 23; i++)
					{
						FrequencyFinder.ByFFT(ref frqsNew, ref ampsNew, wavIn, i * step, FFTsize, trashSize, empty);
						ProgressShower.SetProgress((i - 3) / 20.0);
						if (FrequencyFinder.amplitudeMax > max)
							max = FrequencyFinder.amplitudeMax;
					}

					FrequencyFinder.amplitudeMaxForWholeTrack = max;
					ProgressShower.SetProgress(0.0);
				}

				void ConnectNewPoints()
				{
					int L = ampsNew.Length;

					int[,] distances = new int[L, L];

					for (int n = 0; n < L; n++)
						for (int f = 0; f < L; f++)
						{
							float x = frqsRes[f] - frqsNew[n];
							x /= 20000; //Hz
							x *= 4; //check
							float y = ampsRes[f] - ampsNew[n];

							distances[f, n] = (int)(100000 * MathF.Sqrt(x * x + y * y));
						}

					int[] compares = HungarianAlgorithm.Run(distances);

					float[] ampsCompared = new float[L];
					float[] frqsCompared = new float[L];
					int[] idxsCompared = new int[L];

					for (int oldid = 0; oldid < L; oldid++)
					{
						int newid = compares[oldid];

						ampsCompared[oldid] = ampsNew[newid];
						frqsCompared[oldid] = frqsNew[newid];
						idxsCompared[oldid] = FrequencyFinder.leadIndexes[newid];
					}

					ampsNew = ampsCompared;
					frqsNew = frqsCompared;
					FrequencyFinder.leadIndexes = idxsCompared;
				}

				void DeleteSmallAmplitudes()
				{
					if (amplitudeThreshold > 0)
						for (int i = 1; i < channels; i++)
							if (ampsNew[i] < ampsNew[0] * amplitudeThreshold)
								ampsNew[i] = 0;
				}
			}
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
	}
}
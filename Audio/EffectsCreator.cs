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
			Thread tr = new(Tr);
			tr.Start();

			void Tr()
			{
				//PARAMS:

				int limitSec = 2500;
				int channels = 10;
				int FFTPerSecond = 100;
				float fadeTime = 0.99f;
				bool drawGraph = true;
				int FFTsize = 1024;
				float trashSize = 10;
				float amplitudeThreshold = 0;
				float lfsY = 0.95f;
				float lfsX = 4000f;
				string pointsConnectingX = "logarithmic";
				string pointsConnectingY = "logarithmic";

				Startup(originPath);
				GraphDrawer.Init(256 * 2, 16 * 9 * 2, channels);

				int fadeSamplesLeft = 0;
				float signal = 0;
				int FFTstep = (int)(wavIn.sampleRate / FFTPerSecond);
				int samplesForFade = (int)(FFTstep * fadeTime);
				float maxAmplitudeForNormalization = 0;

				float pi2 = MathF.PI * 2;
				float buf = pi2 / wavIn.sampleRate;


				float[] frqsNew = new float[channels];
				float[] ampsNew = new float[channels];

				float[] frqsOld = new float[channels];
				float[] ampsOld = new float[channels];

				float[] ampsLg = new float[channels];
				int[] idxsLg = new int[channels];

				float[] ampsLgOld = new float[channels];
				int[] idxsLgOld = new int[channels];

				double[] t = new double[channels];
				double[] tOld = new double[channels];


				outName += $" (FFT {FFTsize} ch {channels} tr {trashSize})";
				uint graphStep = wavIn.sampleRate / 60;
				long limit = Math.Min(wavIn.L.Length, wavIn.sampleRate * limitSec);

				bool ok = UserAsker.Ask($"Name: {outName}\nWave type: {waveForm}\nRecreation channels: {channels}\nFade time: {fadeTime}\nFFT size: {FFTsize}\nFFT per second: {FFTPerSecond}\nTrash size: {trashSize}\nLow frq smoothing size X: {lfsX}\nLow frq smoothing size Y: {lfsY}\nAmplitude threshold: {amplitudeThreshold}\nPoints connecting X: {pointsConnectingX}\nPoints connecting Y: {pointsConnectingY}\nDraw graph: {drawGraph}\nSamples: {limit}\nSample rate: {wavOut.sampleRate}\nSeconds: {(int)(limit / wavOut.sampleRate)}");
				if (!ok)
					return;

				FrequencyFinder.CalculateSmoothMask(lfsX, lfsY, FFTsize, wavOut.sampleRate);
				FindAmplitudeMaxForWholeTrack();

				ProgressShower.ShowProgress("Effect FFT multi audio recreation...");

				for (int s = 0; s < limit; s++)
				{
					if (s % FFTstep == 0)
					{
						frqsOld = frqsNew;
						ampsOld = ampsNew;
						ampsNew = new float[channels];
						frqsNew = new float[channels];

						FrequencyFinder.ByFFT(ref frqsNew, ref ampsNew, wavIn, s, FFTsize, trashSize);
						adaptiveCeiling = Math.Max(adaptiveCeiling, FrequencyFinder.amplitudeMax / FrequencyFinder.amplitudeMaxWholeTrack);

						FindLg();
						ConnectNewPoints(pointsConnectingX, pointsConnectingY);
						DeleteSmallAmplitudes();
						SetTimeOld();

						fadeSamplesLeft = samplesForFade;
					}
					else
						fadeSamplesLeft--;

					WriteSample(s);
					DrawGraph(s);
					CheckMaxAmplitude(s);
				}

				Normalize();

				ProgressShower.ShowProgress("Saving...");

				Export(outName);

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

				void FindLg()
				{
					for (int c = 0; c < channels; c++)
					{
						ampsLgOld[c] = ampsLg[c];
						ampsLg[c] = ampsNew[c];
						ampsLg[c] = Math2.ToLogScale(ampsLg[c], 10);
					}

					idxsLgOld = idxsLg;
					idxsLg = Array2.RescaleIndexesToLog2(FrequencyFinder.leadIndexes, FrequencyFinder.spectrum.Length);
				}

				void DrawGraph(int sampleNumber)
				{
					if (sampleNumber % graphStep == 0)
					{
						if (drawGraph)
						{
							//GraphDrawer.Draw($"{sampleNumber}", frqsLg, idxsLg, ampsLg, adaptiveCeiling, FrequencyFinder.amplitudeMaxWholeTrack);
							//GraphDrawer.DrawType3($"{sampleNumber}", idxsLg, ampsNew, adaptiveCeiling, FrequencyFinder.amplitudeMaxWholeTrack);
							GraphDrawer.DrawType2($"{sampleNumber}", idxsLg, ampsNew, adaptiveCeiling, FrequencyFinder.amplitudeMaxWholeTrack);
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

					float[] empty = new float[FFTsize];
					FrequencyFinder.spectrum = new float[FFTsize];
					FrequencyFinder.amplitudeMaxWholeTrack = 0;
					FrequencyFinder.amplitudeMax = 0;

					for (int i = 3; i < 23; i++)
					{
						FrequencyFinder.ByFFT(ref frqsNew, ref ampsNew, wavIn, i * step, FFTsize, trashSize);
						ProgressShower.SetProgress((i - 3) / 20.0);
					}

					ProgressShower.SetProgress(0.0);
				}

				void ConnectNewPoints(string typeX, string typeY)
				{
					int L = ampsNew.Length;
					int[,] distances = new int[L, L];
					float x = 0;
					float y = 0;

					for (int idNew = 0; idNew < L; idNew++)
						for (int idOld = 0; idOld < L; idOld++)
						{
							if (typeX == "logarithmic")
							{
								x = idxsLgOld[idOld] - idxsLg[idNew];
								x /= FrequencyFinder.spectrum.Length;
								x *= 4; //check
							}
							else if (typeX == "linear")
							{
								x = frqsOld[idOld] - frqsNew[idNew];
								x /= 20000; //Hz
								x *= 4; //check
							}
							else
								throw new ArgumentException("Smooth type incorrect");

							if (typeY == "logarithmic")
								y = ampsLgOld[idOld] - ampsLg[idNew];
							else if (typeY == "linear")
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
}
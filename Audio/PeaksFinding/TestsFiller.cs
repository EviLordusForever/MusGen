using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using MusGen;

namespace PeaksFinding
{
	public static class TestsFiller
	{
		public static void Fill(out InputData[] inputData)
		{
			inputData = new InputData[Params._testsCount];			

			for (int test = 0; test < Params._testsCount; test++)
			{
				inputData[test].amplitudes = CreateActualAnswer();
				inputData[test].spectrum = CreateActualQuestion(inputData[test].amplitudes);
			}
		}

		public static float[] CreateActualQuestion(float[] answer)
		{
			float[] signal = new float[AP.FftSize * AP._lc];
			float[] signalLow = new float[AP.FftSize * AP._lc];

			int count = 0;

			for (int index = 0; index < AP.SpectrumSize; index++)
				if (answer[index] != 0)
				{
					float frequency = SpectrumFinder._frequenciesLogarithmic[index];
					float phase = MathE.rnd.NextSingle() * MathF.PI * 2;

					for (int x = 0; x < AP.FftSize * AP._lc; x++)
					{
						float t = 1f * x / AP.SampleRate; //time in seconds
						signal[x] += MathF.Sin(2f * MathF.PI * frequency * t + phase);
					}

					count++;
				}

			for (int x = 0; x < AP.FftSize * AP._lc; x++)
			{
				signal[x] /= count;
				signalLow[x] = signal[x];
			}

			float cutOff = 0.5f * (AP.SampleRate / AP._lc);

			signalLow = KaiserFilter.Make(signalLow, AP.SampleRate, cutOff, AP._kaiserFilterLength_ForProcessing, AP._kaiserFilterBeta, false);

			float[] question = SpectrumFinder.Find(signal, signalLow);

			return question;
		}

		public static float[] CreateActualAnswer()
		{
			float[] answer = new float[AP.SpectrumSize];
			int sinusoidsCount = MathE.rnd.Next(Params._maxSinusoidsCount);

			for (int i = 0; i < sinusoidsCount; i++)
			{
				int index = MathE.rnd.Next(AP.SpectrumSize);
				answer[index] = MathE.rnd.NextSingle();
			}

			return answer;
		}
	}
}

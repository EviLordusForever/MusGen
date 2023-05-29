using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using MusGen;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace PeaksFinding
{
	public static class TestsFiller
	{
		public static InputData Fill()
		{
			string path = $"{DiskE._programFiles}\\PeaksFinderKerasTests.bin";
			if (File.Exists(path))
			{
				Logger.Log("Reading PFML tests from bin...");
				InputData inputData = new InputData();

				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					inputData._data = (float[][][])formatter.Deserialize(stream);
				}

				Logger.Log("Reading PFML tests from bin is Done!");
				return inputData;
			}
			else
			{
				ProgressShower.Show("Generating new PFML tests...");

				InputData inputData = new InputData();

				inputData.questions = new float[Params._testsCount][];
				inputData.answers = new float[Params._testsCount][];

				for (int test = 0; test < Params._testsCount; test++)
				{
					inputData.answers[test] = new float[AP.SpectrumSize];
					inputData.questions[test] = new float[AP.SpectrumSize];

					inputData.answers[test] = CreateActualAnswer();
					inputData.questions[test] = CreateActualQuestion(inputData.answers[test]);

					ProgressShower.Set(1.0 * test / Params._testsCount);
				}

				ProgressShower.Close();
				Logger.Log("PFML tests were filled! Now saving...");

				using (FileStream stream = new FileStream(path, FileMode.Create))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, inputData._data);
				}

				Logger.Log("PFML tests were saved!");

				return inputData;
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
					float frequency = SpectrumFinder._frequenciesLg[index];
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
			float x = MathE.rnd.NextSingle();
			float y = (1 - MathF.Pow(x, Params._sinusoidsCountP));
			int sinusoidsCount = (int)(y * Params._maxSinusoidsCount + 1);

			for (int i = 0; i < sinusoidsCount; i++)
			{
				int index = MathE.rnd.Next(AP.SpectrumSize);
				answer[index] = MathE.rnd.NextSingle();
			}

			return answer;
		}
	}
}

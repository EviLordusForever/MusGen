﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using MusGen;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;

namespace MusGen
{
	public static class TestsFiller2
	{
		public static List<FNadSample[]> _allQuestions;
		public static List<FNadSample> _allAnswers;

		public static void MakeAll()
		{
			ProgressShower.Show("Generating fnads for new tests...");

			string[] _midis = Directory.GetFiles($"{DiskE._programFiles}MIDIS");
			_allAnswers = new List<FNadSample>();
			_allQuestions = new List<FNadSample[]>();

			int accordNotesCount = 0;

			for (int m = 0; m < _midis.Length; m++)
			{
				Midi midi = new Midi();
				midi.Read(_midis[m]);
				Nad nad = midi.ToNad();
				FNad fnad = nad.ToFNad();

				int length = fnad._samples.Length;
				Logger.Log($"{_midis[m]}");
				Logger.Log($"Length: {length} fnad samples.");

				for (int t = 100; t < length; t++)
				{
					if (fnad._samples[t]._deltaTime <= Params._accordMaxTime)
						accordNotesCount++;
					else
					{
						_allAnswers.Add(fnad._samples[t]);
						FNadSample[] subArray = new FNadSample[100];
						Array.Copy(fnad._samples, t - 100, subArray, 0, 100);
						_allQuestions.Add(subArray);
					}
				}

				ProgressShower.Set(1.0 * m / _midis.Length);
			}

			ProgressShower.Close();

			Logger.Log($"Available {_allQuestions.Count} tests.", Brushes.Cyan);
			Logger.Log($"Removed {accordNotesCount} notes via accords.", Brushes.Red);
		}

		public static InputData Fill()
		{
			string path = $"{DiskE._programFiles}\\NNTests2.bin";
			if (File.Exists(path))
			{
				Logger.Log("Reading tests 2 from bin...");
				InputData inputData = new InputData();

				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					inputData._data = (float[][][])formatter.Deserialize(stream);
				}

				Logger.Log("Reading TESTS 2 from bin is Done!");
				Params._testsCount = inputData.questions.Count();
				return inputData;
			}
			else
			{
				MakeAll();

				Params._testsCount = _allQuestions.Count;

				ProgressShower.Show("Generating new tests 2...");

				InputData inputData = new InputData();

				inputData.questions = new float[Params._testsCount][];
				inputData.answers = new float[Params._testsCount][];

				for (int test = 0; test < Params._testsCount; test++)
				{
					inputData.answers[test] = CreateActualAnswer(test);
					inputData.questions[test] = CreateActualQuestion(test);

					ProgressShower.Set(1.0 * test / Params._testsCount);
				}

				ProgressShower.Close();
				Logger.Log("Tests 2 were filled! Now saving...");

				using (FileStream stream = new FileStream(path, FileMode.Create))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, inputData._data);
				}

				Logger.Log("Tests 2 were saved!");

				return inputData;
			}
		}

		public static float[] CreateActualQuestion(int test)
		{
			FNadSample[] fnadSamples = _allQuestions[test];
			FNadSample lastSample = _allAnswers[test];
			float[] question = new float[401];
			for (int i = 0; i < 100; i++)
			{
				question[i * 4 + 0] = fnadSamples[i]._index;
				question[i * 4 + 1] = fnadSamples[i]._amplitude;
				question[i * 4 + 2] = fnadSamples[i]._deltaTime;
				question[i * 4 + 3] = fnadSamples[i]._duration;
			}

			question[100 * 4 + 0] = lastSample._index;

			return question;
		}

		public static float[] CreateActualAnswer(int test)
		{
			float[] answer = new float[3];
			answer[0] = _allAnswers[test]._amplitude;
			answer[1] = _allAnswers[test]._deltaTime;
			answer[0] = _allAnswers[test]._duration;

			return answer;
		}
	}
}

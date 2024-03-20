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

namespace MusGen
{
	public static class TestsFiller1
	{
		public static List<FNadSample[]> _allQuestions;
		public static List<FNadSample[]> _allAnswers;

		public static void MakeAll()
		{
			ProgressShower.Show("Generating fnads for new tests...");

			string[] _midis = Directory.GetFiles($"{DiskE._programFiles}MIDIS");
			_allAnswers = new List<FNadSample[]>();
			_allQuestions = new List<FNadSample[]>();

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
					if (fnad._samples[t]._deltaTime > Params._accordMaxTime)
					{ ////////////////////////////////////////
						_allAnswers.Add(GetAccord(fnad, t));
						FNadSample[] subArray = new FNadSample[100];
						Array.Copy(fnad._samples, t - 100, subArray, 0, 100);
						_allQuestions.Add(subArray);
					}

				ProgressShower.Set(1.0 * m / _midis.Length);
			}

			ProgressShower.Close();

			Logger.Log($"Available {_allQuestions.Count} tests.");

			FNadSample[] GetAccord(FNad fnad, int t)
			{
				List<FNadSample> accord = new List<FNadSample>();
				accord.Add(fnad._samples[t]);

				for (int i = t + 1; i < fnad._samples.Length; i++)
					if (fnad._samples[i]._deltaTime <= Params._accordMaxTime) //////////////
						accord.Add(fnad._samples[i]);
					else
						break;


				return accord.ToArray();
			}
		}

		public static void Filter() //////////////////
		{
			int removedCount = 0;
			int allCount = _allQuestions.Count();

			for (int test = 0; test < _allQuestions.Count(); test++)
			{
				float index01 = _allAnswers[test][0]._index;
				byte noteNumber = SpectrumFinder.NoteNumberByIndex01(index01);

				float index01_2 = _allQuestions[test][99]._index;
				byte noteNumber_2 = SpectrumFinder.NoteNumberByIndex01(index01_2);

				if (noteNumber == noteNumber_2)
				{
					_allAnswers.RemoveAt(test);
					_allQuestions.RemoveAt(test);
					removedCount++;
				}
			}

			Logger.Log($"REMOVED {removedCount} from {allCount} because of repeating.");
			Params._testsCount = _allQuestions.Count();
		}

		public static InputData Fill()
		{		
			string path = $"{DiskE._programFiles}\\NNTests1.bin";
			if (File.Exists(path))
			{
				Logger.Log("Reading tests from bin...");
				InputData inputData = new InputData();

				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					inputData._data = (float[][][])formatter.Deserialize(stream);
				}

				Logger.Log("Reading TESTS from bin is Done!");
				Params._testsCount = inputData.questions.Count();
				return inputData;
			}
			else
			{
				MakeAll();
				Filter();

				Params._testsCount = _allQuestions.Count;

				ProgressShower.Show("Generating new tests...");

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
				Logger.Log("Tests were filled! Now saving...");

				using (FileStream stream = new FileStream(path, FileMode.Create))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, inputData._data);
				}

				Logger.Log("Tests were saved!");

				return inputData;
			}
		}

		public static float[] CreateActualQuestion(int test)
		{
			FNadSample[] fnadSamples = _allQuestions[test];
			float[] question = new float[100];
			for (int i = 0; i < 100; i++)
			{				
				question[i * 1 + 0] = fnadSamples[i]._index;
/*				question[i * 1 + 1] = fnadSamples[i]._amplitude;
				question[i * 1 + 2] = fnadSamples[i]._deltaTime;
				question[i * 1 + 3] = fnadSamples[i]._duration;*/
			}

			return question;
		}

		public static float[] CreateActualAnswer(int test)
		{
			float[] answer = new float[128];

			for (int noteInAccord = 0; noteInAccord < _allAnswers[test].Length; noteInAccord++)
			{
				float index01 = _allAnswers[test][noteInAccord]._index;
				byte noteNumber = SpectrumFinder.NoteNumberByIndex01(index01);
				answer[noteNumber] = 1f;// / _allAnswers[test].Length;
				//break;
			}

			return answer;
		}
	}
}

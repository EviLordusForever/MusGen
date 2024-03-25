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

namespace MusGen
{
	public static class TestsFillerTransformer
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
						FNadSample[] subArray = new FNadSample[t];
						Array.Copy(fnad._samples, 0, subArray, 0, t);
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

		public static InputDataRNN Fill()
		{		
			string path = $"{DiskE._programFiles}\\RNNTests1.bin";
			if (File.Exists(path))
			{
				Logger.Log("Reading tests from bin...");
				InputDataRNN data = new InputDataRNN();

				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					data._data = (float[][][])formatter.Deserialize(stream);
				}

				Logger.Log("Reading TESTS from bin is Done!");
				Params._testsCount = data.questions.Count();
				return data;
			}
			else
			{
				MakeAll();
				Params._testsCount = _allQuestions.Count;

				ProgressShower.Show("Generating new tests...");

				InputDataRNN inputData = new InputDataRNN();

				int maxSequnceLength = FindMaxSequenceLength();

				inputData.questions = new float[Params._testsCount][];
				inputData.answers = new float[Params._testsCount][];

				for (int test = 0; test < Params._testsCount; test++)
				{
					inputData.answers[test] = CreateActualAnswer(test);
					inputData.questions[test] = CreateActualQuestion(test, maxSequnceLength);

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

			int FindMaxSequenceLength()
			{
				int maxSequenceLength = 0;
				for (int i = 0; i < _allQuestions.Count; i++)
					if (_allQuestions[i].Length > maxSequenceLength)
						maxSequenceLength = _allQuestions[i].Length;
				return maxSequenceLength;
			}
		}

		public static float[] CreateActualQuestion(int test, int maxSequenceLength)
		{
			FNadSample[] fnadSamples = _allQuestions[test];
			float[] question = new float[maxSequenceLength];

			for (int i = 0; i < maxSequenceLength; i++)
			{
				int j = i - (maxSequenceLength - fnadSamples.Length);
				if (j >= 0)
					question[i] = fnadSamples[j]._index;
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
				answer[noteNumber] = 1f;
			}

			return answer;
		}
	}
}

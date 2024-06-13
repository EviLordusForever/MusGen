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
using System.Windows.Media;

namespace MusGen
{
	public static class TestsFillerHuge
	{
		public static List<FNadSample[][]> _allQuestions;
		public static List<FNadSample[]> _allAnswers;
		public static int _window = 50;

		public static void MakeAll()
		{
			ProgressShower.Show("Generating fnads for new tests...");

			string[] _midis = Directory.GetFiles($"{DiskE._programFiles}MIDIS");
			_allAnswers = new List<FNadSample[]>();
			_allQuestions = new List<FNadSample[][]>();

			for (int m = 0; m < _midis.Length; m++)
			{
				Midi midi = new Midi();
				midi.Read(_midis[m]);
				Nad nad = midi.ToNad();
				FNad fnad = nad.ToFNad();

				int length = fnad._samples.Length;
				Logger.Log($"{_midis[m]}");
				Logger.Log($"Length: {length} fnad samples.");

				List<FNadSample> samples = new List<FNadSample>();
				samples.AddRange(fnad._samples);

				float averageDeltaTime = FindAverageDeltaTime();
				
				List<FNadSample[]> accords = FillAccords();

				for (int t = _window; t < accords.Count; t++)
				{
					_allAnswers.Add(accords[t]);
					List<FNadSample[]> question = new List<FNadSample[]>();
					for (int i = t - _window; i < t; i++)
						question.Add(accords[i]);
					_allQuestions.Add(question.ToArray());
				}

				ProgressShower.Set(1.0 * m / _midis.Length);

				List<FNadSample[]> FillAccords()
				{
					List<FNadSample[]> accords = new List<FNadSample[]>();

					while (samples.Count > 0)
						accords.Add(GetAccord());

					accords = Normalize(accords);

					return accords;					
				}

				FNadSample[] GetAccord()
				{
					List<FNadSample> accord = new List<FNadSample>();
					accord.Add(samples[0]);
					samples.RemoveAt(0);

					while (samples.Count > 0)
					{
						if (samples[0]._deltaTime <= Params._accordMaxTime)
						{
							accord.Add(samples[0]);
							samples.RemoveAt(0);
						}
						else
							break;
					}

					return accord.ToArray();
				}

				List<FNadSample[]> Normalize(List<FNadSample[]> accords)
				{
					int lows = 0;
					int highs = 0;

					float averageDelta = 0;
					for (int i = 0; i < accords.Count; i++)
						averageDelta += accords[i][0]._deltaTime;

					averageDelta /= accords.Count;

					Logger.Log($"Average delta = {averageDelta}");

					for (int i = 1; i < accords.Count; i++)
						if (accords[i][0]._deltaTime <= averageDelta * 0.5f)
						{
							List<FNadSample> merged = new List<FNadSample>();
							merged.AddRange(accords[i - 1]);
							merged.AddRange(accords[i]);
							accords[i - 1] = merged.ToArray();
							accords.RemoveAt(i);
							lows++;
							i--;
						}

					for (int i = 1; i < accords.Count; i++)
						if (accords[i][0]._deltaTime > averageDelta * 1.5f)
						{
							FNadSample[] clone = new FNadSample[accords[i - 1].Length];
							for (int j = 0; j < clone.Length; j++)
								clone[j] = accords[i - 1][j].DeepClone();

							clone[0]._deltaTime = averageDelta;
							accords.Insert(i, clone);
							accords[i + 1][0]._deltaTime -= averageDelta;
							highs++;
						}

					Logger.Log($"REMOVED {lows} LOWS & {highs} HIGHS!", Brushes.Magenta);

					return accords;
				}

				float FindAverageDeltaTime()
				{
					float summ = 0;
					float count = 0;

					for (int i = 0; i < samples.Count; i++)
						if (samples[i]._deltaTime > Params._accordMaxTime)
						{
							summ += samples[i]._deltaTime;
							count++;
						}

					return summ / count;
				}
			}

			ProgressShower.Close();

			Logger.Log($"Available {_allQuestions.Count} tests.");
		}

		public static InputData Fill()
		{		
			string path = $"{DiskE._programFiles}\\NNTestsHuge.bin";
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
			FNadSample[][] accords = _allQuestions[test];

			float[] question = new float[_window * 128];

			for (int accordNumber = 0; accordNumber < _window; accordNumber++)
			{
				for (int noteInAccord = 0; noteInAccord < accords[accordNumber].Length; noteInAccord++)
				{
					float index01 = accords[accordNumber][noteInAccord]._index;
					byte noteNumber = SpectrumFinder.NoteNumberByIndex01(index01);
					question[accordNumber * 128 + noteNumber] = 1f;
				}
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

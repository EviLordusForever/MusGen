using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using Extensions;
using System.Globalization;
using MusGen.Audio.WorkFlows;
using System.IO;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.Keras.Engine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace MusGen
{
	public static class Generator
	{
		public static int _count = 1000;
		public static int _window = 50;

		public static void Generate()
		{
			int lastNoteNumber = 0;
			int lastNoteNumber2 = 0;
			List<int> noteNumbers = new List<int>();
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "audio files |*.mid;";
			dialog.Title = "Please select mid file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{				
				string path = dialog.FileName;
				string outname = dialog.SafeFileName;
				Midi midi = new Midi();
				midi.Read(path);
				Nad nad = midi.ToNad();
				FNad fnad = nad.ToFNad();
				FNadSample[] fnadSamples = fnad._samples;

				List<FNadSample> samples = new List<FNadSample>();
				for (int i = 0; i < 100; i++)
					samples.Add(fnadSamples[i]);
				
				IModel model1 = ModelManager.LoadModel1();


				float deltaTime = 0;
				int deltasCount = 0;
				for (int i = 0; i < fnadSamples.Length; i++)
				{
					if (fnadSamples[i]._deltaTime > Params._accordMaxTime)
					{
						deltaTime += fnadSamples[i]._deltaTime;
						deltasCount++;
					}
				}
				deltaTime /= deltasCount;

				ProgressShower.Show("Generating...");

				

				for (int j = 0; j < _count; j++)
				{
					float[] question = new float[100];
					for (int i = 0; i < 100; i++)
					{
						question[i * 1 + 0] = samples[i + j]._index;
/*						question[i * 1 + 1] = samples[i + j]._amplitude;
						question[i * 1 + 2] = samples[i + j]._deltaTime;
						question[i * 1 + 3] = samples[i + j]._duration;*/
					}

					Shape shape = new Shape(1, 100);
					Tensor tensor = constant_op.constant(question, shape: shape);
					tensor.shape = shape;

					Tensor ts = model1.predict(tensor);
					float[] answer = ts.ToArray<float>();

					// Preventing repeating:

					for (int i = noteNumbers.Count - 1; i >= 0 && i > noteNumbers.Count - 101; i--)
					{
						float x = noteNumbers.Count - 1 - i;
						//float decreaser = 1 - 1 / (x / 4 + 1);
						float decreaser = 1 - MathF.Exp(-x / 6f);
						answer[noteNumbers[i]] *= decreaser;
					}

					//Finding leading note:

					List<float> notes =	FindLeadingNotes(answer);

					for (int i = 0; i < notes.Count; i++)
					{
						FNadSample result = new FNadSample();
						result._index = notes[i];
						result._amplitude = 1;						

						result._deltaTime = deltaTime;
						if (i > 0)
							result._deltaTime = 0;

						result._absoluteTime = samples.Last()._absoluteTime + result._deltaTime;
						result._duration = result._deltaTime * 1.5f;

						samples.Add(result);
					}

					ProgressShower.Set(1.0 * j / (_count));
				}

				ProgressShower.Close();

				FNad music = new FNad();
				samples.RemoveRange(0, 100);
				music._samples = samples.ToArray();
				music.Export(outname);
				music.ToMidiAndExport(outname);//, 0.2f);
				Logger.Log("DONE.");
				//SaveList(samples, $"{DiskE._programFiles}delme.bin");
				//Reboot();
			}

			List<float> FindLeadingNotes(float[] answer)
			{
				List<float> notes = new List<float>();

				float max = 0;
				for (int noteNumber = 0; noteNumber < 128; noteNumber++)
					if (answer[noteNumber] > max)
						max = answer[noteNumber];

				float threshold = max * Params._accordThreshold;

				for (int noteNumber = 0; noteNumber < 128; noteNumber++)
					if (answer[noteNumber] > threshold)
					{
						float frequency = 440 * MathF.Pow(2, (noteNumber - 69) / 12f);
						ushort index = SpectrumFinder.IndexByFrequency(frequency);
						float index01 = 1f * index / SpectrumFinder._frequenciesLg.Length;

						notes.Add(index01);
						noteNumbers.Add(noteNumber);
					}

				return notes;
			}
		}

		public static void GenerateByHugeModel()
		{
			List<int> noteNumbersGlobal = new List<int>();
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "audio files |*.mid;";
			dialog.Title = "Please select mid file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{
				string path = dialog.FileName;
				string outname = TextE.StringBeforeLast(dialog.SafeFileName, ".");
				Midi midi = new Midi();
				midi.Read(path);
				Nad nad = midi.ToNad();
				FNad fnad = nad.ToFNad();
				FNadSample[] fnadSamples = fnad._samples;

				float averageDeltaTime = FindAverageDeltaTime();

				List<FNadSample> samples = new List<FNadSample>();
				for (int i = 0; i < fnadSamples.Length; i++)
					samples.Add(fnadSamples[i]);

				List<float> deltaTimes = new List<float>();
				List<float[]> accords = new List<float[]>();

				GetAccords();
				Normalize();
				accords.RemoveRange(_window, accords.Count - _window);

				samples.Clear();
				samples.Add(new FNadSample());

				Sequential model1 = ModelManager.LoadHugeModel(false);

				ProgressShower.Show("Generating...");

				//Tensors inputs = model1.inputs;
				//var outputLayer = model1.Layers.Last();
				//Graph graph = new Graph();				

				using (var session = new Session())
				{
					for (int j = 0; j < _count; j++)
					{
						List<float> question = new List<float>();

						for (int i = 0; i < _window; i++)
							question.AddRange(accords[j + i]);

						Shape shape = new Shape(1, _window * 128);
						Tensor tensor = constant_op.constant(question.ToArray(), shape: shape);
						tensor.shape = shape;

						Tensor ts = model1.predict(tensor, 1); //use_multiprocessing: true
						//var res = outputLayer.
						float[] answer = ts.ToArray<float>();


						// Preventing repeating:

						for (int i = noteNumbersGlobal.Count - 1; i >= 0 && i > noteNumbersGlobal.Count - 101; i--)
						{
							float x = noteNumbersGlobal.Count - 1 - i;
							float decreaser = 1 - MathF.Exp(-(x + 7) / 6f);
							answer[noteNumbersGlobal[i]] *= decreaser;
						}

						//

						List<int> notes = FindLeadingNotes(answer);

						float[] accord = new float[128];
						for (int i = 0; i < notes.Count; i++)
						{
							int noteNumber = notes[i];
							accord[noteNumber] = 1;

							FNadSample fnads = new FNadSample();
							fnads._index = SpectrumFinder.Index01ByNoteNumber(noteNumber);
							fnads._amplitude = 1;
							fnads._deltaTime = averageDeltaTime;
							if (i > 0)
								fnads._deltaTime = 0;

							fnads._absoluteTime = samples.Last()._absoluteTime + fnads._deltaTime;
							fnads._duration = averageDeltaTime * 7.975f; ////////////
																////////////////////////
							samples.Add(fnads);
						}
						accords.Add(accord);

						ProgressShower.Set(1.0 * j / (_count));
					}
				}

				ProgressShower.Close();

				FNad music = new FNad();
				music._samples = samples.ToArray();
				//music.Export(outname);
				music.ToMidiAndExport(outname);
				Logger.Log("DONE.");

				float FindAverageDeltaTime()
				{
					float averageDeltaTime = 0;
					int deltasCount = 0;
					for (int i = 0; i < fnadSamples.Length; i++)
					{
						if (fnadSamples[i]._deltaTime > Params._accordMaxTime)
						{
							averageDeltaTime += fnadSamples[i]._deltaTime;
							deltasCount++;
						}
					}
					averageDeltaTime /= deltasCount;
					return averageDeltaTime;
				}

				void GetAccords()
				{
					for (int i = 0; i < samples.Count; i++)
					{
						float[] accord = new float[128];
						float deltaTime = samples[0]._deltaTime;

						float index01 = samples[0]._index;
						int noteNumber = SpectrumFinder.NoteNumberByIndex01(index01);
						accord[noteNumber] = 1;
						
						samples.RemoveAt(0);

						while (samples.Count > 0)
						{
							if (samples[0]._deltaTime <= Params._accordMaxTime)
							{
								index01 = samples[0]._index;
								noteNumber = SpectrumFinder.NoteNumberByIndex01(index01);
								accord[noteNumber] = 1;
								samples.RemoveAt(0);
							}
							else
								break;
						}

						accords.Add(accord);
						deltaTimes.Add(deltaTime);
					}
				}

				void Normalize()
				{
					int lows = 0;
					int highs = 0;

					for (int i = 1; i < accords.Count; i++)
						if (deltaTimes[i] <= averageDeltaTime * 0.5f)
						{
							float[] merged = new float[128];
							for (int j = 0; j < merged.Length; j++)
								if (accords[i - 1][j] > 0 || accords[i][j] > 0)
									merged[j] = 1;

							accords[i - 1] = merged;
							accords.RemoveAt(i);
							deltaTimes.RemoveAt(i);
							lows++;
							i--;
						}

					for (int i = 1; i < deltaTimes.Count; i++)
						if (deltaTimes[i] > averageDeltaTime * 1.5f)
						{
							float[] clone = new float[128];
							for (int j = 0; j < clone.Length; j++)
								clone[j] = accords[i - 1][j];

							accords.Insert(i, clone);
							deltaTimes.Insert(i, averageDeltaTime);
							deltaTimes[i + 1] -= averageDeltaTime;
							highs++;
						}

					Logger.Log($"REMOVED {lows} LOWS & {highs} HIGHS!", Brushes.Magenta);
				}
			}

			List<int> FindLeadingNotes(float[] answer)
			{
				List<int> noteNumbers = new List<int>();

				float max = 0;
				for (int noteNumber = 0; noteNumber < 128; noteNumber++)
					if (answer[noteNumber] > max)
						max = answer[noteNumber];

				float threshold = max * Params._accordThreshold;

				for (int noteNumber = 0; noteNumber < 128; noteNumber++)
					if (answer[noteNumber] > threshold)
					{
						noteNumbersGlobal.Add(noteNumber);
						noteNumbers.Add(noteNumber);
					}

				return noteNumbers;
			}
		}

		private static void SaveList(List<FNadSample> dataList, string filePath)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
			{
				formatter.Serialize(fileStream, dataList);
			}
		}

		private static void Reboot()
		{
			MessageBox.Show("Now please reboot me!");
			Environment.Exit(0);
		}

		private static List<FNadSample> LoadList(string filePath)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
			{
				return (List<FNadSample>)formatter.Deserialize(fileStream);
			}
		}

		private static FNadSample[] ListOfListsToArray(List<List<FNadSample>> list)
		{
			List<FNadSample> result = new List<FNadSample>();

			foreach (List<FNadSample> sublist in list)
				result.AddRange(sublist);

			return result.ToArray();
		}

		public static void GeneratePart2()
		{
			string outname = "RESULT";
			string path = $"{DiskE._programFiles}delme.bin";
			if (File.Exists(path))
			{
				Thread myThread = new Thread(MyThread);
				myThread.Name = "GENERATING part 2";
				myThread.Start();

				void MyThread()
				{
					ProgressShower.Show("Generating...");
					ProgressShower.Set(0.5);

					List<FNadSample> samples0 = LoadList(path);
					List<List<FNadSample>> samples = new List<List<FNadSample>>();

					for (int i = 0; i < 100; i++)
						samples.Add(new List<FNadSample>() { samples0[i] });

					for (int i = 100; i < samples0.Count(); i++)
						if (samples0[i]._deltaTime == -1)
							samples.Add(new List<FNadSample>() { samples0[i] });
						else if (samples0[i]._deltaTime == 0)
							samples.Last().Add(samples0[i]);
						else
							throw new Exception("ERROR IN LIST!!!");

					IModel model2 = ModelManager.LoadModel2();

					for (int finalSampleNumber = 0; finalSampleNumber < _count; finalSampleNumber++)
					{
						float[] question2 = new float[401];
						for (int i = 0; i < 100; i++)
						{
							question2[i * 4 + 0] = samples[finalSampleNumber + i][0]._index;
							question2[i * 4 + 1] = samples[finalSampleNumber + i][0]._amplitude;
							question2[i * 4 + 2] = samples[finalSampleNumber + i][0]._deltaTime;
							question2[i * 4 + 3] = samples[finalSampleNumber + i][0]._duration;
						}

						question2[100 * 4 + 0] = samples[finalSampleNumber + 100][0]._index;

						Shape shape2 = new Shape(1, 401);
						Tensor tensor2 = constant_op.constant(question2, shape: shape2);
						tensor2.shape = shape2;

						Tensor ts2 = model2.predict(tensor2);
						float[] answer2 = ts2.ToArray<float>();

						//

						int currentNumber = finalSampleNumber + 100;

						float amplitude = Math.Max(Math.Min(answer2[0], 1), 0);
						float deltaTime = Math.Max(answer2[1], 0);
						float duration = Math.Max(answer2[2], 0);
						float absoluteTime = samples[currentNumber - 1][0]._absoluteTime + deltaTime;

						for (int numberInAccord = 0; numberInAccord < samples[currentNumber].Count(); numberInAccord++)
						{
							samples[currentNumber][numberInAccord]._amplitude = amplitude;
							if (numberInAccord == 0)
								samples[currentNumber][numberInAccord]._deltaTime = deltaTime;
							else
								samples[currentNumber][numberInAccord]._deltaTime = 0;
							samples[currentNumber][numberInAccord]._duration = duration;
							samples[currentNumber][numberInAccord]._absoluteTime = absoluteTime;
						}

						ProgressShower.Set((finalSampleNumber + _count) / (_count * 2.0));
					}

					ProgressShower.Close();

					FNad music = new FNad();
					samples.RemoveRange(0, 100);
					music._samples = ListOfListsToArray(samples);
					music.Export(outname);
					music.ToMidiAndExport(outname);//, 0.2f);
					File.Delete(path);
					Logger.Log("DONE.");

					Thread.Sleep(10000);
					Environment.Exit(0);


				}
			}
		}
	}
}

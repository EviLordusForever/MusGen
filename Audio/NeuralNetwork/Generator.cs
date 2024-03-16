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
		public static int _count = 1500;

		public static void Generate()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "audio files |*.mid;";
			dialog.Title = "Please select mid file";
			bool? success = dialog.ShowDialog();
			if (success == true)
			{				
				string path = dialog.FileName;
				Midi midi = new Midi();
				midi.Read(path);
				Nad nad = midi.ToNad();
				FNad fnad = nad.ToFNad();
				FNadSample[] fnadSamples = fnad._samples;

				List<FNadSample> samples = new List<FNadSample>();
				for (int i = 0; i < 100; i++)
					samples.Add(fnadSamples[i]);
				
				IModel model1 = ModelManager.LoadModel1();				

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

					for (int i = 0; i < 100; i++)
					{
						float index01_2 = question[i];
						int index_2 = (int)(index01_2 * SpectrumFinder._frequenciesLg.Length);
						float frequency_2 = SpectrumFinder._frequenciesLg[index_2];
						int noteNumber_2 = (int)(69 + 12 * MathF.Log2(frequency_2 / 440));
						float x = 100 - i;
						float decreaser = 1 - 1 / (x / 2 + 1);
						answer[noteNumber_2] *= decreaser;
					}

					//Finding leading note:

					float max = 0;
					float noteNumber = 0;
					for (int i = 0; i < 128; i++)
						if (answer[i] > max)
						{
							max = answer[i];
							noteNumber = i;
						}

					float frequency = 440 * MathF.Pow(2, (noteNumber - 69) / 12f);
					ushort index = SpectrumFinder.IndexByFrequency(frequency);
					float index01 = 1f * index / SpectrumFinder._frequenciesLg.Length;


					FNadSample result = new FNadSample();
					result._index = index01;
					result._amplitude = -1;
					result._deltaTime = -1;
					result._duration = -1;
					result._absoluteTime = -1;

					samples.Add(result);

					ProgressShower.Set(j / (_count * 2.0));
				}

				model1 = null;

				SaveList(samples, $"{DiskE._programFiles}delme.bin");
				Reboot();
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

					List<FNadSample> samples = LoadList(path);
					

					IModel model2 = ModelManager.LoadModel2();

					for (int j = 0; j < _count; j++)
					{
						float[] question2 = new float[401];
						for (int i = 0; i < 100; i++)
						{
							question2[i * 4 + 0] = samples[j + i]._index;
							question2[i * 4 + 1] = samples[j + i]._amplitude;
							question2[i * 4 + 2] = samples[j + i]._deltaTime;
							question2[i * 4 + 3] = samples[j + i]._duration;
						}

						question2[100 * 4 + 0] = samples[j + 100]._index;

						Shape shape2 = new Shape(1, 401);
						Tensor tensor2 = constant_op.constant(question2, shape: shape2);
						tensor2.shape = shape2;

						Tensor ts2 = model2.predict(tensor2);
						float[] answer2 = ts2.ToArray<float>();

						//

						samples[j + 100]._amplitude = Math.Max(Math.Min(answer2[0], 1), 0);
						samples[j + 100]._deltaTime = Math.Max(answer2[1], 0);
						samples[j + 100]._duration = Math.Max(answer2[2], 0);
						samples[j + 100]._absoluteTime = samples[j + 100 - 1]._absoluteTime + samples[j + 100]._deltaTime;

						ProgressShower.Set((j + _count) / (_count * 2.0));
					}

					ProgressShower.Close();

					FNad music = new FNad();
					samples.RemoveRange(0, 100);
					music._samples = samples.ToArray();
					music.ToMidiAndExport(outname);
					File.Delete(path);
					Logger.Log("DONE.");

					Thread.Sleep(10000);
					Environment.Exit(0);
				}
			}
		}
	}
}

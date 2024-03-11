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
	public static class TestsFiller
	{
		public static InputData Fill()
		{
			string path = $"{DiskE._programFiles}\\PeaksFinderKerasTests.bin";
			if (File.Exists(path))
			{
				Logger.Log("Reading tests from bin...");
				InputData inputData = new InputData();

				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					inputData._data = (float[][][])formatter.Deserialize(stream);
				}

				Logger.Log("Reading tests from bin is Done!");
				return inputData;
			}
			else
			{
				ProgressShower.Show("Generating new tests...");

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

		public static float[] CreateActualQuestion(float[] answer)
		{
			return null;
		}

		public static float[] CreateActualAnswer()
		{
			return null;
		}
	}
}

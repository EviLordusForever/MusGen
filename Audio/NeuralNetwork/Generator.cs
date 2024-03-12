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

namespace MusGen
{
	public static class Generator
	{
		public static void Generate(string outname)
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

				IModel model = ModelManager.Load();

				ProgressShower.Show("Generating...");

				for (int j = 0; j < 300; j++)
				{
					float[] question = new float[400];
					for (int i = 0; i < 100; i++)
					{
						question[i * 4 + 0] = samples[i + j]._index;
						question[i * 4 + 1] = samples[i + j]._amplitude;
						question[i * 4 + 2] = samples[i + j]._deltaTime;
						question[i * 4 + 3] = samples[i + j]._duration;
					}

					Shape shape = new Shape(1, 400);
					Tensor tensor = constant_op.constant(question, shape: shape);
					tensor.shape = shape;

					Tensor ts = model.predict(tensor);
					float[] answer = ts.ToArray<float>();

					FNadSample result = new FNadSample();
					result._index = answer[0];
					result._amplitude = answer[1];
					result._deltaTime = answer[2];
					result._duration = answer[3];
					result._absoluteTime = samples.Last()._absoluteTime + result._deltaTime;

					samples.Add(result);

					ProgressShower.Set(j / 300.0);
				}

				ProgressShower.Close();

				FNad music = new FNad();
				samples.RemoveRange(0, 100);
				music._samples = samples.ToArray();
				music.ToMidiAndExport(outname);
				Logger.Log("DONE.");
			}
		}
	}
}

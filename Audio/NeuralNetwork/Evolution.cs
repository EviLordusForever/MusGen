using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using Tensorflow.Keras.Models;
using Tensorflow.Keras.ArgsDefinition;
using static Tensorflow.Binding;
using Tensorflow.Keras;
using Tensorflow.Operations.Activation;
using Tensorflow.Keras.Optimizers;
using Tensorflow.NumPy;
using Newtonsoft.Json;
using Extensions;
using System.Windows.Media;
using System.IO;

namespace MusGen
{
	public static class Evolution
	{
		public static void EVOLVE()
		{
			InputData data = TestsFiller1.Fill();

			NDArray xTrain = np.array(data.questions.SelectMany(x => x).ToArray());
			xTrain = xTrain.reshape(new Shape(Params._testsCount, 100));

			NDArray yTrain = np.array(data.answers.SelectMany(x => x).ToArray());
			yTrain = yTrain.reshape(new Shape(Params._testsCount, 128));

			Sequential model = ModelManager.LoadModel1();

			for (int i = 0; ; i++)
			{
				var history = model.fit(xTrain, yTrain, epochs: 1);
				float mae = history.history["mean_absolute_error"][0];
				float loss = history.history["loss"][0];
				float accuracy = history.history["accuracy"][0];
				Logger.Log($"Epoch {i} done. Mae {mae}. Accuracy {accuracy}. Loss {loss}.");

				DiskE.WriteToProgramFiles("historyM1", "csv", $"{mae};{accuracy};{loss}\n", true);

				if ((i + 1) % Params._savingEvery == 0)
				{
					model.save_weights(Params._model1Path);
					Logger.Log($"Model was saved!", Brushes.Blue);
				}
			}
		}

		public static void EVOLVE_2()
		{
			InputData data = TestsFiller2.Fill();

			NDArray xTrain = np.array(data.questions.SelectMany(x => x).ToArray());
			xTrain = xTrain.reshape(new Shape(Params._testsCount, 401));

			NDArray yTrain = np.array(data.answers.SelectMany(x => x).ToArray());
			yTrain = yTrain.reshape(new Shape(Params._testsCount, 3));

			Sequential model = ModelManager.LoadModel2();

			for (int i = 0; ; i++)
			{
				var history = model.fit(xTrain, yTrain, epochs: 1);
				float mae = history.history["mean_absolute_error"][0];
				float loss = history.history["loss"][0];
				Logger.Log($"Epoch {i} done. Mae {mae}. Loss {loss}.");

				DiskE.WriteToProgramFiles("historyM2", "csv", $"{mae};{loss}\n", true);

				if ((i + 1) % Params._savingEvery == 0)
				{
					model.save_weights(Params._model2Path);
					Logger.Log($"Model 2 was saved!", Brushes.Blue);
				}
			}
		}

		public static void EVOLVE_RNN_1()
		{
			Logger.Log("Started evolution for RNN1.", Brushes.Magenta);

			InputDataRNN data = TestsFillerRNN1.Fill();

			int maxSequenceLength = data.MaxSequenceLength;

			Shape shape1 = new Shape(data.questions.Length, maxSequenceLength);
			NDArray xTrain = np.array(data.questions.SelectMany(x => x).ToArray());
			xTrain = xTrain.reshape(shape1);

			Shape shape2 = new Shape(data.answers.Length, 128);
			NDArray yTrain = np.array(data.answers.SelectMany(x => x).ToArray());
			yTrain = yTrain.reshape(shape2);

			Sequential model = ModelManager.LoadRNN1(maxSequenceLength);

			for (int i = 0; ; i++)
			{
				if (model == null)
					return;
				if (xTrain == null)
					return;
				if (yTrain == null)
					return;

				try
				{
					var history = model.fit(xTrain, yTrain, epochs: 1);

					float mae = history.history["mean_absolute_error"][0];
					float loss = history.history["loss"][0];
					float accuracy = history.history["accuracy"][0];
					Logger.Log($"Epoch {i} done. Mae {mae}. Accuracy {accuracy}. Loss {loss}.");

					DiskE.WriteToProgramFiles("historyRNN1", "csv", $"{mae};{accuracy};{loss}\n", true);
				}
				catch (NullReferenceException ex)
				{
				}
				
				if ((i + 1) % Params._savingEvery == 0)
				{
					model.save_weights(Params._model1Path);
					Logger.Log($"RNN1 was saved!", Brushes.Blue);
				}
			}
		}
	}
}

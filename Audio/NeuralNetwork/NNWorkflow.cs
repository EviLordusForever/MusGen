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
	public static class NNWorkflow
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
	}
}

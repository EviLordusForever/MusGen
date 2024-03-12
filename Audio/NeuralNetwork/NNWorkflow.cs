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
			InputData data = TestsFiller.Fill();

			NDArray xTrain = np.array(data.questions.SelectMany(x => x).ToArray());
			xTrain = xTrain.reshape(new Shape(Params._testsCount, 400));

			NDArray yTrain = np.array(data.answers.SelectMany(x => x).ToArray());
			yTrain = yTrain.reshape(new Shape(Params._testsCount, 4));

			IModel model = ModelManager.Load();

			for (int i = 0; ; i++)
			{
				var history = model.fit(xTrain, yTrain, Params._batchSize, 1);
				float mae = history.history["mean_absolute_error"][0];
				float loss = history.history["loss"][0];
				Logger.Log($"Epoch {i} done. Mae {mae}. Loss {loss}");

				DiskE.WriteToProgramFiles("history", "csv", $"\n{mae};{loss}", true);

				if ((i + 1) % Params._savingEvery == 0)
				{
					model.save_weights(Params._modelPath);
					Logger.Log($"Model was saved!", Brushes.Blue);
				}
			}
		}
	}
}

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

namespace PeaksFinding
{
	public static class Workflow
	{
		public static void Make()
		{
			InputData data = TestsFiller.Fill();

			Shape newShape = new Shape(Params._testsCount, AP.SpectrumSize);

			NDArray xTrain = np.array(data.questions.SelectMany(x => x).ToArray());
			xTrain = xTrain.reshape(newShape);

			NDArray yTrain = np.array(data.answers.SelectMany(x => x).ToArray());
			yTrain = yTrain.reshape(newShape);

			IModel model = ModelManager.GetModel();

			for (int i = 0; i < Params._epochs; i++)
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
				//512 * 2 * 30000
			}
		}
	}
}

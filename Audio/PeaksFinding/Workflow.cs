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

			var model = KerasApi.keras.Sequential();

			var shape = new Shape(AP.SpectrumSize);
			var layer0 = KerasApi.keras.layers.InputLayer(shape);
			model.add(layer0);

			var layer1 = KerasApi.keras.layers.Dense(AP.SpectrumSize, activation: "tanh");
			model.add(layer1);

			var layer2 = KerasApi.keras.layers.Dense(AP.SpectrumSize, activation: "sigmoid");
			model.add(layer2);

			var loss = "mean_squared_error";
			var optimizer = "adam";
			var metrics = new[] { "accuracy" };

			model.compile(optimizer, loss, metrics);

			for (int i = 0; i < Params._epochs; i++)
			{
				var history = model.fit(xTrain, yTrain, Params._batchSize, 1);
				Logger.Log($"Epoch {i} done. Accuracy {history.history["accuracy"][0]}");
			}
		}
	}
}

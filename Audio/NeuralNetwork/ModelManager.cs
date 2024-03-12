using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Extensions;
using System.IO;
using System.Windows.Media;
using Tensorflow.Keras;

namespace MusGen
{
	public static class ModelManager
	{
		public static Sequential _model;

		public static void CreateFirstModel()
		{
			var inputs = KerasApi.keras.Input(shape: 400);

			var layers = KerasApi.keras.layers;

			var act1 = KerasApi.keras.activations.Tanh;
			var layer1 = layers.Dense(50, activation: act1).Apply(inputs);

			var act2 = KerasApi.keras.activations.Tanh;
			var layer2 = layers.Dense(50, activation: act2).Apply(layer1);

			var act4 = KerasApi.keras.activations.Softmax;
			var outputs = layers.Dense(128, activation: act4).Apply(layer2);

			var model = KerasApi.keras.Model(inputs, outputs, "GG");

			var loss = "categorical_crossentropy";
			var optimizer = "adam";
			var metrics = new[] { "mae", "accuracy" };

			model.compile(optimizer, loss, metrics);
		}

		public static Sequential CreateSecondModel()
		{
			_model = KerasApi.keras.Sequential();

			var shape = new Shape(400); //100 * 4
			var layer0 = KerasApi.keras.layers.InputLayer(shape);
			_model.add(layer0);

			var act0 = KerasApi.keras.activations.Tanh;
			var layer1 = KerasApi.keras.layers.Dense(30, activation: act0, use_bias: true);
			_model.add(layer1);

			var act1 = KerasApi.keras.activations.Tanh;
			var layer2 = KerasApi.keras.layers.Dense(30, activation: act1, use_bias: true);
			_model.add(layer2);

			var act2 = KerasApi.keras.activations.Linear;
			var layer3 = KerasApi.keras.layers.Dense(3, activation: act2, use_bias: true);
			_model.add(layer3);

			var loss = "mean_squared_error";
			var optimizer = "adam";
			var metrics = new[] { "mae" };

			_model.compile(optimizer, loss, metrics);

			return _model;
		}		

		public static IModel Load()
		{
			IModel model = CreateSecondModel();
			Logger.Log("Neural Network Model was initialized.", Brushes.Magenta);

			if (File.Exists(Params._modelPath))
			{
				model.load_weights(Params._modelPath);
				Logger.Log("Neural Network Model weights were loaded!", Brushes.Magenta);
			}

			return model;
		}
	}
}

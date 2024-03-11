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

		public static Sequential Create()
		{
			_model = KerasApi.keras.Sequential();

			var shape = new Shape(1000);
			var layer0 = KerasApi.keras.layers.InputLayer(shape);
			_model.add(layer0);

			var act0 = KerasApi.keras.activations.Tanh;
			var layer1 = KerasApi.keras.layers.Dense(50, activation: act0, use_bias: true);
			_model.add(layer1);

			var act1 = KerasApi.keras.activations.Tanh;
			var layer2 = KerasApi.keras.layers.Dense(50, activation: act1, use_bias: true);
			_model.add(layer2);

			var act2 = KerasApi.keras.activations.Tanh;
			var layer3 = KerasApi.keras.layers.Dense(10, activation: act2, use_bias: true);
			_model.add(layer3);

			var loss = "mean_squared_error";
			var optimizer = "adam";
			var metrics = new[] { "mae" };

			_model.compile(optimizer, loss, metrics);

			return _model;
		}

		public static IModel Load()
		{
			IModel model = Create();
			MusGen.Logger.Log("Peaks Finding Model was initialized.");

			if (File.Exists(Params._modelPath))
			{
				model.load_weights(Params._modelPath);
				MusGen.Logger.Log("Peaks Finding Model weights were loaded!", Brushes.Cyan);
			}

			return model;
		}
	}
}

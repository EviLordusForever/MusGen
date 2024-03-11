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

namespace PeaksFinding
{
	public static class PeaksModelManager
	{
		public static IModel GetModel()
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

		public static Sequential Create()
		{
			var model = KerasApi.keras.Sequential();

			var shape = new Shape(MusGen.AP.SpectrumSize);
			var layer0 = KerasApi.keras.layers.InputLayer(shape);
			model.add(layer0);

			var act0 = KerasApi.keras.activations.Tanh;
			var layer1 = KerasApi.keras.layers.Dense(MusGen.AP.SpectrumSize, activation: act0, use_bias: true);
			model.add(layer1);

			var act = KerasApi.keras.activations.Sigmoid;
			var layer2 = KerasApi.keras.layers.Dense(MusGen.AP.SpectrumSize, activation: act, use_bias: true);
			model.add(layer2);

			var loss = "mean_squared_error";
			var optimizer = "adam";
			var metrics = new[] { "mae" };

			model.compile(optimizer, loss, metrics);

			return model;
		}
	}
}

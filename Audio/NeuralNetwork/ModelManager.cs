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
		public static Sequential _model1;
		public static Sequential _model2;

		public static Sequential CreateFirstModel()
		{
			_model1 = KerasApi.keras.Sequential();

			var shape = new Shape(100); //100 * 4
			var layer0 = KerasApi.keras.layers.InputLayer(shape);
			_model1.add(layer0);

			var act1 = KerasApi.keras.activations.Tanh;
			var layer1 = KerasApi.keras.layers.Dense(100, activation: act1, use_bias: true);
			_model1.add(layer1);

			var act2 = KerasApi.keras.activations.Tanh;
			var layer2 = KerasApi.keras.layers.Dense(90, activation: act2, use_bias: true);
			_model1.add(layer2);

			var act3 = KerasApi.keras.activations.Tanh;
			var layer3 = KerasApi.keras.layers.Dense(90, activation: act3, use_bias: true);
			_model1.add(layer3);

			var act4 = KerasApi.keras.activations.Tanh;
			var layer4 = KerasApi.keras.layers.Dense(90, activation: act4, use_bias: true);
			_model1.add(layer4);

			var act5 = KerasApi.keras.activations.Softmax;
			var layer5 = KerasApi.keras.layers.Dense(128, activation: act5, use_bias: true);
			_model1.add(layer5);

			var loss = "categorical_crossentropy";
			var optimizer = "adam";
			var metrics = new[] { "mae", "accuracy" };

			_model1.compile(optimizer, loss, metrics);

			return _model1;
		}

		public static Sequential CreateSecondModel()
		{
			_model2 = KerasApi.keras.Sequential();

			var shape = new Shape(401);
			var layer0 = KerasApi.keras.layers.InputLayer(shape);
			_model2.add(layer0);

			var act1 = KerasApi.keras.activations.Tanh;
			var layer1 = KerasApi.keras.layers.Dense(60, activation: act1, use_bias: true);
			_model2.add(layer1);

			var act2 = KerasApi.keras.activations.Tanh;
			var layer2 = KerasApi.keras.layers.Dense(60, activation: act2, use_bias: true);
			_model2.add(layer2);

			var act3 = KerasApi.keras.activations.Tanh;
			var layer3 = KerasApi.keras.layers.Dense(60, activation: act3, use_bias: true);
			_model2.add(layer3);

			var act4 = KerasApi.keras.activations.Linear;
			var layer4 = KerasApi.keras.layers.Dense(3, activation: act4, use_bias: true);
			_model2.add(layer4);

			var loss = "mean_squared_error";
			var optimizer = "adam";
			var metrics = new[] { "mae" };

			_model2.compile(optimizer, loss, metrics);

			return _model2;
		}		

		public static Sequential LoadModel1()
		{
			Sequential model = CreateFirstModel();
			Logger.Log("Neural Network Model 1 was initialized.", Brushes.Magenta);

			if (File.Exists(Params._model1Path))
			{
				model.load_weights(Params._model1Path);
				Logger.Log("Neural Network Model 1 weights were loaded!", Brushes.Magenta);
			}

			return model;
		}

		public static Sequential LoadModel2()
		{
			Sequential model2 = CreateSecondModel();
			Logger.Log("Neural Network Model 2 was initialized.", Brushes.Magenta);

			if (File.Exists(Params._model2Path))
			{
				model2.load_weights(Params._model2Path);
				Logger.Log("Neural Network Model 2 weights were loaded!", Brushes.Magenta);
			}

			return model2;
		}
	}
}

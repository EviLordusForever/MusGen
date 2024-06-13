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
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow.Keras.Metrics;
using Tensorflow.Keras.Text;

namespace MusGen
{
	public static class ModelManager
	{
		public static Sequential model;
		public static Sequential _model2;
		public static Sequential _modelRNN;

		public static Sequential CreateFirstModel()
		{
			model = keras.Sequential();

			var shape = new Shape(100); //100 * 4
			var layer0 = keras.layers.InputLayer(shape);
			model.add(layer0);

			var act1 = keras.activations.Tanh;
			var layer1 = keras.layers.Dense(600, activation: act1, use_bias: true);
			model.add(layer1);

			var act2 = keras.activations.Tanh;
			var layer2 = keras.layers.Dense(600, activation: act2, use_bias: true);
			model.add(layer2);

			var act3 = keras.activations.Tanh;
			var layer3 = keras.layers.Dense(600, activation: act3, use_bias: true);
			model.add(layer3);

/*			var act4 = keras.activations.Tanh;
			var layer4 = keras.layers.Dense(600, activation: act4, use_bias: true);
			model.add(layer4);*/

			var act5 = keras.activations.Sigmoid;
			//var act5 = keras.activations.Softmax;
			var layer5 = keras.layers.Dense(128, activation: act5, use_bias: true);
			model.add(layer5);

			var loss = "binary_crossentropy";
			var optimizer = "adam";
			var metrics = new[] { "mae", "accuracy" };

			model.compile(optimizer, loss, metrics);

			return model;
		}

		public static Sequential CreateSecondModel()
		{
			_model2 = keras.Sequential();

			var shape = new Shape(401);
			var layer0 = keras.layers.InputLayer(shape);
			_model2.add(layer0);

			var act1 = keras.activations.Tanh;
			var layer1 = keras.layers.Dense(100, activation: act1, use_bias: true);
			_model2.add(layer1);

			var act2 = keras.activations.Tanh;
			var layer2 = keras.layers.Dense(100, activation: act2, use_bias: true);
			_model2.add(layer2);

			var act3 = keras.activations.Tanh;
			var layer3 = keras.layers.Dense(100, activation: act3, use_bias: true);
			_model2.add(layer3);

			var act4 = keras.activations.Linear;
			var layer4 = keras.layers.Dense(3, activation: act4, use_bias: true);
			_model2.add(layer4);

			var loss = "mean_squared_error";
			var optimizer = "adam";
			var metrics = new[] { "mae" };

			_model2.compile(optimizer, loss, metrics);

			return _model2;
		}

		public static Sequential CreateTransformer(int maxSequenceLength)
		{
			// Параметры модели трансформера
			int numLayers = 4;
			int numHeads = 8;
			int keyDim = 64;
			int dff = 512;

			model = keras.Sequential();

			int vocabSize = 128;
			var tokenizer = new Tokenizer(vocabSize);
			var words = new List<string>();
			for (int i = 0; i < 128; i++)
				words.Add(i.ToString());
			tokenizer.fit_on_texts(words);

			var shape = new Shape(maxSequenceLength);
			var inputLayer = keras.layers.Input(shape);
			model.add(inputLayer);

			// Добавление слоев трансформера
			for (int i = 0; i < numLayers; i++)
			{
				var attentionLayer = keras.layers.MultiHeadAttention(numHeads, keyDim);
				model.add(attentionLayer);

				// Слой нормализации после слоя внимания
				var normalizationLayer = keras.layers.LayerNormalization(axis: -1);
				model.add(normalizationLayer);

				// Полносвязный слой
				var denseLayer = keras.layers.Dense(units: dff, activation: keras.activations.Tanh);
				model.add(denseLayer);

				// Еще один слой нормализации после полносвязного слоя
				normalizationLayer = keras.layers.LayerNormalization(axis: -1);
				model.add(normalizationLayer);
			}

			var act5 = keras.activations.Sigmoid;
			var outputLayer = keras.layers.Dense(128, activation: act5, use_bias: true);
			model.add(outputLayer);

			var loss = "binary_crossentropy";
			var optimizer = "adam";
			var metrics = new[] { "mae", "accuracy" };

			model.compile(optimizer, loss, metrics);

			return model;
		}

		public static Sequential CreateHugeModel(bool needCompile)
		{
			model = keras.Sequential();

			var shape = new Shape(50 * 128);
			var layer0 = keras.layers.InputLayer(shape);
			model.add(layer0);

			var act1 = keras.activations.Tanh;
			var layer1 = keras.layers.Dense(100, activation: act1, use_bias: true);
			model.add(layer1);

			var act2 = keras.activations.Tanh;
			var layer2 = keras.layers.Dense(100, activation: act2, use_bias: true);
			model.add(layer2);

			var act3 = keras.activations.Tanh;
			var layer3 = keras.layers.Dense(100, activation: act3, use_bias: true);
			model.add(layer3);

			var act5 = keras.activations.Sigmoid;
			var layer5 = keras.layers.Dense(128, activation: act5, use_bias: true);
			model.add(layer5);

			var loss = "binary_crossentropy";
			var optimizer = "adam";
			var metrics = new[] { "mae", "accuracy" };

			if (needCompile)
				model.compile(optimizer, loss, metrics);

			return model;
		}

		public static Sequential CreateModelRNN(int maxSequenceLength)
		{
			//Does not work.
			model = keras.Sequential();

			var shape = new Shape(maxSequenceLength);
			var layer0 = keras.layers.InputLayer(shape);
			model.add(layer0);

			var act1 = keras.activations.Tanh;
			var layer1 = keras.layers.LSTM(140, act1);
			model.add(layer1);

			var act2 = keras.activations.Softmax;
			var layer2 = keras.layers.Dense(128, activation: act2, use_bias: true);
			model.add(layer2);

			var loss = "categorical_crossentropy";
			var optimizer = "adam";
			var metrics = new[] { "mae", "accuracy" };

			model.compile(optimizer, loss, metrics);

			return model;
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

		public static Sequential LoadHugeModel()
		{
			return LoadHugeModel(true);
		}

		public static Sequential LoadHugeModel(bool needCompile)
		{
			Sequential model = CreateHugeModel(needCompile);
			Logger.Log("Huge model was initialized.", Brushes.Magenta);

			if (File.Exists(Params._hugeModelPath))
			{
				model.load_weights(Params._hugeModelPath);
				Logger.Log("Huge model weights were loaded!", Brushes.Magenta);
			}

			return model;
		}

		public static Sequential LoadTransformer(int maxSequenceLength)
		{
			Sequential model = CreateTransformer(maxSequenceLength);
			Logger.Log("Transformer was initialized.", Brushes.Magenta);

			if (File.Exists(Params._transformerPath))
			{
				model.load_weights(Params._transformerPath);
				Logger.Log("Transformer weights were loaded!", Brushes.Magenta);
			}

			return model;
		}

		public static Sequential LoadRNN1(int maxSequenceLength)
		{
			Sequential model = CreateModelRNN(maxSequenceLength);
			Logger.Log("RNN 1 was initialized.", Brushes.Magenta);

			if (File.Exists(Params._rnn1Path))
			{
				model.load_weights(Params._rnn1Path);
				Logger.Log("RNN 1 weights were loaded!", Brushes.Magenta);
			}

			return model;
		}
	}
}
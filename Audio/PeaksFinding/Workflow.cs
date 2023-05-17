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

namespace PeaksFinding
{
	public static class Workflow
	{
		public static void Make2()
		{
			InputData data = TestsFiller.Fill();

			var model = KerasApi.keras.Sequential();

			var args = new InputLayerArgs();
			args.InputShape = new Shape(AP.SpectrumSize);

			model.add(new InputLayer(args));

			LayerNormalizationArgs args2 = new LayerNormalizationArgs();
			args2.Name = "gg";
			
			model.add(new LayerNormalization(args2));

			DenseArgs denseArgs = new DenseArgs();
			denseArgs.Activation = new Tensorflow.Keras.Activation();
			Dense layer1 = new Dense(denseArgs);

			model.add(layer1);
		}
	}
}

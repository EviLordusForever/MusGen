using System;
using static ELFMusGen.Logger;


namespace ELFMusGen
{
	public static class Builder
	{
		public static NN CreateBasicGoodPerceptron()
		{
			NN nn = new NN();

			nn._horizon = 30;
			nn._inputWindow = 300;
			nn._weightsInitMin = -1f;
			nn._weightsInitMax = 1f;

			nn._gradientCutter = 10f;

			nn._biasInput = 0.01f;

			nn._LEARNING_RATE = 0.001f; //0.05f
			nn._MOMENTUM = 0f; //0.8f

			nn._statisticsRecalculatePeriod = 50;
			nn._validationRecalculatePeriod = 30;

			nn._inputAF = new SoftSign();
			nn._answersAF = new SoftSign();

			nn._testerT = new Tester(nn, 4000, "Graph//ForTraining", "TRAINING", 2, 0, 0);
			nn._testerV = new Tester(nn, 2000, "Graph//ForValidation", "VALIDATION", 2, 0, 0);

			nn._layers.Add(new LayerPerceptron(nn, 8, 300, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 8, 8, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 5, 8, 0, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 1, 5, 0f, new SoftSign())); //40 x 15 = 600

			nn.FillWeightsRandomly();

			Log("New Neural Network created!");
			return nn;
		}

		public static NN CreateBasicGoodPerceptronOriginal()
		{
			NN nn = new NN();

			nn._horizon = 30;
			nn._inputWindow = 300;
			nn._weightsInitMin = -1f;
			nn._weightsInitMax = 1f;

			nn._gradientCutter = 10f;

			nn._biasInput = 0.01f;

			nn._LEARNING_RATE = 0.001f; //0.05f
			nn._MOMENTUM = 0f; //0.8f

			nn._inputAF = new SoftSign();
			nn._answersAF = new SoftSign();

			nn._statisticsRecalculatePeriod = 50;
			nn._validationRecalculatePeriod = 30;

			nn._testerT = new Tester(nn, 21000, "Graph//ForTraining", "TRAINING", 0, 0, 0);
			nn._testerV = new Tester(nn, 8000, "Graph//ForValidation", "VALIDATION", 0, 0, 0);

			nn._layers.Add(new LayerPerceptron(nn, 8, 300, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 8, 8, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 5, 8, 0, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 1, 5, 0f, new SoftSign())); //40 x 15 = 600

			nn.FillWeightsRandomly();

			Log("New Neural Network created!");
			return nn;
		}

		public static NN CreateBasicGoodPerceptronDerivative()
		{
			NN nn = new NN();

			nn._horizon = 30;
			nn._inputWindow = 300;
			nn._weightsInitMin = -1f;
			nn._weightsInitMax = 1f;

			nn._gradientCutter = 10f;

			nn._biasInput = 0.01f;

			nn._LEARNING_RATE = 0.001f; //0.05f
			nn._MOMENTUM = 0f; //0.8f

			nn._inputAF = new SoftSign();
			nn._answersAF = new SoftSign();

			nn._statisticsRecalculatePeriod = 50;
			nn._validationRecalculatePeriod = 30;

			nn._testerT = new Tester(nn, 21000, "Graph//ForTraining", "TRAINING", 1, 0, 0);
			nn._testerV = new Tester(nn, 8000, "Graph//ForValidation", "VALIDATION", 1, 0, 0);

			nn._layers.Add(new LayerPerceptron(nn, 8, 300, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 8, 8, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 5, 8, 0, new SoftSign())); //40 x 15 = 600
			nn._layers.Add(new LayerPerceptron(nn, 1, 5, 0f, new SoftSign())); //40 x 15 = 600

			nn.FillWeightsRandomly();

			Log("New Neural Network created!");
			return nn;
		}

		public static NN CreateClassicMegatron3Subs()
		{
			NN nn = new NN();

			nn._horizon = 60;
			nn._inputWindow = 300;
			nn._weightsInitMin = -1f;
			nn._weightsInitMax = 1f;

			nn._gradientCutter = 10f;

			nn._biasInput = 0.01f;

			nn._LEARNING_RATE = 0.001f; //0.05f
			nn._MOMENTUM = 0f; //0.8f

			nn._statisticsRecalculatePeriod = 50;
			nn._validationRecalculatePeriod = 30;

			nn._inputAF = new SoftSign();
			nn._answersAF = new SoftSign();

			nn._testerT = new Tester(nn, 21000, "Graph//ForTraining", "TRAINING", 2, 0, 0);
			nn._testerV = new Tester(nn, 8000, "Graph//ForValidation", "VALIDATION", 2, 0, 0);

			nn._layers.Add(new LayerMegatron(nn, 3, 271, 30, 1, 0, new SoftSign()));   //136 x 30 x 10 = 
			nn._layers.Add(new LayerCybertron(nn, 3, 271, 5, 15, 0, new SoftSign())); //6 x 136 x 10 = 
			nn._layers.Add(new LayerPerceptron(nn, 10, 15, 0, new SoftSign())); //5 x 60 = 300
			nn._layers.Add(new LayerPerceptron(nn, 5, 10, 0, new SoftSign())); //5 x 5 =  25
			nn._layers.Add(new LayerPerceptron(nn, 1, 5, 0, new SoftSign())); //5 x 5 =  25

			nn.FillWeightsRandomly();

			Log("New Neural Network created!");
			return nn;
		}

		public static NN CreateSmallMegatron2Subs()
		{
			NN nn = new NN();

			nn._horizon = 60;
			nn._inputWindow = 300;
			nn._weightsInitMin = -1f;
			nn._weightsInitMax = 1f;

			nn._gradientCutter = 10f;

			nn._biasInput = 0.01f;

			nn._LEARNING_RATE = 0.001f; //0.05f
			nn._MOMENTUM = 0f; //0.8f

			nn._statisticsRecalculatePeriod = 50;
			nn._validationRecalculatePeriod = 30;

			nn._inputAF = new SoftSign();
			nn._answersAF = new SoftSign();

			nn._testerT = new Tester(nn, 21000, "Graph//ForTraining", "TRAINING", 2, 0, 0);
			nn._testerV = new Tester(nn, 8000, "Graph//ForValidation", "VALIDATION", 2, 0, 0);

			nn._layers.Add(new LayerMegatron(nn, 2, 271, 30, 1, 0, new SoftSign()));   //271 x 30 x 2 = 
			nn._layers.Add(new LayerCybertron(nn, 2, 271, 5, 10, 0, new SoftSign())); //2 x 271 x 5 =
			nn._layers.Add(new LayerPerceptron(nn, 1, 10, 0, new SoftSign())); //10 x 1 = 10

			nn.FillWeightsRandomly();

			Log("New Neural Network created!");
			return nn;
		}
	}
}

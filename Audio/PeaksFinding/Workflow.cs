using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen;
using Microsoft.ML;

namespace PeaksFinding
{
	public static class Workflow
	{
		public static void Make()
		{
			InputData[] inputDatas;
			TestsFiller.Fill(out inputDatas);

            MLContext context = new MLContext();
			IDataView data = context.Data.LoadFromEnumerable<InputData>(inputDatas);
			var trainTestSplit = context.Data.TrainTestSplit(data, testFraction: 0.2);

            var pipeline = context.Transforms.NormalizeMinMax("Features", "spectrum");
			//pipeline.AddLa
            pipeline.Append(context.Transforms.NormalizeMinMax("Label", "amplitudes"));

			var trainer = context.MulticlassClassification.Trainers.LbfgsMaximumEntropy();
			pipeline.Append(trainer);
			//что здесь?

            // Обучение модели
            var model = trainer.Fit(trainTestSplit.TrainSet);

            // Получение прогнозов модели
            IDataView predictions = model.Transform(trainTestSplit.TestSet);
        }
	}
}

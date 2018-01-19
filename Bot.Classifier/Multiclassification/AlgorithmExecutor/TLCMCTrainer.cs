using Bot.ML.Common.Data;
using Bot.VSM.Executor.Convertor;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System.Collections.Generic;
using System.IO;

namespace Bot.Classifier.Multiclassification.AlgorithmExecutor
{
    public class TLCMCTrainer 
    {
        private MCVSMConvertor convertor;                

        public TLCMCTrainer(string modelfilepath)            
        {           
            this.convertor = new MCVSMConvertor(new ModelFilePathInfo(modelfilepath));
        }
       
        private void Train(List<ICExample> trainExamples, string tlcmodelfilepath)
        {
            using (TlcEnvironment host = new TlcEnvironment())
            {
               //
                int FeatureNumber = trainExamples[0].Features.Length;

                SchemaDefinition schema = SchemaDefinition.Create(typeof(ICExample));
                schema["Features"].ColumnType = new VectorType(NumberType.R4, FeatureNumber);
                schema["Label"].ColumnType = NumberType.R4;

            // Create a data view of the training data.
                var trainingData = host.CreateDataView(trainExamples, schemaDefinition: schema);
                var featurized = host.CreateTransform("MinMax{col=Features}", trainingData);

            // Train a predictor.
                var trainingExamples = host.CreateExamples(featurized, "Features", "Label");
                var predictor = host.TrainPredictor(
                "MultiClassLogisticRegression", trainingExamples);

                using (var fs = File.Create(tlcmodelfilepath))
                {
                    host.SaveModel(fs, predictor, trainingExamples);
                }

            }
            return;
        }

        public void Train(List<SourceData> cases, string outputtrainfeatuerfile, string tlcmodelfile)
        {           
            List<ICExample> examples = ((MCVSMConvertor)this.convertor).GenerateBatchFeatures(cases, outputtrainfeatuerfile, true);

            Train(examples, tlcmodelfile);
        }
    }
}

using Bot.ML.Common.Data;
using Bot.VSM.Executor.Convertor;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System.Collections.Generic;
using System.IO;

namespace Bot.TLC.Traininig.Executor
{
    public class BCTrainer
    {       
        private void TrainSVM(List<ICExample> trainExamples, string tlcmodelfilepath)
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
                "LinearSVM", trainingExamples);

                using (var fs = File.Create(tlcmodelfilepath))
                {
                    host.SaveModel(fs, predictor, trainingExamples);
                }

            }
            return;
        }

        private void TrainFastTree(List<ICExample> trainExamples, string tlcmodelfilepath)
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

                // Train a predictor.
                var trainingExamples = host.CreateExamples(trainingData, "Features", "Label");
                
                var predictor = host.TrainPredictor(
                "FastTreeBinaryClassification", trainingExamples);

                using (var fs = System.IO.File.Create(tlcmodelfilepath))
                {
                    host.SaveModel(fs, predictor, trainingExamples);
                }
            }
            return;
        }

        public void Train(ModelFilePathInfo vsmodelfilepath, string PositiveLabel, List<SourceData> cases, string outputtrainfeatuerfile, string tlcmodelfilepath)
        {
           BCVSMConvertor convertor = new BCVSMConvertor(vsmodelfilepath);
           List<ICExample> examples = ((BCVSMConvertor)convertor).GenerateBatchFeatures(PositiveLabel, cases, outputtrainfeatuerfile, true);

            TrainFastTree(examples, tlcmodelfilepath);
        }

        public void Train(ClassificationVSMInfo vsm, string PositiveLabel, List<SourceData> cases, string outputtrainfeatuerfile, string tlcmodelfilepath)
        {
            BCVSMConvertor convertor = new BCVSMConvertor(vsm);
            List<ICExample> examples = ((BCVSMConvertor) convertor).GenerateBatchFeatures(PositiveLabel, cases, outputtrainfeatuerfile, true);

            TrainFastTree(examples, tlcmodelfilepath);            
        }        
    }
}


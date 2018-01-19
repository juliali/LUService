using Bot.ML.Common.Data;
using Bot.VSM.Executor.Convertor;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using Microsoft.MachineLearning.KMeans;
using System.Collections.Generic;
using System.IO;

namespace Bot.Cluster.Clustering.TLCTrainer
{
    public class TLCKMeansTrainer
    {
        private NoLabelVSMConvertor convertor;        

        public TLCKMeansTrainer(string vsmodelfilepath)
        {
            this.convertor = new NoLabelVSMConvertor(new ModelFilePathInfo(vsmodelfilepath));
        }

        private void Train(int K, List<UnlabelExample> trainExamples, string tlcmodelfilepath)
        {
            using (TlcEnvironment host = new TlcEnvironment())
            {
                //
                int FeatureNumber = trainExamples[0].Features.Length;

                SchemaDefinition schema = SchemaDefinition.Create(typeof(UnlabelExample));
                schema["Features"].ColumnType = new VectorType(NumberType.R4, FeatureNumber);                

                // Create a data view of the training data.
                var trainingData = host.CreateDataView(trainExamples, schemaDefinition: schema);
                var featurized = host.CreateTransform("MinMax{col=Features}", trainingData);

                // Train a predictor.
                var trainingExamples = host.CreateExamples(featurized, "Features");                
                
                KMeansPlusPlusTrainer.Arguments trainerArguments = new KMeansPlusPlusTrainer.Arguments { k = K };

                var predictor = host.TrainPredictor(
                trainerArguments, trainingExamples);

                using (var fs = File.Create(tlcmodelfilepath))
                {
                    host.SaveModel(fs, predictor, trainingExamples);
                }

            }
            return;
        }

        public void Train(int K, string trainfilepath, string outputtrainfeatuerfile, string tlcmodelfile)
        {
            List<UnlabelExample> examples = ((NoLabelVSMConvertor)this.convertor).GenerateBatchFeatures(trainfilepath, outputtrainfeatuerfile);

            Train(K, examples, tlcmodelfile);
        }

        
    }
}

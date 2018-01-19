using Bot.ML.Common.Data;
using Bot.VSM.Executor.Convertor;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System.IO;

namespace Bot.Cluster.Clustering.TLCPredictor
{
    public class TLCKMeansPredictor
    {
        private NoLabelVSMConvertor convertor;
        private PredictionEngine<UnlabelExample, UnlabelScoreData> predictionEngine;
        public TLCKMeansPredictor(ModelFilePathInfo vsmodelfile, ModelFilePathInfo tlcmodelfile)
        {
            this.convertor = new NoLabelVSMConvertor(vsmodelfile);
            int fn = GetFeatureNumber();
            this.predictionEngine = CreatePredictionEngine(fn, tlcmodelfile);
        }
        
        private int GetFeatureNumber()
        {
            float[] features = this.convertor.GenerateFeatures("你好", null);
            int featureNum = features.Length;
            return featureNum;
        }        

        protected PredictionEngine<UnlabelExample, UnlabelScoreData> CreatePredictionEngine(int FeatureNumber, ModelFilePathInfo tlcmodelfilepath)
        {
            SchemaDefinition inputschema = SchemaDefinition.Create(typeof(UnlabelExample));
            inputschema["Features"].ColumnType = new VectorType(NumberType.R4, FeatureNumber);

            SchemaDefinition outputschema = SchemaDefinition.Create(typeof(UnlabelScoreData));
            //outputschema["Score"].ColumnType = new VectorType(NumberType.R4, LabelNumber);

            using (var env = new TlcEnvironment())
            {
                //using (var fs = File.OpenRead(tlcmodelfilepath))
                //{
                Stream fs = Bot.ML.Common.Utils.FileUtils.ReadModelAsStream(tlcmodelfilepath);

                    PredictionEngine<UnlabelExample, UnlabelScoreData> predictionEngine =
                    env.CreatePredictionEngine<UnlabelExample, UnlabelScoreData>(fs, inputSchemaDefinition: inputschema, outputSchemaDefinition: outputschema);

                fs.Close();

                    return predictionEngine;
                //}
            }
        }

        public UnlabelPredictionData Predict(string Utterance)
        {
            return Predict(-1, Utterance);
        }

        public UnlabelPredictionData Predict(int Id, string Utterance)
        {
            UnlabelExample example = new UnlabelExample();
            float[] features = this.convertor.GenerateFeatures(Utterance, null);
            example.Features = features;

            UnlabelScoreData score = predictionEngine.Predict(example);

            int clusterIndex = GetClusterIndex(score);

            UnlabelPredictionData data = new UnlabelPredictionData();
            data.Id = Id;
            data.Utterance = Utterance;
            data.ClusterIndex = clusterIndex;

            return data;
        }

        private int GetClusterIndex(UnlabelScoreData score)
        {
            float[] scores = score.Score;
            int index = 0;
            float currentScore = scores[0];

            for(int i = 1; i < scores.Length; i ++)
            {
                if (scores[i] < currentScore)
                {
                    index = i;
                    currentScore = scores[i];
                }
            }

            return index;
        }

        /*
        public void PredictTextTestFile(string testtextfile, string outputfile)
        {
            string[] lines = File.ReadAllLines(testtextfile);

            int totalTestExampleNum = 0;
            int correctTestExampleNum = 0;
            using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
            {
                foreach (string line in lines)
                {
                    string[] tmps = line.Split('\t');
                    string IdStr = tmps[0];

                    try
                    {
                        int Id = int.Parse(IdStr);
                    }
                    catch (Exception e)
                    {
                        w.WriteLine("Id\tUtterance\tExpectedLabel\tActualLabel\tErrorNumber");
                        continue;
                    }

                    string Utterance = tmps[1];
                    string ExpectedLabel = tmps[2];

                    ICTextPredictionInfo result = this.Predict(Utterance);

                    string actualLabel = result.LabelString;

                    int ErrorNum = 1;

                    if (actualLabel == ExpectedLabel)
                    {
                        correctTestExampleNum++;
                        ErrorNum = 0;
                    }

                    w.WriteLine(IdStr + '\t' + Utterance + '\t' + ExpectedLabel + '\t' + actualLabel + '\t' + ErrorNum.ToString());

                }
            }

            float Accurency = (float)correctTestExampleNum / (float)totalTestExampleNum;
            Console.WriteLine("TotalNumber: " + totalTestExampleNum.ToString() + ", CorrectNumber: " + correctTestExampleNum.ToString() + ", Accurency is : " + Accurency.ToString());
        }

        protected ICPredictionInfo GetActualLabel(float[] score)
        {
            int actualLabel = 0;
            float possibility = 0;

            for (int i = 0; i < score.Length; i++)
            {
                if (score[i] > possibility)
                {
                    actualLabel = i;
                    possibility = score[i];
                }
            }

            ICPredictionInfo result = new ICPredictionInfo();
            result.LabelValue = (float)actualLabel;
            result.Possibility = possibility;

            return result;
        }
        */
    }
}

using Microsoft.MachineLearning.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.MachineLearning.Api;
using Bot.VSM.Executor.Convertor;
using Bot.ML.Common.Data;


namespace Bot.Classifier.Prediction.MultipleClassification
{
    public class TLCMCPredictor 
    {
        private Dictionary<float, string> LabelValues;
        private PredictionEngine<ICExample, ICScoredData> predictionEngine;
        private MCVSMConvertor convertor;                

        public TLCMCPredictor(ModelFilePathInfo modelfilepath, ModelFilePathInfo tlcmodelfile)
        {
            this.convertor = new MCVSMConvertor(modelfilepath);
            this.LabelValues = this.convertor.GetLabelValueDict();
            int FeatureNumber = GetFeatureNumber();
            this.predictionEngine = CreatePredictionEngine(FeatureNumber, this.LabelValues.Count, tlcmodelfile);
        }

        private int GetFeatureNumber()
        {
            string testingSentence = "你好";
            float[] featureArray = this.convertor.GenerateFeatures(testingSentence, null);
            int FeatureNumber = featureArray.Length;
            return FeatureNumber;
        }

        protected PredictionEngine<ICExample, ICScoredData> CreatePredictionEngine(int FeatureNumber, int LabelNumber, ModelFilePathInfo tlcmodelfilepath)
        {
            SchemaDefinition inputschema = SchemaDefinition.Create(typeof(ICExample));
            inputschema["Features"].ColumnType = new VectorType(NumberType.R4, FeatureNumber);

            SchemaDefinition outputschema = SchemaDefinition.Create(typeof(ICScoredData));
            outputschema["Score"].ColumnType = new VectorType(NumberType.R4, LabelNumber);

            using (var env = new TlcEnvironment())
            {
                //using (var fs = File.OpenRead(tlcmodelfilepath))
                //{
                Stream fs = Bot.ML.Common.Utils.FileUtils.ReadModelAsStream(tlcmodelfilepath);

                    PredictionEngine<ICExample, ICScoredData> predictionEngine =
                    env.CreatePredictionEngine<ICExample, ICScoredData>(fs, inputSchemaDefinition: inputschema, outputSchemaDefinition: outputschema);

                fs.Close();

                    return predictionEngine;
                //}
            }
        }

        public ICTextPredictionInfo Predict(string Utterance, string EdiResult)
        {
            float[] features = this.convertor.GenerateFeatures(Utterance, EdiResult);

            ICExample example = new ICExample();
            example.Features = features;

            ICScoredData result = predictionEngine.Predict(example);
            float[] score = result.Score;

            ICPredictionOutputInfo actualLabel = GetActualLabel(score);

            ICTextPredictionInfo textLabel = new ICTextPredictionInfo();
            textLabel.Possibility = actualLabel.Possibility;
            textLabel.LabelString = this.LabelValues[actualLabel.LabelValue];

            return textLabel;
        }        

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

                    string EdiResult = null;
                    
                    if (tmps.Length > 3)
                    {
                        EdiResult = tmps[3];
                    }              

                    ICTextPredictionInfo result = this.Predict(Utterance, EdiResult);

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

        protected ICPredictionOutputInfo GetActualLabel(float[] score)
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

            ICPredictionOutputInfo result = new ICPredictionOutputInfo();
            result.LabelValue = (float)actualLabel;
            result.Possibility = possibility;

            return result;
        }
    }
}

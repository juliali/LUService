using Bot.VSM.Executor.Convertor;
using Microsoft.MachineLearning.Api;
using Microsoft.MachineLearning.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bot.ML.Common.Data;

namespace Bot.Classifier.Prediction.BinaryClassification
{

    public class BCScoredData
    {
        public DvBool PredictedLabel;
        public Single Score;
        public Single Probability;
    }

    public class TLCBCPredictor
    {
        private Dictionary<string, PredictionEngine<ICExample, BCScoredData>> predictionEngineDict;
        private Dictionary<string, BCVSMConvertor> convertorDict;

        private HashSet<string> PositiveLabels;
        private string NegtiveLabelString;                

        public TLCBCPredictor(Dictionary<string, ModelFilePathInfo> vsmodelfilepathmap,  Dictionary<string, ModelFilePathInfo> tlcmodelfilemap)
        {
            this.PositiveLabels = new HashSet<string>(vsmodelfilepathmap.Keys);
            this.convertorDict = new Dictionary<string, BCVSMConvertor>();
            this.predictionEngineDict = new Dictionary<string, PredictionEngine<ICExample, BCScoredData>>();            
            foreach (string PositiveLabel in PositiveLabels)
            {                                
                BCVSMConvertor generator = new BCVSMConvertor(vsmodelfilepathmap[PositiveLabel]);

                if (NegtiveLabelString == null)
                {
                    this.NegtiveLabelString = generator.GetNegtiveLabel();
                }

                this.convertorDict.Add(PositiveLabel, generator);

                int FeatureNumber = GetFeatureNumber(generator);

                ModelFilePathInfo tlcmodelfilepath = tlcmodelfilemap[PositiveLabel];
                PredictionEngine<ICExample, BCScoredData> predictionEngine = CreatePredictionEngine(FeatureNumber, 2, tlcmodelfilepath);

                this.predictionEngineDict.Add(PositiveLabel, predictionEngine);
            }
        }

        private int GetFeatureNumber(BCVSMConvertor generator)
        {
            string testingSentence = "你好";
            float[] featureArray = generator.GenerateFeatures(testingSentence, null);
            int FeatureNumber = featureArray.Length;
            return FeatureNumber;
        }

        protected PredictionEngine<ICExample, BCScoredData> CreatePredictionEngine(int FeatureNumber, int LabelNumber, ModelFilePathInfo tlcmodelfilepath)
        {
            SchemaDefinition inputschema = SchemaDefinition.Create(typeof(ICExample));
            inputschema["Features"].ColumnType = new VectorType(NumberType.R4, FeatureNumber);

            SchemaDefinition outputschema = SchemaDefinition.Create(typeof(BCScoredData));

            using (var env = new TlcEnvironment())
            {
                Stream fs = Bot.ML.Common.Utils.FileUtils.ReadModelAsStream(tlcmodelfilepath);

                try
                { 
                PredictionEngine<ICExample, BCScoredData> predictionEngine =
                    env.CreatePredictionEngine<ICExample, BCScoredData>(fs, inputSchemaDefinition: inputschema, outputSchemaDefinition: outputschema);

                    fs.Close();
                    return predictionEngine;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return null;
                
            }
        }

        public ICTextPredictionInfo Predict(string Utterance, string EdiResult)
        {
            List<ICTextPredictionInfo> results = new List<ICTextPredictionInfo>();

            foreach (string PositiveLabel in this.PositiveLabels)
            {
                ICTextPredictionInfo result = SingleModelPredict(PositiveLabel, Utterance, EdiResult);

                if (result.LabelString == PositiveLabel)
                {
                    results.Add(result);
                }
            }

            if (results.Count == 0)
            {
                ICTextPredictionInfo result = new ICTextPredictionInfo();
                result.LabelString = this.NegtiveLabelString;
                result.Possibility = (float)1.0;

                return result;
            }
            else if (results.Count == 1)
            {
                return results[0];
            }
            else
            {
                ICTextPredictionInfo result = results[0];
                for (int i = 1; i < results.Count; i++)
                {
                    if (result.Possibility < results[i].Possibility)
                    {
                        result = results[i];
                    }
                }

                return result;
            }

        }

        private ICTextPredictionInfo SingleModelPredict(string PositiveLabel, string Utterance, string EdiResult)
        {
            BCVSMConvertor convertor = this.convertorDict[PositiveLabel];

            PredictionEngine<ICExample, BCScoredData> predictionEngine = this.predictionEngineDict[PositiveLabel];

            float[] features = convertor.GenerateFeatures(Utterance, EdiResult);

            ICExample example = new ICExample();
            example.Features = features;

            BCScoredData result = predictionEngine.Predict(example);

            ICPredictionOutputInfo actualLabel = GetActualLabelForBinaryClass(PositiveLabel, result);

            ICTextPredictionInfo textLabel = new ICTextPredictionInfo();
            textLabel.Possibility = actualLabel.Possibility;
            textLabel.LabelString = convertor.GetLabelValueDict()[actualLabel.LabelValue];

            return textLabel;

        }

        private ICPredictionOutputInfo GetActualLabelForBinaryClass(string PositiveLabel, BCScoredData scoreData)
        {
            ICPredictionOutputInfo result = new ICPredictionOutputInfo();
            result.Possibility = scoreData.Probability;
            result.LabelValue = scoreData.PredictedLabel.RawValue;

            return result;
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
                    catch(Exception e)
                    {
                        w.WriteLine("Id\tUtterance\tExpectedLabel\tActualLabel\tErrorNumber");
                        continue;
                    }

                    string Utterance = tmps[1];
                    string ExpectedLabel = tmps[2];

                    if (!this.PositiveLabels.Contains(ExpectedLabel))
                    {
                        ExpectedLabel = this.NegtiveLabelString;
                    }

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
    }
}

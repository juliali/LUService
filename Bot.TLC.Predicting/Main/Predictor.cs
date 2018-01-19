using Bot.ML.Common.Data;
using Bot.TLC.Predicting.Executor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Bot.TLC.Predicting.Main
{
    public class Predictor
    {
        private BCPredictor predictor;
        private List<string> PositiveLabels;
        private string NegtiveLabel;

        public Predictor(string bcConfigFilePath)
        {
            string keyword = bcConfigFilePath;
            string configStr = File.ReadAllText(bcConfigFilePath);

            BCConfigInfo configInfo = JsonConvert.DeserializeObject<BCConfigInfo>(configStr);//(BCConfigInfo )(new JavaScriptSerializer()).DeserializeObject(configStr);
            Dictionary <string, BCModelInfo> dict = configInfo.Models.ToDictionary(x => x.PositiveLabel, x => x);

            this.PositiveLabels = dict.Keys.ToList<string>();
            this.NegtiveLabel = configInfo.NegtiveLabel;

            this.predictor = BCPredictor.GetInstance(keyword, configInfo);
        }

        public ICTextPredictionInfo Predict(string Utterance)
        {            
            ICTextPredictionInfo result = this.predictor.PredictUtterance(Utterance, null);

            return result;
        }

        public void Predict(List<SourceData> cases, string outputtestreport)
        {
            int correctTestExampleNum = 0;
            int totalTestExampleNum = 0;

            using (StreamWriter w = new StreamWriter(File.Open(outputtestreport, FileMode.Create), Encoding.UTF8))
            {              
                foreach (SourceData aCase in cases)
                {                    

                    string Utterance = aCase.Utterance;
                    string ExpectedLabel = aCase.Label;

                    if (!this.PositiveLabels.Contains(ExpectedLabel))
                    {
                        ExpectedLabel = this.NegtiveLabel;
                    }                                  

                    ICTextPredictionInfo result = this.Predict(Utterance);

                    string actualLabel = result.LabelString;

                    int ErrorNum = 1;

                    if (actualLabel == ExpectedLabel)
                    {
                        correctTestExampleNum++;
                        ErrorNum = 0;
                    }

                    w.WriteLine(aCase.Id.ToString() + '\t' + Utterance + '\t' + ExpectedLabel + '\t' + actualLabel + '\t' + ErrorNum.ToString());
                    totalTestExampleNum++;
                }
            }

            float Accurency = (float)correctTestExampleNum / (float)totalTestExampleNum;
            Console.WriteLine("TotalNumber: " + totalTestExampleNum.ToString() + ", CorrectNumber: " + correctTestExampleNum.ToString() + ", Accurency is : " + Accurency.ToString());

        }
    }
}

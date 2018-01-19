using System.Collections.Generic;
using Bot.ML.Common.Data;
using Bot.ML.Common.ConfigStore;
using Bot.ML.Common.Controller;

namespace Bot.Classifier.Prediction.BinaryClassification
{
    public class BCPredictor : ICPredictorInterface
    {
        protected HashSet<string> PositiveLabels;
        protected Dictionary<string, ModelFilePathInfo> VSModelMap;
        protected Dictionary<string, ModelFilePathInfo> TLCModelMap;
        protected Dictionary<string, string[]> VSMConfigPathsMap;
        protected TLCBCPredictor tlcTester;
        protected string NegtiveLabel;
        protected static BCConfigStore configStore = BCConfigStore.GetInstance(null, null);

        public BCPredictor(string modelconfigFileResName)
        {
            BCConfigInfo configInfo = configStore.GetBCCongigInfo(modelconfigFileResName, null);///TOBE fixed

            this.PositiveLabels = new HashSet<string>();
            this.VSModelMap = new Dictionary<string, ModelFilePathInfo>();
            this.TLCModelMap = new Dictionary<string, ModelFilePathInfo>();

            this.VSMConfigPathsMap = new Dictionary<string, string[]>();
            this.NegtiveLabel = configInfo.NegtiveLabel;

            foreach(BCModelInfo Model in configInfo.Models)
            {
                string PositiveLabel = Model.PositiveLabel;

                this.PositiveLabels.Add(PositiveLabel);
                this.VSModelMap.Add(PositiveLabel, Model.VSModelPath);
                this.TLCModelMap.Add(PositiveLabel, Model.TLCModelPath);
                this.VSMConfigPathsMap.Add(PositiveLabel, configInfo.VSMConfigPaths);
            }           
        }  

        public BCPredictor(Dictionary<string, string[]> VSMConfigPathsMap,
            Dictionary<string, ModelFilePathInfo> vsmodelMap, 
            Dictionary<string, ModelFilePathInfo> tlcmodelMap,
            string NegativeLabel
            )
        {
            this.VSMConfigPathsMap = VSMConfigPathsMap;
            this.VSModelMap = vsmodelMap;
            this.TLCModelMap = tlcmodelMap;
            this.PositiveLabels = new HashSet<string>(VSModelMap.Keys);
            this.NegtiveLabel = NegativeLabel;
        }

        public ICTextPredictionInfo PredictUtterance(string Utterance, string EdiResult)
        {
            if (this.tlcTester == null)
            {
                this.tlcTester = new TLCBCPredictor(this.VSModelMap, this.TLCModelMap);
            }

            ICTextPredictionInfo result = this.tlcTester.Predict(Utterance, EdiResult);
            return result;
        }

        public void PredictTextFile(string texttestfile, string outputtestfile)
        {
            if (this.tlcTester == null)
            {
                this.tlcTester = new TLCBCPredictor(this.VSModelMap, this.TLCModelMap);
            }

            this.tlcTester.PredictTextTestFile(texttestfile, outputtestfile);
        }
    }
}

using System.Collections.Generic;
using Bot.ML.Common.Data;
using Bot.ML.Common.ConfigStore;

namespace Bot.TLC.Predicting.Executor
{
    public class BCPredictor
    {             
        private Dictionary<string, TLCBCPredictorInParallel> tlcTesterMap;        
        private static BCPredictor instance;

        private TLCBCPredictorInParallel tlcTester;
       
        public static BCPredictor GetInstance(string keyword, BCConfigInfo configInfo)
        {
            if (instance == null)
            {
                instance = new BCPredictor();           
            }

            if (!instance.tlcTesterMap.ContainsKey(keyword))
            {
                instance.SetConfig(keyword, configInfo);               
            }

            instance.tlcTester = instance.tlcTesterMap[keyword];

            return instance;
        }

        private BCPredictor()
        {
            tlcTesterMap = new Dictionary<string, TLCBCPredictorInParallel>();
        }

        private void SetConfig(string keyword, BCConfigInfo configInfo)
        {
            BCConfigStore configStore = BCConfigStore.GetInstance(keyword, configInfo);
            TLCBCPredictorInParallel tlcTester = new TLCBCPredictorInParallel(configStore.GetVSModelMap(keyword), 
                configStore.GetTLCModelMap(keyword));

            tlcTesterMap.Add(keyword, tlcTester);
        }  

        public ICTextPredictionInfo PredictUtterance(string Utterance, string EdiResult)
        {            
            ICTextPredictionInfo result = this.tlcTester.Predict(Utterance, EdiResult);
            return result;
        }        
    }
}

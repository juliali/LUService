using System.Collections.Generic;
using Bot.ML.Common.Data;
using Bot.ML.Common.ConfigStore;
using Bot.ML.Common.Controller;
using Microsoft.ApplicationInsights;
using Bot.ML.Common.Utils;
using Newtonsoft.Json;

namespace TLCService.BinaryClassification
{
    public class BCPredictor : ICPredictorInterface
    {             
        private Dictionary<string, TLCBCPredictorInParallel> tlcTesterMap;        
        private static BCPredictor instance;

        private TLCBCPredictorInParallel tlcTester;

        static TelemetryClient tc = new TelemetryClient();

        public static BCPredictor GetInstance(string modelconfigFileResName)
        {
            if (instance == null)
            {
                instance = new BCPredictor();           
            }

            if (!instance.tlcTesterMap.ContainsKey(modelconfigFileResName))
            {
                instance.SetConfig(modelconfigFileResName);

                tc.TrackEvent("loading vs models & tlc models for " + modelconfigFileResName);
            }

            instance.tlcTester = instance.tlcTesterMap[modelconfigFileResName];

            return instance;
        }

        private BCPredictor()
        {
            tlcTesterMap = new Dictionary<string, TLCBCPredictorInParallel>();
        }

        private void SetConfig(string modelconfigFileResName)
        {
            BCConfigInfo configInfo = JsonConvert.DeserializeObject<BCConfigInfo>(FileUtils.ReadEmbeddedResourceFile(modelconfigFileResName));

            BCConfigStore configStore = BCConfigStore.GetInstance(modelconfigFileResName, configInfo);
            TLCBCPredictorInParallel tlcTester = new TLCBCPredictorInParallel(configStore.GetVSModelMap(modelconfigFileResName), 
                configStore.GetTLCModelMap(modelconfigFileResName));

            tlcTesterMap.Add(modelconfigFileResName, tlcTester);
        }  

        public ICTextPredictionInfo PredictUtterance(string Utterance, string EdiResult)
        {            
            ICTextPredictionInfo result = this.tlcTester.Predict(Utterance, EdiResult);
            return result;
        }        
    }
}

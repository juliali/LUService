using Newtonsoft.Json;
using System.Collections.Generic;
using Bot.ML.Common.Data;

namespace Bot.ML.Common.ConfigStore
{
    public class BCConfigStore
    {
        private static Dictionary<string, BCConfigInfo> configInfoMap;        
        private static Dictionary<string, Dictionary<string, ModelFilePathInfo>> VSModelMap;
        private static Dictionary<string, Dictionary<string, ModelFilePathInfo>> TLCModelMap;

        private static BCConfigStore instance;

        public static BCConfigStore GetInstance(string keyword, BCConfigInfo configInfo)
        {
            if (instance == null)
            {
                instance = new BCConfigStore();
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            { 
                instance.GetBCCongigInfo(keyword, configInfo);
            }

            return instance;
        }

        private BCConfigStore()
        {
            configInfoMap = new Dictionary<string, BCConfigInfo>();
            VSModelMap = new Dictionary<string, Dictionary<string, ModelFilePathInfo>>();
            TLCModelMap = new Dictionary<string, Dictionary<string, ModelFilePathInfo>>();
        }

        public BCConfigInfo GetBCCongigInfo(string keyword, BCConfigInfo configInfo)
        {           
            if (!configInfoMap.ContainsKey(keyword))
            {                
                if (configInfo != null)
                { 
                    configInfoMap.Add(keyword, configInfo);

                    Dictionary<string, ModelFilePathInfo> singleVSModelMap = new Dictionary<string, ModelFilePathInfo>();
                    Dictionary<string, ModelFilePathInfo> singleTLCModelMap = new Dictionary<string, ModelFilePathInfo>();

                    foreach (BCModelInfo Model in configInfo.Models)
                    {
                        singleVSModelMap.Add(Model.PositiveLabel, Model.VSModelPath);
                        singleTLCModelMap.Add(Model.PositiveLabel, Model.TLCModelPath);
                    }

                    VSModelMap.Add(keyword, singleVSModelMap);
                    TLCModelMap.Add(keyword, singleTLCModelMap);
                }                
            }

            return configInfoMap[keyword];
        }

        public Dictionary<string, ModelFilePathInfo> GetVSModelMap(string filePath)
        {
            if (!VSModelMap.ContainsKey(filePath))
            {
                return null;
            }

            return VSModelMap[filePath];
        }

        public Dictionary<string, ModelFilePathInfo> GetTLCModelMap(string filePath)
        {
            if (!TLCModelMap.ContainsKey(filePath))
            {
                return null;
            }

            return TLCModelMap[filePath];
        }
    }
}

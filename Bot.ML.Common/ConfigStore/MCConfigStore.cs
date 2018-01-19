using Newtonsoft.Json;
using Bot.ML.Common.Data;

namespace Bot.ML.Common.ConfigStore
{
    public class MCConfigStore
    {
        private static MCConfigInfo configInfo;
        private static MCConfigStore instance;
        
        private MCConfigStore(string ResourceName) {
            
            string configstr = Bot.ML.Common.Utils.FileUtils.ReadEmbeddedResourceFile(ResourceName);
            configInfo = JsonConvert.DeserializeObject<MCConfigInfo>(configstr);
        }

        public static MCConfigStore GetInstance(string fileName)
        {            
            if (instance == null)
            {
                    instance = new MCConfigStore(fileName);
            }
            return instance;            
        }

        public MCConfigInfo GetMCCongigInfo()
        {
            return configInfo;
        }
    }
}

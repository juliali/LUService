using Bot.ML.Common.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bot.VSM.Common
{
    public class VSMConfigStore
    {
        private static Dictionary<string, Dictionary<string, string>> ConfigFilesDict;       
        private static VSMConfigStore Instance;
        private static Dictionary<string, string> currentOptionDict;

        private VSMConfigStore()
        {
            ConfigFilesDict =  new Dictionary<string, Dictionary<string, string>>();
            currentOptionDict = new Dictionary<string, string>();
        }

        private static void ReadConfigFile(string ConfigFileResPath)
        {
            if (!ConfigFilesDict.ContainsKey(ConfigFileResPath))
            {

                string filecontent = Bot.ML.Common.Utils.FileUtils.ReadEmbeddedResourceFile(ConfigFileResPath);//System.IO.File.ReadAllText(ConfigFilePath);
                ConfigurationInfo configInfo = JsonConvert.DeserializeObject<ConfigurationInfo>(filecontent);
                Dictionary<string, string> ConfigDict = new Dictionary<string, string>();

                foreach (OptionInfo option in configInfo.Options)
                {
                    ConfigDict.Add(option.Name, option.Value);
                }

                ConfigFilesDict.Add(ConfigFileResPath, ConfigDict);
            }
            currentOptionDict = ConfigFilesDict[ConfigFileResPath];
        }

        public string GetOption(string OptionName)
        {
            if (currentOptionDict.ContainsKey(OptionName))
            {                
                
                    return currentOptionDict[OptionName];
                
            }

            return null;
        }

        public static VSMConfigStore GetInstance(string ConfigFilePath)
        {
            if (Instance == null)
            {
                Instance = new VSMConfigStore();
            }

            ReadConfigFile(ConfigFilePath);

            return Instance;
        }

        /*
        public FeatureSelectionInfo GetFeatureSelectionInfo(CutterType type)
        {
            if (!this.FSInfoDict.ContainsKey(type))
            {
                FeatureSelectionInfo info = new FeatureSelectionInfo();
                info.TermCutterType = type;
                info.TermLenght = TermLenDict[type];

                switch (type)
                {
                    case CutterType.NGRAM:
                    case CutterType.WORDBREAK:
                        info.FeatureSelectionPolicy = new NGramFeatureSelectionPolicy();
                        break;

                    default:
                        info.FeatureSelectionPolicy = new RuleBasedFeatureSelectionPolicy();
                        break;
                }

                this.FSInfoDict.Add(type, info);
            }

            return FSInfoDict[type]; ;
        }
        */
    }
}

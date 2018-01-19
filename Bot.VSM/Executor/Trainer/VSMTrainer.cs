using Bot.ML.Common.Data;
using Bot.VSM.Modules.DocProcessor;
using Bot.VSM.Modules.VSGenerator;
using Bot.VSM.VSGenerator;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Bot.VSM.Executor.Trainer
{
    public abstract class VSMTrainer
    {        
        protected string[] ConfigFiles;
        public VSMTrainer(string[] configFilePaths)
        {
            this.ConfigFiles = configFilePaths;            
        }
           
        protected void SaveVSToModelFile(ClassificationVSMInfo Model, string modelfilepath, string modellabelfilepath)
        {
            if (!string.IsNullOrWhiteSpace(modellabelfilepath) && Model.LabelValues.LabelValueDict.Count > 0)
            { 
                using (StreamWriter w = new StreamWriter(File.Open(modellabelfilepath, FileMode.Create), Encoding.UTF8))
                {
                    foreach (KeyValuePair<string, int> kv in Model.LabelValues.LabelValueDict)
                    {
                        w.WriteLine(kv.Key + '\t' + kv.Value.ToString());
                    }
                }
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(modelfilepath,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, Model);
            stream.Close();
        }

        protected VectorSpace GenerateVectorSpace(Dictionary<string, int>LabelValues, string PositiveLabel, string NegtiveLabel, string ConfigFilePath, List<SourceData> cases)
        {
            List<DocumentInfo> Documents = new List<DocumentInfo>();
            DocumentParser DocParser = new DocumentParser(ConfigFilePath);

            foreach (SourceData aCase in cases)
            {
                DocumentInfo Doc = DocParser.Parse(aCase.Id, aCase.Label, aCase.Utterance, null);
                Documents.Add(Doc);
            }

            VectorSpaceGenerator Generator = new VectorSpaceGenerator(LabelValues);
            VectorSpace VS = Generator.Generate(Documents, PositiveLabel, NegtiveLabel);

            return VS;           
        }

        public ClassificationVSMInfo TrainingModel(string PositiveLabel, string NagetiveLabel, /*string trainfilepath*/List<SourceData> cases)
        {
            
            //List<LabledTextFileInfo> cases = Bot.ML.Common.Utils.TextUtils.ReadLabledDataFromTextFile(trainfilepath);

            ClassificationVSMInfo result = new ClassificationVSMInfo();
            LabelValueInfo labelInfo = GetLabelValueInfo(cases, PositiveLabel, NagetiveLabel);

            List<VSMInfo> VSMList = new List<VSMInfo>();

            foreach (string ConfigFilePath  in this.ConfigFiles)
            {
                VectorSpace VS = GenerateVectorSpace(labelInfo.LabelValueDict, PositiveLabel, NagetiveLabel, ConfigFilePath, cases);

                VSMInfo VSM = new VSMInfo();
                VSM.VS = VS;
                VSM.VSMConfigFilePath = ConfigFilePath;//fsStore.GetFeatureSelectionInfo(type);

                VSMList.Add(VSM);
            }
            result.LabelValues = labelInfo;
            result.VSMWithSelectionPolicyList = VSMList;

            return result;
        }

        public LabelValueInfo GetLabelValueInfo(List<SourceData> cases, string PositiveLabel, string NagetiveLabel)
        {
            LabelValueGenerator labelValueGen = new LabelValueGenerator();
            Dictionary<string, int> LabelValueDict = labelValueGen.Generate(cases,  PositiveLabel,  NagetiveLabel);

            LabelValueInfo LabelValues = new LabelValueInfo();
            LabelValues.LabelValueDict = LabelValueDict;
            LabelValues.NegtiveLabel = NagetiveLabel;

            return LabelValues;
        }
    }
}

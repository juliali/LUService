using Bot.ML.Common.Data;
using Bot.VSM.Modules.DocProcessor;
using Bot.VSM.VSGenerator;
using System.Collections.Generic;

namespace Bot.VSM.Executor.Trainer
{
    public class NoLabelVSMTrainer : VSMTrainer
    {
        public NoLabelVSMTrainer(string[] configFilePaths) : base(configFilePaths)
        {
        }

        protected VectorSpace GenerateVectorSpace(string ConfigFilePath, List<UnlabledTextFileInfo> cases)
        {
            List<DocumentInfo> Documents = new List<DocumentInfo>();
            DocumentParser DocParser = new DocumentParser(ConfigFilePath);

            foreach (UnlabledTextFileInfo aCase in cases)
            {
                DocumentInfo Doc = DocParser.Parse(aCase.Id, null, aCase.Utterance, aCase.EdiResult);
                Documents.Add(Doc);
            }

            VectorSpaceGenerator Generator = new VectorSpaceGenerator();
            VectorSpace VS = Generator.Generate(Documents);

            return VS;
        }        

        public void Training(string trainfilepath, string modelfilepath)
        {
            List<UnlabledTextFileInfo> cases = Bot.ML.Common.Utils.TextUtils.ReadUnlabledDataFromTextFile(trainfilepath);

            ClassificationVSMInfo result = new ClassificationVSMInfo();

            List<VSMInfo> VSMList = new List<VSMInfo>();

            foreach (string ConfigFilePath in this.ConfigFiles)
            {
                VectorSpace VS = GenerateVectorSpace(ConfigFilePath, cases);

                VSMInfo VSM = new VSMInfo();
                VSM.VS = VS;
                VSM.VSMConfigFilePath = ConfigFilePath;

                VSMList.Add(VSM);
            }

            result.VSMWithSelectionPolicyList = VSMList;

            SaveVSToModelFile(result, modelfilepath, null);
        }
    }
}

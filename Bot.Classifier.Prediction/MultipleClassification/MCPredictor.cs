using Bot.ML.Common.ConfigStore;
using Bot.ML.Common.Controller;
using Bot.ML.Common.Data;

namespace Bot.Classifier.Prediction.MultipleClassification
{
    public class MCPredictor: ICPredictorInterface
    {
        private string[] VSMConfigPaths;
        private ModelFilePathInfo VSModelPath;
        private ModelFilePathInfo TLCModelPath;

        private string LabelFilePath;

        private TLCMCPredictor Tester;
        public MCPredictor(string modelconfigFileResName)
        {
            MCConfigInfo configInfo = MCConfigStore.GetInstance(modelconfigFileResName).GetMCCongigInfo();
            this.VSModelPath = configInfo.VSModelPath;
            this.LabelFilePath = configInfo.LabelFilePath;
            this.VSMConfigPaths = configInfo.VSMConfigPaths;
            this.TLCModelPath = configInfo.TLCModelPath;

            this.Tester = new TLCMCPredictor(this.VSModelPath,
                this.TLCModelPath);
        }

        public MCPredictor(string[] vsmconfigFiles, ModelFilePathInfo vsmodelfile, ModelFilePathInfo tlcmodelfile)
        {
            this.VSModelPath = vsmodelfile;
            this.TLCModelPath = tlcmodelfile;
            this.VSMConfigPaths = vsmconfigFiles;
            this.LabelFilePath = this.VSModelPath.FileName.Replace(".model", "_label.txt");

            this.Tester = new TLCMCPredictor(this.VSModelPath,
                this.TLCModelPath);
        }

        public ICTextPredictionInfo PredictUtterance(string Utterance, string EdiResult)
        {
            
            ICTextPredictionInfo Result = this.Tester.Predict(Utterance, EdiResult);

            return Result;
        }

        public void PredictTextFile(string texttestfile, string outputtestfile)
        {           
            this.Tester.PredictTextTestFile(texttestfile, outputtestfile);
        }
    }
}


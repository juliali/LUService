using Bot.VSM.Executor.Convertor;
using Bot.VSM.Executor.Trainer;
using Bot.Classifier.Multiclassification.AlgorithmExecutor;
using Bot.ML.Common.Data;
using Bot.ML.Common.Controller;
using Bot.ML.Common.ConfigStore;
using System;
using System.Collections.Generic;

namespace Bot.Classifier.Prediction.Controller
{
    public class MCTrainer : ICTrainerInterface
    {
        private string[] VSMConfigPaths;
        private ModelFilePathInfo VSModelPath;
        private ModelFilePathInfo TLCModelPath;

        private string LabelFilePath;

        public MCTrainer(string modelconfigFileResName)
        {
            MCConfigInfo configInfo = MCConfigStore.GetInstance(modelconfigFileResName).GetMCCongigInfo();
            this.VSModelPath = configInfo.VSModelPath;
            this.LabelFilePath = configInfo.LabelFilePath;
            this.VSMConfigPaths = configInfo.VSMConfigPaths;
            this.TLCModelPath = configInfo.TLCModelPath;
        }

        public MCTrainer(string[] vsmconfigFiles, ModelFilePathInfo vsmodelfile, ModelFilePathInfo tlcmodelfile)
        {
            this.VSModelPath = vsmodelfile;
            this.TLCModelPath = tlcmodelfile;
            this.VSMConfigPaths = vsmconfigFiles;
            this.LabelFilePath = this.VSModelPath.FileName.Replace(".model", "_label.txt");
        }

        public void GenerateVectorSpace(List<SourceData> cases)
        {
            MCVSMTrainer VSTrainer = new MCVSMTrainer(this.VSMConfigPaths);
            VSTrainer.Training(cases, this.VSModelPath.FileName, this.LabelFilePath);

            return;
        }

        public void GenerateTrainingFeaturesAndTrain(List<SourceData> cases, string trainingfeaturefile)
        {
            TLCMCTrainer Trainer = new TLCMCTrainer(this.VSModelPath.FileName);

            Trainer.Train(cases, trainingfeaturefile, this.TLCModelPath.FileName);
        }

        public void GenerateFeaturesFromTextFile(List<SourceData> cases, string featurefile)
        {
            MCVSMConvertor featureGen = new MCVSMConvertor(this.VSModelPath);
            featureGen.GenerateBatchFeatures(cases, featurefile, false);
        }        

        public void GenerateTrainingFeaturesAndTrain(List<SourceData> cases, string filename, string featurepath)
        {
            throw new NotImplementedException();
        }
    }
}
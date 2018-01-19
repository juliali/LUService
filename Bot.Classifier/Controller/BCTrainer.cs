using System.Collections.Generic;
using Bot.VSM.Executor.Trainer;
using Bot.Classifier.Binaryclassification.AlgorithmExecutor;
using Bot.ML.Common.Data;
using Bot.ML.Common.ConfigStore;
using Bot.ML.Common.Controller;
using Bot.ML.Common.Utils;
using System;

namespace Bot.Classifier.Controller
{
    public class BCTrainer : ICTrainerInterface
    {
        private HashSet<string> PositiveLabels;
        private Dictionary<string, ModelFilePathInfo> VSModelMap;
        private Dictionary<string, ModelFilePathInfo> TLCModelMap;
        private Dictionary<string, string[]> VSMConfigPathsMap;        
        
        private string NegtiveLabel;
        private static BCConfigStore configStore = BCConfigStore.GetInstance(null, null);

        public BCTrainer(string modelconfigFileResName)
        {
            BCConfigInfo configInfo = configStore.GetBCCongigInfo(modelconfigFileResName, null); ////TOBE fixed

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

        public BCTrainer(Dictionary<string, string[]> VSMConfigPathsMap,
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

        public void GenerateTrainingFeaturesAndTrain(List<SourceData> cases, string filename, string temporaryfiledir)
        {
            foreach (string PositiveLabel in PositiveLabels)
            {
                ModelFilePathInfo vsmodelfile = this.VSModelMap[PositiveLabel];
                ModelFilePathInfo tlcmodelfile = this.TLCModelMap[PositiveLabel];
                
               // string filename = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(cases);
                filename = filename.Replace("training", "");
                string trainingfeaturefile = temporaryfiledir + filename + "_traininig_feature_BC_" + PositiveLabel + ".tsv";

                TLCBCTrainer tlcTrainer = new TLCBCTrainer();
                tlcTrainer.Train(vsmodelfile, PositiveLabel, cases, trainingfeaturefile, tlcmodelfile.FileName);
            }
        }

       // public void GenerateVectorSpace(string trainingtextfilepath)
       // {
       //     GenerateVectorSpace(TextUtils.ReadSourceDataFromTextFile(trainingtextfilepath));
       // }

        public void GenerateVectorSpace(List<SourceData> cases)
        {
            foreach (string PositiveLable in PositiveLabels)
            {
                string[] configPaths = this.VSMConfigPathsMap[PositiveLable];
                ModelFilePathInfo vsmodelfile = this.VSModelMap[PositiveLable];
                string modellabelfile = vsmodelfile.FileName.Replace(".model", "_label.txt");

                //List<CutterType> types = new List<CutterType>();

                BCVSMTrainer vmsTrainer = new BCVSMTrainer(configPaths, this.NegtiveLabel);
                vmsTrainer.Training(PositiveLable, cases, vsmodelfile.FileName, modellabelfile);                
            }
        }



        /*

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
        */
    }
}

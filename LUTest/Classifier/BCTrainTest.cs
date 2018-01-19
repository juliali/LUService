using Bot.VSM.Executor.Convertor;
using Bot.VSM.Executor.Trainer;
using System.Collections.Generic;
using Bot.ML.Common.Data;
using Bot.ML.Common.Utils;

namespace LUTest.Classifier
{
    public class BCTrainTest
    {
        /*  public static void TrainVSM(string[] configFiles, string PositiveLabel, string traintextfile, string vsmodelfile, string NegtiveLabelString)
          {
              string modellabelfile = vsmodelfile.Replace(".model", "_label.txt");
              BCVSMTrainer vmsTrainer = new BCVSMTrainer(configFiles, NegtiveLabelString);
              vmsTrainer.Training(PositiveLabel, traintextfile, vsmodelfile, modellabelfile);
          }

          public static void GeneratorFeatureFileForTextFile(string vsmodelfile, string PositiveLabel, string input_textfile, string output_featurefile, bool ignoreInvalid)
          {
              BCVSMConvertor featureGen = new BCVSMConvertor(new ModelFilePathInfo(vsmodelfile));
              featureGen.GenerateBatchFeatures(PositiveLabel, input_textfile, output_featurefile, ignoreInvalid);
          }


          public static void TrainVSMAndGenerateTraninigFeatureFileAndTestFeatureFile(string[] configFiles, string[] PositiveLabels, string trainingtextfile, string testtextfile, string temporaryfiledir, string NegtiveLabelString)
          {
              Bot.ML.Common.Utils.FileUtils.CreateOrCleanDir(temporaryfiledir);           

              string filename = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(trainingtextfile);
              filename = filename.Replace("training", "");

              foreach (string PositiveLable in PositiveLabels)
              {
                  string vsmodelfile = temporaryfiledir + "VSM_BC_" + filename + "_" + PositiveLable + ".model";
                  TrainVSM(configFiles, PositiveLable, trainingtextfile, vsmodelfile, NegtiveLabelString);

                  string trainingfeaturefile = temporaryfiledir + Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(trainingtextfile) + "_features_BC_" + PositiveLable + ".tsv";
                  string testfeaturefile = temporaryfiledir + Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(testtextfile) + "_features_BC_" + PositiveLable + ".tsv";

                  GeneratorFeatureFileForTextFile(vsmodelfile, PositiveLable, trainingtextfile, trainingfeaturefile, true);
                  GeneratorFeatureFileForTextFile(vsmodelfile, PositiveLable, testtextfile, testfeaturefile, false);
              }
          }
          

        public static void TrainVSMAndTLCModelAndTest(string[] configPaths, string[] PositiveLabels, string trainingtextfile, string testtextfile, string temporaryfiledir, string testoutputfile, string NegtiveLabelString)
        {
            Bot.ML.Common.Utils.FileUtils.CreateOrCleanDir(temporaryfiledir);            

            string filename = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(trainingtextfile);
            filename = filename.Replace("training", "");

            HashSet<string> PositiveLabelSet = new HashSet<string>(PositiveLabels);
            Dictionary<string, ModelFilePathInfo> vsmodelMap = new Dictionary<string, ModelFilePathInfo>();
            Dictionary<string, ModelFilePathInfo> tlcmodelMap = new Dictionary<string, ModelFilePathInfo>();
           
            Dictionary<string, string[]> configPathsMap = new Dictionary<string, string[]>();
            foreach (string PositiveLabel in PositiveLabelSet)
            {
                string vsmodelfile = temporaryfiledir + "VSM_BC_" + filename + "_" + PositiveLabel + ".model";                

                string tlcmodelfile = temporaryfiledir + "TLC_BC_" + filename + "_" + PositiveLabel + ".zip";
           
                vsmodelMap.Add(PositiveLabel, new ModelFilePathInfo(vsmodelfile));
                tlcmodelMap.Add(PositiveLabel, new ModelFilePathInfo(tlcmodelfile));
                configPathsMap.Add(PositiveLabel, configPaths);

            }
           
            BCTrainer executor = new BCTrainer(configPathsMap, vsmodelMap, tlcmodelMap, NegtiveLabelString);
            List<SourceData> cases = TextUtils.ReadSourceDataFromTextFile(trainingtextfile);
            executor.GenerateVectorSpace(cases);
            executor.GenerateTrainingFeaturesAndTrain(cases, filename, temporaryfiledir);

            BCPredictor predictor = new BCPredictor(configPathsMap, vsmodelMap, tlcmodelMap, NegtiveLabelString);
            predictor.PredictTextFile(testtextfile, testoutputfile);
        }

        public static void TestWithTrainedModels(string modelConfigFilepath, string testtextfile, string testoutputfile)
        {
            BCPredictor predictor = new BCPredictor(modelConfigFilepath);
            predictor.PredictTextFile(testtextfile, testoutputfile);
        }
        */
    }
}

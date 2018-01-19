using System;
using Bot.ML.Common.Data;
using LUTest.Classifier;
using Bot.Cluster.Test;
using Bot.Cluster.Clustering.TLCTrainer;
using System.Collections.Generic;
using Bot.Cluster.Clustering.TLCPredictor;
using System.IO;
using System.Text;
using LUTest.Utils;

namespace LUTest
{
    public class Program
    {
        static string[] labelconfigPaths = new string[] { "Bot.ML.Common.Res.Config.BiGram.json" };

        static string[] labelwithediconfigPaths = new string[] { "..\\..\\..\\..\\MLICModel\\config\\BiGram.json",
        "..\\..\\..\\..\\MLICModel\\config\\EdiSlot.json"};


        static string[] unlabelconfigPaths = new string[] { "..\\..\\..\\..\\MLICModel\\config\\ClusterVSMConfig.json" };


        /*
        public static void RunSuningLUTest()
        {
            string testinputfile = "D:\\data\\suning\\CX_1111\\Suning_Test_cx.tsv";
            string testoutputfile = "D:\\data\\suning\\CX_1111\\output\\MergedReport_2.tsv";

            SuningLUEngineTest test = new SuningLUEngineTest();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");           
            test.TestIntents(testinputfile, testoutputfile);            

            string summaryfile = testoutputfile.Replace(".tsv", "_Summary.tsv");

            TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
            executor.Summary(testoutputfile, summaryfile);
        }        
        */

        /*   public static void TrainVSMAndGenFeatureForMultipleClass()
           {
               string trainingtextfile = "D:\\data\\suning\\CX_1111\\Suning_Training_cx.tsv";
               string testtextfile = "D:\\data\\suning\\CX_1111\\Suning_Test_cx.tsv";

               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();

               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "mc\\output" + "_" + timestamp + "\\";

               MCTrainTest.TrainVSMAndGenerateTraninigFeatureFileAndTestFeatureFile(labelconfigPaths, trainingtextfile, testtextfile, dir);
           }

           public static void TrainAndTestForMultipleClass()
           {
               string trainingtextfile = "D:\\data\\suning\\CX_1111\\Suning_Training_cx.tsv";
               string testtextfile = "D:\\data\\suning\\CX_1111\\Suning_Test_cx.tsv";

               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();

               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "mc\\output" + "_" + timestamp + "\\";

               string testreportfile = dir + "Report_Suning_Test_cx_" + timestamp + ".tsv";
               string testsummaryfile = dir + "Report_Summary_Suning_Test_cx_" + timestamp + ".tsv";

               MCTrainTest.TrainVSMAndTLCModelAndTest(labelconfigPaths, trainingtextfile, testtextfile, dir, testreportfile);

               TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
               executor.Summary(testreportfile, testsummaryfile);
           }

           public static void TrainVSMAndGenFeatureForBinaryClass()
           {
               string trainingtextfile = "D:\\data\\suning\\CX_1111\\Suning_Training_cx.tsv";
               string testtextfile = "D:\\data\\suning\\CX_1111\\Suning_Test_cx.tsv";

               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();
               string testreportfile = trainingtextfile.Replace(".tsv", "_Report.tsv");

               string[] posLabels = {"Payment", "Promotion", "AskParameter", "Delivery", "None" };
               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "bc\\output" + "_" + timestamp + "\\";
               BCTrainTest.TrainVSMAndTLCModelAndTest(labelconfigPaths, posLabels, trainingtextfile, testtextfile, dir, testreportfile, "Other");
           }

           public static void TrainAndTestForBinaryClass()
           {
               var dummyType = typeof(BCTrainer);
               Console.WriteLine(dummyType.FullName);

               string trainingtextfile = "D:\\data\\suning\\CX_1207\\Suning_Training_cx.tsv";
               string testtextfile = "D:\\data\\suning\\CX_1207\\Suning_Test_cx.tsv";

               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();
               string[] posLabels = { "Payment", "Promotion", "AskParameter", "Delivery", "None" };

               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "bc\\output" + "_" + timestamp + "\\";

               string testreportfile = dir + "Report_Suning_Test_cx_" + timestamp + ".tsv";
               string testsummaryfile = dir + "Report_Summary_Suning_Test_cx_" + timestamp + ".tsv";

               BCTrainTest.TrainVSMAndTLCModelAndTest(labelconfigPaths, posLabels, trainingtextfile, testtextfile, dir, testreportfile, "Other");

               TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
               executor.Summary(testreportfile, testsummaryfile);
           }

           public static void TestPredictForICBinaryClass()
           {
               string testtextfile = "D:\\data\\suning\\CX_1111\\Suning_Test_cx.tsv";
               string timestamp = "123";//Utils.GetTimestamp();
               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(testtextfile) + "bc\\output" + "_" + timestamp + "\\";

               string testreportfile = dir + "Report_Suning_Test_cx_" + timestamp + ".tsv";

               BCTrainTest.TestWithTrainedModels("Bot.ML.Common.Res.ModelConfig.ic_bcmodelconfig.json", testtextfile, testreportfile);
           }

           public static void TrainAndTestForBinaryClass_WithEdiResult()
           {
               string trainingtextfile = "D:\\data\\suning\\CX_1111\\Suning_Training_cx_edi.tsv";
               string testtextfile = "D:\\data\\suning\\CX_1111\\Suning_Test_cx_edi.tsv";

               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();
               string[] posLabels = { "Payment", "Promotion", "AskParameter", "Delivery", "None" };

               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "bc\\output" + "_edi_" + timestamp + "\\";

               string testreportfile = dir + "Report_Suning_Test_cx_edi_" + timestamp + ".tsv";
               string testsummaryfile = dir + "Report_Summary_Suning_Test_cx_edi_" + timestamp + ".tsv";

               BCTrainTest.TrainVSMAndTLCModelAndTest(labelwithediconfigPaths, posLabels, trainingtextfile, testtextfile, dir, testreportfile, "Other");

               TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
               executor.Summary(testreportfile, testsummaryfile);
           }

           public static void TrainAndTestForBinaryClass_DA()
           {
               string trainingtextfile = "D:\\data\\suning\\DA_1117\\Suning_Training_DA.tsv";
               string testtextfile = "D:\\data\\suning\\DA_1117\\Suning_Test_DA.tsv";

               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();
               string[] posLabels = { "Desire",  "QuestionYN", "QuestionWH", "ConfirmYes", "ConfirmNo" };

               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "bc\\output" + "_" + timestamp + "\\";

               string fileName = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(testtextfile);           

               string testreportfile = dir + "Report_" + fileName + "_" + timestamp + ".tsv";
               string testsummaryfile = dir + "Report_Summary_" + fileName + "_" + timestamp + ".tsv";

               BCTrainTest.TrainVSMAndTLCModelAndTest(labelconfigPaths, posLabels, trainingtextfile, testtextfile, dir, testreportfile, "Other");

               TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
               executor.Summary(testreportfile, testsummaryfile);
           }

           public static void TestTrainedDAModels()
           {
               string testtextfile = "D:\\data\\suning\\DA_1117\\Suning_Test_DA.tsv";
               string modelconfigfile = "..\\..\\..\\..\\MLICModel\\modelconfig\\da_bcmodelconfig.json";
               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();

               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(testtextfile) + "bc\\output" + "_" + timestamp + "\\";

               Bot.ML.Common.Utils.FileUtils.CreateOrCleanDir(dir);
               string fileName = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(testtextfile);

               string testreportfile = dir + "Report_" + fileName + "_" + timestamp + ".tsv";
               string testsummaryfile = dir + "Report_Summary_" + fileName + "_" + timestamp + ".tsv";

               BCTrainTest.TestWithTrainedModels(modelconfigfile, testtextfile, testreportfile);
               TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
               executor.Summary(testreportfile, testsummaryfile);
           }

           public static void TestTrainedICModelsWithUtterance()
           {
               string modelconfigfilename = "..\\..\\..\\..\\MLICModel\\modelconfig\\ic_bcmodelconfig.json";
               string Utterance = "小米5有红色的吗？";

               BCPredictor executor = new BCPredictor(modelconfigfilename);            
               ICTextPredictionInfo info = executor.PredictUtterance(Utterance, null);

               Console.WriteLine(info.LabelString + ", " + info.Possibility.ToString());
           }
           public static void TestTrainedDAModelsWithUtterance(string[] args)
           {
               if (args.Length <= 0)
               {
                   Console.WriteLine("please specify your utterance.");
                   return;
               }
               string modelconfigfilename = "..\\..\\..\\..\\MLICModel\\modelconfig\\da_bcmodelconfig.json";
               string Utterance = args[0];//"小米5有红色的吗？";

               BCPredictor executor = new BCPredictor(modelconfigfilename);
               ICTextPredictionInfo info = executor.PredictUtterance(Utterance, null);

               Console.WriteLine(info.LabelString + ", " + info.Possibility.ToString());

           }

           public static void TrainClusterFeatures()
           {
               string trainingtextfile = "D:\\data\\suning\\Cluster\\SuningRaw.tsv";
               string timestamp = "1124-3";
               string trainingfeaturefile = "D:\\data\\suning\\Cluster\\SuningRaw_feature_" + timestamp + ".tsv";
               string tlcmodelfile = "D:\\data\\suning\\Cluster\\SuningRaw_tlc_" + timestamp + ".zip";
               string vsmodelfile = trainingtextfile.Replace(".tsv", "VSM_Unlabel_" + timestamp + ".model");

               string Reportfile = "D:\\data\\suning\\Cluster\\ClusterReport_SuningRaw_" + timestamp + ".tsv";
               int ClusterNumber = 6;

               UnlabelFeatureGenTest.TrainVSMAndGenerateTraninigFeatureFile(unlabelconfigPaths, trainingtextfile, trainingfeaturefile, vsmodelfile);

               TLCKMeansTrainer trainer = new TLCKMeansTrainer(vsmodelfile);
               trainer.Train(ClusterNumber, trainingtextfile, trainingfeaturefile, tlcmodelfile);

               string labeledtestfile = "D:\\data\\suning\\Cluster\\suning.tsv";
               List<SourceData> cases = Bot.ML.Common.Utils.TextUtils.ReadSourceDataFromTextFile(labeledtestfile);

               TLCKMeansPredictor predictor = new TLCKMeansPredictor(new ModelFilePathInfo(vsmodelfile), new ModelFilePathInfo(tlcmodelfile));
               using (StreamWriter w = new StreamWriter(File.Open(Reportfile, FileMode.Create), Encoding.UTF8))
               {
                   w.WriteLine(UnlabelPredictionData.GetHeader() + "\tLabel");
                   foreach (SourceData aCase in cases)
                   {
                       UnlabelPredictionData data = predictor.Predict(aCase.Id, aCase.Utterance);
                       w.WriteLine(data.ToString() + '\t' + aCase.Label);
                   }
               }
               //Console.WriteLine(score.Score.ToString());
           }

           public static void GetEdiResults()
           {
               string testfile = "D:\\data\\suning\\CX_1111\\Suning_Training_cx.tsv";
               string testfilewithedit = "D:\\data\\suning\\CX_1111\\Suning_Training_cx_edi.tsv";

               EdiSlotTagger tagger = new EdiSlotTagger();
               tagger.GetResultsForTestFile(testfile, testfilewithedit);
           }

           public static void TestLUService()
           {
               LUServiceTest.TestSingleUtterance("小米5有红色的吗？");
           }

           public static void TestTlcService()
           {
               //HttpTlcClient client = new HttpTlcClient();
               //PureLuResult result = client.Predict("小米5有红色的吗?");

               //Console.WriteLine(result.Intents[0].IntentName);
               string controller = "daprediction";

               string testtextfile = "D:\\data\\suning\\DA_1117\\Suning_Test_DA.tsv";
               string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();
               string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(testtextfile) + "bc\\output" + "_" + timestamp + "\\";

               Bot.ML.Common.Utils.FileUtils.CreateOrCleanDir(dir);
               string fileName = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(testtextfile);

               string testreportfile = dir + "Report_" + fileName + "_" + timestamp + ".tsv";
               string testsummaryfile = dir + "Report_Summary_" + fileName + "_" + timestamp + ".tsv";

               TLCServiceTest.TestFile(controller, testtextfile, testreportfile);

               TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
               executor.Summary(testreportfile, testsummaryfile);
           }

           public static void SplitFile(string filepath, double percentage)
           {
               string trainingfilepath = filepath.Replace(".tsv", "_Training.tsv");
               string testfilepath = filepath.Replace(".tsv", "_Test.tsv");

               DatasetSpliter spliter = new DatasetSpliter(filepath, percentage);
               spliter.Split(trainingfilepath, testfilepath);
           }
   */
        public static void Main(string[] args)
        {
            // TrainAndTestForMultipleClass();   
            // TrainAndTestForBinaryClass();         
            // TestPredictForICBinaryClass();

            // TrainAndTestForBinaryClass_WithEdiResult();

            //TrainClusterFeatures();

            //TrainAndTestForBinaryClass_DA();

            //TestTrainedDAModels();
            //TestTrainedDAModelsWithUtterance();
            //TestTrainedICModelsWithUtterance();

            //GetEdiResults();

            //TestLUService();

            //TestTlcService();

            //SplitFile("D:\\data\\homeadvisor\\Save\\HA_RS_1212.tsv", 0.7);
            //SplitFile("D:\\data\\homeadvisor\\Save\\HA_DA_1212.tsv", 0.7);

            // HomeAdvisorTrainAndTest.TrainAndTestForBinaryClass_DA();
            //HomeAdvisorTrainAndTest.TrainAndTestForBinaryClass_RS();

            //string configfilepath = TLCTrainerTesterTest.TestTrainer();

            string configfilepath = "D:\\data\\suning\\CX_1111\\output_20170104-164444\\Suning_CX_1111_BCConfig.json";
            TLCTrainerTesterTest.TestPredictor(configfilepath);

            Console.WriteLine("Finished!");          
        }
    }
}

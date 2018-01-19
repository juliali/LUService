using LUTest.Classifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUTest
{
    public class HomeAdvisorTrainAndTest
    {
        static string[] labelconfigPaths = new string[] { "Bot.ML.Common.Res.Config.BiGramEn.json" };
/*
        public static void TrainAndTestForBinaryClass_DA()
        {
            string trainingtextfile = "D:\\data\\homeadvisor\\DA_1212\\HA_DA_Training.tsv";
            string testtextfile = "D:\\data\\homeadvisor\\DA_1212\\HA_DA_Test.tsv";

            string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();
            string[] posLabels = { "Desire", "QuestionYN", "QuestionWH", "ConfirmYes", "ConfirmNo" };

            string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "bc\\output" + "_" + timestamp + "\\";

            string fileName = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(testtextfile);

            string testreportfile = dir + "Report_" + fileName + "_" + timestamp + ".tsv";
            string testsummaryfile = dir + "Report_Summary_" + fileName + "_" + timestamp + ".tsv";

            BCTrainTest.TrainVSMAndTLCModelAndTest(labelconfigPaths, posLabels, trainingtextfile, testtextfile, dir, testreportfile, "Other");

            TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
            executor.Summary(testreportfile, testsummaryfile);
        }

        public static void TrainAndTestForBinaryClass_RS()
        {            
            string trainingtextfile = "D:\\data\\homeadvisor\\RS_1212\\HA_RS_Training.tsv";
            string testtextfile = "D:\\data\\homeadvisor\\RS_1212\\HA_RS_Test.tsv";

            string timestamp = Bot.ML.Common.Utils.FileUtils.GetTimestamp();
            string[] posLabels = { "RS"};

            string dir = Bot.ML.Common.Utils.FileUtils.GetFileDirFromPath(trainingtextfile) + "bc\\output" + "_" + timestamp + "\\";

            string fileName = Bot.ML.Common.Utils.FileUtils.GetFileNameFromPath(testtextfile);

            string testreportfile = dir + "Report_" + fileName + "_" + timestamp + ".tsv";
            string testsummaryfile = dir + "Report_Summary_" + fileName + "_" + timestamp + ".tsv";

            BCTrainTest.TrainVSMAndTLCModelAndTest(labelconfigPaths, posLabels, trainingtextfile, testtextfile, dir, testreportfile, "Other");

            TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
            executor.Summary(testreportfile, testsummaryfile);
        }
*/
    }
}

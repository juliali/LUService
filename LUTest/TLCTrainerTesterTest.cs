using Bot.ML.Common.Data;
using Bot.ML.Common.Utils;
using Bot.TLC.Predicting.Main;
using Bot.TLC.Training.Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace LUTest
{
    public class TLCTrainerTesterTest
    {
        private static string GetOutputDirName()
        {
            string dir = "output_" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
            return dir;
        }
        public static string TestTrainer()
        {            
            string trainingtextfile = "D:\\data\\suning\\CX_1111\\Suning_Training_cx.tsv";

            string[] labelconfigPaths = new string[] { "Bot.ML.Common.Res.Config.BiGram.json" };
            string[] PositiveLabels = { "Payment", "Promotion", "AskParameter", "Delivery", "None" };
            string NegtiveLabel = "Other";

            List<SourceData> cases = TextUtils.ReadSourceDataFromTextFile(trainingtextfile);
            
            string containerName = "newtest";
            Trainer trainer = new Trainer(cases, containerName);

            BCConfigInfo configInfo = trainer.Train("Suning_CX_1111", labelconfigPaths, NegtiveLabel, PositiveLabels);
            string json = new JavaScriptSerializer().Serialize(configInfo);

            string dir = "D:\\data\\suning\\CX_1111\\" + GetOutputDirName();
            string outputconfigpath = dir + "\\Suning_CX_1111_BCConfig.json";

            FileUtils.CreateOrCleanDir(dir);
            using (StreamWriter w = new StreamWriter(outputconfigpath, false, Encoding.UTF8))
            {
                w.WriteLine(json);
            }

            return outputconfigpath;
        }

        public static void TestPredictor(string outputconfigpath)
        {
            string testtextfile = "D:\\data\\suning\\CX_1111\\Suning_Test_cx.tsv";

            Predictor predictor = new Predictor(outputconfigpath);
            List<SourceData> cases = TextUtils.ReadSourceDataFromTextFile(testtextfile);

            string dir = FileUtils.GetFileDirFromPath(outputconfigpath);

            string reportfile = dir + "\\Suning_CX_1111_Test_Report.tsv";
            
            predictor.Predict(cases, reportfile);

            string reportsummaryfile = dir + "Suning_CX_1111_Test_Report_Summary.tsv";

            TestResultSummeryExecutor executor = new TestResultSummeryExecutor();
            executor.Summary(reportfile, reportsummaryfile);
            
        }
    }
}

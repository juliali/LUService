using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUTest
{
    public class TestResultInfo
    {
        public int Id;
        public string Utterance;
        public string ExpectedLabel;
        public string ActualLabel;
    }

    public class LabelSummaryInfo
    {
        public string ExpectedLabel;
        public int ExpectedCaseNumber = 0;        
        public Dictionary<string, int> ActualLabelCountMap;

        public double Precision = 0.0;
        public double Recall = 0.0;
        public double F1Score = 0.0;
    }

    public class TestResultSummeryExecutor
    {
        private string[] lines;
        public TestResultSummeryExecutor()
        {
           
        }

        public void Summary(string testreportfile, string outputfile)
        {
            this.lines = System.IO.File.ReadAllLines(testreportfile);
            Dictionary<string, LabelSummaryInfo> summaryResults = new Dictionary<string, LabelSummaryInfo>();

            long totalLatencyMS = 0;
            int testcaseNumber = 0;

            foreach(string line in this.lines)
            {
                string[] tmps = line.Split('\t');

                int Id = -1;

                try
                {
                    Id = int.Parse(tmps[0]);

                    testcaseNumber++;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                TestResultInfo aCase = new TestResultInfo();
                aCase.Id = Id;
                aCase.Utterance = tmps[1];
                aCase.ExpectedLabel = tmps[2];
                aCase.ActualLabel = tmps[3];

                if (tmps.Length > 4)
                {
                    long latency = long.Parse(tmps[4]);
                    totalLatencyMS += latency;
                }

                //if (aCase.ExpectedLabel != aCase.ActualLabel)
                //{
                //    Console.WriteLine("Expected: " + aCase.ExpectedLabel + "; Actual: " + aCase.ActualLabel);
                //}

                if (!summaryResults.ContainsKey(aCase.ExpectedLabel))
                {
                    LabelSummaryInfo labelSummary = new LabelSummaryInfo();
                    labelSummary.ExpectedLabel = aCase.ExpectedLabel;
                    labelSummary.ExpectedCaseNumber = 1;

                    labelSummary.ActualLabelCountMap = new Dictionary<string, int>();
                    labelSummary.ActualLabelCountMap.Add(aCase.ActualLabel, 1);

                    summaryResults.Add(aCase.ExpectedLabel, labelSummary);                   
                }
                else
                {
                    LabelSummaryInfo labelSummary = summaryResults[aCase.ExpectedLabel];
                    labelSummary.ExpectedCaseNumber += 1;

                    if (!labelSummary.ActualLabelCountMap.ContainsKey(aCase.ActualLabel))
                    {                        
                        labelSummary.ActualLabelCountMap.Add(aCase.ActualLabel, 1);
                    }
                    else
                    {
                        labelSummary.ActualLabelCountMap[aCase.ActualLabel] += 1;
                    }

                    summaryResults[aCase.ExpectedLabel] =  labelSummary;
                }
            }

            ////
            int labelNumber = summaryResults.Count;
            string[] labels = summaryResults.Keys.ToArray<string>();

            using (StreamWriter w = new StreamWriter(File.Open(outputfile, FileMode.Create), Encoding.UTF8))
            {
                string header = "Expected\\Actual" + "\tActual_" + string.Join("\tActual_", labels) + '\t' + "Precision" + '\t' + "Recall" + '\t' + "F1Score";
                w.WriteLine(header);

                foreach (string label in labels)
                {
                    string summaryLine = "Expected_" + label;

                    int ExpectedCaseNumber = summaryResults[label].ExpectedCaseNumber;

                    int AcutalEqualExpectedCaseNumber = 0;

                    if (summaryResults[label].ActualLabelCountMap.ContainsKey(label))
                    {
                        AcutalEqualExpectedCaseNumber = summaryResults[label].ActualLabelCountMap[label];
                    }

                    int TotalActualExpectedCaseNumber = 0;

                    foreach (string otherlabel in labels)
                    {
                        int num1 = 0;
                        if (summaryResults[otherlabel].ActualLabelCountMap.ContainsKey(label))
                        {
                            num1 =  summaryResults[otherlabel].ActualLabelCountMap[label];
                        }

                        TotalActualExpectedCaseNumber += num1;

                        int num2 = 0;

                        if (summaryResults[label].ActualLabelCountMap.ContainsKey(otherlabel))
                        {
                            num2 = summaryResults[label].ActualLabelCountMap[otherlabel];
                        }
                        summaryLine += '\t' + num2.ToString();
                    }

                    double precision = 0.0;

                    if (TotalActualExpectedCaseNumber > 0)
                    {
                        precision = (double)AcutalEqualExpectedCaseNumber / (double)TotalActualExpectedCaseNumber;
                    }

                    double recall = 0.0;

                    if (ExpectedCaseNumber > 0)
                    { 
                        recall = (double)AcutalEqualExpectedCaseNumber / (double)ExpectedCaseNumber;
                    }

                    double f1score = 0.0;

                    if (precision > 0 || recall > 0)
                    { 
                        f1score = 2.0 * precision * recall / (precision + recall);
                    }

                    summaryResults[label].Precision = precision;
                    summaryResults[label].Recall = recall;
                    summaryResults[label].F1Score = f1score;

                    summaryLine += '\t' + precision.ToString() + '\t' + recall.ToString() + '\t' + f1score.ToString();

                    w.WriteLine(summaryLine);
                }

                if (totalLatencyMS > 0)
                { 
                    long averageLatencyNumber = totalLatencyMS / ((long)testcaseNumber);                
                    w.WriteLine('\n' + "Total Latency MS: " + totalLatencyMS.ToString() + "; Testcase Number: " + testcaseNumber + "; Average Latency MS: " + averageLatencyNumber);
                }
            }
        }
    }
}

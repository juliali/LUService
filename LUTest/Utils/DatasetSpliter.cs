using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUTest.Utils
{
    public class DatasetSpliter
    {
        private string[] lines;
        private double TrainingDataPercentage;
        public DatasetSpliter(string inputfile, double percentage)
        {
            this.lines = System.IO.File.ReadAllLines(inputfile);
            this.TrainingDataPercentage = percentage;
        }

        public void Split(string trainingdatafile, string testdatafile)
        {
           // string header = lines[0];

            using (StreamWriter w = new StreamWriter(File.Open(trainingdatafile, FileMode.Create), Encoding.UTF8))
            {
                using (StreamWriter w2 = new StreamWriter(File.Open(testdatafile, FileMode.Create), Encoding.UTF8))
                {
                   // w.WriteLine(header);
                   // w2.WriteLine(header);

                    Random random = new Random();
                    

                    for (int i = 0; i < lines.Length; i ++)
                    {
                        double current = random.NextDouble() * 1.0;

                        if (current > TrainingDataPercentage)
                        {
                            w2.WriteLine(lines[i]);
                        }
                        else
                        {
                            w.WriteLine(lines[i]);
                        }
                    }
                }
            }
        }
    }
}

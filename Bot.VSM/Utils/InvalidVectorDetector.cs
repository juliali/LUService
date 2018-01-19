using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.VSM.Utils
{
    public class InvalidVectorDetector
    {
        public InvalidVectorDetector()
        {

        }

        public int Detect(string featurefilepath)
        {
            List<string> result = new List<string>();

            string[] lines = System.IO.File.ReadAllLines(featurefilepath);

            for(int index = 1; index < lines.Length; index ++)
            {
                string line = lines[index];
                string[] tmps = line.Split('\t');
                bool IsValid = false;
                for (int i = 1; i < tmps.Length; i ++)
                {
                    double value = double.Parse(tmps[i]);
                    if (value != 0.0)
                    {
                        IsValid = true;
                        break;
                    }
                }

                if (!IsValid)
                {
                    result.Add(line);
                }
            }            

            return result.Count;
        }
    }
}

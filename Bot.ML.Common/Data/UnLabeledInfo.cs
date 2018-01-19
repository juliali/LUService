using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Data
{
    public class UnlabelExample
    {
        public float[] Features;

        override
        public string ToString()
        {
            string outputstr = "";

            for (int i = 0; i < Features.Length; i++)
            {
                outputstr += Features[i].ToString();
                if (i < Features.Length - 1)
                {
                    outputstr += '\t';
                }
            }

            return outputstr;
        }
    }

    public class UnlabelScoreData
    {
        public float[] Score;
    }

    public class UnlabelPredictionData
    {
        public int Id;
        public string Utterance;
        public int ClusterIndex;

        override
        public string ToString()
        {
            string result = Id.ToString() + '\t' + Utterance + '\t' + ClusterIndex.ToString();

            return result;
        }

        public static string GetHeader()
        {
            string result = "Id\tUtterance\tClusterIndex";
            return result;
        }
    }
}

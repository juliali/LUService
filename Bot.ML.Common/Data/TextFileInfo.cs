using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Data
{
   /* [Serializable]
    public class LabledTextFileInfo
    {
        public int Id = -1;
        public string Utterance = null;
        public string Label = null;
        public string EdiResult = null;

        override
        public string ToString()
        {
            string str = Id.ToString() + '\t' + Utterance + '\t' + Label;

            return str;
        }
    } */

    [Serializable]
    public class UnlabledTextFileInfo
    {
        public int Id = -1;
        public string Utterance = null;
        public string EdiResult = null;

        override
        public string ToString()
        {
            string str = Id.ToString() + '\t' + Utterance;

            return str;
        }
    }

    public class ICTextPredictionInfo
    {
        public string LabelString;
        public float Possibility;

        override
        public string ToString()
        {
            string result = LabelString + '\t' + Possibility.ToString();
            return result;
        }
    }

    public class MCConfigInfo
    {        
        public string[] VSMConfigPaths;
        public ModelFilePathInfo VSModelPath;
        public string LabelFilePath;
        public ModelFilePathInfo TLCModelPath;
    }
}

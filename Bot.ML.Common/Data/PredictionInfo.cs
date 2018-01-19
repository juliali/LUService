using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Data
{

    public class ICPredictionOutputInfo
    {
        public float LabelValue;
        public float Possibility;
    }

    public class ICPredictionInputInfo
    {
        public string Utterance { get; set; } = "";
        public string EdiResult { get; set; } = "";

    }
}

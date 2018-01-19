using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Data
{
    public class RuleBaseExtractorInfo
    {
        public string Regex { get; set; }
        public string Type { get; set; }
        public double BottomLengthPercentage { get; set; } = 0;
    }   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUTest.Data
{
    public class LUSrvRespInfo
    {
        public LUResultsInfo LuResults;
    }

    public class LUResultsInfo
    {
        public SingleEngineRespInfo RuleBased;
        public SingleEngineRespInfo RuleBasedExtractor;
        public SingleEngineRespInfo IntentClassifier;
        public SingleEngineRespInfo EdiModule;
        public SingleEngineRespInfo MLBased;
        public SingleEngineRespInfo LUAggregator;
    }

    public class SingleEngineRespInfo
    {
        public string Source;
        public string RawQuery;
        public IntentRespInfo[] Intents;
        public SegmentRespInfo[] Segements;
    }

    public class IntentRespInfo
    {
        public string IntentName;
        public double Confidence = 0;
    }

    public class SegmentRespInfo
    {
        public string RawTagName;
        public string ExtractedValue;
        public string TagType;
        public string TagName;
        public string Operator;
        public string NormalizedValue;
        public double Confidence = 0;
    }
}

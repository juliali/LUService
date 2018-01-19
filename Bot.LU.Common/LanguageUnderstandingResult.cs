using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.ML.Common
{
    public class LanguageUnderstandingResult
    {

        public List<ComplexLuResult> LuResults;

        public LanguageUnderstandingResult()
        {
            LuResults = new List<ComplexLuResult>();
        }
    }

    public class ComplexLuResult
    {
        public string TableName;
        public string Intent;
        public List<string> TargetColumns;
        public List<Condition> Conditions;
        public string Location;

        public ComplexLuResult()
        {
            TargetColumns = new List<string>();
            Conditions = new List<Condition>();
        }

        public override string ToString()
        {
            return  "\tIntent: " + Intent + "\n" +
                    "\tTable: " + TableName + "\n" +
                    "\tTargetColumns: " + string.Join(",", TargetColumns) + "\n" +
                    "\tConditions: " + string.Join(",", Conditions);
        }
    }

    public class Condition
    {
        public string ColumnName;
        public string ColumnType;
        public List<Tuple<string, string>> ColumnValues;

        public Condition()
        {
            ColumnValues = new List<Tuple<string, string>>();
        }

        public override string ToString()
        {
            return ColumnName + " " +
                   string.Join(",", ColumnValues);
        }
    }

    public class MixedLuResult
    {
        public Dictionary<string, PureLuResult> LuResults { get; set; }
    }

    public class PureLuResult
    {
        public string Source;

        public string RawQuery;

        public List<Intent> Intents = new List<Intent>();
        public List<Segment> Segments = new List<Segment>();

        public override string ToString()
        {
            return string.Join(",", Intents) + " " + string.Join(",", Segments);
        }
    }

    public class Intent
    {
        public string IntentName;
        public string SubIntentName;
        public double Confidence;
        public override string ToString()
        {
            return " Intent:" + IntentName + " Score:" + Confidence + " ";
        }
    }

    public class Segment
    {
        public string RawTagName;
        public string ExtractedValue;
        public string TagType;
        public string TagName;
        public string Operator;
        public string NormalizedValue;
        public double Confidence;

        public override string ToString()
        {
            return TagType + ":" + TagName + ":" + NormalizedValue;
        }
    }

    public static class LanguageUnderstandingExtension
    {
        public static Condition DeepClone(this Condition current)
        {
            var copy = new Condition
            {
                ColumnName = current.ColumnName,
                ColumnType = current.ColumnType,
                ColumnValues = current.ColumnValues.Select(x => new Tuple<string, string>(x.Item1, x.Item2)).ToList()
            };
            return copy;
        }

        public static ComplexLuResult DeepClone(this ComplexLuResult current)
        {
            var copy = new ComplexLuResult
            {
                TableName = current.TableName,
                Intent = current.Intent,
                TargetColumns = current.TargetColumns.Select(x => x).ToList(),
                Conditions = current.Conditions.Select(x => x.DeepClone()).ToList()
            };
            return copy;
        }

        public static LanguageUnderstandingResult DeepClone(this LanguageUnderstandingResult current)
        {
            var copy = new LanguageUnderstandingResult
            {
                LuResults = current.LuResults.Select(x => x.DeepClone()).ToList()
            };
            return copy;
        }

        public static string PrintDebug(this LanguageUnderstandingResult current)
        {
            string debug = "";
            debug += "LuCnt:" + current.LuResults.Count + "\t";
            if (current.LuResults.Count > 0)
                debug += "[0]" + "TableName:" + current.LuResults[0].TableName + " Intent:" + current.LuResults[0].Intent + "\t";
            if (current.LuResults.Count > 1)
                debug += "[1]" + "TableName:" + current.LuResults[1].TableName + " Intent:" + current.LuResults[1].Intent + "\t";

            return debug;
        }

    }
}

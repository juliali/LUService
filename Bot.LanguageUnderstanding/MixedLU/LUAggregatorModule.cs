using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.ML.Common;

namespace Bot.LanguageUnderstanding.MixedLU
{
    class LUAggregatorModule : IPureLuModule
    {
        private List<string> StopWord = new List<string>();
        private List<string> TableEngineBlackList = new List<string>();

        private string StopwordFile;
        private string TableEngineBlacklistFile;

        public LUAggregatorModule(string dir) : base(dir)
        {
        }

        public override void Update(string query, Dictionary<string, PureLuResult> current)
        {
            Intent MergedIntent = MergeIntent(query,current);        
            
            var segmentMultiList4Tartget =
                current.Select(x => x.Value.Segments.Where(y => y.TagType == "Target").ToList()).ToList();
            var segmentMultiList4Constraint =
                current.Select(x => x.Value.Segments.Where(y => y.TagType == "Constraint").ToList()).ToList();

            var constraintList = MergeSegments(segmentMultiList4Constraint);
            var targetList = MergeSegments(segmentMultiList4Tartget);

            var mergedSegList = constraintList;
            mergedSegList.AddRange(targetList);

            current[Constants.LU_AGGREGATOR_MODULE_NAME] = new PureLuResult
            {
                RawQuery = query,
                Source = Constants.LU_AGGREGATOR_MODULE_NAME,
                Intents = new List<Intent>() { MergedIntent },
                Segments = mergedSegList
            };
        }

        private List<Segment> MergeSegments(List<List<Segment>> segmentMultiList)
        {
            var constraintDict = new Dictionary<string, Segment>();
            foreach (var segmentList in segmentMultiList)
            {
                foreach (var segment in segmentList)
                {
                    if (!constraintDict.ContainsKey(segment.TagName))
                    {
                        constraintDict.Add(segment.TagName, segment);
                    }
                    else if (constraintDict[segment.TagName].Confidence < segment.Confidence)
                    {
                        constraintDict[segment.TagName] = segment;
                    }

                }
            }

            var mergedSegList = constraintDict.Values.ToList();

            return mergedSegList;
        }

        private Intent MergeIntent(string query, Dictionary<string, PureLuResult> current)
        {
            //Doesnot merge MLBased Classifier Currently
            var intentMultiList = current.Where(x => x.Key != Constants.MLBASED_MODULE_NAME).Select(x => x.Value.Intents).ToList();

            // Generate TableName Candidates Set
            var intentDict = new Dictionary<string, Intent>();
            var intentScoreMap = new Dictionary<string, double>();

            Intent MergedIntent = null;

            foreach (var intentList in intentMultiList)
            {
                foreach (var intent in intentList)
                {
                    if (!intentDict.ContainsKey(intent.IntentName))
                    {
                        intentDict.Add(intent.IntentName, intent);

                        if (intentScoreMap.ContainsKey(intent.IntentName))
                        {
                            if (intent.Confidence > intentScoreMap[intent.IntentName])
                            {
                                intentScoreMap[intent.IntentName] = intent.Confidence;
                            }
                        }
                        else
                        {
                            intentScoreMap.Add(intent.IntentName, intent.Confidence);
                        }
                    }
                }
            }

            //Pick up Top Intent
            if (intentDict.Count > 0)
            {
                var highestConfidenceIntent = intentDict.Keys.First();
                var highestConfidenceScore = intentScoreMap[highestConfidenceIntent];

                foreach (var intent in intentDict.Keys)
                {
                    if (intentScoreMap[intent] > highestConfidenceScore)
                    {
                        highestConfidenceIntent = intent;
                        highestConfidenceScore = intentScoreMap[intent];
                    }
                }

                MergedIntent = intentDict[highestConfidenceIntent];


            }

            //Add Default Intents
            if (intentDict.Count == 0)
            {
                MergedIntent = GetDefaultIntent(current);
            }

            return MergedIntent;
        }

        private void InitStopWord()
        {
           using (var sr = new StreamReader(GetPath(StopwordFile)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    StopWord.Add(line.Trim());
                }
            }
        }
        private void InitTableEngineBlacklist()
        {
           using (var sr = new StreamReader(GetPath(TableEngineBlacklistFile)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    TableEngineBlackList.Add(line.Trim());
                }
            }
        }

        private Intent GetDefaultIntent(Dictionary<string, PureLuResult> luResults)
        {
            Intent implictIntent = new Intent { IntentName = "Other", SubIntentName = "Other" };

            //No knowledge from Table engine.
            //Drop to other engine.
            var rawQuery = luResults.Values.Where(x => x.RawQuery != null).ElementAt(0).RawQuery;
            foreach (var word in TableEngineBlackList)
            {
                if (rawQuery.Contains(word))
                {
                    return implictIntent;
                }
            }

            //Get All segments         
            var segmentList = new List<Segment>();
            foreach (var lu in luResults.Values)
            {
                if (lu.Segments == null) continue;
                segmentList.AddRange(lu.Segments.Where(x => x.TagType != null && x.TagType.Equals("Constraint")));
            }

            //No Slot extracted
            if (segmentList.Count == 0) return implictIntent;

            //If 50% of the query is extracted by slot tagging
            //"None" intent will be set
            //Final intent would be determined from context manager
            if (IsFullyUnderstood(rawQuery, segmentList))
            {
                return new Intent { IntentName = "None", SubIntentName = null };
            }

            return implictIntent;

        }

        private bool IsFullyUnderstood(string rawQuery, List<Segment> segmentList)
        {
            var hit = new bool[rawQuery.Length];
            foreach (var segment in segmentList)
            {
                var extractedValue = segment.ExtractedValue;
                var idx = rawQuery.IndexOf(extractedValue, 0, StringComparison.Ordinal);

                while (idx > -1)
                {
                    for (var i = 0; i < extractedValue.Length; i++)
                    {
                        hit[idx + i] = true;
                    }
                    idx = rawQuery.IndexOf(extractedValue, idx + extractedValue.Length, StringComparison.Ordinal);
                }
            }

            var queryChar = rawQuery.ToCharArray();
            var extractedParts = string.Empty;
            for (var i = 0; i < hit.Length; i++)
            {
                if (hit[i])
                {
                    extractedParts += queryChar[i];
                }
            }

            //Filted Stopword
            foreach (var w in StopWord)
            {
                rawQuery = rawQuery.Replace(w, string.Empty);
                extractedParts = extractedParts.Replace(w, string.Empty);
            }

            return IsValidRatio(rawQuery.Length, extractedParts.Length);
        }

        private static bool IsValidRatio(int queryLen, int segmentLen)
        {
            if (segmentLen >= queryLen || segmentLen * 1.0 / queryLen >= 0.5)
            {
                return true;
            }
            return false;
        }

        public override void Initialze(dynamic jsonConfig)
        {
            StopwordFile = jsonConfig.Stopwords.Value;
            TableEngineBlacklistFile = jsonConfig.TableEngineBlacklist.Value;

            InitStopWord();
            InitTableEngineBlacklist();
        }
    }
}

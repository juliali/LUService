using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bot.ML.Common;

namespace Bot.LanguageUnderstanding.MixedLU
{
    class RuleBasedModule : IPureLuModule
    {
        private List<List<string>> _targetRules = new List<List<string>>();
        private List<List<string>> _conditionRules = new List<List<string>>();
        private List<List<string>> _intentRules = new List<List<string>>();

        private string targetColumnsPath;
        private string conditionsPath;
        private string queryTypePath;

        public RuleBasedModule(string dir) : base (dir)
        {
        }
        
        public override void Update(string query, Dictionary<string, PureLuResult> current)
        {
            var result = UnderstandQuery(query);
            current["RuleBased"] = result;
        }

        private PureLuResult UnderstandQuery(string query)
        {     
            var targetColumns = ExtractTargetColumns(query);
            var conditions = ExtractConditions(query);
            var intentType = ExtractIntentType(query);

            PureLuResult pureResult = new PureLuResult();
            if (!string.IsNullOrEmpty(intentType))
            {
                pureResult.Intents.Add(new Intent() { IntentName = intentType, Confidence = 1.0 });
            }
            foreach (var target in targetColumns)
            {
                pureResult.Segments.Add(new Segment() { TagType = "Target", TagName = target });
            }
            foreach (var condition in conditions)
            {
                pureResult.Segments.Add(new Segment() { TagType = "Constraint", TagName = condition.ColumnName, Operator = condition.ColumnValues[0].Item1, NormalizedValue = condition.ColumnValues[0].Item2, ExtractedValue = condition.ColumnValues[0].Item2});
            }

            return pureResult;
        }

        private string ExtractIntentType(string query)
        {
            foreach (var rule in _intentRules)
            {
                if (query.Contains(rule[1].Trim()))
                {
                    return rule[0].Trim();
                }
            }
            return string.Empty;
        }

        internal string ExtractTableName(string query)
        {
            return "AskParameter";
        }

        internal List<string> ExtractTargetColumns(string query)
        {
            var targetColumns = new List<string>();

            foreach (var column in _targetRules)
            {
                if (query.Contains(column[1]))
                {
                    targetColumns.Add(column[0]);
                }
            }

            return targetColumns;
        }

        internal List<Condition> ExtractConditions(string query)
        {
            var conditions = new List<Condition>();

            #region Conditions

            foreach (var rule in _conditionRules)
            {
                if (query.Contains(rule[0]))
                {
                    var columnName = rule[1];
                    var columnType = rule[2];
                    var operation = rule[3];
                    var columnValues = new List<Tuple<string, string>>();
                    for (var i = 4; i < rule.Count; i++)
                    {
                        columnValues.Add(new Tuple<string, string>(operation, rule[i]));
                    }
                    conditions.Add(new Condition() { ColumnName = columnName, ColumnType = columnType, ColumnValues = columnValues });
                }
            }

            #endregion

            return conditions;
        }

        public override void Initialze(dynamic jsonConfig)
        {
            targetColumnsPath = jsonConfig.TargetFile.Value;
            conditionsPath = jsonConfig.ConditionFile.Value;
            queryTypePath = jsonConfig.IntentFile.Value;

            using (var sr = new StreamReader(GetPath(targetColumnsPath)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t').Select(x => x.Trim()).ToList();
                    _targetRules.Add(items);
                }
            }

            using (var sr = new StreamReader(GetPath(conditionsPath)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t').Select(x => x.Trim()).ToList();
                    _conditionRules.Add(items);
                }
            }

            using (var sr = new StreamReader(GetPath(queryTypePath)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t').Select(x => x.Trim()).ToList();
                    _intentRules.Add(items);
                }
            }
        }
    }
}

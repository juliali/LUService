using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bot.ML.Common;


namespace Bot.LanguageUnderstanding.RuleBaseLU
{
    public class RuleBaseLuEngine
    {
        private readonly List<List<string>> _targetRules = new List<List<string>>();
        private readonly List<List<string>> _conditionRules = new List<List<string>>();
        private readonly List<List<string>> _queryTypeRules = new List<List<string>>();

        public RuleBaseLuEngine()
        {
            const string targetColumnsPath = "../../../Data/RuleBasedLU/TargetColumns.txt";
            using (var sr = new StreamReader(targetColumnsPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t').Select(x => x.Trim()).ToList();
                    _targetRules.Add(items);
                }
            }

            const string conditionsPath = "../../../Data/RuleBasedLU/Conditions.txt";
            using (var sr = new StreamReader(conditionsPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t').Select(x => x.Trim()).ToList();
                    _conditionRules.Add(items);
                }
            }

            const string queryTypePath = "../../../Data/RuleBasedLU/Intent.txt";
            using (var sr = new StreamReader(queryTypePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line.Trim().StartsWith("###")) continue;
                    var items = line.Split('\t').Select(x => x.Trim()).ToList();
                    _queryTypeRules.Add(items);
                }
            }
        }

        //public LanguageUnderstandingResult Parse(ContextEntry contextManager)
        //{
        //    var ruleBaseResult = UnderstandQuery(contextManager.Query);

        //    var luResult = new LanguageUnderstandingResult {LuResults = ruleBaseResult };

        //    contextManager.UpdateLanguageUnderstandingResult(luResult);

        //    return luResult;
        //}

        // ReSharper disable once UnusedMember.Local
        private List<ComplexLuResult> UnderstandQuery(string query)
        {
            var tableName = ExtractTableName(query);
            var targetColumns = ExtractTargetColumns(query);
            var conditions = ExtractConditions(query);
            var queryType = ExtractQueryType(query, targetColumns);

            var result = new ComplexLuResult
            {
                TableName = tableName,
                Intent = queryType,
                TargetColumns = targetColumns,
                Conditions = conditions
            };

            return new List<ComplexLuResult>() { result };
        }

        private string ExtractQueryType(string query, List<string> targetColumns)
        {
            foreach (var rule in _queryTypeRules)
            {
                if (query.Contains(rule[1].Trim()))
                {
                    return rule[0].Trim();
                }
            }
            return targetColumns.Count > 0 ? "Query" : "Others";
        }
        
        internal string ExtractTableName(string query)
        {
            return "Phone";
        }

        internal List<string>  ExtractTargetColumns(string query)
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
                    var operation  = rule[3];
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
    }
}

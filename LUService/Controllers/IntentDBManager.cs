using Bot.ML.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUService.Controllers
{
    public class RulebasedClassifier
    {
        public Int64 ClassifierId;
        public Int64 LabelId;
        public string IntentName;
        public string SubIntentName;
        public Int64 RuleId;
        public string RegexRule;
        public string RuleType;
    }

    public class Intent
    {
        public long IntentId;
        public string IntentName;
        public string SubIntentName;
    }

    public class IntentDBManager
    {
        private static DatabaseLoader loader = null;

        static IntentDBManager()
        {
            if (loader == null)
            {
                string sqlConn = System.Configuration.ConfigurationManager.AppSettings["LUToolConnectionString"];
                loader = new DatabaseLoader(sqlConn);
            }
        }

        /*
        #region Intent Management
        private static IEnumerable<Intent> LoadIntents(string sql)
        {
            var results = loader.Load(sql);
            foreach (var r in results)
            {
                Intent intent = new Intent
                {
                    IntentId = long.Parse(r["IntentId"]),
                    IntentName = r["IntentName"],
                    SubIntentName = r["SubIntentName"]
                };

                yield return intent;
            }
        }

        //Retrieve
        public static IEnumerable<Intent> GetIntents()
        {
            string sqlCommand = "SELECT [LU.Intent].Id AS IntentId, IntentName,SubIntentName FROM [LU.Intent]";
            return LoadIntents(sqlCommand);
        }

        public static IEnumerable<Intent> GetIntents(string intent, string subintent)
        {
            string sqlCommand = "SELECT [LU.Intent].Id AS IntentId, IntentName,SubIntentName FROM [LU.Intent] WHERE IntentName='" + intent + "' AND SubIntentName='" + subintent + "'";
            return LoadIntents(sqlCommand);
        }

        public static IEnumerable<Intent> GetIntents(Int64 intentId)
        {
            string sqlCommand = "SELECT [LU.Intent].Id AS IntentId, IntentName,SubIntentName FROM [LU.Intent] WHERE Id= " + intentId;
            return LoadIntents(sqlCommand);
        }

        public bool IsIntentExist(string intent, string subintent)
        {
            string sqlCommand = "SELECT [LU.Intent].Id AS IntentId, IntentName,SubIntentName FROM [LU.Intent] WHERE IntentName='" + intent + "' AND SubIntentName='" + subintent + "'";
            var results = LoadIntents(sqlCommand);
            if (results.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Intent InsertIntent(string intent, string subintent)
        {
            if(string.IsNullOrEmpty(intent) || string.IsNullOrEmpty(subintent))
            {
                return null;
            }

            var intents = GetIntents(intent, subintent);
            if(intents.Any())
            {
                return intents.First();
            }
            else
            {
                string sql = string.Format("INSERT INTO [LU.Intent](IntentName,SubIntentName) values('{0}','{1}') select @@IDENTITY as 'Identity'",intent,subintent);
                Int64 intentId = loader.Insert(sql);

                return new Intent
                {
                    IntentId = intentId,
                    IntentName = intent,
                    SubIntentName = subintent
                };
            }
        }

        private static bool DeleteIntentDefinition(Int64 intentId)
        {
            string sql = string.Format("DELETE FROM [LU.Intent] WHERE Id = {0}", intentId);
            var result = loader.ExecuteCommand(sql);

            if (result > 0 && result != int.MaxValue)
            {
                return true;
            }

            return false;
        }

        public static bool DeleteIntent(Int64 intentId)
        {
            //Delete the all the Intent Rules attached to this intent
            var intentRules = GetIntentRulesbyIntentId(intentId);
            foreach(var rule in intentRules)
            {
                if (!DeleteIntentRule(rule.RuleId))
                {
                    return false;
                }
            }

            //delete intent
            if(!DeleteIntentDefinition(intentId))
            {
                return false;
            }

            return true;
        }

        public static Intent UpdateIntent(Int64 intentId, string intent, string subintent)
        {
            var intents = GetIntents(intentId);
            if (intents.Any())
            {
                string sql = string.Format("Update [LU.Intent] SET IntentName = '{0}', SubIntentName = '{1}' WHERE Id = {2}", intent, subintent, intentId);
                var result = loader.ExecuteCommand(sql);

                if (result > 0 && result != int.MaxValue)
                {
                    return new Intent
                    {
                        IntentId = intentId,
                        IntentName = intent,
                        SubIntentName = subintent
                    };
                }

            }

            return null;
        }

        #endregion
        */

        #region Intent Rule Management
        private static IEnumerable<RulebasedClassifier> LoadIntentRules(string sql)
        {
            var results = loader.Load(sql);
            foreach (var r in results)
            {
                RulebasedClassifier intent = new RulebasedClassifier
                {
                    LabelId = long.Parse(r["LabelId"]),
                    ClassifierId = long.Parse(r["ClassifierId"]),
                    //IntentName = r["IntentName"],
                    //SubIntentName = r["SubIntentName"],
                    RuleId = long.Parse(r["Id"]),
                    RegexRule = r["RegexRule"],
                    RuleType = r["RuleType"]
                };

                yield return intent;
            }
        }

        public static IEnumerable<RulebasedClassifier> GetIntentRules(int classifierId)
        {
            string sqlCommand = "SELECT * FROM RulebasedClassifier WHERE ClassifierId = " + classifierId;
            return LoadIntentRules(sqlCommand);
        }

        public static IEnumerable<RulebasedClassifier> GetIntentRules(int classifierId, int labelId)
        {
            string sqlCommand = string.Format("SELECT * FROM RulebasedClassifier WHERE ClassifierId = {0} AND LabelId = {1}", classifierId, labelId);
            return LoadIntentRules(sqlCommand);
        }

        public static IEnumerable<RulebasedClassifier> GetRulesbyRuleId(Int64 ruleId, Int64 classifierId)
        {
            string sqlCommand = string.Format("SELECT * FROM RulebasedClassifier WHERE ClassifierId = {0} AND Id = {1}", classifierId, ruleId);
            return LoadIntentRules(sqlCommand);
        }

        public static RulebasedClassifier InsertRule(Int64 classifierId, Int64 labelId , string regexRule, string ruleType)
        {

            string sql = string.Format("INSERT INTO [RulebasedClassifier](ClassifierId,LabelId,RegexRule,RuleType) values({0},{1},'{2}','{3}') select @@IDENTITY as 'Identity'", classifierId, labelId, regexRule, ruleType);
            Int64 intentRuleId = loader.Insert(sql);

            return new RulebasedClassifier
            {
                LabelId = labelId,
                ClassifierId = classifierId,
                //IntentName = intent,
                //SubIntentName = subintent,
                RuleId = intentRuleId,
                RegexRule = regexRule,
                RuleType = ruleType
            };
        }

        public static bool DeleteIntentRule(Int64 ruleId)
        {
            string sql = string.Format("DELETE FROM [RulebasedClassifier] WHERE Id = {0}", ruleId);
            var result = loader.ExecuteCommand(sql);

            if (result > 0 && result != int.MaxValue)
            {
                return true;
            }

            return false;
        }

        public static RulebasedClassifier UpdateIntentRule(Int64 classifierId, Int64 ruleId, string regexRule, string ruleType)
        {
            var intentRules = GetRulesbyRuleId(ruleId,classifierId);
            if (intentRules.Any())
            {
                var intent = intentRules.First();
                string sql = string.Format("Update [RulebasedClassifier] SET RegexRule = '{0}', RuleType = '{1}' WHERE Id = {2} AND ClassifierId = {3}", regexRule, ruleType, ruleId, classifierId);
                var result = loader.ExecuteCommand(sql);

                if(result > 0 && result != int.MaxValue)
                {
                    return new RulebasedClassifier
                    {
                        LabelId = intent.LabelId,
                        IntentName = intent.IntentName,
                        SubIntentName = intent.SubIntentName,
                        RuleId = ruleId,
                        RegexRule = regexRule,
                        RuleType = ruleType
                    };
                }

            }

            return null;
        }

        #endregion
    }
}
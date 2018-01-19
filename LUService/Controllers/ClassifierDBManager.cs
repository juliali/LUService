using Bot.ML.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUService.Controllers
{
    public class Classifier
    {
        public Int64 ClassifierId;
        public string Name;
        public int Type;
        public bool IsBuiltIn;
        public string ProjectName;
        public string DatasetName;
    }

    public class ClassifierDBManager
    {
        private static DatabaseLoader loader = null;

        static ClassifierDBManager()
        {
            if (loader == null)
            {
                string sqlConn = System.Configuration.ConfigurationManager.AppSettings["LUToolConnectionString"];
                loader = new DatabaseLoader(sqlConn);
            }
        }

        #region Intent Rule Management
        private static IEnumerable<Classifier> LoadClassifier(string sql)
        {
            var results = loader.Load(sql);
            foreach (var r in results)
            {
                Classifier classifier = new Classifier
                {
                    ClassifierId = long.Parse(r["Id"]),
                    Name = r["Name"],
                    Type = int.Parse(r["Type"]),
                    IsBuiltIn = bool.Parse(r["IsBuiltIn"]),
                    ProjectName = r["ProjectName"],
                    DatasetName = r["DatasetName"]
                };

                yield return classifier;
            }
        }

        public static IEnumerable<Classifier> GetClassifiers()
        {
            string sqlCommand = "SELECT * FROM Classifier";
            return LoadClassifier(sqlCommand);
        }

        public static IEnumerable<Classifier> GetClassifier(int classifierId)
        {
            string sqlCommand = "SELECT * FROM Classifier WHERE Id = " + classifierId + "";
            return LoadClassifier(sqlCommand);
        }

        public static IEnumerable<Classifier> GetClassifier(string classifierName)
        {
            string sqlCommand = "SELECT * FROM Classifier WHERE Name = '" + classifierName + "'";
            return LoadClassifier(sqlCommand);
        }

        public static Classifier InsertClassifier(string name, int type, string projectName, string datasetName,bool isBuiltIn = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var classifiers = GetClassifier(name);
            if (classifiers.Any())
            {
                return classifiers.First();
            }
            else
            {
                string sql = string.Format("INSERT INTO [Classifier](Name,Type,IsBuiltIn,ProjectName,DatasetName) values('{0}',{1}, {2}, '{3}','{4}') select @@IDENTITY as 'Identity'", name, type, isBuiltIn?1:0, projectName,datasetName);
                Int64 classifierId = loader.Insert(sql);

                if(classifierId == -1)
                {
                    return null;
                }

                return new Classifier
                {
                    ClassifierId = classifierId,
                    Name = name,
                    Type = type,
                    IsBuiltIn = isBuiltIn,
                    ProjectName = projectName,
                    DatasetName = datasetName
                };
            }
        }

        public static bool DeleteIntentRule(Int64 intentId, string regexRule, string ruleType)
        {
            string sql = string.Format("DELETE FROM [LU.IntentTriggerRule] WHERE IntentId = {0} AND RegexRule = '{1}' AND RuleType = '{2}'", intentId, regexRule, ruleType);
            var result = loader.ExecuteCommand(sql);

            if (result > 0 && result != int.MaxValue)
            {
                return true;
            }

            return false;
        }

        public static bool DeleteClassifier(Int64 classifierId)
        {
            string sql = string.Format("DELETE FROM [Classifier] WHERE Id = {0}", classifierId);
            var result = loader.ExecuteCommand(sql);

            if (result > 0 && result != int.MaxValue)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
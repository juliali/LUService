using Bot.LanguageUnderstanding.MixedLU;
using Bot.ML.Common;
using Bot.ML.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace LUService.Controllers
{
    public class RulebasedClassifierController : ApiController
    {
        [HttpGet]
        public IEnumerable<RulebasedClassifier> Get(int classifierId, int labelId)
        {
            return IntentDBManager.GetIntentRules(classifierId, labelId);
        }

        [HttpGet]
        public IEnumerable<RulebasedClassifier> Get(int classifierId)
        {
            return IntentDBManager.GetIntentRules(classifierId);
        }     

       //Create a new rule
       [HttpPost]
        public RulebasedClassifier Post([FromBody]RulebasedClassifier rule)
        {
            var newRule = IntentDBManager.InsertRule(rule.ClassifierId, rule.LabelId, rule.RegexRule, rule.RuleType);

            return newRule;
        }

        [HttpDelete]
        public bool Delete([FromUri]Int64 Id)
        {
            return IntentDBManager.DeleteIntentRule(Id);
        }

        [HttpPut]
        public RulebasedClassifier Put([FromBody]RulebasedClassifier rule)
        {
            return IntentDBManager.UpdateIntentRule(rule.ClassifierId,rule.RuleId,rule.RegexRule,rule.RuleType);
        }
    }
}

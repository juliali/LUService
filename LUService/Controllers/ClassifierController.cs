using System;
using System.Collections.Generic;
using System.Web.Http;

namespace LUService.Controllers
{
    public class ClassifierController : ApiController
    {
        [HttpGet]
        public IEnumerable<Classifier> Get(int classifierId)
        {
            return ClassifierDBManager.GetClassifier(classifierId);
        }

        [HttpGet]
        public IEnumerable<Classifier> Get()
        {
            return ClassifierDBManager.GetClassifiers();
        }

        //Create a new Classifier
        [HttpPost]
        public Classifier Post([FromBody]Classifier classifier)
        {
            var newRule = ClassifierDBManager.InsertClassifier(classifier.Name, classifier.Type, classifier.ProjectName, classifier.DatasetName, classifier.IsBuiltIn);

            return newRule;
        }

        [HttpDelete]
        public bool Delete([FromUri]Int64 Id)
        {
            return IntentDBManager.DeleteIntentRule(Id);
        }
    }
}

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
    public class IntentsController : ApiController
    {
        /*
        [HttpGet]
        public IEnumerable<Intent> Get(string intent, string subintent)
        {
            return IntentDBManager.GetIntents(intent, subintent);
        }

        [HttpGet]
        public IEnumerable<Intent> Get()
        {
            return IntentDBManager.GetIntents();
        }

        //Create a new intent
        [HttpPost]
        public Intent Post([FromBody]Intent intent)
        {
            return IntentDBManager.InsertIntent(intent.IntentName, intent.SubIntentName);
        }

        [HttpDelete]
        public bool Delete([FromUri]Int64 Id)
        {
            return IntentDBManager.DeleteIntent(Id);
        }

        [HttpPut]
        public Intent Put([FromBody]Intent intent)
        {
            return IntentDBManager.UpdateIntent(intent.IntentId, intent.IntentName, intent.SubIntentName);
        }

        */
    }
}

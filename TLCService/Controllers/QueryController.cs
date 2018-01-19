//using ChitchatClassifer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace TLCService.Controllers
{
    public class QueryController : ApiController
    {

        //static TelemetryClient tc = new TelemetryClient();
        //private Predictor predictor;

        // GET /query?doc=XXX
        //[Route("api/query/")]
        public string Get([FromUri]string doc, [FromUri]int needResponse = 0, [FromUri]int useRudeRule = 0)
        {
            //tc.TrackEvent("chitchat_query");
            string answer = Predictor.Instance.GetAnswer(doc);
            return answer;
        }
    }
}

using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TLCService.BinaryClassification;

namespace TLCService.Controllers
{
    public class HAController : ApiController
    {
        private const string DA_Model_Config_FilePath = "Bot.ML.Common.Res.ModelConfig.ha_da_bcmodelconfig.json";

        // GET api/ha/dapredict
        [HttpGet]
        public ICTextPredictionInfo DAPredict([FromUri]string Utterance, [FromUri] string EdiResult = null)
        {
            string utterance = Utterance;
            string ediResult = EdiResult;

            if (string.IsNullOrWhiteSpace(ediResult))
            {
                ediResult = null;
            }
            BCPredictor prediction = BCPredictor.GetInstance(DA_Model_Config_FilePath);
            ICTextPredictionInfo result = prediction.PredictUtterance(utterance, ediResult);

            return result;
        }

        private const string IC_Model_Config_FilePath = "Bot.ML.Common.Res.ModelConfig.ha_rs_bcmodelconfig.json";

        // GET api/ha/rspredict
        [HttpGet]
        public ICTextPredictionInfo RSPredict([FromUri]string Utterance, [FromUri] string EdiResult = null)
        {
            string utterance = Utterance;
            string ediResult = EdiResult;

            if (string.IsNullOrWhiteSpace(ediResult))
            {
                ediResult = null;
            }

            BCPredictor prediction = BCPredictor.GetInstance(IC_Model_Config_FilePath);
            ICTextPredictionInfo result = prediction.PredictUtterance(utterance, ediResult);

            return result;
        }
    }
}
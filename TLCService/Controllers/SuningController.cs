using Bot.ML.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TLCService.BinaryClassification;

namespace TLCService.Controllers
{
    public class SuningController : ApiController
    {
        private const string DA_Model_Config_FilePath = "Bot.ML.Common.Res.ModelConfig.da_bcmodelconfig.json";

        // GET api/suning/dapredict
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

        private const string IC_Model_Config_FilePath = "Bot.ML.Common.Res.ModelConfig.ic_bcmodelconfig.json";

        // GET api/suning/icpredict
        [HttpGet]
        public ICTextPredictionInfo ICPredict([FromUri]string Utterance, [FromUri] string EdiResult = null)
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
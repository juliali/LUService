using Bot.ML.Common.Data;

namespace Bot.ML.Common.Controller
{
    public interface ICPredictorInterface
    {
             

        ICTextPredictionInfo PredictUtterance(string Utterance, string EdiResult);
    }
}

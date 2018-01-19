using Microsoft.MachineLearning.Data;
using System;

namespace Bot.TLC.Predicting.Data
{
    public class BCScoredData
    {
        public DvBool PredictedLabel;
        public Single Score;
        public Single Probability;
    }
}

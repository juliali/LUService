using Bot.ML.Common.Data;
using System.Collections.Generic;

namespace Bot.ML.Common.Controller
{
    public interface ICTrainerInterface
    {
        void GenerateVectorSpace(List<SourceData> cases);

        void GenerateTrainingFeaturesAndTrain(List<SourceData> cases, string filename, string featurepath);   
    }
}

using Bot.ML.Common.Data;

namespace Bot.VSM.Modules.TextToVector.Policy
{
    public class AllValidSelectionPolicy : SelectionPolicy
    {
        public AllValidSelectionPolicy(string ConfigFilePath, VectorSpace VS)
            : base(ConfigFilePath, VS)
        {
        }

        protected override bool IsValidTerm(TermSpaceInfo Info)
        {
            return true;
        }
    }
}

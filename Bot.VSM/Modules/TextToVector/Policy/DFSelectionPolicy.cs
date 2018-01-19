using System;
using Bot.ML.Common.Data;

namespace Bot.VSM.Modules.TextToVector.Policy
{
    public class DFSelectionPolicy : SelectionPolicy
    {
        
        private const int DEFAULT_BOTTOMDF = 2;        
        private int BottomDF = DEFAULT_BOTTOMDF;
               
        public DFSelectionPolicy(string ConfigFilePath, VectorSpace VS)
            : base(ConfigFilePath, VS)
        {
            string DFstr = this.configStore.GetOption("BottomDF");
            if (!string.IsNullOrWhiteSpace(DFstr))
            {
                try
                {
                    int df = int.Parse(DFstr);
                    this.BottomDF = df;
                }
                catch(Exception)
                {

                }
            }                       
        }        

        override       
        protected bool IsValidTerm(TermSpaceInfo Info)
        {
            if (Info.IsResidentTerm)
            {
                return true;
            }

            if (Info.DFValue >= this.BottomDF)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}

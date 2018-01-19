using System;
using System.Linq;
using Bot.ML.Common.Data;

namespace Bot.VSM.Modules.TextToVector.Policy
{
    public class DFEntropySelectionPolicy : SelectionPolicy
    {
        
        private const int DEFAULT_BOTTOMDF = 2;
        private const double DEFAULT_ENTROPYPERCENTAGE = 0.3;

        private int BottomDF = DEFAULT_BOTTOMDF;

        private double BottomLineEntropy = -1;
        

        public DFEntropySelectionPolicy(string ConfigFilePath, VectorSpace VS)
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
            double EntropyPercentage = DEFAULT_ENTROPYPERCENTAGE;
            string EPStr = this.configStore.GetOption("EntropyPercentage");
            if (!string.IsNullOrWhiteSpace(EPStr))
            {
                try
                {
                    double ep = double.Parse(EPStr);
                    EntropyPercentage = ep;
                }
                catch (Exception)
                {

                }
            }

            GetBottomLineEntropy(EntropyPercentage);

        }

        private void GetBottomLineEntropy(double EntorpyPercentage)
        {

            int TotalTermNumber = this.VS.TotalTermNumber;
            int TopEntropyNumber = (int)(TotalTermNumber * EntorpyPercentage);

            this.BottomLineEntropy = VS.EntropyValueList.ElementAt(TopEntropyNumber - 1);

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
                if (BottomLineEntropy < 0.0)
                {
                    return true;
                }
                else
                {
                    if (Info.Entropy <= BottomLineEntropy)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }


}

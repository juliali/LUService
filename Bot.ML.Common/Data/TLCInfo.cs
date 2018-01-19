using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.ML.Common.Data
{

    public class ICExample
    {
        public float Label = -1;

        // Vector size. By default, arrays are considered variable-length, and a linear predictor can't
        // consume them.
        // Instead of attribute, you can specify the vector size at runtime, using the SchemaDefinition objects.

        public float[] Features;

        override
        public string ToString()
        {
            string outputstr = "";

            if (Label > -1)
            {
                outputstr += Label.ToString();

                foreach (float Feature in Features)
                {
                    outputstr += '\t' + Feature.ToString();
                }
            }
            else
            {
                for (int i = 0; i < Features.Length; i ++)
                {
                    outputstr += Features[i].ToString();
                    if (i < Features.Length - 1)
                    {
                        outputstr += '\t';
                    }

                }
            }

            return outputstr;
        }
    }

    public class ICScoredData
    {
        public float[] Score;
    }
}

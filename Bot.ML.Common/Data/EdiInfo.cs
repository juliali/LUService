namespace Bot.ML.Common.Data
{
    public class EdiInfo
    {
        public double Confidence = 1;
        public SlotInfo[] Segments;
        public string Text;
    }

    public class SlotInfo
    {
        public string Confidence;
        public int Start = 0;
        public int End = 0;
        public int Length = 0;

        public string Tag;
        public string Value;
    }
}

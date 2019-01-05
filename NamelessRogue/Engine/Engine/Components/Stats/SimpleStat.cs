 

namespace NamelessRogue.Engine.Engine.Components.Stats
{
    public abstract class SimpleStat {
        private int value;
        private int minValue;
        private int maxValue;

        public SimpleStat(int value, int minValue, int maxValue)
        {

            this.value = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        public int MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
    }
}

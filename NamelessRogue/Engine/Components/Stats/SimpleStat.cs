 

namespace NamelessRogue.Engine.Components.Stats
{
    public class SimpleStat {
        private int value;
        private int minValue;
        private int maxValue;

		public SimpleStat()
		{
		}

		public SimpleStat(int value, int minValue, int maxValue)
        {

            this.value = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public int Value
        {
            get { return value; }
            set {
                this.value = value;
                this.value = this.value > maxValue ? maxValue : this.value;
                this.value = this.value < minValue ? minValue : this.value;
            }
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

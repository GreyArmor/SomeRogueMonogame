 

namespace NamelessRogue.Engine.Engine.Components.Stats
{
    public abstract class SimpleStat : Component {
        private int value;
        private int minValue;
        private int maxValue;

        public SimpleStat(int value, int minValue, int maxValue)
        {

            this.value = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public int GetValue() {
            return value;
        }

        public void SetValue(int value) {
            this.value = value;
        }

        public int GetMinValue() {
            return minValue;
        }

        public void SetMaxValue(int maxValue) {
            this.maxValue = maxValue;
        }

        public int GetMaxValue() {
            return maxValue;
        }
    }
}

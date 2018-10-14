 

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

        public int getValue() {
            return value;
        }

        public void setValue(int value) {
            this.value = value;
        }

        public int getMinValue() {
            return minValue;
        }

        public void setMaxValue(int maxValue) {
            this.maxValue = maxValue;
        }

        public int getMaxValue() {
            return maxValue;
        }
    }
}

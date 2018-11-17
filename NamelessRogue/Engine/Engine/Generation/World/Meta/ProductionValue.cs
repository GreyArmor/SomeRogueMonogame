namespace NamelessRogue.Engine.Engine.Generation.World.Meta
{
    public class ProductionValue
    {
        public ProductionValue(int food, int manufacturing, int culture, int science, int mana, int health)
        {
            Food = food;
            Manufacturing = manufacturing;
            Culture = culture;
            Science = science;
            Mana = mana;
            Health = health;
        }

        public int Food { get; set; }
        public int Manufacturing { get; set; }
        public int Culture { get; set; }
        public int Science { get; set; }
        public int Mana { get; set; }
        public int Health { get; set; }
    }
}

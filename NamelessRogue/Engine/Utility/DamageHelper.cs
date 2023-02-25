using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Status;

namespace NamelessRogue.Engine.Utility
{
    public class DamageHelper {
        public static void ApplyDamage(IEntity target, IEntity source, int damage)
        {
            Damage d = target.GetComponentOfType<Damage>();
            if(d==null)
            {
                d = new Damage(source,damage);
                target.AddComponent(d);
            }
            else
            {
                d.DamageValue = d.DamageValue+damage;
            }
        }
    }
}

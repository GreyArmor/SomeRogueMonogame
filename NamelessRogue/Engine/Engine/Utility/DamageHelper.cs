using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Status;

namespace NamelessRogue.Engine.Engine.Utility
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
                d.setDamage(d.getDamage()+damage);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Generation.Items
{
    public class ItemsGenerator
    {

        private Random random;
        public ItemsGenerator(Random random)
        {
            this.random = new Random(random.Next());
        }

        public class WeaponGenerationParameters
        {
            public string MadeInCivilization { get; private set; }
            public string Author { get; private set; }
            public bool IsUniqueName { get; private set; }

            public string WeaponName { get; private set; }

            public WeaponStats Stats { get; private set; }

            public Drawable Representation { get; private set; }

            public Item ItemData { get; private set; }

            public WeaponGenerationParameters(string weaponName, WeaponStats stats, Drawable representation, Item itemData,  bool isUniqueName,
                string madeInCivilization = "", string author = "")
            {
                MadeInCivilization = madeInCivilization;
                Author = author;
                IsUniqueName = isUniqueName;
                WeaponName = weaponName;
                Stats = stats;
                Representation = representation;
                ItemData = itemData;
            }

            

        }

        public IEntity GenerateWeapon(WeaponGenerationParameters parameters)
        {
            Entity weapon = new Entity();
            string weaponsName = "";
            string weaponsDescription = "";
            if (parameters.IsUniqueName)
            {
                //todo
            }
            else
            {
                weaponsName += parameters.WeaponName;
                if (parameters.MadeInCivilization != "")
                {
                    weaponsName += $" of {parameters.MadeInCivilization}";
                }
            }

            weaponsName += $" {parameters.Stats.MinimumDamage}-{parameters.Stats.MaximumDamage}";
            if (parameters.Author !="")
            {
                weaponsDescription += $"Made by {parameters.Author}.";
            }

            weaponsDescription += $"Minimum damage {parameters.Stats.MinimumDamage}\n" +
                                  $" Maximum damage {parameters.Stats.MaximumDamage}\n " +
                                  $"Range {parameters.Stats.Range}\n"+
                                  $"Attack type {parameters.Stats.AttackType}\n";

            if (parameters.Stats.AmmoInClip > 0)
            {
                weaponsDescription += $"Ammo type {parameters.Stats.AmmoType}\n" +
                                      $"Max ammo in clip {parameters.Stats.AmmoInClip}\n";
            }
                                 


            weapon.AddComponent(new Description(weaponsName, weaponsDescription));
            weapon.AddComponent((Item)parameters.ItemData.Clone());
            weapon.AddComponent(new Drawable(parameters.Representation.getRepresentation(), parameters.Representation.getCharColor()));
            weapon.AddComponent(new Equipment(new List<Slot>(){}));
            weapon.AddComponent((WeaponStats)parameters.Stats.Clone());
            return weapon;


        }




    }
}

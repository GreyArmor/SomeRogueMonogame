using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using NamelessRogue.Engine.Components.Environment;

namespace NamelessRogue.Engine.Factories
{
    public static class ItemLibrary {
        public static List<ItemTemplateData> ItemData = new List<ItemTemplateData>();
        public static Dictionary<string, ItemTemplateData> ItemDataById = new Dictionary<string, ItemTemplateData>();
        public static void LoadItemData(NamelessGame game)
        {
            var items = Directory.GetFiles(Environment.CurrentDirectory + Constants.GameObjectRelativePath, "*.nrif", SearchOption.AllDirectories);
            var itemsHolder = game.PlayerEntity.GetComponentOfType<ItemsHolder>();
            foreach (var itemPath in items)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ItemTemplateData));
                TextReader reader = new StreamReader(itemPath);

                var itemData = (ItemTemplateData)serializer?.Deserialize(reader);

                if (itemData != null && itemData.IconPath != null && itemData.IconPath != string.Empty)
                {
                    var iconPath = Path.GetDirectoryName(itemPath) + "\\" + itemData.IconPath;
                    FileStream fileStream = new FileStream(iconPath, FileMode.Open);
                    Texture2D texture = Texture2D.FromStream(game.GraphicsDevice, fileStream);
                    var iconFileName = Path.GetFileName(iconPath);
                    ImGuiImageLibrary.Textures.Remove(iconFileName);
                    ImGuiImageLibrary.Textures.Add(iconFileName, UIRenderSystem.ImGuiRendererInstance.BindTexture(texture));

                    SpriteLibrary.RemoveStaticSprite(iconFileName);
                    SpriteLibrary.AddStaticSprite(iconFileName, texture);


                    fileStream.Close();
                    fileStream.Dispose();
                }

                ItemData.Add(itemData);
                ItemDataById.Add(itemData.Id, itemData);
            }
        }

        public static void ClearData()
        {
            ItemData.Clear();
            ItemDataById.Clear();
        }

        public static Entity CreateItemFromData(NamelessGame game, ItemTemplateData data)
        {
            Entity entity = new Entity();

            var iconFileName = Path.GetFileName(data.IconPath);
            entity.AddComponent(new UiIconComponent(iconFileName));
            entity.AddComponent(new Drawable(iconFileName, new Color(1f)));
            entity.AddComponent(new SpritedObject(true, iconFileName));

            entity.AddComponent(new Description(data.Name, data.Description));

            entity.AddComponent(new Item(data.ItemType, 0, data.ItemQuality, 1, 1, "CorpoCorp Inc."));

            if (data.WeaponTemplateData != null)
            {
                var wtd = data.WeaponTemplateData;
                entity.AddComponent(new Equipment(Slot.LefHand, Slot.RightHand));
                entity.AddComponent(new WeaponStats(wtd.MinimumDamage, wtd.MaximumDamage, wtd.Range, wtd.AttackType, wtd.AmmoType, wtd.AmmoInClip, 0));
            }

            if (data.ArmorTemplateData != null)
            {
                var atd = data.ArmorTemplateData;
                entity.AddComponent(new Equipment(data.PossibleSlots.ToArray()));
                entity.AddComponent(new ArmorStats() { DamageType = atd.DamageType, Value = new SimpleStat(atd.ArmorValue, atd.ArmorValue, atd.ArmorValue) });
                entity.AddComponent(new ResistanceStat() { DamageType = atd.ResistType, Value = new SimpleStat(atd.ResistValue, atd.ArmorValue, atd.ResistValue) });
            }        

            if (data.AssociatedBuffs.Any())
            {
                if (data.ItemType == ItemType.Consumable)
                {
                    var consumableComponent = new Consumable(data.AssociatedBuffs.Select(x => x.BuffId));
                    entity.AddComponent(consumableComponent);
                }
                else
                {
                    var onHitBuffs = new OnHitBuffs(data.AssociatedBuffs.Select(x => x.BuffId));
                    entity.AddComponent(onHitBuffs);
                }
            }

            //  entity.AddComponent(new EquipmentSlot(Slot.RightHand));
            return entity;

        }

    }
}


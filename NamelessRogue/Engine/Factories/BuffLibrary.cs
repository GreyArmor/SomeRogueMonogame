using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Factories
{
    public static class BuffLibrary
    {
        public static List<BuffTemplateData> Data = new List<BuffTemplateData>();
        public static Dictionary<string, BuffTemplateData> DataById = new Dictionary<string, BuffTemplateData>();
        public static void LoadData(NamelessGame game)
        {
            var buffs = Directory.GetFiles(Environment.CurrentDirectory + Constants.GameObjectRelativePath, "*.nrbf", SearchOption.AllDirectories);
            foreach (var itemPath in buffs)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BuffTemplateData));
                TextReader reader = new StreamReader(itemPath);

                var buffData = (BuffTemplateData)serializer?.Deserialize(reader);

                if (buffData != null && buffData.IconPath != null && buffData.IconPath != string.Empty)
                {
                    var iconPath = Path.GetDirectoryName(itemPath) + "\\" + buffData.IconPath;
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

                Data.Add(buffData);
                DataById.Add(buffData.Id, buffData);
            }
        }

        public static void ClearData()
        {
            Data.Clear();
            DataById.Clear();
        }

        public static Entity CreateBuffFromData(NamelessGame game, BuffTemplateData buff)
        {
            Entity entity = new Entity();

            var iconFileName = Path.GetFileName(buff.IconPath);
            entity.AddComponent(new UiIconComponent(iconFileName));
            entity.AddComponent(new Description(buff.Description));
            entity.AddComponent(new ModifierComponent());
            if (!buff.IsAppliedImmediately)
            {
                entity.AddComponent(new TimedModifier() { TurnsToLast = buff.Duration });
            }
            var buffComponent = new Buff();
            buffComponent.Health = buff.HealthModificator;
            buffComponent.Energy = buff.EnergyModificator;
            buffComponent.Damage = buff.DamageModificator;
            buffComponent.Armor = buff.ArmorModificator;
            buffComponent.Resistance = buff.ResistanceModificator;
            buffComponent.PermanentModifier = buff.PermanentModifier;
            buffComponent.IsAppliedImmediately = buff.IsAppliedImmediately;
            buffComponent.Duration = buff.Duration;
            buffComponent.IsDamageOverTime = buff.IsDamageOverTime;

            if (buff.ArmorModificator != 0)
            {
                entity.AddComponent(new ArmorStats() { DamageType = DamageType.Physical, Value = new SimpleStat(buff.ArmorModificator, -999, 999) });
            }
            if (buff.ResistanceModificator != 0)
            {
                entity.AddComponent(new ResistanceStat() { DamageType = DamageType.Physical, Value = new SimpleStat(buff.ResistanceModificator, -999, 999) });
            }
            if (buff.DamageModificator != 0)
            {
                entity.AddComponent(new WeaponStats() { DamageType = DamageType.Physical, MinimumDamage = buff.DamageModificator, MaximumDamage = buff.DamageModificator });
            }
            if (buff.HealthModificator != 0 || buff.EnergyModificator != 0)
            {
                entity.AddComponent(new CharacterStats() { Health = new SimpleStat(buff.HealthModificator, 0, 999), Energy = new SimpleStat(buff.EnergyModificator, 0, 999) });
            }

            entity.AddComponent(buffComponent);
            return entity;

        }

    }
}


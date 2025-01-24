using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NamelessRogue.Engine.Factories
{
    public class AbilityFactory
    {
        public static List<AbilityTemplateData> Data = new List<AbilityTemplateData>();
        public static Dictionary<string, AbilityTemplateData> DataById = new Dictionary<string, AbilityTemplateData>();
        public static void LoadData(NamelessGame game)
        {
            var abilities = Directory.GetFiles(Environment.CurrentDirectory + Constants.GameObjectRelativePath, "*.nraf", SearchOption.AllDirectories);
            foreach (var path in abilities)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AbilityTemplateData));
                TextReader reader = new StreamReader(path);

                var data = (AbilityTemplateData)serializer?.Deserialize(reader);

                if (data != null && data.IconPath != null && data.IconPath != string.Empty)
                {
                    var iconPath = Path.GetDirectoryName(path) + "\\" + data.IconPath;
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

                Data.Add(data);
                DataById.Add(data.Id, data);
            }
        }

        public static void ClearData()
        {
            Data.Clear();
            DataById.Clear();
        }

        public static Entity CreateFromData(NamelessGame game, AbilityTemplateData data)
        {
            Entity entity = new Entity();

            var iconFileName = Path.GetFileName(data.IconPath);
            entity.AddComponent(new UiIconComponent(iconFileName));
            entity.AddComponent(new Description(data.Name, data.Description));
            var abilityParams = new AbilityParameters();

            abilityParams.TargetMode = data.TargetMode;
            abilityParams.ActivationMode = data.ActivationMode;
            abilityParams.AreaOfEffect = data.AreaOfEffect;
            abilityParams.IsActive = data.IsActive;
            abilityParams.Range = data.Range;
            abilityParams.CooldownTurns = data.CooldownTurns;
            abilityParams.EnergyCost = data.EnergyCost;
            abilityParams.ActionPointsCost = data.ActionPointsCost;
            abilityParams.AbilityActions = new List<AbilityAction>(data.AbilityActions);

            entity.AddComponent(abilityParams);

            if (data.AssociatedBuffs.Any())
            {
                var onHitBuffs = new AssociatedBuffs(data.AssociatedBuffs.Select(x => x.BuffId));
                entity.AddComponent(onHitBuffs);
            }
            return entity;
        }


    }
}

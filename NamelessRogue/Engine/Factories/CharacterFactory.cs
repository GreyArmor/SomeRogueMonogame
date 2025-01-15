using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Components.WorldBoardComponents;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Entity = NamelessRogue.Engine.Infrastructure.Entity;

namespace NamelessRogue.Engine.Factories
{
    public class CharacterFactory {
        public static List<CharacterTemplateData> CharacterData = new List<CharacterTemplateData>();
        public static Dictionary<string, CharacterTemplateData> CharacterDataById = new Dictionary<string, CharacterTemplateData>();


        public static void LoadCharacters()
        {
            var characters = Directory.GetFiles(Environment.CurrentDirectory + Constants.GameObjectRelativePath + "\\Characters\\", "*.nrcf", SearchOption.AllDirectories);
            foreach (var charactersFile in characters)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CharacterTemplateData));
                TextReader reader = new StreamReader(charactersFile);
                var data = (CharacterTemplateData)serializer.Deserialize(reader);
                CharacterData.Add(data);
                CharacterDataById.Add(data.Id, data);
            }
        }

        public static void ClearData()
        {
            CharacterData.Clear();
            CharacterDataById.Clear();
        }

        public static Entity CreateSimplePlayerCharacter(int x,int y, int z, NamelessGame game)
        {          
            var position = new Position(x, y, z);
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Drawable("Xelanoi", new Engine.Utility.Color(1),null, "", true));
            playerCharacter.AddComponent(new SpritedObject(false, "Xelanoi", SpriteLibrary.SpritesAnimated["Xelanoi"]._animations.First().Key));
            playerCharacter.AddComponent(new Character("Xelanoi"));
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());
            playerCharacter.AddComponent(new InputComponent());
            playerCharacter.AddComponent(position);
            playerCharacter.AddComponent(new Description("Player",""));
            var holder = new ItemsHolder();
            playerCharacter.AddComponent(holder);          
            playerCharacter.AddComponent(new OccupiesTile());
			var stats = new CharacterStats();
            stats.Health.Value = 100;
            stats.Health.MaxValue = 100;
            stats.Energy.Value = 100;
            stats.Energy.MaxValue = 100;

            playerCharacter.AddComponent(stats);

            Entity playerAccumulatorEntity = new Entity();
            playerAccumulatorEntity.AddComponent(new ArmorStats());
            playerAccumulatorEntity.AddComponent(new WeaponStats());
            playerAccumulatorEntity.AddComponent(new ResistanceStat());
            playerAccumulatorEntity.AddComponent(new CharacterStats());

            var modifiersCollection = new ModifiersCollection(playerAccumulatorEntity);

            playerCharacter.AddComponent(modifiersCollection);
            playerCharacter.AddComponent(new EquipmentSlots(holder, modifiersCollection, game));

            playerCharacter.AddComponent(new AbilityHolder());
            playerCharacter.AddComponent(new AbilityBinder());

            playerCharacter.AddComponent(new ActionPoints() { Points = 100 });
            game.WorldProvider.MoveEntity(playerCharacter, position.Point);

            return playerCharacter;
        }

        public static Entity CreateWorldBoardPlayer(int x, int y, int z)
        {
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());   
            playerCharacter.AddComponent(new Position(x, y, z));
            playerCharacter.AddComponent(new WorldBoardPlayer());

			return playerCharacter;
        }


        public static Entity CreateCharacterFromData(NamelessGame game, Vector3Int position, CharacterTemplateData data)
        {
            var spritePath = "Content\\GameObjects\\Characters\\" + data.SpritePath;
            var spriteFileName = Path.GetFileName(spritePath);
            SpriteLibrary.RemoveAnimatedSprite(spriteFileName);
            SpriteLibrary.AddAnimatedSprite(spriteFileName, spritePath);

            var sprite = SpriteLibrary.SpritesAnimated[spriteFileName];;

            var pos = new Position(position.X,position.Y, position.Z);
            Entity character = new Entity();
            character.AddComponent(new Character(data.FactionId));
            character.AddComponent(new AIControlled() { Affinity = Affinity.Hostile });
           
            character.AddComponent(pos);
            character.AddComponent(new Drawable(Path.GetFileName(data.SpritePath), new Engine.Utility.Color(1), castsShadow: data.CastsShadow));
            character.AddComponent(new SpritedObject(false, spriteFileName, sprite._animations.Keys.First()));
            character.AddComponent(new Description(data.Name, data.Description));
            var holder = new ItemsHolder();
            character.AddComponent(holder);
            character.AddComponent(new OccupiesTile());
            character.AddComponent(new FlowMoveComponent()); 
            

            var stats = new CharacterStats();
            stats.Health.Value = data.Health;
            stats.Health.MaxValue = data.Energy;
            stats.MovementSpeed.Value = data.MovementSpeed;
            stats.Immobile = data.Immobile;

            if(stats.Immobile)
            {
                character.AddComponent(new HostileTurretAI());
            }
            else
            {
                character.AddComponent(new FollowPlayerAi());
            }

            var atd = data.ArmorTemplateData;
            var wtd = data.WeaponTemplateData;           
            stats.Armor.Add(new ArmorStats() { DamageType = atd.DamageType, Value = new SimpleStat(atd.ArmorValue, 0, 999) });
            stats.WeaponStats.Add(new WeaponStats(wtd.MinimumDamage, wtd.MaximumDamage, wtd.Range, wtd.AttackType));

            character.AddComponent(stats);


            character.AddComponent(new DroppedItemsComponent(data.DroppedItems.Select(x => new DroppedItem() { ItemId = x.ItemId, Probability = x.Probability})));

            Entity accumulatorEntiry = new Entity();
            accumulatorEntiry.AddComponent(new CharacterStats());

            character.AddComponent(new ModifiersCollection(accumulatorEntiry));

            character.AddComponent(new ActionPoints() { Points = 100 });
            game.WorldProvider.MoveEntity(character, position);

            return character;
        }

    }
}

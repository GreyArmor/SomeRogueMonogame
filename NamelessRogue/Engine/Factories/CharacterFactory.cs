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
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = NamelessRogue.Engine.Infrastructure.Entity;

namespace NamelessRogue.Engine.Factories
{
    public class CharacterFactory {
        
        public static Entity CreateSimplePlayerCharacter(int x,int y, int z, NamelessGame game)
        {
            var position = new Position(x, y, z);
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Character("Player"));
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());
            playerCharacter.AddComponent(new InputComponent());
            playerCharacter.AddComponent(position);
            playerCharacter.AddComponent(new Drawable("Window", new Engine.Utility.Color(0.9,0.9,0.9)));
            playerCharacter.AddComponent(new Description("Player",""));
            var holder = new ItemsHolder();
            playerCharacter.AddComponent(holder);          
            playerCharacter.AddComponent(new OccupiesTile());
            playerCharacter.AddComponent(new FlowMoveComponent());
			playerCharacter.AddComponent(new SpriteModel3D(game, "AnimatedCharacters\\EasyChar_2023-10-31T21_44_08.635Z.sf"));
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

            playerCharacter.AddComponent(new ActionPoints() { Points = 100 });
            playerCharacter.AddComponent(new Camera3D(game));
            game.WorldProvider.MoveEntity(playerCharacter, position.Point);

            return playerCharacter;
        }

        public static Entity CreateDummyrCharacter(int x, int y, int z, NamelessGame game)
        {
            var position = new Position(x, y, z);
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Character("Enemy"));
            playerCharacter.AddComponent(new AIControlled() { Affinity = Affinity.Hostile });
            playerCharacter.AddComponent(new BasicAi());
            playerCharacter.AddComponent(position);
            playerCharacter.AddComponent(new Drawable("drone_recon", new Engine.Utility.Color(0.9, 0.9, 0.9)));
            playerCharacter.AddComponent(new SpritedObject(false, "idle_2"));
            playerCharacter.AddComponent(new Description("Enemy", ""));
            var holder = new ItemsHolder();
            playerCharacter.AddComponent(holder);
            playerCharacter.AddComponent(new OccupiesTile());
            playerCharacter.AddComponent(new FlowMoveComponent());
         
            var stats = new CharacterStats();
            stats.Health.Value = 100;
            stats.Health.MaxValue = 100;
            stats.Armor.Add(new ArmorStats() { DamageType = DamageType.Physical, Value = new SimpleStat(2, 0, 10) });

            playerCharacter.AddComponent(stats);

            Entity playerAccumulatorEntity = new Entity();
            playerAccumulatorEntity.AddComponent(new CharacterStats());

           
            playerCharacter.AddComponent(new ModifiersCollection(playerAccumulatorEntity));

            playerCharacter.AddComponent(new ActionPoints() { Points = 100 });
            playerCharacter.AddComponent(new Camera3D(game));
            game.WorldProvider.MoveEntity(playerCharacter, position.Point);

            return playerCharacter;
        }

        public static Entity CreateWorldBoardPlayer(int x, int y, int z)
        {
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());
            playerCharacter.AddComponent(new InputComponent());
            playerCharacter.AddComponent(new Position(x, y, z));
            playerCharacter.AddComponent(new WorldBoardPlayer());

			return playerCharacter;
        }
    }
}

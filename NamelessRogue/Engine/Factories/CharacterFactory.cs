using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.AI.Pathfinder;
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
using System.Linq;

namespace NamelessRogue.Engine.Factories
{
    public class CharacterFactory {
        
        public static Entity CreateSimplePlayerCharacter(int x,int y, int z, NamelessGame game)
        {
            var position = new Position(x, y, z);
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Character());
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());
            playerCharacter.AddComponent(new InputComponent());
            playerCharacter.AddComponent(position);
            playerCharacter.AddComponent(new Drawable("Window", new Engine.Utility.Color(0.9,0.9,0.9)));
            playerCharacter.AddComponent(new Description("Player",""));
            var holder = new ItemsHolder();
            playerCharacter.AddComponent(holder);
            playerCharacter.AddComponent(new EquipmentSlots(holder, game));
            playerCharacter.AddComponent(new OccupiesTile());
            playerCharacter.AddComponent(new FlowMoveComponent());
			playerCharacter.AddComponent(new SpriteModel3D(game, "AnimatedCharacters\\EasyChar_2023-10-31T21_44_08.635Z.sf"));
			playerCharacter.AddComponent(new SelectionData());
            playerCharacter.AddComponent(new SelectedUnitsData());
			playerCharacter.AddComponent(new GroupsHolder());
			var stats = new Stats();
            stats.Health.Value = 100;
            stats.Health.MaxValue = 100;

            playerCharacter.AddComponent(stats);

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

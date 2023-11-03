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

namespace NamelessRogue.Engine.Factories
{
    public class CharacterFactory {
        
        public static Entity CreateSimplePlayerCharacter(int x,int y, NamelessGame game)
        {
            var position = new Position(x, y);
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Character());
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());
            playerCharacter.AddComponent(new InputComponent());
            playerCharacter.AddComponent(position);
            playerCharacter.AddComponent(new Drawable('@', new Engine.Utility.Color(0.9,0.9,0.9)));
            playerCharacter.AddComponent(new Description("Player",""));
            var holder = new ItemsHolder();
            playerCharacter.AddComponent(holder);
            playerCharacter.AddComponent(new EquipmentSlots(holder, game));
            playerCharacter.AddComponent(new OccupiesTile());
            playerCharacter.AddComponent(new FlowMoveComponent());
			playerCharacter.AddComponent(new SpriteModel3D(game, "AnimatedCharacters\\EasyChar_2023-10-31T21_44_08.635Z.sf"));
			playerCharacter.AddComponent(new Position3D());
			playerCharacter.AddComponent(new SelectionData());

			var stats = new Stats();
            stats.Health.Value = 100;
            stats.Health.MaxValue = 100;

            stats.Stamina.Value = 100;
            stats.Stamina.MaxValue = 100;

          
            stats.Attack.Value = 25;
            stats.Defence.Value = 10;
            stats.AttackSpeed.Value = 100;
            stats.MoveSpeed.Value = 100;

            playerCharacter.AddComponent(stats);

            playerCharacter.AddComponent(new ActionPoints() { Points = 100 });
            playerCharacter.AddComponent(new Camera3D(game));
            game.WorldProvider.MoveEntity(playerCharacter, position.Point);

            return playerCharacter;
        }

        public static Entity CreateWorldBoardPlayer(int x, int y)
        {
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());
            playerCharacter.AddComponent(new InputComponent());
            playerCharacter.AddComponent(new Position(x, y));
            playerCharacter.AddComponent(new WorldBoardPlayer());
            return playerCharacter;
        }


        public static Entity CreateBlankKnight(NamelessGame game, Vector2 facingNormal, int x,int y, string factionId = "") {
            Entity npc = new Entity();
            var position = new Position(x, y);
			var position3D = new Position3D(new Vector3(x, y, 0), facingNormal);
            npc.AddComponent(position3D);
			npc.AddComponent(new Character());
            npc.AddComponent(new InputComponent());
            npc.AddComponent(new Movable());
            npc.AddComponent(position);
            npc.AddComponent(new Drawable('K', new Engine.Utility.Color(1f, 0, 0)));
            npc.AddComponent(new Description("Very scary dummy knight",""));
            npc.AddComponent(new OccupiesTile());
            npc.AddComponent(new AIControlled());
            npc.AddComponent(new BasicAi());

            var stats = new Stats();
            stats.Health.Value = 100;
            stats.Health.MaxValue = 100;

            stats.Stamina.Value = 100;
            stats.Stamina.MaxValue = 100;


            stats.Attack.Value = 25;
            stats.Defence.Value = 10;
            stats.AttackSpeed.Value = 100;
            stats.MoveSpeed.Value = 100;
            stats.AttackRange.Value = 1;
            stats.VisionRange.Value = 100;

            stats.FactionId = factionId;

            var sprite = new SpriteModel3D(game, "AnimatedCharacters\\EasyChar_2023-10-31T21_44_08.635Z.sf");
            npc.AddComponent(sprite);

			npc.AddComponent(new ActionPoints(){Points = 100});
         //   game.WorldProvider.MoveEntity(npc, position.Point);
            return npc;
        }

        public static void CreateNpcField(Rectangle rect, Vector2 facingNormal, string factionId, NamelessGame game)
        {
            for (int i = rect.Left; i < rect.Right; i++)
            {
                for (int j = rect.Top; j < rect.Bottom; j++)
                {
                    CreateBlankKnight(game, facingNormal, i, j, factionId);
                }
            }
        }

	
	
	
    }
}

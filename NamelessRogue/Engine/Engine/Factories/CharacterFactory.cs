using NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Components.WorldBoardComponents;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class CharacterFactory {
        
        public static Entity CreateSimplePlayerCharacter(int x,int y)
        {
            Entity playerCharacter = new Entity();
            playerCharacter.AddComponent(new Character());
            playerCharacter.AddComponent(new Player());
            playerCharacter.AddComponent(new InputReceiver());
            playerCharacter.AddComponent(new FollowedByCamera());
            playerCharacter.AddComponent(new InputComponent());
            playerCharacter.AddComponent(new Position(x,y));
            playerCharacter.AddComponent(new Drawable('@', new Engine.Utility.Color(0.9,0.9,0.9)));
            playerCharacter.AddComponent(new Description("Player",""));
            var holder = new ItemsHolder();
            playerCharacter.AddComponent(holder);
            playerCharacter.AddComponent(new EquipmentSlots(holder));
            playerCharacter.AddComponent(new OccupiesTile());
           

             var stats = new Stats();
            stats.Health.Value = 100;
            stats.Health.MaxValue = 100;

            stats.Stamina.Value = 100;
            stats.Stamina.MaxValue = 100;

          
            stats.Attack.Value = 25;
            stats.Defence.Value = 10;
            stats.AttackSpeed.Value = 100;
            stats.MoveSpeed.Value = 100;

            stats.Strength.Value = 10;
            stats.Reflexes.Value = 10;
            stats.Perception.Value = 10;
            stats.Willpower.Value = 10;
            stats.Imagination.Value = 10;
            stats.Wit.Value = 10;

            playerCharacter.AddComponent(stats);

            playerCharacter.AddComponent(new ActionPoints() { Points = 100 });

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


        public static Entity CreateBlankNpc(int x,int y) {
            Entity npc = new Entity();
            npc.AddComponent(new Character());
            npc.AddComponent(new InputComponent());
            npc.AddComponent(new Movable());
            npc.AddComponent(new Position(x, y));
            npc.AddComponent(new Drawable('D', new Engine.Utility.Color(1f, 0, 0)));
            npc.AddComponent(new Description("Very scary dummy dragon",""));
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

            stats.Strength.Value = 10;
            stats.Reflexes.Value = 10;
            stats.Perception.Value = 10;
            stats.Willpower.Value = 10;
            stats.Imagination.Value = 10;
            stats.Wit.Value = 10;

            npc.AddComponent(stats);
            npc.AddComponent(new ActionPoints());

            return npc;
        }

	
	
	
    }
}

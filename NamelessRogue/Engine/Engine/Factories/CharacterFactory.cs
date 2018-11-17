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
            playerCharacter.AddComponent(new ItemsHolder());
            playerCharacter.AddComponent(new OccupiesTile());


            playerCharacter.AddComponent(new Health(100,0,100));
            playerCharacter.AddComponent(new Stamina(100,0,100));
            playerCharacter.AddComponent(new Strength(10,0,100));
            playerCharacter.AddComponent(new Agility(10,0,10));
            playerCharacter.AddComponent(new Endurance(10, 0, 100));
            playerCharacter.AddComponent(new Imagination(10,0,10));
            playerCharacter.AddComponent(new Willpower(10,0,10));
            playerCharacter.AddComponent(new Wit(10,0,10));

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

            npc.AddComponent(new Health(100, 0, 100));
            npc.AddComponent(new Stamina(100, 0, 100));
            npc.AddComponent(new Strength(10, 0, 100));
            npc.AddComponent(new Agility(10, 0, 10));
            npc.AddComponent(new Endurance(10, 0, 100));
            npc.AddComponent(new Imagination(10, 0, 10));
            npc.AddComponent(new Willpower(10, 0, 10));
            npc.AddComponent(new Wit(10, 0, 10));
            return npc;
        }

	
	
	
    }
}

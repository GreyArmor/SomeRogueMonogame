using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Factories
{
    public static class ResourceLibrary
    {
        public static List<MapResource> Resources { get; } = new List<MapResource>();

        static ResourceLibrary()
        {
            #region food resources

            MapResource wheat = new MapResource();
            wheat.Info.Name = "Wheat";
            wheat.Info.ProductionModifier = new ProductionValue(2, 1, 0, 0, 0, 1);
            wheat.AppearsOn.AddRange(new[] {Biomes.Plains});
            wheat.Level = 1;
            wheat.Representation = 'W';
            wheat.CharColor = new Color(Microsoft.Xna.Framework.Color.Yellow);

            MapResource rice = new MapResource();
            rice.Info.Name = "Rice";
            rice.Info.ProductionModifier = new ProductionValue(2, 1, 0, 0, 0, 1);
            rice.AppearsOn.AddRange(new[] {Biomes.Swamp});
            rice.Level = 1;
            rice.Representation = 'R';
            rice.CharColor = new Color(Microsoft.Xna.Framework.Color.White);

            MapResource game = new MapResource();
            game.Info.Name = "Game";
            game.Info.ProductionModifier = new ProductionValue(2, 1, 0, 0, 0, 1);
            game.AppearsOn.AddRange(new[] {Biomes.Forest, Biomes.SnowDesert, Biomes.Tundra});
            game.Level = 1;
            game.Representation = 'G';
            game.CharColor = new Color(Microsoft.Xna.Framework.Color.Gray);

            MapResource banana = new MapResource();
            banana.Info.Name = "Banana";
            banana.Info.ProductionModifier = new ProductionValue(2, 1, 0, 0, 0, 1);
            banana.AppearsOn.AddRange(new[] {Biomes.Jungle});
            banana.Level = 1;
            banana.Representation = 'B';
            banana.CharColor = new Color(Microsoft.Xna.Framework.Color.Yellow);

            MapResource fish = new MapResource();
            fish.Info.Name = "Fish";
            fish.Info.ProductionModifier = new ProductionValue(2, 1, 0, 0, 0, 1);
            fish.AppearsOn.AddRange(new[] {Biomes.Sea, Biomes.River, Biomes.Lake});
            fish.Level = 1;
            fish.Representation = 'F';
            fish.CharColor = new Color(Microsoft.Xna.Framework.Color.LightBlue);


            MapResource oasis = new MapResource();
            oasis.Info.Name = "Oasis";
            oasis.Info.ProductionModifier = new ProductionValue(2, 1, 0, 0, 0, 1);
            oasis.AppearsOn.AddRange(new[] {Biomes.Desert, Biomes.Savannah});
            oasis.Level = 1;
            oasis.Representation = 'O';
            oasis.CharColor = new Color(Microsoft.Xna.Framework.Color.Green);

            Resources.Add(wheat);
            Resources.Add(rice);
            Resources.Add(game);
            Resources.Add(banana);
            Resources.Add(fish);
            Resources.Add(oasis);

            #endregion

            #region manufacturing resources

            MapResource stone = new MapResource();
            stone.Info.Name = "Stone";
            stone.Info.ProductionModifier = new ProductionValue(0, 2, 0, 1, 0, 0);
            stone.AppearsOn.AddRange(new[] {Biomes.Plains, Biomes.Beach, Biomes.Desert, Biomes.Mountain, Biomes.Swamp, Biomes.Savannah});
            stone.Level = 1;
            stone.Representation = 'S';
            stone.CharColor = new Color(Microsoft.Xna.Framework.Color.Gray);

            MapResource bronze = new MapResource();
            bronze.Info.Name = "Bronze";
            bronze.Info.ProductionModifier = new ProductionValue(0, 2, 0, 1, 0, 0);
            bronze.AppearsOn.AddRange(new[]
                {Biomes.Plains, Biomes.Hills, Biomes.Desert, Biomes.Mountain, Biomes.Swamp, Biomes.Savannah});
            bronze.Level = 1;
            bronze.Representation = 'B';
            bronze.CharColor = new Color(Microsoft.Xna.Framework.Color.SandyBrown);

            MapResource silver = new MapResource();
            silver.Info.Name = "Silver";
            silver.Info.ProductionModifier = new ProductionValue(0, 2, 0, 1, 0, 0);
            silver.AppearsOn.AddRange(new[]
                {Biomes.Plains, Biomes.Hills, Biomes.Desert, Biomes.Mountain, Biomes.Swamp,Biomes.Savannah});
            silver.Level = 1;
            silver.Representation = 'S';
            silver.CharColor = new Color(Microsoft.Xna.Framework.Color.Silver);

            MapResource gold = new MapResource();
            gold.Info.Name = "Gold";
            gold.Info.ProductionModifier = new ProductionValue(0, 2, 0, 1, 0, 0);
            gold.AppearsOn.AddRange(new[] {Biomes.Plains, Biomes.Hills, Biomes.Desert, Biomes.Mountain, Biomes.Swamp, Biomes.Savannah });
            gold.Level = 1;
            gold.Representation = 'G';
            gold.CharColor = new Color(Microsoft.Xna.Framework.Color.Gold);

            MapResource copper = new MapResource();
            copper.Info.Name = "Copper";
            copper.Info.ProductionModifier = new ProductionValue(0, 2, 0, 1, 0, 0);
            copper.AppearsOn.AddRange(new[]
                {Biomes.Plains, Biomes.Hills, Biomes.Desert, Biomes.Mountain, Biomes.Swamp, Biomes.Savannah});
            copper.Level = 1;
            copper.Representation = 'C';
            copper.CharColor = new Color(Microsoft.Xna.Framework.Color.RosyBrown);

            Resources.Add(stone);
            Resources.Add(bronze);
            Resources.Add(silver);
            Resources.Add(gold);
            Resources.Add(copper);

            #endregion

            #region culture resources

            MapResource coffee = new MapResource();
            coffee.Info.Name = "Coffee";
            coffee.Info.ProductionModifier = new ProductionValue(1, 0, 1, 1, 1, 0);
            coffee.AppearsOn.AddRange(new[] {Biomes.Forest, Biomes.Jungle});
            coffee.Level = 1;
            coffee.Representation = 'C';
            coffee.CharColor = new Color(Microsoft.Xna.Framework.Color.DarkViolet);

            MapResource ivory = new MapResource();
            ivory.Info.Name = "Ivory";
            ivory.Info.ProductionModifier = new ProductionValue(0, 0, 2, 0, 1, 0);
            ivory.AppearsOn.AddRange(new[] {Biomes.Desert, Biomes.Jungle});
            ivory.Level = 1;
            ivory.Representation = 'I';
            ivory.CharColor = new Color(Microsoft.Xna.Framework.Color.White);

            MapResource jade = new MapResource();
            jade.Info.Name = "Jade";
            jade.Info.ProductionModifier = new ProductionValue(0, 0, 1, 1, 1, 0);
            jade.AppearsOn.AddRange(new[] {Biomes.Hills, Biomes.Mountain,});
            jade.Level = 1;
            jade.Representation = 'J';
            jade.CharColor = new Color(Microsoft.Xna.Framework.Color.Green);


            MapResource marble = new MapResource();
            marble.Info.Name = "Marble";
            marble.Info.ProductionModifier = new ProductionValue(0, 2, 2, 0, 0, 0);
            marble.AppearsOn.AddRange(new[]
                {Biomes.Plains, Biomes.Beach, Biomes.Desert, Biomes.Mountain, Biomes.Swamp});
            marble.Level = 1;
            marble.Representation = 'M';
            marble.CharColor = new Color(Microsoft.Xna.Framework.Color.White);

            MapResource silk = new MapResource();
            silk.Info.Name = "Silk";
            silk.Info.ProductionModifier = new ProductionValue(0, 0, 2, 1, 0, 0);
            silk.AppearsOn.AddRange(new[] {Biomes.Jungle, Biomes.Forest});
            silk.Level = 1;
            silk.Representation = 'S';
            silk.CharColor = new Color(Microsoft.Xna.Framework.Color.White);

            MapResource tobacco = new MapResource();
            tobacco.Info.Name = "Tobacco";
            tobacco.Info.ProductionModifier = new ProductionValue(0, 0, 2, 1, 1, -1);
            tobacco.AppearsOn.AddRange(new[] {Biomes.Jungle, Biomes.Forest, Biomes.Plains});
            tobacco.Level = 1;
            tobacco.Representation = 'T';
            tobacco.CharColor = new Color(Microsoft.Xna.Framework.Color.Gray);

            Resources.Add(coffee);
            Resources.Add(ivory);
            Resources.Add(jade);
            Resources.Add(marble);
            Resources.Add(silk);
            Resources.Add(tobacco);

            #endregion

            #region science resources

            MapResource mercury = new MapResource();
            mercury.Info.Name = "Mercury";
            mercury.Info.ProductionModifier = new ProductionValue(0, 0, 0, 3, 0, -1);
            mercury.AppearsOn.AddRange(new[] {Biomes.Forest, Biomes.Jungle});
            mercury.Level = 1;
            mercury.Representation = 'M';
            mercury.CharColor = new Color(Microsoft.Xna.Framework.Color.Silver);

            MapResource sulfur = new MapResource();
            sulfur.Info.Name = "Sulfur";
            sulfur.Info.ProductionModifier = new ProductionValue(0, 0, 0, 3, 0, -1);
            sulfur.AppearsOn.AddRange(new[] {Biomes.Mountain});
            sulfur.Level = 1;
            sulfur.Representation = 'S';
            sulfur.CharColor = new Color(Microsoft.Xna.Framework.Color.Yellow);

            MapResource aluminum = new MapResource();
            aluminum.Info.Name = "Aluminum";
            aluminum.Info.ProductionModifier = new ProductionValue(0, 0, 1, 2, 0, 0);
            aluminum.AppearsOn.AddRange(new[] {Biomes.Forest, Biomes.Jungle});
            aluminum.Level = 1;
            aluminum.Representation = 'A';
            aluminum.CharColor = new Color(Microsoft.Xna.Framework.Color.Silver);


            Resources.Add(mercury);
            Resources.Add(sulfur);
            Resources.Add(aluminum);

            #endregion

            #region health resources

            MapResource medicalHerbs = new MapResource();
            medicalHerbs.Info.Name = "Medical herbs";
            medicalHerbs.Info.ProductionModifier = new ProductionValue(0, 0, 1, 2, 0, 3);
            medicalHerbs.AppearsOn.AddRange(new[] {Biomes.Forest, Biomes.Jungle, Biomes.Plains, Biomes.Tundra, Biomes.Savannah});
            medicalHerbs.Level = 1;
            medicalHerbs.Representation = 'H';
            medicalHerbs.CharColor = new Color(Microsoft.Xna.Framework.Color.Green);

            MapResource freshwater = new MapResource();
            freshwater.Info.Name = "Freshwater";
            freshwater.Info.ProductionModifier = new ProductionValue(1, 0, 0, 0, 1, 2);
            freshwater.AppearsOn.AddRange(new[] {Biomes.Forest, Biomes.Jungle, Biomes.Plains, Biomes.Tundra, Biomes.Mountain, Biomes.Desert});
            freshwater.Level = 1;
            freshwater.Representation = 'W';
            freshwater.CharColor = new Color(Microsoft.Xna.Framework.Color.Blue);

            Resources.Add(medicalHerbs);
            Resources.Add(freshwater);

            #endregion

            #region mana resources

            MapResource spiritForest = new MapResource();
            spiritForest.Info.Name = "Spirit forest";
            spiritForest.Info.ProductionModifier = new ProductionValue(0, 0, 1, 0, 3, 0);
            spiritForest.AppearsOn.AddRange(new[] { Biomes.Forest, Biomes.Jungle, Biomes.Plains, Biomes.Tundra });
            spiritForest.Level = 1;
            spiritForest.Representation = 'S';
            spiritForest.CharColor = new Color(Microsoft.Xna.Framework.Color.YellowGreen);

            MapResource elementalMeadows = new MapResource();
            elementalMeadows.Info.Name = "Elemental meadows";
            elementalMeadows.Info.ProductionModifier = new ProductionValue(0, 0, 1, 1, 3, 0);
            elementalMeadows.AppearsOn.AddRange(new[] { Biomes.Plains});
            elementalMeadows.Level = 1;
            elementalMeadows.Representation = 'E';
            elementalMeadows.CharColor = new Color(Microsoft.Xna.Framework.Color.Yellow);

            MapResource darkBog = new MapResource();
            darkBog.Info.Name = "Dark bog";
            darkBog.Info.ProductionModifier = new ProductionValue(0, 0, 1, 0, 3, -1);
            darkBog.AppearsOn.AddRange(new[] { Biomes.Swamp });
            darkBog.Level = 1;
            darkBog.Representation = 'B';
            darkBog.CharColor = new Color(Microsoft.Xna.Framework.Color.DarkGray);

            MapResource ancientCoralReef = new MapResource();
            ancientCoralReef.Info.Name = "Ancient coral reef";
            ancientCoralReef.Info.ProductionModifier = new ProductionValue(1, 0, 1, 0, 3, 0);
            ancientCoralReef.AppearsOn.AddRange(new[] { Biomes.Sea, Biomes.Lake, Biomes.River });
            ancientCoralReef.Level = 1;
            ancientCoralReef.Representation = 'C';
            ancientCoralReef.CharColor = new Color(Microsoft.Xna.Framework.Color.OrangeRed);


            Resources.Add(spiritForest);
            Resources.Add(elementalMeadows);
            Resources.Add(darkBog);
            Resources.Add(ancientCoralReef);
            #endregion
        }

        public static MapResource GetRandomResource(Biomes biome, Random r)
        {
            var biomesResources = Resources.Where(x => x.AppearsOn.Contains(biome)).ToArray();
            var rndNum = r.Next(0, biomesResources.Length);
            return biomesResources[rndNum];
        }

    }
}

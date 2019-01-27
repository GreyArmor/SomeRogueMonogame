using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Factories
{
    public static class ArtifactLibrary
    {
        public static List<MapArtifact> Artifacts { get; } = new List<MapArtifact>();
        
        static ArtifactLibrary()
        {
            AddArtifact("Tree Of Life", new ProductionValue(3, 0, 1, 0, 3, 3),'T', new Color(Microsoft.Xna.Framework.Color.Green), 50);
            AddArtifact("Ancient ruins", new ProductionValue(0, 0, 1, 2, 1, 0), 'R', new Color(Microsoft.Xna.Framework.Color.Gray), 300);
            AddArtifact("Mana confluence", new ProductionValue(0, 1, 0, 1, 3, -1), 'C', new Color(Microsoft.Xna.Framework.Color.OrangeRed), 10);
            AddArtifact("Mana pillars", new ProductionValue(0, 1, 1, 1, 2, 0), 'P', new Color(Microsoft.Xna.Framework.Color.OrangeRed), 50);
            AddArtifact("Ancient tomb", new ProductionValue(0, 0, 1, 2, 1, 0), 't', new Color(Microsoft.Xna.Framework.Color.Gray), 300);
            AddArtifact("Bottomless pit", new ProductionValue(0, 0, 2, 2, 2, 0), 'p', new Color(Microsoft.Xna.Framework.Color.Gray), 150);
            AddArtifact("Meteor crater", new ProductionValue(0, 3, 0, 3, 1, 0), 'c', new Color(Microsoft.Xna.Framework.Color.Red), 100);
            AddArtifact("Irradiated land", new ProductionValue(-2, 0, 0, 2, 0, -2), 'I', new Color(Microsoft.Xna.Framework.Color.PaleGreen), 300);
            AddArtifact("Cave system", new ProductionValue(0, 1, 0, 1, 1, 0), 'O', new Color(Microsoft.Xna.Framework.Color.White), 500);
            AddArtifact("Rich soil", new ProductionValue(2, 0, 0, 0, 0, 2), 'R', new Color(Microsoft.Xna.Framework.Color.Green), 100);
            AddArtifact("Geysers", new ProductionValue(0, 2, 0, 2, 1, 0), 'G', new Color(Microsoft.Xna.Framework.Color.YellowGreen), 100);
            AddArtifact("Meteor field", new ProductionValue(0, 2, 0, 1, 1, 0), 'M', new Color(Microsoft.Xna.Framework.Color.Gray), 150);
            AddArtifact("Freshwater lake", new ProductionValue(0, 0, 1, 0, 0, 3), 'L', new Color(Microsoft.Xna.Framework.Color.Blue), 150);
            //AddArtifact("Fallen star", new ProductionValue(0, 3, 0, 3, 1, 0));
        }

        public static void AddArtifact(string name, ProductionValue value, char representation, Color color, int timeOfLife)
        {
            var artifact = new MapArtifact();
            artifact.Info.Name = name;
            artifact.Info.ProductionModifier = value;
            artifact.Representation = representation;
            artifact.CharColor = color;
            artifact.TimeOfLife = timeOfLife;
            artifact.TimeLeft = timeOfLife;
            Artifacts.Add(artifact);
        }


        public static MapArtifact GetRandomArtifact(Random random)
        {
            var randomArti = random.Next(0, Artifacts.Count);
            return new MapArtifact()
            {
                Info = new ObjectInfo()
                {
                    Name = Artifacts[randomArti].Info.Name,
                    ProductionModifier = Artifacts[randomArti].Info.ProductionModifier
                },
                Representation = Artifacts[randomArti].Representation,
                CharColor = Artifacts[randomArti].CharColor
            };
        }


    }
}

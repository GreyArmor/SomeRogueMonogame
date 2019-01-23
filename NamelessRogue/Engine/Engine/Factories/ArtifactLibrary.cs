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
            AddArtifact("Tree Of Life", new ProductionValue(3, 0, 1, 0, 3, 3),'T', new Color(Microsoft.Xna.Framework.Color.Green));
            AddArtifact("Volcano", new ProductionValue(-3, 0, 1, 2, 3, -3), 'V', new Color(Microsoft.Xna.Framework.Color.DarkRed));
            AddArtifact("Ancient ruins", new ProductionValue(0, 0, 1, 3, 1, 0), 'R', new Color(Microsoft.Xna.Framework.Color.Gray));
            AddArtifact("Mana confluence", new ProductionValue(0, 1, 0, 1, 3, -1), 'C', new Color(Microsoft.Xna.Framework.Color.OrangeRed));
            AddArtifact("Ancient tomb", new ProductionValue(0, 0, 1, 2, 1, 0), 't', new Color(Microsoft.Xna.Framework.Color.Gray));
            AddArtifact("Bottomless pit", new ProductionValue(0, 0, 2, 2, 2, 0), 'p', new Color(Microsoft.Xna.Framework.Color.Gray));
            AddArtifact("Fallen star", new ProductionValue(0, 3, 0, 3, 1, 0), 'S', new Color(Microsoft.Xna.Framework.Color.Yellow));
            AddArtifact("Irradiated land", new ProductionValue(-2, 0, 0, 2, 0, -2), 'I', new Color(Microsoft.Xna.Framework.Color.PaleGreen));
            //AddArtifact("Fallen star", new ProductionValue(0, 3, 0, 3, 1, 0));
        }

        public static void AddArtifact(string name, ProductionValue value, char representation, Color color)
        {
            var artifact = new MapArtifact();
            artifact.Info.Name = name;
            artifact.Info.ProductionModifier = value;
            artifact.Representation = representation;
            artifact.CharColor = color;
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

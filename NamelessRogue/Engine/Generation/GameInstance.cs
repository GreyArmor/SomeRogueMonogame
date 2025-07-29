using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp.Random;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Utility;
using NamelessRogue.Engine.Components;

namespace NamelessRogue.Engine.Generation
{
    public class GameInstance : Component
    {
        public GameInstance() { }
        public GameInstance(int seed)//, WorldTemplate template)
        {
            Seed = seed;
            //  Template = template;
            GlobalRandom = new InternalRandom(seed);
            NamesGenerator = new NamesGenerator();
            GlobalRandom = new InternalRandom(seed);
            TerrainGen = new TerrainGenerator(GlobalRandom, 500);//(int)template.WorldSize.X);
            string[] cyberpunkCityNames = new string[]
             {
                "Neotoka", "Veltrix", "Draxylon", "Cindara", "Ombervek", "Zentara", "Crynova", "Halcyrex", "Virelon", "Noxmere",
                "Tekhama", "Dravokh", "Miranex", "Sornveil", "Quantrex", "Luxara", "Strathide", "Valkarion", "Nethspire", "Xyvanta",
                "Korvath", "Zephuron", "Graveth", "Orsimar", "Thrallax", "Cryndale", "Veltrion", "Zarnova", "Threxium", "Omnivar",
                "Xarneth", "Lytraxis", "Korvexa", "Synmaris", "Tarkion", "Blyxara", "Quanthel", "Dramore", "Zethralis", "Nivora",
                "Hexvane", "Tornexis", "Valkhera", "Xeraphon", "Noktara", "Cindros", "Vorentha", "Skarnyx", "Jentharis", "Obvex",
                "Luxmire", "Thraven", "Krythana", "Sornex", "Ylvaron", "Zarnith", "Delvora", "Myrenthos", "Praxion", "Xevelar",
                "Orvaxa", "Zynthral", "Corthanix", "Vireth", "Halzonis", "Tarnyx", "Grivara", "Quarnyx", "Lorvex", "Kandros",
                "Nythera", "Draxmere", "Zalvaron", "Nuxareth", "Orzava", "Zethron", "Cryonova", "Voltraxis", "Myrelon", "Vantross",
                "Droxara", "Nyxveil", "Kaltora", "Ossiren", "Virexia", "Thrallaxis", "Jynthar", "Noctira", "Valcore", "Zypheron",
                "Drakaris", "Quarvox", "Lytheron", "Hexmoor", "Xylandra", "Skelvora", "Crixalis", "Ytheros", "Blyndrix", "Omnithal",
                "Tarvex", "Zarnyx", "Valkeron", "Grynthel", "Luxithra", "Orvolis", "Mechron", "Sundrax", "Exonith", "Quireth"
             };
            CyberpunkTemplate = new CultureTemplate("Cyberpunk", cyberpunkCityNames);
        }
        public int Seed { get; set; }
        public CultureTemplate CyberpunkTemplate { get; private set; }
        public int Turn { get; set; }
        public NamesGenerator NamesGenerator { get; }
        public WorldTemplate Template { get; }
        public int WorldMapResolution { get; set; } = 500;
        public InternalRandom GlobalRandom { get; set; }
        public TerrainGenerator TerrainGen { get; set; }
    }
}

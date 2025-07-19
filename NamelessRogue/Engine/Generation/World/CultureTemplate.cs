using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markov;
using NamelessRogue.Engine.Utility;
using RogueSharp.Random;

namespace NamelessRogue.Engine.Generation.World
{
    public class CultureTemplate
    {
        public string TemplateName { get; set; }

        private Markov.MarkovChain<char> townChain;

        public CultureTemplate()
        { }

        public CultureTemplate(string templateName, string townNames)//, string landNames)
        {
            TemplateName = templateName;

            TownNames = townNames;
            townChain = new MarkovChain<char>(2);
            List<string> townList = townNames.ToLower().Split(' ').ToList();
            foreach (var str in townList)
            {
                townChain.Add(str);
            }
        }

        public CultureTemplate(string templateName, string[] townNames)//, string landNames)
        {
            TemplateName = templateName;

            foreach (var str in townNames)
            {
                TownNames += str + ' ';
            }
            townChain = new MarkovChain<char>(2);
            List<string> townList = TownNames.ToLower().Split(' ').ToList();
            foreach (var str in townNames)
            {
                townChain.Add(str);
            }
        }

        public string TownNames { get; set; }


        public string GetTownName(InternalRandom random)
        {
            if (townChain == null)
            {
                List<string> townList = TownNames.ToLower().Split(' ').ToList();
                foreach (var str in townList)
                {
                    townChain.Add(str);
                }
            }


            char[] charName = null;
            while (charName == null || charName.Length<3 || charName.Length>10)
            {
                charName = townChain.Chain(random.Next()).ToArray();
            }
            return new string(charName).FirstCharToUpper();
        }

    }
}

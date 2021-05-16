using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markov;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Generation.World
{
    public class CultureTemplate
    {
        public string TemplateName { get; }

        private Markov.MarkovChain<char> townChain;

        public CultureTemplate()
        {}

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
            //foreach (var str in landlists)
            //{
            //    landChain.Add(str);
            //}
        }

        public string TownNames { get; set; }


        public string GetTownName(Random random)
        {
            if (townChain == null)
            {
                List<string> townList = TownNames.ToLower().Split(' ').ToList();
                foreach (var str in townList)
                {
                    townChain.Add(str);
                }
            }

            return new string(townChain.Chain(random).ToArray()).FirstCharToUpper();
        }

    }
}

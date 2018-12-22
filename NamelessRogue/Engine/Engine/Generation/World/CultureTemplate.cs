using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markov;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class CultureTemplate
    {
        public string TemplateName { get; }

        Markov.MarkovChain<char> townChain = new MarkovChain<char>(2);
        Markov.MarkovChain<char> maleChain = new MarkovChain<char>(2);
        Markov.MarkovChain<char> femaleChain = new MarkovChain<char>(2);
      //  Markov.MarkovChain<char> landChain = new MarkovChain<char>(2);

        public CultureTemplate(string templateName, string townNames, string maleNames, string femaleNames)//, string landNames)
        {
            TemplateName = templateName;

            List<string> townList = townNames.ToLower().Split(' ').ToList();
            List<string> malelist = maleNames.ToLower().Split(' ').ToList();
            List<string> femalelist = femaleNames.ToLower().Split(' ').ToList();
            //  List<string> landlists = landNames.ToLower().Split(' ').ToList();

            foreach (var str in townList)
            {
                townChain.Add(str);
            }

            foreach (var str in malelist)
            {
                maleChain.Add(str);
            }

            foreach (var str in femalelist)
            {
                femaleChain.Add(str);
            }

            //foreach (var str in landlists)
            //{
            //    landChain.Add(str);
            //}
        }


        public string GetTownName(Random random)
        {
            return new string(townChain.Chain(random).ToArray()).FirstCharToUpper();
        }

        public string GetMaleName(Random random)
        {
            return new string(maleChain.Chain(random).ToArray()).FirstCharToUpper();
        }

        public string GetFemaleName(Random random)
        {
            return new string(femaleChain.Chain(random).ToArray()).FirstCharToUpper();
        }

        //public string GetLandName(Random random)
        //{
        //    return new string(landChain.Chain(random).ToArray());
        //}



    }
}

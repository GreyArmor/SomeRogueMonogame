﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markov;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using RogueSharp.Random;

namespace NamelessRogue.Engine.Generation
{
    public class NamesGenerator
    {
        Markov.MarkovChain<char> countryChain = new MarkovChain<char>(2);

        public string GetCountryName(InternalRandom random)
        {
            return new string(countryChain.Chain(random.Next()).ToArray());
        }

        public NamesGenerator()
        {
            List<string> countryList =
                "afghanistan africa albania algeria andorra angola antigua arabia argentina armenia ascension australia austria bahamas bahrain bangladesh barbados belarus belgium belize bermuda bolivia bosnia botswana brazil britain brunei bulgaria burundi cambodia cameroon canada chad chile china china colombia congo costarica croatia cuba cyprus cyrenaica czech denmark ecuador egypt emirates eritrea estonia ethiopia falklands finland france gambia georgia germany ghana greece greenland grenada guam guatemala guernsey guinea guinea haiti hongkong hungary iceland india indonesia iran iraq ireland israel italy jamaica japan jordan kashmir kenya korea kosovo kurdistan kuwait laos latvia lebanon libya liechtenstein lithuania luxembourg macau macedonia madagascar malaysia maldives mali malta mexico micronesia monaco mongolia morocco mozambique nepal netherlands nicaragua niger nigeria norway norway oman pakistan palestine panama paraguay peru philippines poland portugal romania ross russia rwanda salvador saudi serbia seychelles singapore slovakia slovenia somalia spain sudan sudan svalbard sweden switzerland syria taiwan taiwan thailand timor tobago trinidad tunisia turkey turkmenistan uganda ukraine unitedkingdom uruguay uzbekistan vanuatu vatican venezuela vietnam yemen zealand zimbabwe"
                    .Split(' ').ToList();

            foreach (var str in countryList)
            {
                countryChain.Add(str);
            }
        }
    }
}

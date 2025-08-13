using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation
{

    public enum GeneratorName
    {
        English,
        EasternSlavic,
        Chinese
    }

    public interface ICharacterNameGenerator
    {
        string GenerateName(bool isMale); 
    }

    public static class CharacterNameGenerators
    {
        static Dictionary<GeneratorName, ICharacterNameGenerator> generators;

        static CharacterNameGenerators()
        {
            generators = new Dictionary<GeneratorName, ICharacterNameGenerator>();
            generators.Add(GeneratorName.English, new EnglishNameGenerator());
            generators.Add(GeneratorName.EasternSlavic, new EasternSlavicGenerator());
            generators.Add(GeneratorName.Chinese, new ChineseNamesGenerator());
        }
        public static string GetName(GeneratorName generatorName, bool isMale)
        {
            var generator = generators[generatorName];
            return generator.GenerateName(isMale);
        }
    }
}

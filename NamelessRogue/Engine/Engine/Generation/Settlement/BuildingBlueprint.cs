using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Generation.Settlement
{

    public enum BlueprintCell
    {
        Nothing, Wall, Door, Window, IndoorsFurniture, OutdoorsFurniture, Bed
    }
    public class BuildingBlueprint
    {
        public static Dictionary<char,BlueprintCell> BlueprintCellsDictionary { get; }

        static BuildingBlueprint()
        {
            BlueprintCellsDictionary = new Dictionary<char, BlueprintCell>();
            BlueprintCellsDictionary.Add(' ',BlueprintCell.Nothing);
            BlueprintCellsDictionary.Add('#', BlueprintCell.Wall);
            BlueprintCellsDictionary.Add('D', BlueprintCell.Door);
            BlueprintCellsDictionary.Add('W', BlueprintCell.Window);
            BlueprintCellsDictionary.Add('?', BlueprintCell.IndoorsFurniture);
            BlueprintCellsDictionary.Add('!', BlueprintCell.OutdoorsFurniture);
            BlueprintCellsDictionary.Add('B', BlueprintCell.Bed);
        }

        public BlueprintCell[][] Matrix { get; set; }
        public BuildingBlueprint(string blueprintString)
        {
            Matrix = ParseString(blueprintString);
        }

        private BlueprintCell[][] ParseString(string blueprintString)
        {
            var strings = blueprintString.Split('\n');

            BlueprintCell[][] result = new BlueprintCell[strings.Length][];
            for (var index = 0; index < strings.Length; index++)
            {
                var s = strings[index];
                result[index] = new BlueprintCell[s.Length];
                for (int i = 0; i < s.Length; i++)
                {
                    result[index][i] = BlueprintCellsDictionary[s[i]];
                }
            }

            return result;
        }
    }
}

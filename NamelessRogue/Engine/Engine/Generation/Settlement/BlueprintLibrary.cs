using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Generation.Settlement
{
    public static class BlueprintLibrary
    {
        public static List<BuildingBlueprint> Blueprints { get; } = new List<BuildingBlueprint>();
        static BlueprintLibrary()
        {
            var house = new BuildingBlueprint("#######D##\n" +
                                              "WB       #\n" +
                                              "#B       #\n" +
                                              "W        #\n" +
                                              "#        #\n" +
                                              "W ?      #\n" +
                                              "#        #\n" +
                                              "W?       #\n" +
                                              "#???     #\n" +
                                              "#######D##\n");
            Blueprints.Add(house);


        }
    }
}

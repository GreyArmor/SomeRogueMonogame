using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Utility
{
    public static class TileBitmaskingEncoding
    {
        public static string Pillar = "000" +
                                      "010" +
                                      "000";

        public static string WallVertical0 =  "010" +
                                              "010" +
                                              "010";

        public static string WallVertical1 = "000" +
                                             "010" +
                                             "010";

        public static string WallVertical2 =  "010" +
                                              "010" +
                                              "000";

        public static string WallHorizontal0 =   "000" +
                                                "111" +
                                                "000";

        public static string WallHorizontal1 = "000" +
                                               "011" +
                                               "000";

        public static string WallHorizontal2 = "000" +
                                               "110" +
                                               "000";

        public static string CornerTopRight  = "000" +
                                               "110" +
                                               "010";

        public static string CornerTopLeft =   "000" +
                                               "011" +
                                               "010";

        public static string CornerBotRight = "010" +
                                              "110" +
                                              "000";

        public static string CornerBotLeft =   "010" +
                                               "011" +
                                               "000";

        public static string InterserctionCenter =  "010" +
                                                    "111" +
                                                    "010";

        public static string InterserctionLeft =   "010" +
                                                   "011" +
                                                   "010";

        public static string InterserctionTop =    "000" +
                                                   "111" +
                                                   "010";

        public static string InterserctionBot =    "010" +
                                                   "111" +
                                                   "000";

        public static string InterserctionRight =  "010" +
                                                   "110" +
                                                   "010";
    }
}

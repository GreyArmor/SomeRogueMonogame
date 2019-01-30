 

using System;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;

/**
 * Created by Admin on 05.12.2017.
 */
namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    public class TileNoiseInterpreter {
        public static Tuple<TerrainTypes, Biomes> GetTerrain(double noiseValue, double forest, double swamp, double lake, double desert, double temperature,  int resolutionZoomed, double dX, double dY)
        {
            if (noiseValue>1){
                noiseValue=1;
            }
            if (noiseValue<0){
                noiseValue=0;
            }
            
            var temperatureCoef = dY / resolutionZoomed + temperature;


            TerrainTypes? t = null;
            Biomes? b = null;
            if(noiseValue>0.80) {
                t  = TerrainTypes.Snow;
                b = Biomes.SnowDesert;
            }
            else if(noiseValue>0.75) {
                t  = TerrainTypes.HardRocks;
                b = Biomes.Mountain;
            }

            else if(noiseValue>0.7) {
                t  = TerrainTypes.Rocks;
                b = Biomes.Mountain;
            }
            else if(noiseValue>0.65) {
                t  = TerrainTypes.LightRocks;
                b = Biomes.Mountain;
            }
            else if(noiseValue>0.51) {
                t  = TerrainTypes.Grass;

                if (temperatureCoef <= 0.15 || temperatureCoef >= 0.85)
                {
                    b = Biomes.SnowDesert;
                }
                else if (temperatureCoef <= 0.6 && temperatureCoef >= 0.4)
                {
                    b = Biomes.Savannah;
                }
                else
                {
                    b = Biomes.Plains;
                }
            }
            else if(noiseValue>=0.5) {
                t  = TerrainTypes.Sand;
                b = Biomes.Beach;
            }
            else if(noiseValue<0.5) {
                t  =  TerrainTypes.Water;
                b = Biomes.Sea;
            }

            if (t != null)
            {

                if (t != TerrainTypes.Water && t!=TerrainTypes.Rocks && t!=TerrainTypes.HardRocks&&t!=TerrainTypes.HardRocks)
                {
                    if (lake >= 0.9f)
                    {
                        t = TerrainTypes.Water;
                        b = Biomes.Lake;
                    }
                    else if (desert>=0.8f)
                    {
                        t = TerrainTypes.Sand;
                       
                        if (temperatureCoef <= 0.15 || temperatureCoef >= 0.85)
                        {
                            b = Biomes.SnowDesert;
                        }
                        else
                        {
                            b = Biomes.Desert;
                        }
                    
                    }
                    else if(swamp >=0.8f)
                    {
                        t = TerrainTypes.Dirt;
                        b = Biomes.Swamp;
                    }
                    else if (forest>=0.8f)
                    {
                        t = TerrainTypes.Grass;

                        if (temperatureCoef <= 0.15 || temperatureCoef >= 0.85)
                        {
                            b = Biomes.Tundra;
                        }
                        else if (temperatureCoef <= 0.6 && temperatureCoef >= 0.5)
                        {
                            b = Biomes.Jungle;
                        }
                        else
                        {
                            b = Biomes.Forest;
                        }

                       
                    }
                }

                return new Tuple<TerrainTypes, Biomes>(t.Value,b.Value);
            }
            else
            {
                return new Tuple<TerrainTypes, Biomes>(TerrainTypes.Nothingness, Biomes.None);
            }
        }

    }
}

 

using System;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;

/**
 * Created by Admin on 05.12.2017.
 */
namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    public class TileNoiseInterpreter {
        public static Tuple<Terrain, Biome> GetTerrain(double noiseValue, double forest, double swamp, double lake, double desert, double temperature,  int resolutionZoomed, double x, double y)
        {
            if (noiseValue>1){
                noiseValue=1;
            }
            if (noiseValue<0){
                noiseValue=0;
            }

            var temperatureCoef = y / resolutionZoomed + temperature;


            Terrain t = null;
            Biome b = null;
            if(noiseValue>0.8) {
                t  = TerrainLibrary.Terrains[TerrainTypes.Snow];
                b = BiomesLibrary.Biomes[Biomes.SnowDesert];
            }
            else if(noiseValue>0.75) {
                t  = TerrainLibrary.Terrains[TerrainTypes.HardRocks];
                b = BiomesLibrary.Biomes[Biomes.Mountain];
            }

            else if(noiseValue>0.7) {
                t  = TerrainLibrary.Terrains[TerrainTypes.Rocks];
                b = BiomesLibrary.Biomes[Biomes.Mountain];
            }
            else if(noiseValue>0.65) {
                t  = TerrainLibrary.Terrains[TerrainTypes.LightRocks];
                b = BiomesLibrary.Biomes[Biomes.Hills];
            }
            else if(noiseValue>0.51) {
                t  = TerrainLibrary.Terrains[TerrainTypes.Grass];

                if (temperatureCoef <= 0.15 || temperatureCoef >= 0.85)
                {
                    b = BiomesLibrary.Biomes[Biomes.SnowDesert];
                }
                else if (temperatureCoef <= 0.6 && temperatureCoef >= 0.4)
                {
                    b = BiomesLibrary.Biomes[Biomes.Savannah];
                }
                else
                {
                    b = BiomesLibrary.Biomes[Biomes.Plains];
                }
            }
            else if(noiseValue>=0.5) {
                t  = TerrainLibrary.Terrains[TerrainTypes.Sand];
                b = BiomesLibrary.Biomes[Biomes.Beach];
            }
            else if(noiseValue<0.5) {
                t  = TerrainLibrary.Terrains[TerrainTypes.Water];
                b = BiomesLibrary.Biomes[Biomes.Sea];
            }

            if (t != null)
            {

                if (t.Type != TerrainTypes.Water && t.Type != TerrainTypes.Rocks && t.Type != TerrainTypes.HardRocks&& t.Type != TerrainTypes.HardRocks)
                {
                    if (lake >= 0.95f)
                    {
                        t = TerrainLibrary.Terrains[TerrainTypes.Water];
                        b = BiomesLibrary.Biomes[Biomes.Lake];
                    }
                    else if (desert>=0.8f)
                    {
                        t = TerrainLibrary.Terrains[TerrainTypes.Sand];
                       
                        if (temperatureCoef <= 0.15 || temperatureCoef >= 0.85)
                        {
                            b = BiomesLibrary.Biomes[Biomes.SnowDesert];
                        }
                        else
                        {
                            b = BiomesLibrary.Biomes[Biomes.Desert];
                        }
                    
                    }
                    else if(swamp >=0.9f)
                    {
                        t = TerrainLibrary.Terrains[TerrainTypes.Dirt];
                        b = BiomesLibrary.Biomes[Biomes.Swamp];
                    }
                    else if (forest>=0.8f)
                    {
                        t = TerrainLibrary.Terrains[TerrainTypes.Grass];

                        if (temperatureCoef <= 0.15 || temperatureCoef >= 0.85)
                        {
                            b = BiomesLibrary.Biomes[Biomes.Tundra];
                        }
                        else if (temperatureCoef <= 0.6 && temperatureCoef >= 0.5)
                        {
                            b = BiomesLibrary.Biomes[Biomes.Jungle];
                        }
                        else
                        {
                            b = BiomesLibrary.Biomes[Biomes.Forest];
                        }

                       
                    }
                }

                return new Tuple<Terrain, Biome>(t,b);
            }
            else
            {
                return new Tuple<Terrain, Biome>(null,null);
            }
        }

    }
}

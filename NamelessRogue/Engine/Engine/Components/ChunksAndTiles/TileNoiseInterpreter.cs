 

using NamelessRogue.Engine.Engine.Infrastructure;

/**
 * Created by Admin on 05.12.2017.
 */
namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    public class TileNoiseInterpreter {
        public static TerrainTypes GetTerrain(double noiseValue)
        {
            if (noiseValue>1){
                noiseValue=1;
            }
            if (noiseValue<0){
                noiseValue=0;
            }
            // System.out.print(noiseValue);
            // System.out.print("\n");
            TerrainTypes? t = null;
            if(noiseValue>0.80) {
                t  = TerrainTypes.Snow;
            }
            else if(noiseValue>0.75) {
                t  = TerrainTypes.HardRocks;
            }

            else if(noiseValue>0.7) {
                t  = TerrainTypes.Rocks;
            }
            else if(noiseValue>0.65) {
                t  = TerrainTypes.LightRocks;
            }
            else if(noiseValue>0.51) {
                t  = TerrainTypes.Grass;
            }
            else if(noiseValue>=0.5) {
                t  = TerrainTypes.Sand;
            }
            else if(noiseValue<0.5) {
                t  =  TerrainTypes.Water;
            }

            if (t != null)
            {
                return t.Value;
            }
            else
            {
                return TerrainTypes.Nothingness;
            }
        }
    }
}

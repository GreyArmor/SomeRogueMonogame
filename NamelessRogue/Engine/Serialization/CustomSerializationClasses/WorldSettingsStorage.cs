
// AUTOGENERATED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
namespace NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses
{
    [FlatBufferTable]
    public class WorldSettingsStorage : IStorage<NamelessRogue.Engine.Generation.WorldSettings>
    {
        [FlatBufferItem(0)]  public string Id { get; set; }
        [FlatBufferItem(1)]  public string ParentEntityId { get; set; }
        [FlatBufferItem(2)]  public Int32 ContinentTilesPerCivilization { get; set; }

        [FlatBufferItem(3)]  public Int32 ContinentTilesPerArtifact { get; set; }

        [FlatBufferItem(4)]  public Int32 ContinentTilesPerResource { get; set; }

        [FlatBufferItem(5)]  public Single WorldMapScale { get; set; }

        [FlatBufferItem(6)]  public TerrainGeneratorStorage TerrainGen { get; set; }

        [FlatBufferItem(7)]  public Int32 Seed { get; set; }

        [FlatBufferItem(8)]  public Int32 WorldBoardWidth { get; set; }

        [FlatBufferItem(9)]  public Int32 WorldBoardHeight { get; set; }

        [FlatBufferItem(10)]  public InternalRandomStorage GlobalRandom { get; set; }

        public void FillFrom(NamelessRogue.Engine.Generation.WorldSettings component)
        {

            this.ContinentTilesPerCivilization = component.ContinentTilesPerCivilization;

            this.ContinentTilesPerArtifact = component.ContinentTilesPerArtifact;

            this.ContinentTilesPerResource = component.ContinentTilesPerResource;

            this.WorldMapScale = component.WorldMapScale;

            this.TerrainGen = component.TerrainGen;

            this.Seed = component.Seed;

            this.WorldBoardWidth = component.WorldBoardWidth;

            this.WorldBoardHeight = component.WorldBoardHeight;

            this.GlobalRandom = component.GlobalRandom;

        }

        public void FillTo(NamelessRogue.Engine.Generation.WorldSettings component)
        {

            component.ContinentTilesPerCivilization = this.ContinentTilesPerCivilization;

            component.ContinentTilesPerArtifact = this.ContinentTilesPerArtifact;

            component.ContinentTilesPerResource = this.ContinentTilesPerResource;

            component.WorldMapScale = this.WorldMapScale;

            component.TerrainGen = this.TerrainGen;

            component.Seed = this.Seed;

            component.WorldBoardWidth = this.WorldBoardWidth;

            component.WorldBoardHeight = this.WorldBoardHeight;

            component.GlobalRandom = this.GlobalRandom;

        }

        public static implicit operator NamelessRogue.Engine.Generation.WorldSettings (WorldSettingsStorage thisType)
        {
            if (thisType == null) { return null; }
            NamelessRogue.Engine.Generation.WorldSettings result = new NamelessRogue.Engine.Generation.WorldSettings();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator WorldSettingsStorage (NamelessRogue.Engine.Generation.WorldSettings  component)
        {
            if (component == null) { return null; }
            WorldSettingsStorage result = new WorldSettingsStorage();
            result.FillFrom(component);
            return result;
        }

    }

}
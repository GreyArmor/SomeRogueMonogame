using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using static NamelessRogue.Engine.Generation.World.WorldBoardGenerator;

namespace NamelessRogue.Engine.UI
{
	public enum WorldGenAction
	{
		None,
		Generate,
		Exit
	}


	public class WorldGenerationParameters
	{
        public string seed = "";
        public string name = "";
		public DateTime time;
		public WorldSizeName worldSize;
		public int worldSizeValue = 0;
		public WaterLevel waterLevel;
        internal int neighboringCityCount;
    }

	public class GenerateWorldFileCommand : ICommand
	{
		public GenerateWorldFileCommand(WorldGenerationParameters parameters)
        {
            Parameters = parameters;
        }

        public WorldGenerationParameters Parameters { get; }
    }
	public class WorldGenerationUI : BaseScreen
	{
		public bool LocalMapDisplay { get; set; } = false;
		public WorldGenAction Action { get; set; } = WorldGenAction.None;
        WorldSizeName[] worldSizes = (WorldSizeName[])Enum.GetValues(typeof(WorldSizeName));
        string[] worldSizesNames = Enum.GetNames(typeof(WorldSizeName));
        WaterLevel[] waterLevel = (WaterLevel[])Enum.GetValues(typeof(WaterLevel));
        string[] waterLevelNames = Enum.GetNames(typeof(WaterLevel));

        int selectedWorldSize = 0;
        int selectedWaterLevel = 0;
        Dictionary<WorldSizeName, int> worldSizeValues = new Dictionary<WorldSizeName, int>();

		System.Numerics.Vector2 buttonSize;
		System.Numerics.Vector2 sidebarSize;
        System.Numerics.Vector2 smallButtonSize;

        private string[] currentFiles;
        int currentFile = 0;

        WorldGenerationParameters worldGenerationParameters;
		Random random;
        public WorldGenerationUI(NamelessGame game) : base(game)
		{
			buttonSize = new System.Numerics.Vector2(game.Settings.HudWidth, 50);
			smallButtonSize = new System.Numerics.Vector2(100, 30);
			sidebarSize = new System.Numerics.Vector2(uiSize.X/3*2, uiSize.Y);
            random = new Random();
				
            worldSizeValues = new Dictionary<WorldSizeName, int>();
			worldSizeValues.Add(WorldSizeName.Tiny, 100);
            worldSizeValues.Add(WorldSizeName.Small, 250);
            worldSizeValues.Add(WorldSizeName.Medium, 500);
            worldSizeValues.Add(WorldSizeName.Big, 750);
            worldSizeValues.Add(WorldSizeName.Large, 1000);

			worldGenerationParameters = new WorldGenerationParameters();
            worldGenerationParameters.seed = random.Next().ToString();
			worldGenerationParameters.name = game.CurrentGame.CyberpunkTemplate.GetTownName(game.CurrentGame.GlobalRandom);
			worldGenerationParameters.waterLevel = WaterLevel.NoWater;
			worldGenerationParameters.worldSize = WorldSizeName.Medium;
			selectedWorldSize = 2;
            worldGenerationParameters.worldSizeValue = worldSizeValues[WorldSizeName.Medium];
            worldGenerationParameters.time = RandomDateTimeGenerator.RandomDate(new DateTime(2125, 1, 1), new DateTime(2175, 1, 1));
        }

        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);


            ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
            ImGui.SetWindowSize(uiSize);

            //ImGui.SetCursorPos(menuPosition);
            //{
            ImGui.BeginChild("menu", sidebarSize, false);
            {
                ImGui.Text("Name");
                ImGui.InputText("##nameInput", ref worldGenerationParameters.name, 30);
                ImGui.SameLine();
                if (ButtonWithSound("New ##newname", smallButtonSize)) { worldGenerationParameters.name = game.CurrentGame.CyberpunkTemplate.GetTownName(game.CurrentGame.GlobalRandom); }
                ImGui.Spacing();
                ImGui.Text("Seed");
                ImGui.InputText("##seedinput", ref worldGenerationParameters.seed, 30);
                ImGui.SameLine();
                if (ButtonWithSound("New ##newseed", smallButtonSize)) { worldGenerationParameters.seed = random.Next().ToString(); }
                ImGui.Spacing();
                ImGui.Text("Size");
                ImGui.Combo("##worldSizes", ref selectedWorldSize, worldSizesNames, worldSizesNames.Length, worldSizesNames.Length);
                var worldSizeValue = worldSizeValues[worldSizes[selectedWorldSize]];
                ImGui.Text($@"{worldSizeValue.ToString()} by {worldSizeValue.ToString()}");
                ImGui.Text("Water lever");
                ImGui.Combo("##WaterType", ref selectedWaterLevel, waterLevelNames, waterLevelNames.Length, waterLevelNames.Length);

                ImGui.Text("Number of neighboring cities (0-6)");
                ImGui.InputInt("##NeighborsCount", ref worldGenerationParameters.neighboringCityCount);
                _restrainValue(ref worldGenerationParameters.neighboringCityCount, 0, 6);
                ImGui.Text("Current date");
                ImGui.Text(worldGenerationParameters.time.ToShortDateString());
                ImGui.SameLine();
                if (ButtonWithSound("New ##newtime", smallButtonSize)) { worldGenerationParameters.time = RandomDateTimeGenerator.RandomDate(new DateTime(2125, 1, 1), new DateTime(2175, 1, 1)); }

                worldGenerationParameters.worldSizeValue = worldSizeValue;
                worldGenerationParameters.worldSize = worldSizes[selectedWorldSize];
                worldGenerationParameters.waterLevel = waterLevel[selectedWaterLevel];

                if (ButtonWithSound("Generate", buttonSize, worldGenerationParameters.seed.Any()))
                {
                    game.Commander.EnqueueCommand(new GenerateWorldFileCommand(worldGenerationParameters));
                }
                if (ButtonWithSound("Exit", buttonSize)) { Action = WorldGenAction.Exit; }

                currentFiles = Directory.EnumerateFiles("Worlds").ToArray();
            }

            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.BeginChild("##currentItems", new Vector2((uiSize.X / 3 - 100), uiSize.Y - 50), true);
            {
                ImGui.BeginChild("##listChild");
                {
                    ImGui.Text("World files");

                    if (currentFiles != null && currentFiles.Any())
                    {                       
                        ImGui.ListBox("##currentItemsByType", ref currentFile, currentFiles, currentFiles.Length, currentFiles.Length);
                    }
                }
                ImGui.EndChild();
                if (ButtonWithSound("Save", buttonSize)) { Action = WorldGenAction.Exit; }
                if (ButtonWithSound("Load", buttonSize)) { Action = WorldGenAction.Exit; }
            }
            ImGui.EndChild();
           

            //}
            ImGui.PopFont();
            ImGui.End();
        }
	}
}

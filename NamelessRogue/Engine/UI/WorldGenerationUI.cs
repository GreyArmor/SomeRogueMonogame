using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public string description = "";
		public DateTime time;
		public WorldSizeName worldSize;
		public int worldSizeValue = 0;
		public WaterLevel waterLevel;
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

		System.Numerics.Vector2 menuPosition;
		System.Numerics.Vector2 buttonSpacing = new System.Numerics.Vector2(0, 5);
		System.Numerics.Vector2 buttonSize;
		System.Numerics.Vector2 shiftVector;
		System.Numerics.Vector2 sidebarSize;
        System.Numerics.Vector2 smallButtonSize;

        WorldGenerationParameters worldGenerationParameters;
		Random random;
        public WorldGenerationUI(NamelessGame game) : base(game)
		{
			buttonSize = new System.Numerics.Vector2(game.Settings.HudWidth, 50);
			smallButtonSize = new System.Numerics.Vector2(40);
            shiftVector = new System.Numerics.Vector2(0, buttonSpacing.Y + buttonSize.Y);
			sidebarSize = new System.Numerics.Vector2(0, uiSize.Y);
            random = new Random();
			
            worldSizeValues = new Dictionary<WorldSizeName, int>();
			worldSizeValues.Add(WorldSizeName.Tiny, 100);
            worldSizeValues.Add(WorldSizeName.Small, 250);
            worldSizeValues.Add(WorldSizeName.Medium, 500);
            worldSizeValues.Add(WorldSizeName.Big, 750);
            worldSizeValues.Add(WorldSizeName.Large, 1000);

			worldGenerationParameters = new WorldGenerationParameters();
            worldGenerationParameters.seed = random.Next().ToString();
            worldGenerationParameters.name = "Defaul World Name";
        }

		public override void DrawLayout()
		{
			menuPosition = new System.Numerics.Vector2(10, 0);
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
                ImGui.Text("Seed");
				ImGui.InputText("##seedinput", ref worldGenerationParameters.seed, 30);
				ImGui.SameLine();
                if (ButtonWithSound(" ", smallButtonSize)) { worldGenerationParameters.seed = random.Next().ToString(); }
                ImGui.Spacing();
				ImGui.Text("Size");
                ImGui.Combo("##worldSizes", ref selectedWorldSize, worldSizesNames, worldSizesNames.Length, worldSizesNames.Length);
                ImGui.Text("Water lever");
                ImGui.Combo("##WaterType", ref selectedWaterLevel, waterLevelNames, waterLevelNames.Length, waterLevelNames.Length);

                var worldSizeValue = worldSizeValues[worldSizes[selectedWorldSize]];
				ImGui.Text($@"{worldSizeValue.ToString()} by {worldSizeValue.ToString()}");

				worldGenerationParameters.worldSizeValue = worldSizeValue;
				worldGenerationParameters.worldSize = worldSizes[selectedWorldSize];
				worldGenerationParameters.waterLevel = waterLevel[selectedWaterLevel];

                worldGenerationParameters.time = new DateTime();

                if (ButtonWithSound("Generate", buttonSize, worldGenerationParameters.seed.Any())) {
					game.Commander.EnqueueCommand(new GenerateWorldFileCommand(worldGenerationParameters));
				}
				if (ButtonWithSound("Exit", buttonSize)) { Action = WorldGenAction.Exit; }
			}

			//ImGui.EndChild();
			//}
			ImGui.PopFont();
			ImGui.End();
		}
	}
}

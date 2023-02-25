using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.ViewModels
{

	public enum WorldBoardScreenAction
	{
		ReturnToGame,
		WorldMap,
		LocalMap,
		TerrainMode,
		RegionsMode,
		PoliticalMode,
		ArtifactMode,
	}
	public class MapScreenViewModel
	{
		private NamelessGame game;
		public WorldBoardScreenAction Mode { get; set; }
		public MapScreenViewModel(NamelessGame game)
		{
			this.game = game;
		}
	}
}


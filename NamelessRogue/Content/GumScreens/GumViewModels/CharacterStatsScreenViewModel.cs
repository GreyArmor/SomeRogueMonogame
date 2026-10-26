using Gum.Mvvm;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Screens;
using NamelessRogue.shell;
using System;
using System.Linq;

namespace NamelessRogue.Content.GumScreens.GumViewModels
{
	public class CharacterStatsScreenViewModel : ViewModel
	{
		internal CharacterStatsScreenViewModel(NamelessGame game, CharacterStatsScreen screen)
		{
			var stats = CombatSystem.GetAccumulatedStats(game.PlayerEntity);

			var labels = Enum.GetNames(typeof(DamageType)).ToArray();
			var values = labels.Select(x => (float)Random.Shared.Next(0, 100)).ToArray();

			var armorRadar = new RadarChart(labels);
			armorRadar.SetValues(values);
			screen.ArmorContainer.AddChild(armorRadar);

			values = labels.Select(x => (float)Random.Shared.Next(0, 100)).ToArray();
			var resRadar = new RadarChart(labels);
			resRadar.SetValues(values);
			screen.ResistancesContainer.AddChild(resRadar);
		}
	}
}

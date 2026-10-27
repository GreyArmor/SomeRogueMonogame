using Gum.Mvvm;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Screens;
using NamelessRogue.shell;
using Newtonsoft.Json.Bson;
using System;
using System.Linq;

namespace NamelessRogue.Content.GumScreens.GumViewModels
{
	public class CharacterStatsScreenViewModel : ViewModel
	{
		private readonly NamelessGame game;
		private readonly CharacterStatsScreen screen;
		//stricttly speaking, these should not me in a view model... but whatever
		private RadarChart armorRadar;
		private RadarChart resRadar;
		private RadarChart damageRadar;

		internal CharacterStatsScreenViewModel(NamelessGame game, CharacterStatsScreen screen)
		{			
			var labels = Enum.GetNames(typeof(DamageType)).ToArray();	
			armorRadar = new RadarChart("Armor", labels);
		
			screen.ArmorContainer.AddChild(armorRadar);
			resRadar = new RadarChart("Resistances", labels);			
			screen.ResistancesContainer.AddChild(resRadar);

			damageRadar = new RadarChart("Damage", labels);
			screen.DamageContainer.AddChild(damageRadar);
			this.game = game;
			this.screen = screen;

			UpdateValues();
		}

		public void UpdateValues()
		{

			var characterComponent = game.PlayerEntity.GetComponentOfType<Character>();
			var description = game.PlayerEntity.GetComponentOfType<Description>();
			var stats = CombatSystem.GetAccumulatedStats(game.PlayerEntity);

			var damageTypes = Enum.GetValues(typeof(DamageType));
			var labels = Enum.GetNames(typeof(DamageType)).ToArray();
			var values = labels.Select(x => 0f).ToArray();

			for (var i = 0; i < values.Length; i++)
			{
				var damageType = (DamageType)damageTypes.GetValue(i);
				var armor = stats.Armor.FirstOrDefault(x => x.DamageType == damageType);
				values[i] = armor == null ? 0 : armor.Value.Value;
			}
			armorRadar.SetValues(values);

			for (var i = 0; i < values.Length; i++)
			{
				var damageType = (DamageType)damageTypes.GetValue(i);
				var res = stats.Resistances.FirstOrDefault(x => x.DamageType == damageType);
				values[i] = res == null ? 0 : res.Value.Value;
			}
			resRadar.SetValues(values);

			for (var i = 0; i < values.Length; i++)
			{
				var damageType = (DamageType)damageTypes.GetValue(i);
				var damage = stats.WeaponStats.FirstOrDefault(x => x.DamageType == damageType);
				values[i] = damage == null ? 0 : damage.MaximumDamage;
			}
			damageRadar.SetValues(values);

			screen.CharacterNameLabel.Text = description.Name;
			screen.HealthLabel.Text = $@"Health:{stats.Health.Value}";
			screen.EnergyLabel.Text = $@"Energy:{stats.Energy.Value}";
			screen.MoneyLabel.Text = $@"$:{stats.Money}";
			screen.MoveSpeedLabel.Text = $@"Movement speed:{stats.MovementSpeed.Value}";
			screen.NeuralLoadLabel.Text = $@"Neural load:{stats.NeuralLoad.Value}";
			screen.VisionRangeLabel.Text = $@"Vision range:{stats.VisionRange.Value}";
		}
	}
}

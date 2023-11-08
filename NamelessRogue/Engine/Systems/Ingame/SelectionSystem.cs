using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
	internal class SelectionSystem : BaseSystem
	{
		public override HashSet<Type> Signature { get; } = new HashSet<Type>();
			
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			var player = namelessGame.PlayerEntity;
			var selectionData = player.GetComponentOfType<SelectionData>();

			if (selectionData.SelectionState == SelectionState.End)
			{
				//select units here

				var camera = namelessGame.PlayerEntity.GetComponentOfType<Camera3D>();

				var viewport = namelessGame.GraphicsDevice.Viewport;
				var vecStart = selectionData.SelectionStart.ToVector2();
				var vecEnd = selectionData.SelectionEnd.ToVector2();
				var start = new Vector3(vecStart, 0);
				var end = new Vector3(vecEnd, 0);

				var box = BoundingBox.CreateFromPoints(new List<Vector3>(){start,end});

				var units = namelessGame.GetEntitiesByComponentClass<GroupTag>();

				List<IEntity> selectedUnits = new List<IEntity>();

				foreach (var unit in units)
				{
					var position = unit.GetComponentOfType<Position3D>();
					var screenPos = viewport.Project(position.WorldPosition.Value, camera.Projection, camera.View, Matrix.Identity);
					screenPos.Z = 0;
					if (box.Contains(screenPos) == ContainmentType.Contains || box.Contains(screenPos) == ContainmentType.Intersects)
					{
						selectedUnits.Add(unit);
					}
				}

				var groups = selectedUnits.Select(x => x.GetComponentOfType<GroupTag>().GroupId).GroupBy(tag=>tag);

				var selectedGroups = namelessGame.PlayerEntity.GetComponentOfType<SelectedUnitsData>();

				if (groups.Any())
				{
					selectedGroups.SelectedGroups.Clear();
					foreach (var group in groups)
					{	
						selectedGroups.SelectedGroups.Add(group.Key);
					}
				}

				selectionData.SelectionState = SelectionState.None;
				selectionData.SelectionStart = Point.Zero;
				selectionData.SelectionEnd = Point.Zero;
			}

			while (namelessGame.Commander.DequeueCommand(out SelectionCommand command))
			{
				switch (command.State)
				{
					case SelectionState.Start:
						selectionData.SelectionStart = command.CursorPosition;
						selectionData.SelectionState = SelectionState.Start;
						break;
					case SelectionState.Drag:
						selectionData.SelectionEnd = command.CursorPosition;
						selectionData.SelectionState = SelectionState.Drag;
						break;
					case SelectionState.End:
						selectionData.SelectionEnd = command.CursorPosition;
						selectionData.SelectionState = SelectionState.End;
						break;
					default:
						selectionData.SelectionState = SelectionState.None;
						break;
				}
			}
		}
	}
}

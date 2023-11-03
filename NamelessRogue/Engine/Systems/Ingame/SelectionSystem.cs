using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
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

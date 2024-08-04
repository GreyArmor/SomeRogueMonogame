
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Systems._3DView
{
	public class UpdateChunkCommand : ICommand
	{
		public Point ChunkToUpdate { get; private set; }
		public UpdateChunkCommand(Point chunktoUpdate)
		{
			ChunkToUpdate = chunktoUpdate;
		}

	}
}

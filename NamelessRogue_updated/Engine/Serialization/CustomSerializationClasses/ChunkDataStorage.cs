using FlatSharp.Attributes;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class ChunkDataStorage : IStorage<ChunkData>
	{
		public void FillFrom(ChunkData component)
		{
			throw new NotImplementedException();
		}

		public void FillTo(ChunkData component)
		{
			throw new NotImplementedException();
		}

		public static implicit operator ChunkData(ChunkDataStorage thisType)
		{
			ChunkData result = new ChunkData();
			thisType.FillTo(result);
			return result;
		}

		public static implicit operator ChunkDataStorage(ChunkData component)
		{
			ChunkDataStorage result = new ChunkDataStorage();
			result.FillFrom(result);
			return result;
		}

	}
}

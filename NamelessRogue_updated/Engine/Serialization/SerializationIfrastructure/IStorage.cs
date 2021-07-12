using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.SerializationIfrastructure
{
	public interface IStorage<T>
	{
		public void FillTo(T component);
		public void FillFrom(T component);
	}
}

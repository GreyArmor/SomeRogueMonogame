using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Infrastructure
{
	public class Commander : Component
	{
		Dictionary<Type, Queue<ICommand>> Commands { get; } = new Dictionary<Type, Queue<ICommand>>();

		public void EnqueueCommand(ICommand command)
		{
			Queue<ICommand> value;
			if (Commands.TryGetValue(command.GetType(), out value))
			{
				value.Enqueue(command);
			}
			else
			{
				value = new Queue<ICommand>();
				value.Enqueue(command);
			}
		}

		public CommandType DequeueCommand<CommandType>() where CommandType : ICommand
		{
			Queue<ICommand> value;
			Commands.TryGetValue(typeof(CommandType), out value);

			if (value == null || value.Count == 0)
			{
				return default; 
			}

			return (CommandType)value.Dequeue();
		}

		public bool DequeueCommand<CommandType>(out CommandType command) where CommandType : ICommand
		{
			Queue<ICommand> value;
			Commands.TryGetValue(typeof(CommandType), out value);

			if (value == null || value.Count==0)
			{
				command = default;
				return false;
			}
			command = (CommandType)value.Dequeue();
			return true;
		}



		public override IComponent Clone()
		{
			throw new NotImplementedException();
		}
	}
}

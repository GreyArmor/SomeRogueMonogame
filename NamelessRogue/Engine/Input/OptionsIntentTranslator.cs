using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Input
{
	public class OptionsIntentTranslator : IKeyIntentTraslator
	{
		public virtual List<Intent> Translate(Keys[] keyCodes, char lastCommand, MouseState mouseState)
		{
			List<Intent> result = new List<Intent>();
			return result;
		}
	}

	public class CharacterstatsScreenItentTranslator : IKeyIntentTraslator
	{
		public virtual List<Intent> Translate(Keys[] keyCodes, char lastCommand, MouseState mouseState)
		{
			List<Intent> result = new List<Intent>();
			return result;
		}
	}
}

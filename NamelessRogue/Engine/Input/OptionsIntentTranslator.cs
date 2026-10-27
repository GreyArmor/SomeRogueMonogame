using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

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
			for (int i = 0; i < keyCodes.Length; i++)
			{
				var keyCode = keyCodes[i];
				Intent intent = new Intent(keyCodes.ToList(), lastCommand);
				result.Add(intent);
				switch (keyCode)
				{
					case Keys.Escape:
						intent.Intention = IntentEnum.Escape;
						break;
					default:
						break;
						
				}
			}
			return result;
		}
	}
}

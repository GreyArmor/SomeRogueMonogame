 



/**
 * Created by Admin on 13.06.2017.
 */

using System;

namespace NamelessRogue.Engine.Engine.Components.UI
{
    public class Description : Component {
        public Description(String name, String text)
        {
            Name = name;
            Text = text;
        }
        public String Name;
        public String Text;

        public override IComponent Clone()
        {
            return new Description(Name, Text);
        }
    }
}

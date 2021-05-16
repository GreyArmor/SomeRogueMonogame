 



/**
 * Created by Admin on 13.06.2017.
 */

using System;

namespace NamelessRogue.Engine.Components.UI
{
    public class Description : Component {
        public Description(String name = "", String text = "")
        {
            Name = name;
            Text = text;
        }
        public String Name { get; set; }
        public String Text { get; set; }

        public override IComponent Clone()
        {
            return new Description(Name, Text);
        }
    }
}

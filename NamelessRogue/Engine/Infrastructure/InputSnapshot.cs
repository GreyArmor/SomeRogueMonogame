using System.Collections.Generic;

namespace NamelessRogue.Engine.Infrastructure
{
    public class InputSnapshot
    {
        public IEnumerable<object> KeyEvents { get; internal set; }
    }
}
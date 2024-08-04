
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;
namespace NamelessRogue.Engine.Systems.PickUpItems
{
    public class WorldGenSystem : BaseSystem
    {
        public WorldGenSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }
        public override HashSet<Type> Signature { get; }
        public bool InventoryNeedsUpdate { get; private set; }

        public override void Update(GameTime gameTime, NamelessGame game)
        {

            switch (UIController.Instance.WorldGenScreen.Action)
            {
                case WorldGenAction.Exit:
                    game.ContextToSwitch = ContextFactory.GetMainMenuContext(game);
                    break;
                case WorldGenAction.Generate:
                    break;
                default:
                    break;
            }
            UIController.Instance.WorldGenScreen.Action = WorldGenAction.None;
        }


    }
}

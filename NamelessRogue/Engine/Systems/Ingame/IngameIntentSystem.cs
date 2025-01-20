using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class IngameIntentSystem : BaseSystem
    {
        IngameIntentSystemSubProcessor currentSubProcessor;
        PlayerMovementSubProcessor playerMovementSubProcessor = new PlayerMovementSubProcessor();
        WeaponTargetingSubProcessor weaponTargetingSubProcessor = new WeaponTargetingSubProcessor();
        AbilityTargetingSubProcessor abilityTargetingSubProcessor = new AbilityTargetingSubProcessor();
        OptionsPopupSubprocessor OptionsPopupSubprocessor = new OptionsPopupSubprocessor();

        public IngameIntentSystem()
        {
            currentSubProcessor = playerMovementSubProcessor;
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }

        public override HashSet<Type> Signature { get; }

        public List<IntentEnum> SingleKeyPressIntents { get; set; } = new List<IntentEnum>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            if (!namelessGame.IsActive) { return; }

            while (namelessGame.Commander.DequeueCommand(out IngameIntentSystemModeSwitchCommand command))
            {
                switch (command.Mode)
                {
                    case IngameIntentSystemMode.PlayerMovement:
                        currentSubProcessor = playerMovementSubProcessor;
                        break;
                    case IngameIntentSystemMode.FireWeapon:
                        currentSubProcessor = weaponTargetingSubProcessor;
                        break;
                    case IngameIntentSystemMode.QuickBarAiming:
                        currentSubProcessor = abilityTargetingSubProcessor; 
                        //TODO: refactor
                        abilityTargetingSubProcessor.AbilityIndex = (int)command.CommandData;
                        break;
                    case IngameIntentSystemMode.OptionsPopup:
                        currentSubProcessor = OptionsPopupSubprocessor;
                        OptionsPopupSubprocessor.OptionItems = (List<Abstraction.IEntity>)command.CommandData;

                        break;
                }
            }

            var playerEntity = namelessGame.PlayerEntity;
            InputComponent inputComponent = playerEntity.GetComponentOfType<InputComponent>();
            if (inputComponent != null && !inputComponent.IsDelayed)
            {
                foreach (Intent intent in inputComponent.Intents)
                {
                    if (SingleKeyPressIntents.Contains(intent.Intention))
                    {
                        continue;
                    }

                    currentSubProcessor.Process(namelessGame, this, intent);
                }

                foreach (var intent in SingleKeyPressIntents.ToList())
                {
                    if (!inputComponent.Intents.Any(x => x.Intention == intent))
                    {
                        SingleKeyPressIntents.Remove(intent);
                    }
                }
                inputComponent.Intents.Clear();
            }
        }
    }
}

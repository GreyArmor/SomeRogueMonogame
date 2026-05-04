// TargetModeValues.cs
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using System;
using System.Collections.Generic;

namespace NamelessRogueDataEditor.ViewModels
{
    public static class TargetModeValues
    {
        public static Array All => Enum.GetValues(typeof(TargetMode));
    }
    public static class ActivationModeValues
    {
        public static Array All => Enum.GetValues(typeof(ActivationMode));
    }
    public static class AbilityActionValues
    {
        public static Array All => Enum.GetValues(typeof(AbilityAction));
    }

    public static class SlotValues
    {
        public static Array All => Enum.GetValues(typeof(Slot));
    }
}

//Code for CharacterStatsScreen
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using NamelessRogue.Components.Neon.Controls;
using RenderingLibrary.Graphics;
using System.Linq;
namespace NamelessRogue.Screens;
partial class CharacterStatsScreen : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("CharacterStatsScreen") ?? throw new System.InvalidOperationException("Could not find an element named CharacterStatsScreen - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new CharacterStatsScreen(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(CharacterStatsScreen)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("CharacterStatsScreen", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ContainerRuntime ContainerInstance { get; protected set; }
    public ContainerRuntime LeftContainer { get; protected set; }
    public ContainerRuntime RightContainer { get; protected set; }
    public ContainerRuntime ArmorContainer { get; protected set; }
    public ContainerRuntime ResistancesContainer { get; protected set; }
    public StackPanel StackPanelInstance { get; protected set; }
    public Label HealthLabel { get; protected set; }
    public Label EnergyLabel { get; protected set; }

    public CharacterStatsScreen(InteractiveGue visual) : base(visual)
    {
    }
    public CharacterStatsScreen()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ContainerInstance = this.Visual?.GetGraphicalUiElementByName("ContainerInstance") as global::Gum.GueDeriving.ContainerRuntime;
        LeftContainer = this.Visual?.GetGraphicalUiElementByName("LeftContainer") as global::Gum.GueDeriving.ContainerRuntime;
        RightContainer = this.Visual?.GetGraphicalUiElementByName("RightContainer") as global::Gum.GueDeriving.ContainerRuntime;
        ArmorContainer = this.Visual?.GetGraphicalUiElementByName("ArmorContainer") as global::Gum.GueDeriving.ContainerRuntime;
        ResistancesContainer = this.Visual?.GetGraphicalUiElementByName("ResistancesContainer") as global::Gum.GueDeriving.ContainerRuntime;
        StackPanelInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<StackPanel>(this.Visual,"StackPanelInstance");
        HealthLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"HealthLabel");
        EnergyLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"EnergyLabel");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

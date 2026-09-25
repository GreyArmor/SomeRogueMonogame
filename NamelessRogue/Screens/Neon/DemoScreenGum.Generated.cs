//Code for Neon/DemoScreenGum
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
namespace NamelessRogue.Screens.Neon;
partial class DemoScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Neon/DemoScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named Neon/DemoScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new DemoScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(DemoScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Neon/DemoScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ContainerRuntime DemoSettingsMenu { get; protected set; }
    public RectangleRuntime Background { get; protected set; }
    public ContainerRuntime MenuTitle { get; protected set; }
    public ContainerRuntime MenuTitle1 { get; protected set; }
    public ContainerRuntime MenuItems { get; protected set; }
    public TextRuntime TitleText { get; protected set; }
    public TextRuntime TitleText1 { get; protected set; }
    public Label ResolutionLabel { get; protected set; }
    public ListBox ResolutionBox { get; protected set; }
    public Button DetectResolutionsButton { get; protected set; }
    public Button ShowDialogButton { get; protected set; }
    public Button ShowToastButton { get; protected set; }
    public CheckBox FullScreenCheckbox { get; protected set; }
    public Label MusicLabel { get; protected set; }
    public Slider MusicSlider { get; protected set; }
    public Label SoundLabel { get; protected set; }
    public Slider SoundSlider { get; protected set; }
    public Label ControlLabel { get; protected set; }
    public RadioButton RadioButtonInstance { get; protected set; }
    public RadioButton RadioButtonInstance1 { get; protected set; }
    public RadioButton RadioButtonInstance2 { get; protected set; }
    public Label DifficultyLabel { get; protected set; }
    public RectangleRuntime Background1 { get; protected set; }
    public ComboBox ComboBoxInstance { get; protected set; }
    public ContainerRuntime ButtonContainer { get; protected set; }
    public Button ButtonConfirmInstance { get; protected set; }
    public Button ButtonDenyInstance { get; protected set; }
    public ContainerRuntime DemoDialog { get; protected set; }
    public ContainerRuntime MarginContainer { get; protected set; }
    public Label LabelInstance { get; protected set; }
    public TextBox TextBoxInstance { get; protected set; }
    public PasswordBox TextBoxInstance1 { get; protected set; }
    public TextBox MultiLineTextBox { get; protected set; }
    public DialogBox DialogBoxInstance { get; protected set; }
    public Window WindowStandardInstance { get; protected set; }
    public Label LabelInstance1 { get; protected set; }

    public DemoScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public DemoScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        DemoSettingsMenu = this.Visual?.GetGraphicalUiElementByName("DemoSettingsMenu") as global::Gum.GueDeriving.ContainerRuntime;
        Background = this.Visual?.GetGraphicalUiElementByName("Background") as global::Gum.GueDeriving.RectangleRuntime;
        MenuTitle = this.Visual?.GetGraphicalUiElementByName("MenuTitle") as global::Gum.GueDeriving.ContainerRuntime;
        MenuTitle1 = this.Visual?.GetGraphicalUiElementByName("MenuTitle1") as global::Gum.GueDeriving.ContainerRuntime;
        MenuItems = this.Visual?.GetGraphicalUiElementByName("MenuItems") as global::Gum.GueDeriving.ContainerRuntime;
        TitleText = this.Visual?.GetGraphicalUiElementByName("TitleText") as global::Gum.GueDeriving.TextRuntime;
        TitleText1 = this.Visual?.GetGraphicalUiElementByName("TitleText1") as global::Gum.GueDeriving.TextRuntime;
        ResolutionLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"ResolutionLabel");
        ResolutionBox = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ListBox>(this.Visual,"ResolutionBox");
        DetectResolutionsButton = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Button>(this.Visual,"DetectResolutionsButton");
        ShowDialogButton = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Button>(this.Visual,"ShowDialogButton");
        ShowToastButton = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Button>(this.Visual,"ShowToastButton");
        FullScreenCheckbox = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<CheckBox>(this.Visual,"FullScreenCheckbox");
        MusicLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"MusicLabel");
        MusicSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Slider>(this.Visual,"MusicSlider");
        SoundLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"SoundLabel");
        SoundSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Slider>(this.Visual,"SoundSlider");
        ControlLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"ControlLabel");
        RadioButtonInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<RadioButton>(this.Visual,"RadioButtonInstance");
        RadioButtonInstance1 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<RadioButton>(this.Visual,"RadioButtonInstance1");
        RadioButtonInstance2 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<RadioButton>(this.Visual,"RadioButtonInstance2");
        DifficultyLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"DifficultyLabel");
        Background1 = this.Visual?.GetGraphicalUiElementByName("Background1") as global::Gum.GueDeriving.RectangleRuntime;
        ComboBoxInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ComboBox>(this.Visual,"ComboBoxInstance");
        ButtonContainer = this.Visual?.GetGraphicalUiElementByName("ButtonContainer") as global::Gum.GueDeriving.ContainerRuntime;
        ButtonConfirmInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Button>(this.Visual,"ButtonConfirmInstance");
        ButtonDenyInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Button>(this.Visual,"ButtonDenyInstance");
        DemoDialog = this.Visual?.GetGraphicalUiElementByName("DemoDialog") as global::Gum.GueDeriving.ContainerRuntime;
        MarginContainer = this.Visual?.GetGraphicalUiElementByName("MarginContainer") as global::Gum.GueDeriving.ContainerRuntime;
        LabelInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"LabelInstance");
        TextBoxInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<TextBox>(this.Visual,"TextBoxInstance");
        TextBoxInstance1 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<PasswordBox>(this.Visual,"TextBoxInstance1");
        MultiLineTextBox = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<TextBox>(this.Visual,"MultiLineTextBox");
        DialogBoxInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<DialogBox>(this.Visual,"DialogBoxInstance");
        WindowStandardInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Window>(this.Visual,"WindowStandardInstance");
        LabelInstance1 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"LabelInstance1");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

//Code for Neon/Controls/RadioButton (Container)
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary.Graphics;
using System.Linq;
namespace NamelessRogue.Components.Neon.Controls;
partial class RadioButton : global::Gum.Forms.Controls.RadioButton
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Neon/Controls/RadioButton") ?? throw new System.InvalidOperationException("Could not find an element named Neon/Controls/RadioButton - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new RadioButton(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(RadioButton)] = template;
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(global::Gum.Forms.Controls.RadioButton)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Neon/Controls/RadioButton", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum RadioButtonCategory
    {
        EnabledOn,
        EnabledOff,
        DisabledOn,
        DisabledOff,
        HighlightedOn,
        HighlightedOff,
        PushedOn,
        PushedOff,
        FocusedOn,
        FocusedOff,
        HighlightedFocusedOn,
        HighlightedFocusedOff,
        DisabledFocusedOn,
        DisabledFocusedOff,
    }

    RadioButtonCategory? _radioButtonCategoryState;
    public RadioButtonCategory? RadioButtonCategoryState
    {
        get => _radioButtonCategoryState;
        set
        {
            _radioButtonCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("RadioButtonCategory"))
                {
                    var category = Visual.Categories["RadioButtonCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "RadioButtonCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public CircleRuntime RadioBackground { get; protected set; }
    public CircleRuntime Radio { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }
    public CircleRuntime FocusedIndicator { get; protected set; }


    public RadioButton(InteractiveGue visual) : base(visual)
    {
    }
    public RadioButton()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        RadioBackground = this.Visual?.GetGraphicalUiElementByName("RadioBackground") as global::Gum.GueDeriving.CircleRuntime;
        Radio = this.Visual?.GetGraphicalUiElementByName("Radio") as global::Gum.GueDeriving.CircleRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.CircleRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

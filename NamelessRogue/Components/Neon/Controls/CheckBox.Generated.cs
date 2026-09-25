//Code for Neon/Controls/CheckBox (Container)
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
partial class CheckBox : global::Gum.Forms.Controls.CheckBox
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Neon/Controls/CheckBox") ?? throw new System.InvalidOperationException("Could not find an element named Neon/Controls/CheckBox - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new CheckBox(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(CheckBox)] = template;
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(global::Gum.Forms.Controls.CheckBox)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Neon/Controls/CheckBox", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum CheckBoxCategory
    {
        EnabledOff,
        HighlightedOff,
        FocusedOff,
        HighlightedFocusedOff,
        PushedOff,
        DisabledOff,
        DisabledFocusedOff,
        EnabledOn,
        HighlightedOn,
        FocusedOn,
        HighlightedFocusedOn,
        PushedOn,
        DisabledOn,
        DisabledFocusedOn,
        EnabledIndeterminate,
        HighlightedIndeterminate,
        FocusedIndeterminate,
        HighlightedFocusedIndeterminate,
        PushedIndeterminate,
        DisabledIndeterminate,
        DisabledFocusedIndeterminate,
    }

    CheckBoxCategory? _checkBoxCategoryState;
    public CheckBoxCategory? CheckBoxCategoryState
    {
        get => _checkBoxCategoryState;
        set
        {
            _checkBoxCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("CheckBoxCategory"))
                {
                    var category = Visual.Categories["CheckBoxCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "CheckBoxCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public RectangleRuntime CheckBoxBackground { get; protected set; }
    public RectangleRuntime BoxBorder { get; protected set; }
    public TextRuntime InnerCheck { get; protected set; }
    public RectangleRuntime DashIndicator { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }
    public RectangleRuntime FocusedIndicator { get; protected set; }


    public CheckBox(InteractiveGue visual) : base(visual)
    {
    }
    public CheckBox()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        CheckBoxBackground = this.Visual?.GetGraphicalUiElementByName("CheckBoxBackground") as global::Gum.GueDeriving.RectangleRuntime;
        BoxBorder = this.Visual?.GetGraphicalUiElementByName("BoxBorder") as global::Gum.GueDeriving.RectangleRuntime;
        InnerCheck = this.Visual?.GetGraphicalUiElementByName("InnerCheck") as global::Gum.GueDeriving.TextRuntime;
        DashIndicator = this.Visual?.GetGraphicalUiElementByName("DashIndicator") as global::Gum.GueDeriving.RectangleRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.RectangleRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

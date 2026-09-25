//Code for Neon/Controls/ComboBox (Container)
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
namespace NamelessRogue.Components.Neon.Controls;
partial class ComboBox : global::Gum.Forms.Controls.ComboBox
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Neon/Controls/ComboBox") ?? throw new System.InvalidOperationException("Could not find an element named Neon/Controls/ComboBox - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ComboBox(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ComboBox)] = template;
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(global::Gum.Forms.Controls.ComboBox)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Neon/Controls/ComboBox", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum ComboBoxCategory
    {
        Enabled,
        Disabled,
        Highlighted,
        Pushed,
        HighlightedFocused,
        Focused,
        DisabledFocused,
    }

    ComboBoxCategory? _comboBoxCategoryState;
    public ComboBoxCategory? ComboBoxCategoryState
    {
        get => _comboBoxCategoryState;
        set
        {
            _comboBoxCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("ComboBoxCategory"))
                {
                    var category = Visual.Categories["ComboBoxCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "ComboBoxCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public RectangleRuntime Background { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }
    public ListBox ListBoxInstance { get; protected set; }
    public TextRuntime IconInstance { get; protected set; }
    public RectangleRuntime BorderInstance { get; protected set; }
    public RectangleRuntime FocusedIndicator { get; protected set; }

    public ComboBox(InteractiveGue visual) : base(visual)
    {
    }
    public ComboBox()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Background = this.Visual?.GetGraphicalUiElementByName("Background") as global::Gum.GueDeriving.RectangleRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        ListBoxInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ListBox>(this.Visual,"ListBoxInstance");
        IconInstance = this.Visual?.GetGraphicalUiElementByName("IconInstance") as global::Gum.GueDeriving.TextRuntime;
        BorderInstance = this.Visual?.GetGraphicalUiElementByName("BorderInstance") as global::Gum.GueDeriving.RectangleRuntime;
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.RectangleRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

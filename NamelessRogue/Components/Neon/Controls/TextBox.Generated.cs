//Code for Neon/Controls/TextBox (Container)
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
partial class TextBox : global::Gum.Forms.Controls.TextBox
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Neon/Controls/TextBox") ?? throw new System.InvalidOperationException("Could not find an element named Neon/Controls/TextBox - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new TextBox(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(TextBox)] = template;
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(global::Gum.Forms.Controls.TextBox)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Neon/Controls/TextBox", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum TextBoxCategory
    {
        Enabled,
        Highlighted,
        Focused,
        Disabled,
    }
    public enum LineModeCategory
    {
        Single,
        Multi,
    }

    TextBoxCategory? _textBoxCategoryState;
    public TextBoxCategory? TextBoxCategoryState
    {
        get => _textBoxCategoryState;
        set
        {
            _textBoxCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("TextBoxCategory"))
                {
                    var category = Visual.Categories["TextBoxCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "TextBoxCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }

    LineModeCategory? _lineModeCategoryState;
    public LineModeCategory? LineModeCategoryState
    {
        get => _lineModeCategoryState;
        set
        {
            _lineModeCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("LineModeCategory"))
                {
                    var category = Visual.Categories["LineModeCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "LineModeCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public RectangleRuntime FocusedIndicator { get; protected set; }
    public RectangleRuntime Background { get; protected set; }
    public ContainerRuntime ClipContainer { get; protected set; }
    public RectangleRuntime SelectionInstance { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }
    public TextRuntime PlaceholderTextInstance { get; protected set; }
    public RectangleRuntime CaretInstance { get; protected set; }
    public RectangleRuntime BorderInstance { get; protected set; }




    public TextBox(InteractiveGue visual) : base(visual)
    {
    }
    public TextBox()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.RectangleRuntime;
        Background = this.Visual?.GetGraphicalUiElementByName("Background") as global::Gum.GueDeriving.RectangleRuntime;
        ClipContainer = this.Visual?.GetGraphicalUiElementByName("ClipContainer") as global::Gum.GueDeriving.ContainerRuntime;
        SelectionInstance = this.Visual?.GetGraphicalUiElementByName("SelectionInstance") as global::Gum.GueDeriving.RectangleRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        PlaceholderTextInstance = this.Visual?.GetGraphicalUiElementByName("PlaceholderTextInstance") as global::Gum.GueDeriving.TextRuntime;
        CaretInstance = this.Visual?.GetGraphicalUiElementByName("CaretInstance") as global::Gum.GueDeriving.RectangleRuntime;
        BorderInstance = this.Visual?.GetGraphicalUiElementByName("BorderInstance") as global::Gum.GueDeriving.RectangleRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

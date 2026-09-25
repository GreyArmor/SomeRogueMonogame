//Code for Neon/Controls/Label (Text)
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
partial class Label : global::Gum.Forms.Controls.Label
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.TextRuntime();
            var element = ObjectFinder.Self.GetElementSave("Neon/Controls/Label") ?? throw new System.InvalidOperationException("Could not find an element named Neon/Controls/Label - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new Label(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(Label)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Neon/Controls/Label", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum TextCategory
    {
        Normal,
        Strong,
    }

    TextCategory? _textCategoryState;
    public TextCategory? TextCategoryState
    {
        get => _textCategoryState;
        set
        {
            _textCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("TextCategory"))
                {
                    var category = Visual.Categories["TextCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "TextCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }

    public Label(InteractiveGue visual) : base(visual)
    {
    }
    public Label()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

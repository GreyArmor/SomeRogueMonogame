//Code for Neon/Controls/Window (Container)
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
partial class Window : global::Gum.Forms.Window
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Neon/Controls/Window") ?? throw new System.InvalidOperationException("Could not find an element named Neon/Controls/Window - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new Window(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(Window)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Neon/Controls/Window", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public RectangleRuntime Background { get; protected set; }
    public Panel InnerPanelInstance { get; protected set; }
    public Panel TitleBarInstance { get; protected set; }
    public RectangleRuntime TitleBarFill { get; protected set; }
    public RectangleRuntime TitleBarSeparator { get; protected set; }
    public Panel BorderTopLeftInstance { get; protected set; }
    public Panel BorderTopRightInstance { get; protected set; }
    public Panel BorderBottomLeftInstance { get; protected set; }
    public Panel BorderBottomRightInstance { get; protected set; }
    public Panel BorderTopInstance { get; protected set; }
    public Panel BorderBottomInstance { get; protected set; }
    public Panel BorderLeftInstance { get; protected set; }
    public Panel BorderRightInstance { get; protected set; }

    public Window(InteractiveGue visual) : base(visual)
    {
    }
    public Window()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Background = this.Visual?.GetGraphicalUiElementByName("Background") as global::Gum.GueDeriving.RectangleRuntime;
        InnerPanelInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"InnerPanelInstance");
        TitleBarInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"TitleBarInstance");
        TitleBarFill = this.Visual?.GetGraphicalUiElementByName("TitleBarFill") as global::Gum.GueDeriving.RectangleRuntime;
        TitleBarSeparator = this.Visual?.GetGraphicalUiElementByName("TitleBarSeparator") as global::Gum.GueDeriving.RectangleRuntime;
        BorderTopLeftInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderTopLeftInstance");
        BorderTopRightInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderTopRightInstance");
        BorderBottomLeftInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderBottomLeftInstance");
        BorderBottomRightInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderBottomRightInstance");
        BorderTopInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderTopInstance");
        BorderBottomInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderBottomInstance");
        BorderLeftInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderLeftInstance");
        BorderRightInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Panel>(this.Visual,"BorderRightInstance");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}

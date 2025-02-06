using ImGuiNET;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.UI
{
    internal class DialogScreen : BaseScreen
    {

        public string NpcName { get; set; } = "";
        public int CurrentSelectedOptionIndex;
        public DialogData CurrentDialogData { get; set; } = null;
        public DialogScreen(NamelessGame game) : base(game)
        {
        }

        public override void DrawLayout()
        {
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
            {

            }
            ImGui.End();
        }
    }
}

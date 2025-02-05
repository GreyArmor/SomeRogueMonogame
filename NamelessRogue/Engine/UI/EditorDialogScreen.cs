using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Numerics;
using System.IO;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.Editor;
using System.Xml.Serialization;
using static System.Windows.Forms.Design.AxImporter;
using System.Data;
using RogueSharp;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using Microsoft.CodeAnalysis.Options;

namespace NamelessRogue.Engine.UI
{

    public enum EditorDialogScreenAction
    {
        None,
        Exit,
    }
    public class EditorDialogScreen : BaseScreen
    {
        Vector2 buttonSize = new Vector2(200, 50);
        private string[] currentFiles;
        private string[] currentFilesNames;
        private string contentDialogDirectoryPath;
        private string contentDirectoryPath;
        private int currentSelectedFile;
        private string name = "";
        private string responseText = "";

        DialogDataViewModel data = new DialogDataViewModel();
        private string currentFilePath;

        public EditorDialogScreenAction Action { get; set; } = EditorDialogScreenAction.None;
        public EditorDialogScreen(NamelessGame game) : base(game)
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentDialogDirectoryPath = projectDirectory + "\\Content\\GameObjects\\Dialogs\\";
            contentDirectoryPath = projectDirectory + "\\Content\\";
#else
            contentDialogDirectoryPath = workingDirectory +  "\\Content\\GameObjects\\Dialogs";
            contentDirectoryPath = workingDirectory + "\\Content\\";
#endif
        }

        public override void DrawLayout()
        {
            var fieldsSizeX = (uiSize.X / 3) * 2;

            if (!Directory.Exists(contentDialogDirectoryPath))
            {
                Directory.CreateDirectory(contentDialogDirectoryPath);
            }

            currentFiles = Directory.GetFiles(contentDialogDirectoryPath);
            currentFilesNames = currentFiles.Select(path => System.IO.Path.GetFileName(path)).ToArray();

            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.AlwaysAutoResize);
            {

                ImGui.BeginChild("##fields", new Vector2(fieldsSizeX, uiSize.Y), false, ImGuiWindowFlags.None);
                {

                    if (ButtonWithSound("Save", buttonSize) && name.Any())
                    {
                        Save();
                    }
                    ImGui.SameLine();
                    if (ButtonWithSound("Load", buttonSize) && currentFiles.Any())
                    {
                        Load();
                    }

                    ImGui.SameLine();
                    if (ButtonWithSound("Delete", buttonSize))
                    {
                       if (currentFiles.Any())
                       {
                           File.Delete(currentFiles[currentSelectedFile]);
                            currentSelectedFile = 0;
                       }
                    }
                    ImGui.SameLine();
                    if (ButtonWithSound("Add new dialog", buttonSize) && currentFiles.Any())
                    {
                        name = "";                            
                        data = new DialogDataViewModel();
                    }


                    ImGui.SameLine();
                    if (ButtonWithSound("Back", buttonSize))
                    {
                        Action = EditorDialogScreenAction.Exit;
                    }

                    ImGui.Text("Name");
                    ImGui.SetNextItemWidth(fieldsSizeX);
                    ImGui.InputText("##Name", ref name, 128);

                    DrawDialogTree(data, "Root");


                }
                ImGui.EndChild();

            }
            ImGui.SameLine();

            ImGui.BeginChild("##currentItems", new Vector2((uiSize.X / 3 - 100), uiSize.Y - 50), true);
            {
                ImGui.Text("Dialog files");
                if (currentFiles != null)
                {
                    ImGui.SetNextItemWidth(uiSize.Y - 50);
                    ImGui.ListBox("##currentItemsByType", ref currentSelectedFile, currentFilesNames, currentFiles.Length);
                }
            }
            ImGui.EndChild();
            ImGui.End();
        }


        private void DrawDialogTree(DialogDataViewModel data, string treeStack)
        {
            if (data == null)
            { return; }

            ImGui.PushID(data.Id);
            // ImGui.SetNextItemOpen(true);
            if (ImGui.TreeNode(treeStack))
            {
                ImGui.PopID();
                ImGui.TreePop();
                var inputTextSize = new Vector2((200) * 2, 50);

                ImGui.Text("Response");
                ImGui.InputTextMultiline("##Response", ref data.Response, 10000, inputTextSize, ImGuiInputTextFlags.None);
                if (ButtonWithSound("Add dialog option", buttonSize))
                {
                    data.Options.Add(new DialogOptionViewModel());
                }

                int childIndex = 0;
                foreach (var dialogOption in data.Options.ToList())
                {
                    ImGui.PushID(dialogOption.Id);
                    //    ImGui.SetNextItemOpen(true);
                    if (ImGui.TreeNode(treeStack + @$"->[{childIndex}]"))
                    {
                        ImGui.PopID();
                        ImGui.Text("Option text");
                        ImGui.InputTextMultiline("##Option text input" + dialogOption.Id, ref dialogOption.OptionText, 10000, inputTextSize, ImGuiInputTextFlags.None);

                        if (ButtonWithSound("Add child dialog" + "##" + dialogOption.Id, buttonSize))
                        {
                            dialogOption.DialogData = new DialogDataViewModel();
                        }
                        ImGui.SameLine();
                        if (ButtonWithSound("Remove this" + "##" + dialogOption.Id, buttonSize))
                        {
                            data.Options.Remove(dialogOption);
                        }

                        DrawDialogTree(dialogOption.DialogData, treeStack + @$"->[{childIndex}] -> Response");
                        ImGui.TreePop();
                    }
                    childIndex++;
                }
            }
        }


        private void Save()
        {
            var directory = this.contentDialogDirectoryPath;

            var newItemPath = directory + name + ".nrdf";
            if (File.Exists(newItemPath))
            {
                TextReader reader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DialogData));
                    reader = new StreamReader(newItemPath);
                    var oldData = (DialogData)serializer.Deserialize(reader);
                    this.data.Id = oldData.Id;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    data.Id = "";
                }
            }

        
            if (data.Id == "" || data.Id == null)
            {
                data.Id = Guid.NewGuid().ToString();
            }

            DialogData searializationData = new DialogData();
           
            data.UnloadToData(searializationData);
            
            using (TextWriter writer = new StreamWriter(newItemPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(DialogData));
                ser.Serialize(writer, searializationData);
            }
        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DialogData));

            currentFilePath = currentFiles[currentSelectedFile];

            if (currentFilePath == null || !currentFilePath.Any())
            {
                return;
            }

            TextReader reader = new StreamReader(currentFilePath);
            var dtd = (DialogData)serializer.Deserialize(reader);

            data = new DialogDataViewModel();
            data.LoadFromData(dtd);

            reader.Close();
        }
    }
    public class DialogDataViewModel
    {
        public string Id;
        public string Response;
        public List<DialogOptionViewModel> Options;

        public DialogDataViewModel()
        {

            Id = Guid.NewGuid().ToString();
            Response = "";
            Options = new List<DialogOptionViewModel>();
        }

        public void LoadFromData(DialogData data)
        {
            Id = data.Id;
            Response = data.Response;
            Options = new List<DialogOptionViewModel>();
            foreach (var dialogOption in data.Options)
            {
                var dialogOptionView = new DialogOptionViewModel();
                dialogOptionView.LoadFromData(dialogOption);
                Options.Add(dialogOptionView);
            }
        }

        public void UnloadToData(DialogData data)
        {
            data.Id = Id;
            data.Response = Response;
            data.Options = new DialogOption[this.Options.Count];
            int index = 0;
            foreach (var dialogOptionViewModel in Options)
            {
                var dialogOption = new DialogOption();
                dialogOptionViewModel.UnloadToData(dialogOption);
                data.Options[index] = dialogOption;
                index++;
            }
        }
    }

    public class DialogOptionViewModel
    {
        public string Id;
        public string OptionText;
        public DialogDataViewModel DialogData;

        public DialogOptionViewModel()
        {
            Id = Guid.NewGuid().ToString();
            OptionText = "";
            DialogData = null;
        }
        public void LoadFromData(DialogOption data)
        {
            Id = data.Id;
            OptionText = data.OptionText;
            if (data.DialogData != null)
            {
                DialogData = new DialogDataViewModel();
                DialogData.LoadFromData(data.DialogData);
            }
        }

        public void UnloadToData(DialogOption data)
        {
            data.Id = Id;
            data.OptionText = OptionText;
            if (DialogData != null)
            {
                if(data.DialogData == null)
                {
                    data.DialogData = new DialogData();
                }

                DialogData.UnloadToData(data.DialogData);
            }
        }
    }
}


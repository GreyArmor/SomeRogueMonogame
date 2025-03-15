using ImGuiNET;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace NamelessRogue.Engine.UI
{
    public class EditorQuestScreen : BaseScreen
    {
        private QuestTemplateData data;
        private string[] currentFiles = Array.Empty<string>();
        private int currentSelectedFile = 0;
        private string[] currentFilesNames = Array.Empty<string>();
        private string contentQuestDirectoryPath = "";
        private string contentDirectoryPath = "";
        QuestType[] questTypes = (QuestType[]) Enum.GetValues(typeof(QuestType));
        string[] questTypesNames = Enum.GetNames(typeof(QuestType));
        public EditorQuestScreen(NamelessGame game) : base(game)
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentQuestDirectoryPath = projectDirectory + "\\Content\\GameObjects\\Quests\\";
            contentDirectoryPath = projectDirectory + "\\Content\\";
#else
            contentQuestDirectoryPath = workingDirectory + "\\Content\\GameObjects\\Quests\\";
            contentDirectoryPath = workingDirectory + "\\Content\\";
#endif
        }
        Vector2 inputTextSize = new Vector2((200) * 2, 50);
        private string fileName = "";
        private string currentFilePath;

        public override void DrawLayout()
        {

            var fieldsSizeX = (uiSize.X / 3) * 2;

            if (!Directory.Exists(contentQuestDirectoryPath))
            {
                Directory.CreateDirectory(contentQuestDirectoryPath);
            }

            currentFiles = Directory.GetFiles(contentQuestDirectoryPath);
            currentFilesNames = currentFiles.Select(path => System.IO.Path.GetFileName(path)).ToArray();

            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollWithMouse);
            {

                ImGui.BeginChild("##fields", new Vector2(fieldsSizeX, uiSize.Y - 100), false, ImGuiWindowFlags.None);
                {

                    if (ButtonWithSound("Save", buttonSize))
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
                    if (ButtonWithSound("Add new quest", buttonSize))
                    {
                        data = new QuestTemplateData();
                        data.id = Guid.NewGuid().ToString();
                        data.name = "";
                        data.description = "";
                    }


                    ImGui.SameLine();
                    if (ButtonWithSound("Back", buttonSize))
                    {
                        game.ContextToSwitch = ContextFactory.GetEditorsPickerContext(game); 
                    }                    

                    DrawQuestTree(data, "Root");


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


        private void DrawQuestTree(QuestTemplateData data, string treeStack)
        {
            if (data == null)
            { return; }

            ImGui.PushID(data.Id);
            // ImGui.SetNextItemOpen(true);
            if (ImGui.TreeNode(treeStack))
            {
                ImGui.PopID();
                ImGui.TreePop();

                ImGui.Text("File name");
                ImGui.InputText("##Name", ref data.name, 128);


                ImGui.Text("Description");
                var _temp = data.Description;
                ImGui.InputTextMultiline("##Description", ref _temp, 10000, inputTextSize, ImGuiInputTextFlags.None);
                data.Description = _temp;


                if (ButtonWithSound("Add quest branch", buttonSize))
                {
                    data.QuestBranches.Add(new QuestBranch());
                }
                int childIndex = 0;
                foreach (var branch in data.QuestBranches.ToList())
                {
                    ImGui.PushID(branch.Id);
                    //    ImGui.SetNextItemOpen(true);
                    if (ImGui.TreeNode(treeStack + @$"->[{childIndex}]"))
                    {                     

                        ImGui.PopID(); 
                        ImGui.BeginChild(branch.Id + "border" + childIndex, Vector2.Zero, true);
                        {
                            ImGui.Text("Description");
                            ImGui.InputTextMultiline("##node text input" + branch.Id, ref branch.description, 10000, inputTextSize, ImGuiInputTextFlags.None);

                            ImGui.Text("Money reward");
                            ImGui.DragInt("##moneyDrag", ref branch.moneyReward);

                            ImGui.Text("Item rewards");
                            ButtonWithSound("Add##items" + branch.Id, Vector2.Zero);
                            var items = new string[3] { "","",""};
                            var currentItem = 0;
                            ImGui.ListBox("##itemReward", ref currentItem, items, items.Length);
                            
                            ImGui.Text("Quest type");
                            ImGui.Combo("##QT", ref branch.QuestTypeIndex, questTypesNames, questTypesNames.Length);
                            branch.QuestType = questTypes[branch.QuestTypeIndex];
                            ImGui.Separator();
                            QuestTypeUI(branch.QuestType, branch.Id);
                            ImGui.Separator();


                        }
                        ImGui.EndChild();
                        ImGui.TreePop();
                        
                    }
                    childIndex++;
                }
            }
        }       


        private void QuestTypeUI(QuestType questType, string id)
        {
            switch (questType)
            {
                case QuestType.DeliverToNpc:
                    {
                        ImGui.Text("Deliver to whom?");
                        ButtonWithSound("Pick npc##items" + id, Vector2.Zero);
                        ImGui.Text("NpcId");
                        ImGui.Text("Deliver what items?");
                        ButtonWithSound("Add item##items" + id, Vector2.Zero);
                        var items = new string[3] { "", "", "" };
                        var currentItem = 0;
                        ImGui.ListBox("##itemReward", ref currentItem, items, items.Length);
                    }
                    break;
                case QuestType.DeliverToLocation:
                    {
                        ImGui.Text("Deliver where?");
                        ButtonWithSound("Pick npc##items" + id, Vector2.Zero);
                        ImGui.Text("NpcId");
                        ImGui.Text("Deliver what items?");
                        ButtonWithSound("Add item##items" + id, Vector2.Zero);
                        var items = new string[3] { "", "", "" };
                        var currentItem = 0;
                        ImGui.ListBox("##itemReward", ref currentItem, items, items.Length);
                    }
                    break;
                case QuestType.GetFromNpc:
                    break;
                case QuestType.GetFromLocation:
                    break;
                case QuestType.KillNpc:
                    break;
            }
        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(QuestTemplateData));

            currentFilePath = currentFiles[currentSelectedFile];

            if (currentFilePath == null || !currentFilePath.Any())
            {
                return;
            }

            TextReader reader = new StreamReader(currentFilePath);
            data = (QuestTemplateData)serializer.Deserialize(reader);

            reader.Close();
        }

        private void Save()
        {
            var directory = this.contentQuestDirectoryPath;

            var newItemPath = directory + data.Name + ".nrqf";
            if (File.Exists(newItemPath))
            {
                TextReader reader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(QuestTemplateData));
                    reader = new StreamReader(newItemPath);
                    var oldData = (QuestTemplateData)serializer.Deserialize(reader);
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


            using (TextWriter writer = new StreamWriter(newItemPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(QuestTemplateData));
                ser.Serialize(writer, data);
            }
        }
    }


}

using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Systems.Editors;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Schema;
using System.Xml.Serialization;
using static NamelessRogue.Engine.Generation.Editor.QuestTemplateData;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NamelessRogue.Engine.UI
{
    public enum EditorLocationScreenActions
    {
        None,
        SaveItem,
        LoadItem,
        Back,
    }

    public class EdirtorLocationScreen : BaseScreen
    {
        Vector2 buttonSize = new Vector2(200, 50);

        ItemType[] itemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));
        DamageType[] damageTypes = (DamageType[])Enum.GetValues(typeof(DamageType));
        AttackType[] attackTypes = (AttackType[])Enum.GetValues(typeof(AttackType));
        AmmoType[] ammoTypes = (AmmoType[])Enum.GetValues(typeof(AmmoType));

        Slot[] armorSlots = ((Slot[])Enum.GetValues(typeof(Slot))).Except(new List<Slot>() { Slot.LefHand, Slot.RightHand }).ToArray();

        string[] itemsTypesNames = Enum.GetNames(typeof(ItemType));
        string[] damageTypesNames = Enum.GetNames(typeof(DamageType));
        string[] attackTypesNames = Enum.GetNames(typeof(AttackType));
        string[] ammoTypesNames = Enum.GetNames(typeof(AmmoType));
        string[] armorSlotsNames;

        public EditorBuffScreenActions EditorBuffScreenActions { get; set; } = EditorBuffScreenActions.None;


        public EdirtorLocationScreen(NamelessGame game) : base(game)
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentBuffDirectoryPath = projectDirectory + "\\Content\\GameObjects\\Locations\\";
            contentDirectoryPath = projectDirectory + "\\Content\\";
#else
            contentBuffDirectoryPath = workingDirectory +  "\\Content\\GameObjects\\Locations";
            contentDirectoryPath = workingDirectory + "\\Content\\";
#endif
        }

        Vector2 iconSize = new Vector2(64, 64);


        BuildingTemplateData data = new BuildingTemplateData();
        int currentSelectedFile = 0;

        string contentBuffDirectoryPath;
        string contentDirectoryPath = string.Empty;
        bool fileIsPicking = false;

        string iconPath = string.Empty;
        string iconFileName = string.Empty;
        int currentIconCombpBoxItem = 0;
        string selectedIconFile = "";
        private string[] currentFilesOfSelectedItemType;
        private string[] currentFilesOfSelectedItemTypeNames;
        private string currentFilePath;
        private bool floorPickerDialog;
        private string selectedTiledFile;
        /// <summary>
        /// SOMEBODY TOUCHA MY SPAGHET
        /// </summary>
        public override void DrawLayout()
        {
            if (!Directory.Exists(contentBuffDirectoryPath)) { Directory.CreateDirectory(contentBuffDirectoryPath); }

            currentFilesOfSelectedItemType = Directory.GetFiles(contentBuffDirectoryPath);
            currentFilesOfSelectedItemTypeNames = currentFilesOfSelectedItemType.Select(x => Path.GetFileName(x)).ToArray();

            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
            {
                if (fileIsPicking)
                {
                    var fileExtensions = new List<string>() { "*.jpg", "*.png" };

                    void _fillTreeRecursive(string path)
                    {
                        List<string> topDirectoryFiles = new List<string>();
                        List<string> subdirectoryFiles = new List<string>();
                        foreach (string extension in fileExtensions)
                        {
                            topDirectoryFiles.AddRange(Directory.GetFiles(path, extension, SearchOption.TopDirectoryOnly));
                        }

                        foreach (string extension in fileExtensions)
                        {
                            subdirectoryFiles.AddRange(Directory.GetFiles(path, extension, SearchOption.AllDirectories));
                        }
                        if (topDirectoryFiles.Any() || subdirectoryFiles.Any())
                        {
                            ImGui.PushID(path.GetHashCode());
                            if (ImGui.TreeNode(Path.GetFileName(path)))
                            {
                                ImGui.PopID();

                                subdirectoryFiles = subdirectoryFiles.Except(topDirectoryFiles).ToList();

                                foreach (var file in topDirectoryFiles)
                                {
                                    ImGui.PushID(path.GetHashCode() + file.GetHashCode());

                                    var flags = file == selectedIconFile ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.Leaf;
                                    ImGui.TreeNodeEx(Path.GetFileName(file), flags);
                                    if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                                    {
                                        selectedIconFile = file;
                                    }
                                    ImGui.TreePop();
                                    ImGui.PopID();
                                }

                                if (subdirectoryFiles.Any())
                                {
                                    var subdirectories = Directory.GetDirectories(path);
                                    foreach (var subdirectory in subdirectories)
                                    {
                                        _fillTreeRecursive(subdirectory);
                                    }
                                }
                                ImGui.TreePop();
                            }
                        }
                    }



                    var previousPos = ImGui.GetCursorPos();
                    var center = ImGui.GetMainViewport().GetCenter();
                    bool p_open = true;

                    ImGui.OpenPopup("FilePickerDialogPopup");
                    ImGui.SetNextWindowPos(new Vector2());
                    //ImGui.SetNextWindowSize(new Vector2(200, 200));

                    if (ImGui.BeginPopupModal("FilePickerDialogPopup", ref p_open, ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.SetNextItemOpen(true);
                        _fillTreeRecursive(contentDirectoryPath);

                        // ImGui.Combo("files", ref currentIconCombpBoxItem, files.ToArray(), files.Count);
                        if (ImGui.Button("Open"))
                        {
                            ImGui.CloseCurrentPopup();
                            iconPath = selectedIconFile;// files[currentIconCombpBoxItem];
                            FileStream fileStream = new FileStream(iconPath, FileMode.Open);
                            Texture2D texture = Texture2D.FromStream(game.GraphicsDevice, fileStream);
                            iconFileName = Path.GetFileName(iconPath);
                            ImGuiImageLibrary.Textures.Remove(iconFileName);
                            ImGuiImageLibrary.Textures.Add(iconFileName, UIRenderSystem.ImGuiRendererInstance.BindTexture(texture));
                            fileStream.Close();
                            fileStream.Dispose();
                            fileIsPicking = false;
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Cancel"))
                        {
                            ImGui.CloseCurrentPopup();
                            fileIsPicking = false;
                        }
                        ImGui.EndPopup();
                    }
                }
                var fieldsSizeX = (uiSize.X / 3) * 2;
                //      if (!fileIsPicking)
                {
                    ImGui.BeginChild("##fields", new Vector2(fieldsSizeX, uiSize.Y), false, ImGuiWindowFlags.None);
                    {

                        if (ButtonWithSound("Save", buttonSize) && data.name.Any())
                        {
                            Save();
                            // EditorItemScreenActions = EditorItemScreenActions.Back;
                        }
                        ImGui.SameLine();
                        if (ButtonWithSound("Load", buttonSize) && currentFilesOfSelectedItemType.Any())
                        {
                            Load();
                        }

                        ImGui.SameLine();
                        if (ButtonWithSound("Delete", buttonSize))
                        {
                            if (currentFilesOfSelectedItemType.Any())
                            {
                                File.Delete(currentFilesOfSelectedItemType[currentSelectedFile]);
                                currentFilesOfSelectedItemType = null;
                            }
                        }

                        ImGui.SameLine();
                        if (ButtonWithSound("Pick icon", buttonSize))
                        {
                            fileIsPicking = true;
                        }

                        ImGui.SameLine();
                        if (ButtonWithSound("Back", buttonSize))
                        {
                            EditorBuffScreenActions = EditorBuffScreenActions.Back;
                        }

                        if (iconFileName != string.Empty)
                        {
                            ImGui.SetCursorPosX(fieldsSizeX / 2 - iconSize.X / 2);
                            ImGui.BeginChild("##iconFrame", iconSize, true, ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
                            ImGui.SetCursorPos(new Vector2(0));
                            ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], iconSize);
                            ImGui.SetCursorPos(new Vector2(0));
                            ImGui.Image(ImGuiImageLibrary.Textures[iconFileName], iconSize, new Vector2(), new Vector2(1), new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1));
                            ImGui.EndChild();
                        }

                        if (data.id != "" || data.id != null)
                        {
                            ImGui.Text($@"Item Id = {data.id}");
                        }

                        ImGui.Text("Name");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.InputText("##Name", ref data.name, 128);
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Text("Description");
                        var inputTextSize = new Vector2((uiSize.X / 3) * 2, 400);
                        ImGui.InputTextMultiline("##Description", ref data.description, 10000, inputTextSize, ImGuiInputTextFlags.None);

                        void _restrainValue(ref int value, int minValue = 0, int maxValue = 999)
                        {
                            value = value <= minValue ? minValue : value;
                            value = value >= maxValue ? maxValue : value;
                        }


                        ImGui.Text("SizeX / SizeY");

                        int sizeX = (int)data.size.X;
                        int sizeY = (int)data.size.Y;
                        ImGui.DragInt("##sizeX", ref sizeX);
                        ImGui.DragInt("##sizeY", ref sizeY);

                        data.size = new Vector2(sizeX, sizeY);

                        if (ButtonWithSound("Add", buttonSize, true))
                        {
                            floorPickerDialog = true;
                        }


                        var fileExtensions = new List<string>() { "*.tmx" };
                        if (floorPickerDialog)
                        {
                            ImGui.OpenPopup("Floor picker dialog");
                            ImGui.SetNextWindowPos(new Vector2());
                            bool drop_open = true;
                            if (ImGui.BeginPopupModal("Floor picker dialog", ref drop_open, ImGuiWindowFlags.AlwaysAutoResize))
                            {
                                ImGui.SetNextItemOpen(true);
                                _fillTreeRecursive(contentDirectoryPath, fileExtensions, ref selectedTiledFile);

                                // ImGui.Combo("files", ref currentIconCombpBoxItem, files.ToArray(), files.Count);
                                if (ImGui.Button("Open"))
                                {
                                    var floorFileRef = new FileReference() { Id = selectedTiledFile, Path = Path.GetRelativePath(contentDirectoryPath, selectedTiledFile) };
                                    var buildingFloorTemplate = new BuildingFloor() { Floor = 0, TiledFilePath = floorFileRef };
                                    data.TiledFilePaths.Add(buildingFloorTemplate);
                                    floorPickerDialog = false;
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.SameLine();
                                if (ImGui.Button("Cancel"))
                                {
                                    floorPickerDialog = false;
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.EndPopup();
                            }
                        }

                        int counter = 0;
                        foreach (var associatedBuilding in data.TiledFilePaths.ToList())
                        {
                           // ImGui.Separator();
                            ImGui.SetNextItemWidth(fieldsSizeX / 2);
                            ImGui.BeginChild("##associatedBuffText" + counter, new Vector2(fieldsSizeX / 2, buttonSize.Y / 2));
                            ImGui.Text(Path.GetFileName(associatedBuilding.TiledFilePath.Path));                          
                            ImGui.EndChild();
                            ImGui.InputInt("Floor ##intdrg" + counter, ref associatedBuilding.floor);
                            ImGui.SameLine();
                            if (ButtonWithSound("Remove ##" + counter, buttonSize / 2, true))
                            {
                                data.TiledFilePaths.Remove(associatedBuilding);
                            }
                            counter++;
                        }

                    }
                    ImGui.EndChild();
                    ImGui.SameLine();
                    ImGui.BeginChild("##currentItems", new Vector2((uiSize.X / 3 - 100), uiSize.Y - 50), true);
                    {
                        ImGui.Text("Items of type");

                        if (currentFilesOfSelectedItemTypeNames != null)
                        {
                            ImGui.SetNextItemWidth(uiSize.Y - 50);
                            ImGui.ListBox("##currentItemsByType", ref currentSelectedFile, currentFilesOfSelectedItemTypeNames, currentFilesOfSelectedItemTypeNames.Length, currentFilesOfSelectedItemTypeNames.Length);
                        }
                    }
                    ImGui.EndChild();
                }
            }
            ImGui.End();
        }

        private void Save()
        {
            var directory = contentBuffDirectoryPath;

            var newItemPath = directory + data.name + ".nrlf";
            if (File.Exists(newItemPath))
            {
                TextReader reader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BuildingTemplateData));
                    reader = new StreamReader(newItemPath);
                    var oldData = (BuildingTemplateData)serializer.Deserialize(reader);
                    data.id = oldData.Id;
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
            else
            {
                data.Id = "";
            }
            if (data.Id == "" || data.Id == null)
            {
                data.Id = Guid.NewGuid().ToString();
            }

            if (iconPath != string.Empty)
            {
                if (!Directory.Exists(directory + "\\Icons\\"))
                {
                    Directory.CreateDirectory(directory + "\\Icons\\");
                }
                // File.Delete(iconPath);

                var newIconLocation = directory + "Icons\\" + iconFileName;
                if (iconPath != newIconLocation)
                {
                    File.Copy(iconPath, newIconLocation, true);
                }
               // data.IconPath = Path.GetRelativePath(directory, directory + "\\Icons\\" + iconFileName);
            }

            using (TextWriter writer = new StreamWriter(newItemPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(BuildingTemplateData));
                ser.Serialize(writer, data);
                currentFilesOfSelectedItemType = null;
            }
        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BuildingTemplateData));

            currentFilePath = currentFilesOfSelectedItemType[currentSelectedFile];

            if (currentFilePath == null || !currentFilePath.Any())
            {
                return;
            }

            TextReader reader = new StreamReader(currentFilePath);
            var btd = (BuildingTemplateData)serializer.Deserialize(reader);

            data = btd;

            reader.Close();
        }
    }
}


using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
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
using System.Xml.Serialization;

namespace NamelessRogue.Engine.UI
{
    public enum EditorBuffScreenActions
    {
        None,
        SaveItem,
        LoadItem,
        Back,
    }

    public class EditorBuffScreen : BaseScreen
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


        public EditorBuffScreen(NamelessGame game) : base(game)
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentBuffDirectoryPath = projectDirectory + "\\Content\\GameObjects\\Buffs\\";
            contentDirectoryPath = projectDirectory + "\\Content\\";
#else
            contentBuffDirectoryPath = workingDirectory +  "\\Content\\GameObjects\\Buffs";
            contentDirectoryPath = workingDirectory + "\\Content\\";
#endif
        }

        Vector2 iconSize = new Vector2(64, 64);

        int currentDamageTypeIndex = 0;
        int currentAttackTypeIndex = 0;
        int currentArmorTypeIndex = 0;
        int currentResistTypeIndex = 0;
        int currentAmmoTypeIndex = 0;
        int currentArmorSlotTypeIndex = 0;

        string name = "";
        string description = "";
        int maxDamage = 10;
        int minDamage = 0;
        int ammoInClip = 0;
        int weaponRange = 1;
        int currentSelectedFile = 0;

        int armorValue = 10;
        int resistValue = 10;

        int duration = 1;
        int healthModValue = 1;
        int energyModValue = 1;
        int damageModValue = 10;
        int armorModValue = 10;
        int resistModValue = 10;

        bool isAppliedImmediately = false;
        bool isDoT = false;

        bool isResMod = false;
        bool isArmorMod = false;
        bool isDamageMod = false;
        string contentBuffDirectoryPath;
        string contentDirectoryPath = string.Empty;
        bool fileIsPicking = false;

        string iconPath = string.Empty;
        string iconFileName = string.Empty;
        int currentIconCombpBoxItem = 0;
        string selectedIconFile = "";
        private string itemId;
        private string[] currentFilesOfSelectedItemType;
        private string[] currentFilesOfSelectedItemTypeNames;

        void _fillTreeRecursive(string path, IEnumerable<string> fileExtensions, ref string selectedFile)
        {
            ImGui.SetNextItemOpen(true);
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
                            selectedFile = file;
                        }
                        ImGui.TreePop();
                        ImGui.PopID();
                    }

                    if (subdirectoryFiles.Any())
                    {
                        var subdirectories = Directory.GetDirectories(path);
                        foreach (var subdirectory in subdirectories)
                        {
                            _fillTreeRecursive(subdirectory, fileExtensions, ref selectedFile);
                        }
                    }
                    ImGui.TreePop();
                }
            }
        }



        /// <summary>
        /// SOMEBODY TOUCHA MY SPAGHET
        /// </summary>
        public override void DrawLayout()
        {
            if(!Directory.Exists(contentBuffDirectoryPath)) { Directory.CreateDirectory(contentBuffDirectoryPath); }

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
                    
                        if (ButtonWithSound("Save", buttonSize) && name.Any())
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

                        if (itemId != "" || itemId == null)
                        {
                            ImGui.Text($@"Item Id = {itemId}");
                        }

                        ImGui.Text("Name");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.InputText("##Name", ref name, 128);
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Text("Description");
                        var inputTextSize = new Vector2((uiSize.X / 3) * 2, 400);
                        ImGui.InputTextMultiline("##Description", ref description, 10000, inputTextSize, ImGuiInputTextFlags.None);

                        void _restrainValue(ref int value, int minValue = 0, int maxValue = 999)
                        {
                            value = value <= minValue ? minValue : value;
                            value = value >= maxValue ? maxValue : value;
                        }

                         
                        {

                            ImGui.Checkbox("Applied immediately?", ref isAppliedImmediately);
                            ImGui.SameLine();
                            ImGui.Checkbox("Damage over time?", ref isDoT);

                            ImGui.Checkbox("Modifies resources?", ref isResMod);
                            ImGui.SameLine();
                            ImGui.Checkbox("Modifies damage?", ref isDamageMod);
                            ImGui.SameLine();
                            ImGui.Checkbox("Modifies armor?", ref isArmorMod);

 
                            if (!isAppliedImmediately)
                            {
                                ImGui.Text("Duration");
                                ImGui.SetNextItemWidth(fieldsSizeX);
                                ImGui.DragInt("##duration", ref duration, 1, 1, 999);
                                _restrainValue(ref duration);
                            }

                            if (isResMod)
                            {
                                ImGui.Separator();
                                ImGui.Text("Health");
                                ImGui.SetNextItemWidth(fieldsSizeX);
                                ImGui.DragInt("##healthModValue", ref healthModValue, 1, -999, 999);
                                _restrainValue(ref healthModValue, -999, 999);

                                ImGui.Text("Energy");
                                ImGui.SetNextItemWidth(fieldsSizeX);
                                ImGui.DragInt("##energyModValue", ref energyModValue, 1, -999, 999);
                                _restrainValue(ref energyModValue, -999, 999);
                                ImGui.Separator();
                            }

                            if (isDamageMod)
                            {
                                ImGui.Separator();
                                ImGui.Text("Damage");
                                ImGui.SetNextItemWidth(fieldsSizeX);
                                ImGui.DragInt("##damageModValue", ref damageModValue, 1, -999, 999);
                                _restrainValue(ref damageModValue, -999, 999);
                                ImGui.Separator();
                            }

                            if (isArmorMod)
                            {
                                ImGui.Separator();
                                ImGui.Text("Armor");
                                ImGui.SetNextItemWidth(fieldsSizeX);
                                ImGui.DragInt("##armorModValue", ref armorModValue, 1, -999, 999);
                                _restrainValue(ref armorModValue, -999, 999);

                                ImGui.Text("Resistance");
                                ImGui.SetNextItemWidth(fieldsSizeX);
                                ImGui.DragInt("#resistanceModValue", ref resistModValue, 1, -999, 999);
                                _restrainValue(ref resistModValue, -999, 999);
                                ImGui.Separator();
                            }

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

            var newItemPath = directory + name + ".nrbf";
            if (File.Exists(newItemPath))
            {
                TextReader reader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BuffTemplateData));
                    reader = new StreamReader(currentFilesOfSelectedItemType[currentSelectedFile]);
                    var oldData = (BuffTemplateData)serializer.Deserialize(reader);
                    itemId = oldData.Id;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    itemId = "";
                }
            }

            BuffTemplateData data = new BuffTemplateData();
            if (itemId == "" || itemId == null)
            {
                itemId = Guid.NewGuid().ToString();
            }

            data.Id = itemId;
            data.Name = name;
            data.Description = description;

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
                data.IconPath = Path.GetRelativePath(directory, directory + "\\Icons\\" + iconFileName);
            }
            data.Duration = duration;
            data.IsAppliedImmediately = isAppliedImmediately;
            data.IsDamageOverTime = isDoT;
            data.HealthModificator = healthModValue;
            data.EnergyModificator = energyModValue;
            data.DamageModificator = damageModValue;
            data.ArmorModificator = armorModValue;
            data.ResistanceModificator = resistModValue;

            using (TextWriter writer = new StreamWriter(newItemPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(BuffTemplateData));
                ser.Serialize(writer, data);
                currentFilesOfSelectedItemType = null;
            }
        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BuffTemplateData));
            TextReader reader = new StreamReader(currentFilesOfSelectedItemType[currentSelectedFile]);
            var btd = (BuffTemplateData)serializer.Deserialize(reader);

            if (btd.IconPath != null && btd.IconPath != string.Empty)
            {
                iconPath = Path.GetDirectoryName(currentFilesOfSelectedItemType[currentSelectedFile]) + "\\" + btd.IconPath;
                FileStream fileStream = new FileStream(iconPath, FileMode.Open);
                Texture2D texture = Texture2D.FromStream(game.GraphicsDevice, fileStream);
                iconFileName = Path.GetFileName(iconPath);
                ImGuiImageLibrary.Textures.Remove(iconFileName);
                ImGuiImageLibrary.Textures.Add(iconFileName, UIRenderSystem.ImGuiRendererInstance.BindTexture(texture));
                fileStream.Close();
                fileStream.Dispose();
            }
            itemId = btd.Id;
            name = btd.Name;
            description = btd.Description;
            isAppliedImmediately = btd.IsAppliedImmediately;
            isDoT = btd.IsDamageOverTime;
            isResMod = btd.HealthModificator != 0 || btd.EnergyModificator != 0;
            isArmorMod = btd.ArmorModificator != 0;
            isDamageMod = btd.DamageModificator != 0;
            duration = btd.Duration;
            healthModValue = btd.HealthModificator;
            energyModValue = btd.EnergyModificator;
            damageModValue = btd.DamageModificator;
            armorModValue = btd.ArmorModificator;
            resistModValue = btd.ResistanceModificator;
            reader.Close();
        }
    }
}


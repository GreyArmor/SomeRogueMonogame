using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content.Pipeline;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
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
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NamelessRogue.Engine.UI
{
    public enum EditorAbilityScreenActions
    {
        None,
        SaveItem,
        LoadItem,
        Back,
    }
    public class EditorAbilityScreen : BaseScreen
    {
        Vector2 buttonSize = new Vector2(200, 50);

        ActivationMode[] abilityActivationModes = (ActivationMode[])Enum.GetValues(typeof(ActivationMode));

        string[] abilityActivationModesNames = ((ActivationMode[])Enum.GetValues(typeof(ActivationMode))).Select(x => x.ToString()).ToArray();
        int abilityActivationModesIndex = 0;

        TargetMode[] abilityTargetModes = (TargetMode[])Enum.GetValues(typeof(TargetMode));
        string[] abilityTargetModesNames = ((TargetMode[])Enum.GetValues(typeof(TargetMode))).Select(x => x.ToString()).ToArray();
        int abilityTargetModesIndex = 0;

        int areaOfEffect = 0;

        bool isActive;

        int range = 0;

        int cooldownTurns  = 0;

        int energyCost = 0;

        int actionPointsCost = 0;


        string[] currentFilesOfSelectedItemType = null;
        string[] currentFilesOfSelectedItemTypeNames = null;
        public EditorAbilityScreenActions EditorAbilityScreenActions { get; set; } = EditorAbilityScreenActions.None;
        public EditorAbilityScreen(NamelessGame game) : base(game)
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentDirectoryPath = projectDirectory + "\\Content";
#else
            contentDirectoryPath = workingDirectory+"\\Content";
#endif
        }

        Vector2 iconSize = new Vector2(64, 64);

        int abilityActivationTypeIndex = 0;

        string name = "";
        string description = "";

        int currentSelectedFile = 0;


        string contentDirectoryPath = string.Empty;
        bool fileIsPicking = false;

        string iconPath = string.Empty;
        string iconFileName = string.Empty;
        int currentIconCombpBoxItem = 0;
        string selectedIconFile = "";
        private string itemId;

        private List<AssociatedBuff> AssociatedBuffs = new List<AssociatedBuff>();
        private bool buffsPickerDialogue;
        private string selectedBuffFile;

        /// <summary>
        /// SOMEBODY TOUCHA MY SPAGHET
        /// </summary>
        public override void DrawLayout()
        {
            var directory = contentDirectoryPath + "\\GameObjects\\Abilities\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                currentFilesOfSelectedItemType = Directory.GetFiles(directory);
                currentFilesOfSelectedItemTypeNames = currentFilesOfSelectedItemType.Select(x => Path.GetFileName(x)).ToArray();

            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollWithMouse);
            {
                if (fileIsPicking)
                {
                    var fileExtensions = new List<string>() { "*.jpg", "*.png" };

                    var previousPos = ImGui.GetCursorPos();
                    var center = ImGui.GetMainViewport().GetCenter();
                    bool p_open = true;

                    ImGui.OpenPopup("FilePickerDialogPopup");
                    ImGui.SetNextWindowPos(new Vector2());
                    //ImGui.SetNextWindowSize(new Vector2(200, 200));

                    if (ImGui.BeginPopupModal("FilePickerDialogPopup", ref p_open, ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.SetNextItemOpen(true);
                        _fillTreeRecursive(contentDirectoryPath, fileExtensions, ref selectedIconFile);

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
                    ImGui.BeginChild("##fields", new Vector2(fieldsSizeX, uiSize.Y - 100), false, ImGuiWindowFlags.None);
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
                            EditorAbilityScreenActions = EditorAbilityScreenActions.Back;
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

                        ImGui.Text("Activation mode");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Combo("##AAM", ref abilityActivationModesIndex, abilityActivationModesNames, abilityActivationModesNames.Length);

                        ImGui.Text("Target mode");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Combo("##ATM", ref abilityTargetModesIndex, abilityTargetModesNames, abilityTargetModesNames.Length);

                        ImGui.Text("Area of effect");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##areaOfEffect", ref areaOfEffect);

                        ImGui.Text("Cooldown turns");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##cooldownTurns", ref cooldownTurns);

                        ImGui.Text("Range");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##abilityRange", ref range);

                        ImGui.Text("Energy cost");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##abilityEnergyCost", ref energyCost);


                        ImGui.Text("Action points cost");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##abilityActionCost", ref actionPointsCost);

                        ImGui.Separator();
                        ImGui.Text("Buffs");
                        ImGui.Separator();

                        var fileExtensions = new List<string>() { "*.nrbf" };
                        if (ButtonWithSound("Add", buttonSize, true))
                        {
                            buffsPickerDialogue = true;
                        }

                        if (buffsPickerDialogue)
                        {
                            ImGui.OpenPopup("Buff picker dialog");
                            ImGui.SetNextWindowPos(new Vector2());
                            bool drop_open = true;
                            if (ImGui.BeginPopupModal("Buff picker dialog", ref drop_open, ImGuiWindowFlags.AlwaysAutoResize))
                            {
                                ImGui.SetNextItemOpen(true);
                                _fillTreeRecursive(contentDirectoryPath, fileExtensions, ref selectedBuffFile);

                                // ImGui.Combo("files", ref currentIconCombpBoxItem, files.ToArray(), files.Count);
                                if (ImGui.Button("Open"))
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(BuffTemplateData));
                                    TextReader reader = new StreamReader(selectedBuffFile);

                                    var buffData = (BuffTemplateData)serializer?.Deserialize(reader);

                                    AssociatedBuffs.Add(new AssociatedBuff() { BuffId = buffData.Id, Path = Path.GetRelativePath(contentDirectoryPath, selectedBuffFile) });
                                    buffsPickerDialogue = false;
                                    reader.Close();
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.SameLine();
                                if (ImGui.Button("Cancel"))
                                {
                                    buffsPickerDialogue = false;
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.EndPopup();
                            }
                        }

                        ImGui.SameLine();
                        ImGui.Separator();
                        ImGui.SetNextItemWidth(fieldsSizeX);

                        ImGui.SetNextItemWidth(fieldsSizeX);

                        int counter = 0;
                        foreach (var associatedBuff in AssociatedBuffs.ToList())
                        {
                            ImGui.Separator();
                            ImGui.SetNextItemWidth(fieldsSizeX / 2);
                            ImGui.BeginChild("##associatedBuffText" + counter, new Vector2(fieldsSizeX / 2, buttonSize.Y / 2));
                            ImGui.Text(Path.GetFileName(associatedBuff.Path));
                            ImGui.EndChild();
                            ImGui.SameLine();
                            if (ButtonWithSound("Remove ##" + counter, buttonSize / 2, true))
                            {
                                AssociatedBuffs.Remove(associatedBuff);
                            }
                            counter++;
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
        }


        private void Save()
        {
            var directory = contentDirectoryPath + "\\GameObjects\\Abilities\\";


            var newItemPath = directory + name + ".nraf";
            if (File.Exists(newItemPath))
            {
                TextReader reader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AbilityTemplateData));
                    reader = new StreamReader(newItemPath);
                    var oldData = (AbilityTemplateData)serializer.Deserialize(reader);
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
            else
            {
                itemId = "";
            }

            AbilityTemplateData data = new AbilityTemplateData();
            if (itemId == "" || itemId == null)
            {
                itemId = Guid.NewGuid().ToString();
            }

            data.Id = itemId;
            data.Name = name;
            data.Description = description;
            data.ActivationMode = abilityActivationModes[abilityActivationModesIndex];
            data.TargetMode = abilityTargetModes[abilityTargetModesIndex];
            data.Range = range;
            data.EnergyCost = energyCost;
            data.ActionPointsCost = actionPointsCost;
            data.CooldownTurns = cooldownTurns;


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

            data.AssociatedBuffs = AssociatedBuffs.ToList();

            using (TextWriter writer = new StreamWriter(newItemPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(AbilityTemplateData));
                ser.Serialize(writer, data);
                currentFilesOfSelectedItemType = null;
            }
        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AbilityTemplateData));
            TextReader reader = new StreamReader(currentFilesOfSelectedItemType[currentSelectedFile]);
            var data = (AbilityTemplateData)serializer.Deserialize(reader);

            itemId = data.Id; 
            name = data.Name; 
            description = data.Description; 
            abilityActivationModes[abilityActivationModesIndex] = data.ActivationMode; 
            abilityTargetModes[abilityTargetModesIndex] = data.TargetMode; 
            range = data.Range; 
            energyCost = data.EnergyCost;
            actionPointsCost = data.ActionPointsCost;
            cooldownTurns = data.CooldownTurns;

            if (data.IconPath != null && data.IconPath != string.Empty)
            {
                iconPath = Path.GetDirectoryName(currentFilesOfSelectedItemType[currentSelectedFile]) + "\\" + data.IconPath;
                FileStream fileStream = new FileStream(iconPath, FileMode.Open);
                Texture2D texture = Texture2D.FromStream(game.GraphicsDevice, fileStream);
                iconFileName = Path.GetFileName(iconPath);
                ImGuiImageLibrary.Textures.Remove(iconFileName);
                ImGuiImageLibrary.Textures.Add(iconFileName, UIRenderSystem.ImGuiRendererInstance.BindTexture(texture));
                fileStream.Close();
                fileStream.Dispose();
            }
            itemId = data.Id;
            name = data.Name;
            description = data.Description;

            AssociatedBuffs = data.AssociatedBuffs.ToList();

            reader.Close();
        }
    }
}


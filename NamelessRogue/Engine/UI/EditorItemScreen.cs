using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content.Pipeline;
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
    public enum EditorItemScreenActions
    {
        None,
        SaveItem,
        LoadItem,
        Back,
    }
    public class EditorItemScreen : BaseScreen
    {
        Vector2 buttonSize = new Vector2(200, 50);

        ItemType[] itemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));
        DamageType[] damageTypes = (DamageType[])Enum.GetValues(typeof(DamageType));
        AttackType[] attackTypes = (AttackType[])Enum.GetValues(typeof(AttackType));
        AmmoType[] ammoTypes = (AmmoType[])Enum.GetValues(typeof(AmmoType));

        Slot[] armorSlots = ((Slot[])Enum.GetValues(typeof(Slot))).Except(new List<Slot>() { Slot.LefHand, Slot.RightHand}).ToArray();

        string[] itemsTypesNames = Enum.GetNames(typeof(ItemType));
        string[] damageTypesNames = Enum.GetNames(typeof(DamageType));
        string[] attackTypesNames = Enum.GetNames(typeof(AttackType));
        string[] ammoTypesNames = Enum.GetNames(typeof(AmmoType));
        string[] armorSlotsNames;

        ItemType currentItemType = ItemType.Weapon;
        string[] currentFilesOfSelectedItemType = null;
        string[] currentFilesOfSelectedItemTypeNames = null;
        public EditorItemScreenActions EditorItemScreenActions { get; set; } = EditorItemScreenActions.None;
        public EditorItemScreen(NamelessGame game) : base(game)
        {

            armorSlotsNames = armorSlots.Select(x => x.ToString()).ToArray();

            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentDirectoryPath = projectDirectory+"\\Content";
#else
            contentDirectoryPath = workingDirectory+"\\Content";
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

        int chargesValue = 1;
        int duration = 1;
        int healthModValue = 1;
        int energyModValue = 1;
        int damageModValue = 10;
        int armorModValue = 10;
        int resistModValue = 10;

        bool isThrowable = false;
        bool isAppliedImmediately = false;
        bool isDoT = false;

        bool isResMod = false;
        bool isArmorMod = false;
        bool isDamageMod = false;

        string contentDirectoryPath = string.Empty;
        bool fileIsPicking = false;

        string iconPath = string.Empty;
        string iconFileName = string.Empty;
        int currentIconCombpBoxItem = 0;
        string selectedIconFile = "";
        /// <summary>
        /// SOMEBODY TOUCHA MY SPAGHET
        /// </summary>
        public override void DrawLayout()
        {
           
            if (currentFilesOfSelectedItemType == null)
            {
                var directory = "";               
                switch (currentItemType)
                {
                    case ItemType.Weapon:
                        directory = contentDirectoryPath + "\\GameObjects\\Weapons";
                        break;
                    case ItemType.Armor:
                        directory = contentDirectoryPath + "\\GameObjects\\Armor";
                        break;
                    case ItemType.Consumable:
                        directory = contentDirectoryPath + "\\GameObjects\\Consumable";
                        break;
                    case ItemType.Supplies:
                        directory = contentDirectoryPath + "\\GameObjects\\Supplies";
                        break;
                    case ItemType.Ammo:
                        directory = contentDirectoryPath + "\\GameObjects\\Ammo";
                        break;
                    case ItemType.Misc:
                        directory = contentDirectoryPath + "\\GameObjects\\Misc";
                        break;
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                currentFilesOfSelectedItemType = Directory.GetFiles(directory);
                currentFilesOfSelectedItemTypeNames = currentFilesOfSelectedItemType.Select(x => Path.GetFileName(x)).ToArray();
            }

            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
            {
                if (fileIsPicking)
                {                   
                    var fileExtensions = new List<string>() { "*.jpg", "*.ace", "*.png" };
            
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
                        ImGui.BeginTabBar("##itemtabs");
                        {
                            var pressed = ImGui.TabItemButton("Weapon");
                            if (pressed)
                            {
                                currentItemType = ItemType.Weapon;
                                currentFilesOfSelectedItemType = null;
                            }
                            pressed = ImGui.TabItemButton("Armor");
                            if (pressed)
                            {
                                currentItemType = ItemType.Armor;
                                currentFilesOfSelectedItemType = null;
                            }
                            pressed = ImGui.TabItemButton("Consumable");
                            if (pressed)
                            {
                                currentItemType = ItemType.Consumable;
                                currentFilesOfSelectedItemType = null;
                            }
                            pressed = ImGui.TabItemButton("Supplies");
                            if (pressed)
                            {
                                currentItemType = ItemType.Supplies;
                                currentFilesOfSelectedItemType = null;
                            }
                            pressed = ImGui.TabItemButton("Ammo");
                            if (pressed)
                            {
                                currentItemType = ItemType.Ammo;
                                currentFilesOfSelectedItemType = null;
                            }
                            pressed = ImGui.TabItemButton("Misc");
                            if (pressed)
                            {
                                currentItemType = ItemType.Misc;
                                currentFilesOfSelectedItemType = null;
                            }
                        }
                        ImGui.EndTabBar();
                        if (ButtonWithSound("Save", buttonSize) && name.Any())
                        {
                            var directory = "";
                            switch (currentItemType)
                            {
                                case ItemType.Weapon:
                                    directory = contentDirectoryPath + "\\GameObjects\\Weapons\\";
                                    break;
                                case ItemType.Armor:
                                    directory = contentDirectoryPath + "\\GameObjects\\Armor\\";
                                    break;
                                case ItemType.Consumable:
                                    directory = contentDirectoryPath + "\\GameObjects\\Consumable\\";
                                    break;
                                case ItemType.Supplies:
                                    directory = contentDirectoryPath + "\\GameObjects\\Supplies\\";
                                    break;
                                case ItemType.Ammo:
                                    directory = contentDirectoryPath + "\\GameObjects\\Ammo\\";
                                    break;
                                case ItemType.Misc:
                                    directory = contentDirectoryPath + "\\GameObjects\\Misc\\";
                                    break;
                            }

                            ItemTemplateData data = new ItemTemplateData();
                            data.Name = name;
                            data.Description = description;
                            data.ItemType = currentItemType;

                            if (iconPath != string.Empty)
                            {
                                if(!Directory.Exists(directory + "\\Icons\\"))
                                {
                                    Directory.CreateDirectory(directory + "\\Icons\\");
                                }
                                // File.Delete(iconPath);

                                var newIconLocation = directory + "Icons\\" + iconFileName;
                                if (iconPath != newIconLocation) {
                                    File.Copy(iconPath, newIconLocation, true);
                                }
                                data.IconPath = Path.GetRelativePath(directory, directory + "\\Icons\\" + iconFileName);
                            }                          

                            if (currentItemType == ItemType.Weapon)
                            {
                                var wtd = new WeaponTemplateData();
                                wtd.DamageType = damageTypes[currentDamageTypeIndex];
                                wtd.AttackType = attackTypes[currentAttackTypeIndex];
                                wtd.MinimumDamage = minDamage;
                                wtd.MaximumDamage = maxDamage;
                                wtd.AmmoInClip = ammoInClip;
                                wtd.Range = weaponRange;
                                wtd.AmmoType = ammoTypes[currentAmmoTypeIndex];
                                data.WeaponTemplateData = wtd;
                                data.PossibleSlots = new List<Slot>() { Slot.LefHand, Slot.RightHand };
                            }
                            else if (currentItemType == ItemType.Armor)
                            {
                                var atd = new ArmorTemplateData();
                                atd.ResistValue = resistValue;
                                atd.ArmorValue = armorValue;
                                data.ArmorTemplateData = atd;
                                data.PossibleSlots = new List<Slot>() { armorSlots[currentArmorSlotTypeIndex] };
                            }
                            else if (currentItemType == ItemType.Consumable)
                            {
                                var citd = new ConsumableItemTemplateData();
                                citd.IsThrowable = isThrowable;
                                citd.Charges = chargesValue;
                                citd.Duration = duration;
                                citd.IsAppliedImmediately = isAppliedImmediately;
                                citd.IsDamageOverTime = isDoT;
                                citd.HealthModificator = healthModValue;
                                citd.EnergyModificator = energyModValue;
                                citd.DamageModificator = damageModValue;
                                citd.ArmorModificator = armorModValue;
                                citd.ResistanceModificator = resistModValue;
                                data.ConsumableItemTemplateData = citd;
                            }

                            using (TextWriter writer = new StreamWriter(directory + name + ".xml"))
                            {
                                XmlSerializer ser = new XmlSerializer(typeof(ItemTemplateData));
                                ser.Serialize(writer, data);
                                currentFilesOfSelectedItemType = null;
                            }
                            // EditorItemScreenActions = EditorItemScreenActions.Back;
                        }
                        ImGui.SameLine();
                        if (ButtonWithSound("Load", buttonSize) && currentFilesOfSelectedItemType.Any())
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ItemTemplateData));
                            TextReader reader = new StreamReader(currentFilesOfSelectedItemType[currentSelectedFile]);
                            var itemData = (ItemTemplateData)serializer.Deserialize(reader);

                            if (itemData.IconPath != null && itemData.IconPath != string.Empty)
                            {
                                iconPath = Path.GetDirectoryName(currentFilesOfSelectedItemType[currentSelectedFile])+"\\" + itemData.IconPath;
                                FileStream fileStream = new FileStream(iconPath, FileMode.Open);
                                Texture2D texture = Texture2D.FromStream(game.GraphicsDevice, fileStream);
                                iconFileName = Path.GetFileName(iconPath);
                                ImGuiImageLibrary.Textures.Remove(iconFileName);
                                ImGuiImageLibrary.Textures.Add(iconFileName, UIRenderSystem.ImGuiRendererInstance.BindTexture(texture));
                                fileStream.Close();
                                fileStream.Dispose();
                            }

                            name = itemData.Name;
                            description = itemData.Description;
                            var wtd = itemData.WeaponTemplateData;
                            var atd = itemData.ArmorTemplateData;
                            var citd = itemData.ConsumableItemTemplateData;

                            if (wtd != null)
                            {
                                currentDamageTypeIndex = Array.IndexOf(damageTypes, wtd.DamageType);
                                currentAttackTypeIndex = Array.IndexOf(attackTypes, wtd.AttackType);
                                currentAmmoTypeIndex = Array.IndexOf(ammoTypes, wtd.AmmoType);
                                minDamage = wtd.MinimumDamage;
                                maxDamage = wtd.MaximumDamage;
                                weaponRange = wtd.Range;
                                ammoInClip = wtd.AmmoInClip;
                            }
                            if (atd != null)
                            {                                
                                armorValue = atd.ArmorValue;
                                resistValue = atd.ResistValue;
                                currentArmorTypeIndex = Array.IndexOf(damageTypes, atd.DamageType);
                                currentResistTypeIndex = Array.IndexOf(damageTypes, atd.ResistType);

                                currentArmorSlotTypeIndex = Array.IndexOf(armorSlots, itemData.PossibleSlots[0]);
                            }
                            if (citd != null)
                            {
                                isThrowable = citd.IsThrowable;
                                isAppliedImmediately = citd.IsAppliedImmediately;
                                isDoT = citd.IsDamageOverTime;
                                isResMod = citd.HealthModificator != 0 || citd.EnergyModificator != 0;
                                isArmorMod = citd.ArmorModificator != 0;
                                isDamageMod = citd.DamageModificator != 0;
                                chargesValue = citd.Charges;
                                duration = citd.Duration;
                                healthModValue = citd.HealthModificator;
                                energyModValue = citd.EnergyModificator;
                                damageModValue = citd.DamageModificator;
                                armorModValue = citd.ArmorModificator;
                                resistModValue = citd.ResistanceModificator;
                              
                            }
                                reader.Close();
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
                            EditorItemScreenActions = EditorItemScreenActions.Back;
                        }

                        if (iconFileName != string.Empty)
                        {
                            ImGui.SetCursorPosX(fieldsSizeX / 2 - iconSize.X / 2);
                            ImGui.BeginChild("##iconFrame", iconSize, true, ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
                            ImGui.SetCursorPos(new Vector2(0));
                            ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], iconSize);
                            ImGui.SetCursorPos(new Vector2(0));
                            ImGui.Image(ImGuiImageLibrary.Textures[iconFileName], iconSize, new Vector2(), new Vector2(1), new Vector4(1,1,1,1), new Vector4(1,1,1,1));
                            ImGui.EndChild();
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

                        if (currentItemType == ItemType.Weapon)
                        {
                            ImGui.Text("Damage type");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.Combo("##DT", ref currentDamageTypeIndex, damageTypesNames, damageTypesNames.Length);
                            ImGui.Text("Ammo type");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.Combo("##ammoT", ref currentAmmoTypeIndex, ammoTypesNames, ammoTypesNames.Length);
                            ImGui.Text("Attack type");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.Combo("##AT", ref currentAttackTypeIndex, attackTypesNames, attackTypes.Length);
                            ImGui.Text("Minimum damage");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.DragInt("##MinD", ref minDamage, 1, 1, 999);
                            _restrainValue(ref minDamage);
                            ImGui.Text("Maximum damage");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.DragInt("##maxD", ref maxDamage, 1, 1, 999);
                            _restrainValue(ref maxDamage);
                            ImGui.Text("Ammo in clip");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.DragInt("##ammoInClip", ref ammoInClip, 1, 1, 999);
                            _restrainValue(ref ammoInClip);
                            ImGui.Text("Weapon range");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.DragInt("##range", ref weaponRange, 1, 1, 999);
                            _restrainValue(ref weaponRange);
                        }
                        else if (currentItemType == ItemType.Armor)
                        {
                            ImGui.Text("Slot");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.Combo("##Slot", ref currentArmorSlotTypeIndex, armorSlotsNames, armorSlots.Length);
                            ImGui.Text("Armor type");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.Combo("##AT", ref currentArmorTypeIndex, damageTypesNames, damageTypesNames.Length);
                            ImGui.Text("Resist type");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.Combo("##RT", ref currentResistTypeIndex, damageTypesNames, damageTypesNames.Length);
                            ImGui.Text("Armor value");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.DragInt("##armorValue", ref armorValue, 1, 1, 999);
                            _restrainValue(ref armorValue);
                            ImGui.Text("Resist value");
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.DragInt("##resistAalue", ref resistValue, 1, 1, 999);
                            _restrainValue(ref resistValue);
                        }
                        else if (currentItemType == ItemType.Consumable)
                        {
                            ImGui.Checkbox("Throwable?", ref isThrowable);
                            ImGui.SameLine();
                            ImGui.Checkbox("Applied immediately?", ref isAppliedImmediately);
                            ImGui.SameLine();
                            ImGui.Checkbox("Damage over time?", ref isDoT);

                            ImGui.Checkbox("Modifies resources?", ref isResMod);
                            ImGui.SameLine();
                            ImGui.Checkbox("Modifies damage?", ref isDamageMod);
                            ImGui.SameLine();
                            ImGui.Checkbox("Modifies armor?", ref isArmorMod);
            
                    
                            ImGui.SetNextItemWidth(fieldsSizeX);
                            ImGui.Text("Charges");
                            ImGui.DragInt("##Charges", ref chargesValue, 1, 1, 999);
                            _restrainValue(ref chargesValue);

                            if (!isAppliedImmediately)
                            {
                                ImGui.Text("Duration");
                                ImGui.SetNextItemWidth(fieldsSizeX);
                                ImGui.DragInt("##duration", ref duration, 1, 1, 999);
                                _restrainValue(ref duration);
                            }

                            if(isResMod)
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
    }
}


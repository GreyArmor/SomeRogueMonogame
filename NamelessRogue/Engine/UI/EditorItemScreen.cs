using ImGuiNET;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
        string[] itemsTypesNames = Enum.GetNames(typeof(ItemType));
        string[] damageTypesNames = Enum.GetNames(typeof(DamageType));
        string[] attackTypesNames = Enum.GetNames(typeof(AttackType));
        string[] ammoTypesNames = Enum.GetNames(typeof(AmmoType));

        ItemType currentItemType = ItemType.Weapon;
        string[] currentFilesOfSelectedItemType = null;
        string[] currentFilesOfSelectedItemTypeNames = null;
        public EditorItemScreenActions EditorItemScreenActions { get; set; } = EditorItemScreenActions.None;
        public EditorItemScreen(NamelessGame game) : base(game)
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentDirectoryPath = projectDirectory+"\\Content";
#else
            contentDirectoryPath = workingDirectory+"\\Content";
#endif
        }

        int currentDamageTypeIndex = 0;
        int currentAttackTypeIndex = 0;
        int currentArmorTypeIndex = 0;
        int currentResistTypeIndex = 0;
        int currentAmmoTypeIndex = 0;
        string name = "";
        string description = "";
        int maxDamage = 10;
        int minDamage = 0;
        int ammoInClip = 0;
        int weaponRange = 1;
        int currentSelectedFile = 0;

        int armorValue = 10;
        int resistValue = 10;

        string contentDirectoryPath = string.Empty;

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
                ImGui.BeginChild("##fields", new Vector2((uiSize.X / 3) * 2, uiSize.Y));
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
                            data.WeapomTemplateData = wtd;
                        }
                        else if (currentItemType == ItemType.Armor)
                        {
                            var atd = new ArmorTemplateData();
                            atd.ResistValue = resistValue;
                            atd.ArmorValue = armorValue;
                            data.ArmorTemplateData = atd;
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
                    if (ButtonWithSound("Load", buttonSize))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ItemTemplateData));
                        TextReader reader = new StreamReader(currentFilesOfSelectedItemType[currentSelectedFile]);
                        var itemData = (ItemTemplateData)serializer.Deserialize(reader);

                        name = itemData.Name;
                        description = itemData.Description;
                        var wtd = itemData.WeapomTemplateData;
                        var atd = itemData.ArmorTemplateData;

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
                        }
                        reader.Close();
                    }

                    ImGui.SameLine();
                    if (ButtonWithSound("Delete", buttonSize))
                    {
                        File.Delete(currentFilesOfSelectedItemType[currentSelectedFile]);
                        currentFilesOfSelectedItemType = null;
                    }

                    ImGui.SameLine();
                    if (ButtonWithSound("Back", buttonSize))
                    {
                        EditorItemScreenActions = EditorItemScreenActions.Back;
                    }
                    ImGui.Text("Name");
                    ImGui.InputText("##Name", ref name, 128);
                    ImGui.Text("Description");
                    var inputTextSize = new Vector2((uiSize.X / 3) * 2, 500);
                    ImGui.InputTextMultiline("##Description", ref description, 10000, inputTextSize, ImGuiInputTextFlags.None);

                    if (currentItemType == ItemType.Weapon)
                    {
                        ImGui.Text("Damage type");
                        ImGui.Combo("##DT", ref currentDamageTypeIndex, damageTypesNames, damageTypesNames.Length);
                        ImGui.Text("Ammo type");
                        ImGui.Combo("##ammoT", ref currentAmmoTypeIndex, ammoTypesNames, ammoTypesNames.Length);
                        ImGui.Text("Attack type");
                        ImGui.Combo("##AT", ref currentAttackTypeIndex, attackTypesNames, attackTypes.Length);
                        ImGui.Text("Minimum damage");
                        ImGui.DragInt("##MinD", ref minDamage, 1, 1, 999);
                        ImGui.Text("Maximum damage");
                        ImGui.DragInt("##maxD", ref maxDamage, 1, 1, 999);
                        ImGui.Text("Ammo in clip");
                        ImGui.DragInt("##ammoInClip", ref ammoInClip, 1, 1, 999);
                        ImGui.Text("Weapon range");
                        ImGui.DragInt("##range", ref weaponRange, 1, 1, 999);
                    }
                    else if (currentItemType == ItemType.Armor)
                    {
                        ImGui.Text("Armor type");
                        ImGui.Combo("##AT", ref currentArmorTypeIndex, damageTypesNames, damageTypesNames.Length);
                        ImGui.Text("Resist type");
                        ImGui.Combo("##RT", ref currentResistTypeIndex, damageTypesNames, damageTypesNames.Length);
                        ImGui.Text("Armor value");
                        ImGui.DragInt("##armorValue", ref armorValue, 1, 1, 999);
                        ImGui.Text("Resist value");
                        ImGui.DragInt("##resistAalue", ref resistValue, 1, 1, 999);
                    }
                }
                ImGui.EndChild();
                ImGui.SameLine();
                //  ImGui.SetCursorPos(new Vector2((uiSize.X / 3) * 2, 0));
                ImGui.BeginChild("##currentItems", new Vector2((uiSize.X / 3), uiSize.Y), true);
                {
                    ImGui.Text("Items of type");
                    if (currentFilesOfSelectedItemTypeNames != null)
                    {
                        ImGui.ListBox("##currentItemsByType", ref currentSelectedFile, currentFilesOfSelectedItemTypeNames, currentFilesOfSelectedItemTypeNames.Length);
                    }
                }
                ImGui.EndChild();
            }
            ImGui.End();

        }
    }
}


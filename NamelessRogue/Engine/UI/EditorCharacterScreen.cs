using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content.Pipeline;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Systems.Editors;
using NamelessRogue.shell;
using SharpDX.Direct2D1;
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
    public enum EditorCharacterScreenActions
    {
        None,
        SaveItem,
        LoadItem,
        Back,
    }

    public class EditorCharacterScreen : BaseScreen
    {
        Vector2 buttonSize = new Vector2(200, 50);
        DamageType[] damageTypes = (DamageType[])Enum.GetValues(typeof(DamageType));
        AttackType[] attackTypes = (AttackType[])Enum.GetValues(typeof(AttackType));
        AmmoType[] ammoTypes = (AmmoType[])Enum.GetValues(typeof(AmmoType));



        string[] itemsTypesNames = Enum.GetNames(typeof(ItemType));
        string[] damageTypesNames = Enum.GetNames(typeof(DamageType));
        string[] attackTypesNames = Enum.GetNames(typeof(AttackType));
        string[] ammoTypesNames = Enum.GetNames(typeof(AmmoType));
        string[] armorSlotsNames;

        ItemType currentItemType = ItemType.Weapon;
        string[] currentFilesOfSelectedItemType = null;
        string[] currentFilesOfSelectedItemTypeNames = null;
        public EditorCharacterScreenActions EditorCharacterScreenActions { get; set; } = EditorCharacterScreenActions.None;

        public List<DroppedItemTemplate> DroppedItems { get; set; } = new List<DroppedItemTemplate>();

        public EditorCharacterScreen(NamelessGame game) : base(game)
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            contentCharacterDirectoryPath = projectDirectory + "\\Content\\GameObjects\\Characters";
            contentDirectoryPath = projectDirectory + "\\Content\\";
#else
            contentCharacterDirectoryPath = workingDirectory + "\\Content\\GameObjects\\Characters\\";
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
        int health = 10;
        int energy = 0;
        int movementSpeed = 50;
        int visionRange = 10;
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

        bool castsShadow = false;
        bool isFlying = false;
        bool immobile = false;

        string contentCharacterDirectoryPath = string.Empty;
        string contentDirectoryPath = string.Empty;
        bool fileIsPicking = false;

        string characterId = "";

        string spritePath = string.Empty;
        string spriteFileName = string.Empty;
        int currentIconCombpBoxItem = 0;
        string selectedIconFile = "";

        string selectedDroppableItemFile = "";

        string[] currentSpriteAnimations = null;
        int currentAnimationIndex = 0;

        bool droppableItemPickerDialog = false;
        int selectedDroppableIndex = 0;

        /// <summary>
        /// SOMEBODY TOUCHA MY SPAGHET
        /// </summary>
        public override void DrawLayout()
        {
            var directory = contentCharacterDirectoryPath;

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
                    var fileExtensions = new List<string>() { "*.ase" };
                    var previousPos = ImGui.GetCursorPos();
                    var center = ImGui.GetMainViewport().GetCenter();
                    bool p_open = true;

                    ImGui.OpenPopup("FilePickerDialogPopup");
                    ImGui.SetNextWindowPos(new Vector2());

                    if (ImGui.BeginPopupModal("FilePickerDialogPopup", ref p_open, ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.SetNextItemOpen(true);
                        _fillTreeRecursive(contentDirectoryPath, fileExtensions, ref selectedIconFile);
                        if (ImGui.Button("Open"))
                        {
                            ImGui.CloseCurrentPopup();
                            spritePath = selectedIconFile;
                            spriteFileName = Path.GetFileName(spritePath);
                            SpriteLibrary.RemoveAnimatedSprite(spriteFileName);
                            SpriteLibrary.AddAnimatedSprite(spriteFileName, spritePath);

                            var sprite = SpriteLibrary.SpritesAnimated[spriteFileName];
                            currentSpriteAnimations = sprite._animations.Keys.ToArray();

                            var changeSpriteCommand = new CharacterScreeChangeSpriteCommand(spriteFileName);
                            game.Commander.EnqueueCommand(changeSpriteCommand);

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
                    ImGui.BeginChild("##fields", new Vector2(fieldsSizeX, uiSize.Y - 100), false, ImGuiWindowFlags.AlwaysAutoResize);
                    {
                        if (ButtonWithSound("Save", buttonSize) && name.Any())
                        {
                            Save(directory);
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
                                currentSelectedFile--;
                            }
                        }

                        ImGui.SameLine();
                        if (ButtonWithSound("Pick sprite", buttonSize))
                        {
                            fileIsPicking = true;
                        }

                        ImGui.SameLine();
                        if (ButtonWithSound("Back", buttonSize))
                        {
                            EditorCharacterScreenActions = EditorCharacterScreenActions.Back;
                        }

                        if(characterId!="" || characterId == null)
                        {
                            ImGui.Text($@"Character Id = {characterId}");
                        }

                        ImGui.Text("Name");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.InputText("##Name", ref name, 128);
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Text("Description");
                        var inputTextSize = new Vector2((uiSize.X / 3) * 2, 400);
                        ImGui.InputTextMultiline("##Description", ref description, 10000, inputTextSize, ImGuiInputTextFlags.None);

                        if (currentSpriteAnimations != null)
                        {
                            ImGui.BeginChild("emptySpace", iconSize);
                            ImGui.EndChild();

                            var marginVector = new Vector2(7);
                            var cursorPos = ImGui.GetItemRectMin();
                            var spritePos = new Vector2(fieldsSizeX / 2 - iconSize.X / 2, cursorPos.Y) + marginVector;
                            ImGui.Text("Animations");
                            ImGui.SetNextItemWidth(fieldsSizeX);

                            var itemChanged = ImGui.Combo("##AmP", ref currentAnimationIndex, currentSpriteAnimations, currentSpriteAnimations.Length);
                            if (itemChanged)
                            {
                                var changeSpriteCommand = new CharacterScreeChangeSpriteAnimationCommand(currentSpriteAnimations[currentAnimationIndex]);
                                game.Commander.EnqueueCommand(changeSpriteCommand);
                            }

                            ImGui.GetBackgroundDrawList().AddRectFilled(spritePos, spritePos + iconSize, ColorToUInt(System.Drawing.Color.Black));

                            EditorCharacterScreenSpriteRenderSystem.SpritePosition = spritePos;
                            EditorCharacterScreenSpriteRenderSystem.SpriteSize = iconSize;
                        }

                        void _restrainValue(ref int value, int minValue = 0, int maxValue = 999)
                        {
                            value = value <= minValue ? minValue : value;
                            value = value >= maxValue ? maxValue : value;
                        }

                        ImGui.Text("Casts shadow?");
                        ImGui.SameLine();
                        ImGui.Checkbox("##CastsShadowValue", ref castsShadow);

                        ImGui.Text("Flying?");
                        ImGui.SameLine();
                        ImGui.Checkbox("##FlyingValue", ref isFlying);

                        ImGui.SameLine();

                        ImGui.Text("Immobile?"); 
                        ImGui.SameLine();
                        ImGui.Checkbox("##ImmobileValue", ref immobile);

                        ImGui.Text("Health");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##HealthValue", ref health, 1, 1, 999);

                        ImGui.Text("Energy");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##EnergyValue", ref energy, 1, 1, 999);

                       
                        ImGui.Text("Movement speed");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##MovementSpeed", ref movementSpeed, 1, 1, 999);

                        ImGui.Text("Vision range");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##VisionRange", ref visionRange, 1, 1, 999);


                        ImGui.Text("Damage type");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Combo("##DT", ref currentDamageTypeIndex, damageTypesNames, damageTypesNames.Length);
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

                        ImGui.Text("Armor type");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Combo("##ArmT", ref currentArmorTypeIndex, damageTypesNames, damageTypesNames.Length);

                        ImGui.Text("Resist type");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.Combo("##RT", ref currentResistTypeIndex, damageTypesNames, damageTypesNames.Length);
                        ImGui.Text("Armor value");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##armorValue", ref armorValue, 1, 1, 999);
                        _restrainValue(ref armorValue);
                        ImGui.Text("Resist value");
                        ImGui.SetNextItemWidth(fieldsSizeX);
                        ImGui.DragInt("##resistValue", ref resistValue, 1, 1, 999);
                        _restrainValue(ref resistValue);

                        ImGui.Separator();
                        ImGui.Text("Droppable items (Name - Probability)");

                        var fileExtensions = new List<string>() { "*.nrif" };
                        if (ButtonWithSound("Add", buttonSize, true))
                        {
                            droppableItemPickerDialog = true;
                        }

                        if (droppableItemPickerDialog)
                        {
                            ImGui.OpenPopup("Droppable item picker dialog");
                            ImGui.SetNextWindowPos(new Vector2());
                            bool drop_open = true;
                            if (ImGui.BeginPopupModal("Droppable item picker dialog", ref drop_open, ImGuiWindowFlags.AlwaysAutoResize))
                            {
                                ImGui.SetNextItemOpen(true);
                                _fillTreeRecursive(contentDirectoryPath, fileExtensions, ref selectedDroppableItemFile);

                                // ImGui.Combo("files", ref currentIconCombpBoxItem, files.ToArray(), files.Count);
                                if (ImGui.Button("Open"))
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(ItemTemplateData));
                                    TextReader reader = new StreamReader(selectedDroppableItemFile);

                                    var itemData = (ItemTemplateData)serializer?.Deserialize(reader);

                                    DroppedItems.Add(new DroppedItemTemplate() { ItemId = itemData.Id, Path = Path.GetRelativePath(directory, selectedDroppableItemFile), Probability = 0 });
                                    droppableItemPickerDialog = false;
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.SameLine();
                                if (ImGui.Button("Cancel"))
                                {
                                    droppableItemPickerDialog = false;
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.EndPopup();
                            }
                        }

                        ImGui.SameLine();
                        ImGui.Separator();
                        ImGui.SetNextItemWidth(fieldsSizeX);

                        int droppableItemCounter = 0;
                        foreach (var droppableItem in DroppedItems.ToList())
                        {
                            ImGui.Separator();
                            ImGui.SetNextItemWidth(fieldsSizeX / 2);
                            ImGui.BeginChild("##itemDropText" + droppableItemCounter, new Vector2(fieldsSizeX / 2, buttonSize.Y / 2));
                            ImGui.Text(Path.GetFileName(droppableItem.Path));
                            ImGui.EndChild();
                            ImGui.SameLine();

                            ImGui.SetNextItemWidth(fieldsSizeX / 2 - buttonSize.X);
                            int probability = droppableItem.Probability;
                            ImGui.DragInt("##itemDropProbability" + droppableItem.Path, ref probability, 1, 1, 100);
                            droppableItem.Probability = probability;
                            ImGui.SameLine();
                            if (ButtonWithSound("Remove ##" + droppableItemCounter, buttonSize / 2, true))
                            {
                                DroppedItems.Remove(droppableItem);
                            }
                            droppableItemCounter++;
                        }
                    }                   
                    ImGui.EndChild();
                    ImGui.SameLine();
                    ImGui.BeginChild("##currentCharacters", new Vector2((uiSize.X / 3 - 100), uiSize.Y - 50), true);
                    {
                    
                        ImGui.Text("Characters");

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

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CharacterTemplateData));
            TextReader reader = new StreamReader(currentFilesOfSelectedItemType[currentSelectedFile]);
            var data = (CharacterTemplateData)serializer.Deserialize(reader);

            if (data.SpritePath != null && data.SpritePath != string.Empty)
            {
                spritePath = contentCharacterDirectoryPath + "\\" + data.SpritePath;
                spriteFileName = Path.GetFileName(spritePath);
                SpriteLibrary.RemoveAnimatedSprite(spriteFileName);
                SpriteLibrary.AddAnimatedSprite(spriteFileName, spritePath);

                var sprite = SpriteLibrary.SpritesAnimated[spriteFileName];
                currentSpriteAnimations = sprite._animations.Keys.ToArray();

                var changeSpriteCommand = new CharacterScreeChangeSpriteCommand(spriteFileName);
                game.Commander.EnqueueCommand(changeSpriteCommand);
            }

            characterId = data.Id;
            name = data.Name;
            description = data.Description;
            health = data.Health;
            energy = data.Energy;
            movementSpeed = data.MovementSpeed;
            visionRange = data.VisionRange;
            castsShadow = data.CastsShadow;
            isFlying = data.IsFlying;
            immobile = data.Immobile;
            var wtd = data.WeaponTemplateData;
            var atd = data.ArmorTemplateData;

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

                //currentArmorSlotTypeIndex = Array.IndexOf(armorSlots, itemData.PossibleSlots[0]);
            }

            DroppedItems = data.DroppedItems;

            reader.Close();
        }
        //"C:\\Users\\user\\source\\repos\\SomeRogueMonogame\\NamelessRogue\\Content\\GameObjects\\Characters"
        // C:\\Users\\user\\source\\repos\\SomeRogueMonogame\\NamelessRogue\\Content\\GameObjects\\Characters
        private void Save(string directory)
        {

            var newCharacterPath = directory + "\\" + name + ".nrcf";

            if (File.Exists(newCharacterPath))
            {
                TextReader reader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CharacterTemplateData));
                    reader  = new StreamReader(currentFilesOfSelectedItemType[currentSelectedFile]);
                    var oldData = (CharacterTemplateData)serializer.Deserialize(reader);
                    characterId = oldData.Id;
                    reader.Close();
                }
                catch (Exception ex)
                {
                    if(reader != null)
                    {
                        reader.Close();
                    }    
                    characterId = "";
                }
            }

            CharacterTemplateData data = new CharacterTemplateData(); 
                      
            if(characterId == "" || characterId ==null)
            {
                characterId = Guid.NewGuid().ToString();
            }

            data.Id = characterId;
            data.Name = name;
            data.Description = description;

        

            data.Health = health;
            data.Energy = energy;
            data.MovementSpeed = movementSpeed;
            data.VisionRange = visionRange;
            data.CastsShadow = castsShadow;
            data.IsFlying = isFlying;
            data.Immobile = immobile;
            if (spritePath != string.Empty)
            {
                if (!Directory.Exists(directory + "\\Sprites\\"))
                {
                    Directory.CreateDirectory(directory + "\\Sprites\\");
                }

                var newIconLocation = directory + "\\Sprites\\" + spriteFileName;
                if (spritePath != newIconLocation)                {
                    
                    File.Delete(newIconLocation);
                    File.Copy(spritePath, newIconLocation, true);
                }
                data.SpritePath = Path.GetRelativePath(directory, directory + "\\Sprites\\" + spriteFileName);
            }
            var wtd = new WeaponTemplateData();
            wtd.DamageType = damageTypes[currentDamageTypeIndex];
            wtd.AttackType = attackTypes[currentAttackTypeIndex];
            wtd.MinimumDamage = minDamage;
            wtd.MaximumDamage = maxDamage;
            wtd.AmmoInClip = ammoInClip;
            wtd.Range = weaponRange;
            wtd.AmmoType = ammoTypes[currentAmmoTypeIndex];
            data.WeaponTemplateData = wtd;

            var atd = new ArmorTemplateData();
            atd.ResistValue = resistValue;
            atd.ArmorValue = armorValue;
            data.ArmorTemplateData = atd;

            data.DroppedItems = DroppedItems;

            using (TextWriter writer = new StreamWriter(newCharacterPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(CharacterTemplateData));
                ser.Serialize(writer, data);
                currentFilesOfSelectedItemType = null;
            }
        }
    }
}


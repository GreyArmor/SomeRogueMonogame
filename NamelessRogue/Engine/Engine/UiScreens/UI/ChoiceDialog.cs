using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.UiScreens.UI
{
    public class ChoiceOption
    {
        public string Text { get; set; }
        public object Id { get; set; }
    }
    public class ChoiceDialog : Window
    {
        private Table optionsTable;

        public ChoiceDialog(params ChoiceOption[] options)
        {
            OptionsTable = new Table();
            char hotkey = char.MinValue;
            for (int i = 0; i < options.Count(); i++)
            {
                var option = options[i];

                if (i == 0)
                {
                    hotkey = HotkeyHelper.alphabet.First();
                }
                else
                {
                    hotkey = HotkeyHelper.GetNextKey(hotkey);
                }


                var tableItem = new TableItem(2);
                tableItem.Tag = option;
                tableItem.Hotkey = hotkey;
                tableItem.Cells[0].Widgets.Add(new Label() { Text = hotkey.ToString(), HorizontalAlignment = HorizontalAlignment.Center });
                tableItem.Cells[1].Widgets.Add(new Label() { Text = option.Text, });
                OptionsTable.Items.Add(tableItem);
            }

            OptionsTable.SelectedIndex = 0;
            OptionsTable.Width = 200;
            Content = OptionsTable;
        }
        public Table OptionsTable { get => optionsTable; set => optionsTable = value; }
    }
}

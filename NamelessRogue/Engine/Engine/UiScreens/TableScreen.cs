using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.UiScreens.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public class TableScreen<BondedSystemType> : BaseGuiScreen <BondedSystemType> where BondedSystemType : ISystem
    {

        public Table SelectedTable { get; set; }
        public bool DialogOpened { get => dialogOpened; private set => dialogOpened = value; }

        protected TableItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (value == null)
                {
                    1.ToString();
                }
                selectedItem = value;
            }
        }

        Table previouslySelectedTable;
        protected bool dialogOpened;

        protected TableItem selectedItem;
        public void SelectTable(Table table)
        {
            SelectedTable = table;
        }


        public void ScrollSelectedTableDown()
        {

            if (SelectedTable.SelectedIndex == null)
            {
                SelectedTable.SelectedIndex = 0;
                return;
            }

            var prevIndex = SelectedTable.SelectedIndex.Value;
            SelectedTable.OnKeyDown(Keys.Down); /* += 1;*/

            int nextIndex = SelectedTable.SelectedIndex.Value;

            bool move = false;
            if (SelectedTable.SelectedIndex == prevIndex)
            {
                nextIndex = 0;
                move = true;
            }

            if (SelectedTable.Items.Any())
            {
                SelectedTable.SelectedIndex = nextIndex;
                if (move)
                {
                    SelectedTable.OnKeyDown(Keys.Down);
                    SelectedTable.OnKeyDown(Keys.Up);
                }
            }
        }

        public void ScrollSelectedTableUp()
        {

            if (SelectedTable.SelectedIndex == null)
            {
                SelectedTable.SelectedIndex = 0;
                return;
            }
            var prevIndex = SelectedTable.SelectedIndex.Value;
            SelectedTable.OnKeyDown(Keys.Up); /* -= 1;*/
            int nextIndex = SelectedTable.SelectedIndex.Value;
            bool move = false;
            if (SelectedTable.SelectedIndex == prevIndex)
            {
                nextIndex = SelectedTable.Items.Count - 1;
                move = true;
            }

            if (SelectedTable.Items.Any())
            {
                SelectedTable.SelectedIndex = nextIndex;
                if (move)
                {
                    SelectedTable.OnKeyDown(Keys.Up);
                    SelectedTable.OnKeyDown(Keys.Down);

                }
            }

        }

        public void OpenDialog(ChoiceDialog dialog, NamelessGame game)
        {
            if (dialogOpened)
            {
                return;
            }

            dialogOpened = true;
            previouslySelectedTable = SelectedTable;
            dialog.OptionsTable.SelectedIndex = 0;
            dialog.ShowModal(game.Desktop);
            SelectTable(dialog.OptionsTable);
        }

        public Action OnCloseDialog = () => {};

        public void CloseDialog(ChoiceDialog dialog)
        {
            dialogOpened = false;
            SelectTable(previouslySelectedTable);
            dialog.Close();
            OnCloseDialog.Invoke();
        }


    }
}

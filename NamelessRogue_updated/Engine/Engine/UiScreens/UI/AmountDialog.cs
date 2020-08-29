using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace NamelessRogue.Engine.Engine.UiScreens.UI
{
    public class AmountDialog : Window
    {
        public ImageTextButton ButtonOk { get; private set; }

        public ImageTextButton ButtonCancel { get; private set; }

        public TextBox Amount { get; private set; }
        public AmountDialog() : base()
        {

            var windowGrid = new Grid();
            windowGrid.RowsProportions.Add(new Proportion());
            windowGrid.RowsProportions.Add(new Proportion());

            Amount = new TextBox();
            Amount.GridRow = 0;
            Amount.ValueChanging += Amount_ValueChanging;

           
            var buttonsGrid = new HorizontalStackPanel()
            {
                GridRow = 1,
                Spacing = 8,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            ButtonOk = new ImageTextButton
            {
                Text = "Ok"
            };

            ButtonOk.Click += (sender, args) =>
            {
                Result = true;
                Close();
            };

            buttonsGrid.Widgets.Add(ButtonOk);

            ButtonCancel = new ImageTextButton
            {
                Text = "Cancel",
                GridColumn = 1
            };

            ButtonCancel.Click += (sender, args) =>
            {
                Result = false;
                Close();
            };

 
            buttonsGrid.Widgets.Add(ButtonCancel);

            windowGrid.Widgets.Add(Amount);
            windowGrid.Widgets.Add(buttonsGrid);

            InternalChild.Widgets.Add(windowGrid);

            

        }

        private void Amount_ValueChanging(object sender, Myra.Utility.ValueChangingEventArgs<string> e)
        {
            e.NewValue = Regex.Match(e.NewValue, @"\d+").Value;
        }

        public override void OnKeyDown(Keys k)
        {}

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.UiScreens.UI
{
    public class PickableItemList : Panel
    {

        public class Item
        {
            public string Text { get; }
            public object LinkedData { get; }
            public Item(string text, object linkedData)
            {
                Text = text;
                LinkedData = linkedData;
            }
        }

        private const int lineWidth = 20;
        public List<Item> Items { get; set; }

        public Item SelectedItem { get; set; }
        int linesInView;
        private int scrolledTo = 0;

        public int SelectedItemIndex
        {
            get {return Items.IndexOf(SelectedItem); }
        }

        public void Select(int index)
        {
            if (Items.Any())
            {
                SelectedItem = Items[index];
                RebuildList();
            }

            var distanceToScrolled = (index - scrolledTo);
            if (distanceToScrolled > linesInView-1)
            {
                ScrollTo(index - (linesInView - 1));
            }
            else if(distanceToScrolled<0)
            {
                ScrollTo(scrolledTo + distanceToScrolled);
            }


        }


        public void ScrollTo(int index)
        {
            scrolledTo = index;
            Scrollbar.Value = index* lineWidth;
        }


        public PickableItemList(Vector2 size, Vector2 position):base(size,PanelSkin.ListBackground,Anchor.TopLeft,position)
        { 
            Items = new List<Item>();
            PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;

            _scrollbar = new VerticalScrollbar(0, 10, Anchor.CenterRight, offset: new Vector2(-8, 0));
            _scrollbar.Value = 0;
            _scrollbar.Visible = true;
            AddChild(_scrollbar, false);


        }

        public void AddItem(Item item)
        {
            Items.Add(item);
            RebuildList();
        }

        private void RebuildList()
        {
            //Children.Clear();
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (SelectedItem == item)
                {
                    AddChild(new Label(">" + Items[i].Text, Anchor.TopLeft, offset: new Vector2(0, i * lineWidth)));
                }
                else
                {
                    AddChild(new Label(Items[i].Text, Anchor.TopLeft, offset: new Vector2(0, i * lineWidth)));
                }

            }

           
            linesInView = (int) ((this.Size.Y-60) / lineWidth);
            linesInView.ToString();

            Scrollbar.Max = (uint) Items.Count* lineWidth;
            Scrollbar.StepsCount = Scrollbar.Max;
        }

        public void ClearItems()
        {
            Items.Clear();
            //Children.Clear();
        }
    }
}

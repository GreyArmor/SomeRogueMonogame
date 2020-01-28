using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace NamelessRogue.Engine.Engine.UiScreens.UI
{

    public class TableCell : Panel
    {}

    public class TableItem : Panel, ISelectorItem
    {
        private Grid _internalGrid;
        public TableItem(int columns)
        {
            _internalGrid = new Grid();
            _internalGrid.ColumnSpacing = columns;
            Cells = new ObservableCollection<TableCell>();
            for (int i = 0; i < columns; i++)
            {
                var tableCell = new TableCell();
                Cells.Add(tableCell);
                _internalGrid.Widgets.Add(tableCell);
            }
            this.Widgets.Add(_internalGrid);
            Columns = columns;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        public ObservableCollection<TableCell> Cells { get; set; }
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (isSelected)
                {
                    Background = new SolidBrush(Color.WhiteSmoke);
                }
                else
                {
                    Background = new SolidBrush(Color.Transparent);
                }
            }
        }

        public int Columns { get; }
    }
    public class Table : Selector<ScrollViewer, TableItem>
    {

        public Table() : base(new ScrollViewer())
        {
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        protected override void InsertItem(TableItem item, int index)
        {
          //  Items.Insert(index,item);
        }

        protected override void RemoveItem(TableItem item)
        {
           // Items.Remove(item);
        }

        protected override void Reset()
        {}
    }
}

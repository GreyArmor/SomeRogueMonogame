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
    {
        public TableCell()
        {
            Visible = true;
        }
    }

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
                tableCell.GridColumn = i;
            }
            this.Widgets.Add(_internalGrid);
            Columns = columns;
            Visible = true;
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
                    Background = new SolidBrush(Color.Gray);
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
        private readonly VerticalStackPanel _box;
        public Table() : base(new ScrollViewer())
        {
            Visible = true;
            _box = new VerticalStackPanel();
            InternalChild.Content = _box;

        }

        protected override void InsertItem(TableItem item, int index)
        {
            _box.Widgets.Insert(index, item);
        }

        protected override void RemoveItem(TableItem item)
        {
            _box.Widgets.Remove(item);
            if (SelectedItem == item)
            {
                SelectedItem = null;
            }
        }

        protected override void Reset()
        {
            while (_box.Widgets.Count > 0)
            {
                RemoveItem((TableItem)_box.Widgets[0]);
            }
        }
    }
}

using System.Collections.ObjectModel;

namespace Kogler.Framework
{
    public class MenuItemsCollection : ObservableCollection<MenuItem>
    {
        public MenuItemsCollection() : this(null) { }

        public MenuItemsCollection(MenuItem parent)
        {
            Parent = parent;
        }

        public MenuItem Parent { get; set; }

        protected override void InsertItem(int index, MenuItem item)
        {
            item.Parent = this.Parent;
            base.InsertItem(index, item);
        }

        public void AddToHierarchy(params MenuItem[] items)
        {
            
        }
    }
}
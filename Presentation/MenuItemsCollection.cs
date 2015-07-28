using System;
using System.Collections.ObjectModel;
using System.Linq;

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
            item.Parent = Parent;
            base.InsertItem(index, item);
        }

        public void AddToHierarchy(params MenuItem[] items)
        {
            var siblings = items.Where(i => string.IsNullOrEmpty(i.GroupName)).ToArray();
            var childs = items.Where(i => !siblings.Contains(i));
            foreach (var sibling in siblings)
            {
                Add(sibling);
            }
            foreach (var child in childs)
            {
                var groups = child.GroupName.Split(new[]{ MenuItem.GroupNameSeparator}, StringSplitOptions.RemoveEmptyEntries);
                child.GroupName = string.Join(MenuItem.GroupNameSeparator, groups.Skip(1));
                var parent = Items.FirstOrDefault(a => a.Text == groups[0]);
                if (parent == null)
                {
                    parent = new MenuItem {Text = groups[0]};
                    Add(parent);
                }
                parent.Items.AddToHierarchy(child);
            }
        }
    }
}
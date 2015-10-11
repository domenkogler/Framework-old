using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

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

        public void AddToHierarchy(IEnumerable<MenuItem> items)
        {
            AddToHierarchy(items.ToArray());
        }

        public void AddToHierarchy(params MenuItem[] items)
        {
            AddToHierarchyRecursive(items);
            ResetHierrarchyGroupNames();
        }

        private void AddToHierarchyRecursive(params MenuItem[] items)
        {
            var siblings = items.Where(i => string.IsNullOrEmpty(i.HierrarchyGroupName)).ToArray();
            var childs = items.Where(i => !siblings.Contains(i));
            foreach (var sibling in siblings)
            {
                Add(sibling);
            }
            foreach (var child in childs)
            {
                var groups = child.HierrarchyGroupName.Split(new[] { MenuItem.GroupNameSeparator }, StringSplitOptions.RemoveEmptyEntries);
                child.HierrarchyGroupName = string.Join(MenuItem.GroupNameSeparator, groups.Skip(1));
                var parent = Items.FirstOrDefault(a => a.Text == groups[0]);
                if (parent == null)
                {
                    parent = new MenuItem { Text = groups[0] };
                    Add(parent);
                }
                parent.Items.AddToHierarchyRecursive(child);
            }
        }

        private void ResetHierrarchyGroupNames()
        {
            Items.SelectMany(i => i.Deep).Apply(m => m.HierrarchyGroupName = m.GroupName);
        }
    }
}
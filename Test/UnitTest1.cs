using System;
using Kogler.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MenuItemsCollection()
        {
            var collection = new MenuItemsCollection();
            var s = MenuItem.GroupNameSeparator;
            var items = new[]
            {
                new MenuItem {GroupName = $"Group1{s}Subgroup1", Text = "Item1"},
                new MenuItem {GroupName = $"Group2{s}Subgroup1{s}Subgroup1", Text = "Item2"},
                new MenuItem {GroupName = $"Group1{s}Subgroup1", Text = "Item3"},
                new MenuItem {GroupName = $"Group2", Text = "Item4"},
                new MenuItem {GroupName = $"Group2{s}Subgroup1{s}Subgroup2", Text = "Item5"},
                new MenuItem {GroupName = $"Group1", Text = "Item6"},
                new MenuItem {GroupName = $"Group1{s}Subgroup1", Text = "Item7"},
            };

            collection.AddToHierarchy(items);
            Assert.AreEqual(collection.Count, 2);
            Assert.AreEqual(collection[0].Items.Count, 2);
        }
    }
}

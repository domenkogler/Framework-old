using Kogler.Framework;
using Xunit;

namespace Framework.Test
{
    public class MenuItemCollectionsTests
    {
        [Fact]
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
            Assert.Equal(collection.Count, 2);
            Assert.Equal(collection[0].Text, "Group1");
            Assert.Equal(collection[0].Items[1].Text, "Item6");
            Assert.Equal(collection[0].Items[0].Items[2].Text, "Item7");
            Assert.Equal(collection[1].Items[0].Items[1].Items[0].Text, "Item5");
        }
    }
}

using Kogler.Framework;
using Shouldly;
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
            collection.Count.ShouldBe(2);
            collection[0].Text.ShouldBe("Group1");
            collection[0].Items[1].Text.ShouldBe("Item6");
            collection[0].Items[0].Items[2].Text.ShouldBe("Item7");
            collection[1].Items[0].Items[1].Items[0].Text.ShouldBe("Item5");
        }
    }
}

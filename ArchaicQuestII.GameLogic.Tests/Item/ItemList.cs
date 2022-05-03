using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Item;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Item
{

    public class ItemListTest
    {
        private ItemList items = new ItemList();


        [Fact]
        public void Returns_correct_item_count()
        {
            var apple = new GameLogic.Item.Item();
            apple.Id = 0;
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var sword = new GameLogic.Item.Item();
            sword.Id = 1;
            sword.Name = "sword";
            sword.Description = new Description()
            {
                Room = "sword"
            };

            this.items.Add(apple);
            this.items.Add(sword);
            this.items.Add(sword);

            var expected = new List<ItemObj> { new ItemObj() { Id = 0, Name = "apple" }, new ItemObj() { Id = 1, Name = "(2) sword" } };

            Assert.Equal(items.List(true).ToList()[0].Name, expected[0].Name);
            Assert.Equal(items.List(true).ToList()[1].Name, expected[1].Name);
        }

    }
}

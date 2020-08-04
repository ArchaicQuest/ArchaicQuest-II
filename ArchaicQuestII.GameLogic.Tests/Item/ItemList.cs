using Moq;
using System;
using System.Collections.Generic;
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
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var sword = new GameLogic.Item.Item();
            sword.Name = "sword";
            sword.Description = new Description()
            {
                Room = "sword"
            };

            this.items.Add(apple);
            this.items.Add(sword);
            this.items.Add(sword);

            var expected = new List<string> { "apple", "(2) sword" };

            Assert.Equal(items.List(), expected);
        }

    }
}

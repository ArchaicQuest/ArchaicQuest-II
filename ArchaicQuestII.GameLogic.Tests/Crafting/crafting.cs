using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Item;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Crafting
{
   
    public class CraftingTest
    {
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<ICache> _cache;
        private readonly Mock<IUpdateClientUI> _updateClientUi;
        private readonly Mock<IDice> _dice;

        public CraftingTest()
        {
            _writer = new Mock<IWriteToClient>();
            _cache = new Mock<ICache>();
            _updateClientUi = new Mock<IUpdateClientUI>();
            _dice = new Mock<IDice>();
        }

        [Fact]
        public void Returns_error_if_player_is_not_standing()
        {

            var player = new Player()
            {
                ConnectionId = "1",
                Name = "Malleus",
                Status = CharacterStatus.Status.Sleeping,
                Inventory = new ItemList()
            };

            new GameLogic.Crafting.Crafting(_writer.Object, _cache.Object, _dice.Object, _updateClientUi.Object).ListCrafts(player);

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("<p>You can't do that while sleeping.</p>")), "1"), Times.Once());
        }

        [Fact]
        public void Returns_error_if_no_materials()
        {

            var player = new Player()
            {
                ConnectionId = "1",
                Name = "Malleus",
                Status = CharacterStatus.Status.Standing,
                Inventory = new ItemList()
            };
 
           new GameLogic.Crafting.Crafting(_writer.Object, _cache.Object, _dice.Object, _updateClientUi.Object).ListCrafts(player);

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("<p>You don't have any materials to craft a thing.</p>")), "1"), Times.Once());
        }

        [Fact]
        public void Returns_error_if_no_recipes_configured()
        {

            var item = new GameLogic.Item.Item()
            {
                Name = "Wood",
                ItemType = GameLogic.Item.Item.ItemTypes.Material,
            };

            var player = new Player()
            {
                ConnectionId = "1",
                Name = "Malleus",
                Status = CharacterStatus.Status.Standing,
                Inventory = new ItemList(){item}
            };

            new GameLogic.Crafting.Crafting(_writer.Object, _cache.Object, _dice.Object, _updateClientUi.Object).ListCrafts(player);

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("<p>No crafting recipes have been set up.</p>")), "1"), Times.Once());
        }

        [Fact]
        public void Returns_list_of_recipes()
        {

            var item = new GameLogic.Item.Item()
            {
                Name = "Wood",
                ItemType = GameLogic.Item.Item.ItemTypes.Material,
            };

            var player = new Player()
            {
                ConnectionId = "1",
                Name = "Malleus",
                Status = CharacterStatus.Status.Standing,
                Inventory = new ItemList() { item, item }
            };

            var recipe = new CraftingRecipes()
            {
                Description = "",
                Title = "Wooden sword",
                CraftingMaterials = new List<CraftingMaterials>()
                {
                    new CraftingMaterials()
                    {
                        Quantity = 2,
                        Material = "Wood"
                    }
                }
            };

            var listOfRecipes = new List<CraftingRecipes>();
            listOfRecipes.Add(recipe);
            _cache.Setup(x => x.GetCraftingRecipes()).Returns(listOfRecipes);

            new GameLogic.Crafting.Crafting(_writer.Object, _cache.Object, _dice.Object, _updateClientUi.Object).ListCrafts(player);

            var sb = new StringBuilder();
            sb.Append("<p>You can craft the following items:</p");
            sb.Append("<table class='simple'>");
            sb.Append($"<tr><td>Name</td><td>Materials</td></tr>");
            sb.Append($"<tr><td>Wooden sword</td><td>Wood x2, </td></tr>");
            sb.Append($"</table>");
            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains(sb.ToString())), "1"), Times.Once());
        }

        [Fact]
        public void Returns_error_if_not_enough_quantity_of_materials() {

            var item = new GameLogic.Item.Item()
            {
                Name = "Wood",
                ItemType = GameLogic.Item.Item.ItemTypes.Material,
            };

            var player = new Player()
            {
                ConnectionId = "1",
                Name = "Malleus",
                Status = CharacterStatus.Status.Standing,
                Inventory = new ItemList() { item }
            };

            var recipe = new CraftingRecipes()
            {
                Description = "",
                Title = "Wooden sword",
                CraftingMaterials = new List<CraftingMaterials>()
                {
                    new CraftingMaterials()
                    {
                        Quantity = 2,
                        Material = "Wood"
                    }
                }
            };

            var listOfRecipes = new List<CraftingRecipes>();
            listOfRecipes.Add(recipe);
            _cache.Setup(x => x.GetCraftingRecipes()).Returns(listOfRecipes);

            new GameLogic.Crafting.Crafting(_writer.Object, _cache.Object, _dice.Object, _updateClientUi.Object).ListCrafts(player);
 
            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("<p>No crafting recipes found with the current materials you have.</p>")), "1"), Times.Once());
        }

    }
}

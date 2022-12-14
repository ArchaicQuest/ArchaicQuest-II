using Moq;
using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Crafting
{

    public class CookingTest
    {
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<IUpdateClientUI> _updateClientUi;
        private readonly Mock<IDice> _dice;
        private readonly Mock<IGain> _gain;

        public CookingTest()
        {
            _writer = new Mock<IWriteToClient>();
            _updateClientUi = new Mock<IUpdateClientUI>();
            _dice = new Mock<IDice>();
            _gain = new Mock<IGain>();
        }

        [Fact]
        public void Returns_food_item()
        {

            var player = new Player()
            {
                ConnectionId = "1",
                Name = "Malleus",
                Status = CharacterStatus.Status.Sleeping,
                Inventory = new ItemList()
            };

            var room = new Room();


            var pot = new GameLogic.Item.Item()
            {
                Name = "cook pot",
                Container = new Container()
                {
                    IsOpen = true,
                    Items = new ItemList()
                    {
                        new GameLogic.Item.Item()
                        {
                            Name = "Carrot",
                            ItemType = GameLogic.Item.Item.ItemTypes.Material,
                            Modifier = new Modifier()
                            {
                                HP = 5
                            }
                        },
                        new GameLogic.Item.Item()
                        {
                            Name = "Mango",
                            ItemType = GameLogic.Item.Item.ItemTypes.Material,
                            Modifier = new Modifier()
                            {
                                Mana = 10
                            }
                        },
                        new GameLogic.Item.Item()
                        {
                            Name = "Melon",
                            ItemType = GameLogic.Item.Item.ItemTypes.Material,
                            Modifier = new Modifier()
                            {
                                HP = 5
                            }
                        }
                    }
                },
                ItemType = GameLogic.Item.Item.ItemTypes.Cooking
            };

            _dice.Setup(x => x.Roll(1, It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);

            var ingredient = new Tuple<GameLogic.Item.Item, int>(new GameLogic.Item.Item()
            {
                Name = "Mango",
                ItemType = GameLogic.Item.Item.ItemTypes.Material,
                Modifier = new Modifier()
                {
                    Mana = 10
                }
            }, 3);



            var ingredients = new List<Tuple<GameLogic.Item.Item, int>>()
            {
                ingredient
            };


            var food = new Cooking(_writer.Object, _dice.Object, _gain.Object, _updateClientUi.Object).GenerateCookedItem(player, room, ingredients);

            Assert.NotNull(food);
        }




    }
}

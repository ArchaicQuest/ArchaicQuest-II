using Moq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Item
{

    public class RandomItem
    {
        private readonly Mock<RandomWeapons> _randomWeapon;
        private readonly Mock<RandomLeatherItems> _randomLeatherItems;
        private readonly Mock<RandomStuddedLeatherItems> _randomStuddedLeatherArmour;
        private readonly Mock<RandomChainMailItems> _randomChainMailArmour;
        private readonly Mock<RandomPlateMailItems> _randomPlateMailArmour;
        private readonly Mock<RandomClothItems> _randomClothItems;

        private readonly Mock<IItemHandler> _itemHandler;

        public RandomItem()
        {
            _randomWeapon = new Mock<RandomWeapons>();
            _randomClothItems = new Mock<RandomClothItems>();
            _randomLeatherItems = new Mock<RandomLeatherItems>();
            _randomStuddedLeatherArmour = new Mock<RandomStuddedLeatherItems>();
            _randomChainMailArmour = new Mock<RandomChainMailItems>();
            _randomPlateMailArmour = new Mock<RandomPlateMailItems>();
            _itemHandler = new Mock<IItemHandler>();
        }

        [Fact]
        public void Returns_RandomItem()
        {

            var player = new Player
            {
                Level = 5
            };

            _randomWeapon.Setup(x => x.CreateRandomWeapon(player, false)).Returns(new GameLogic.Item.Item()
            {
                Name = "a Steel Axe",
                ItemType = GameLogic.Item.Item.ItemTypes.Weapon,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = 0,
            });

            var item = _itemHandler.Object.WeaponDrop(player);
            Assert.Equal("a steel axe", item.Name.ToLower());
            Assert.Equal(item.Level, 5);
        }

        [Fact]
        public void Returns_a_pair_of_trousers()
        {

            var player = new Player
            {
                Level = 5
            };

            var item = new RandomClothItems().CreateRandomItem(player, false);

            Assert.Equal("a pair of silk cloth trousers", item.Name);
            Assert.Equal(5, item.Level);
        }

        [Fact]
        public void Returns_some_trousers()
        {

            var player = new Player
            {
                Level = 5
            };

            var item = new RandomClothItems().CreateRandomItem(player, false);

            Assert.Equal("some silk cloth trousers", item.Name);
            Assert.Equal(5, item.Level);
        }

    }
}

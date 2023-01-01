using Moq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Item
{

    public class RandomItem
    {
        private readonly Mock<IRandomWeapon> _randomWeapon;
        private readonly Mock<IRandomLeatherItems> _randomLeatherItems;
        private readonly Mock<IRandomStuddedLeatherArmour> _randomStuddedLeatherArmour;
        private readonly Mock<IRandomChainMailArmour> _randomChainMailArmour;
        private readonly Mock<IRandomPlateMailArmour> _randomPlateMailArmour;
        private readonly Mock<IRandomClothItems> _randomClothItems;

        public RandomItem()
        {
            _randomWeapon = new Mock<IRandomWeapon>();
            _randomClothItems = new Mock<IRandomClothItems>();
            _randomLeatherItems = new Mock<IRandomLeatherItems>();
            _randomStuddedLeatherArmour = new Mock<IRandomStuddedLeatherArmour>();
            _randomChainMailArmour = new Mock<IRandomChainMailArmour>();
            _randomPlateMailArmour = new Mock<IRandomPlateMailArmour>();
        }

        [Fact]
        public void Returns_RandomItem()
        {

            var player = new Player();
            player.Level = 5;
            
            _randomWeapon.Setup(x => x.CreateRandomWeapon(player, false)).Returns(new GameLogic.Item.Item()
            {
                Name = "a Steel Axe",
                ItemType = GameLogic.Item.Item.ItemTypes.Weapon,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = 0,
            });

            var item = new GameLogic.Item.RandomItem(_randomWeapon.Object, _randomClothItems.Object, _randomLeatherItems.Object, _randomStuddedLeatherArmour.Object, _randomChainMailArmour.Object, _randomPlateMailArmour.Object).WeaponDrop(player);
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

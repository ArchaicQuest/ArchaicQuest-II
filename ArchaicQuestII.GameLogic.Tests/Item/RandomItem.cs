using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;
using Xunit;
using Damage = ArchaicQuestII.GameLogic.Item.Damage;

namespace ArchaicQuestII.GameLogic.Tests.Item
{
   
    public class RandomItem
    {
        private readonly Mock<IDice> _dice;
        private readonly Mock<IRandomWeapon> _randomWeapon;
        public RandomItem()
        {

            _dice = new Mock<IDice>();
            _randomWeapon = new Mock<IRandomWeapon>();
        }
    
        [Fact]
        public void Returns_RandomItem()
        {

            var player = new Player();
            player.Level = 5;

            _dice.Setup(x => x.Roll(1, 0, 15)).Returns(5);

            _dice.Setup(x => x.Roll(1, 0, 11)).Returns(9);
            _randomWeapon.Setup(x => x.CreateRandomWeapon(player, false)).Returns(new GameLogic.Item.Item()
            {
                Name = "a Steel Axe",
                ItemType = GameLogic.Item.Item.ItemTypes.Weapon,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = 0,

            });
          

       
            var item = new GameLogic.Item.RandomItem(_dice.Object, _randomWeapon.Object).WeaponDrop(player);
            Assert.Equal(item.Name, "a Steel Axe");
            Assert.Equal(item.Level,5);
        }

    }
}

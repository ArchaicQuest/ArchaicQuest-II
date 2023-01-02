using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Core
{
    public class DiceBag
    {
        [Fact]
        public void Returns_fixed_roll()
        {
            var die = Utilities.DiceBag.Roll("10");

            Assert.Equal(10, die);
        }
        
        [Fact]
        public void Returns_correct_standard_roll()
        {
            var die = Utilities.DiceBag.Roll("2d10");

            Assert.InRange(die, 2, 20);
        }
        
        [Fact]
        public void Returns_incorrect_roll()
        {
            var die = Utilities.DiceBag.Roll("dfeef");

            Assert.Equal(0, die);
        }
        
        [Fact]
        public void Returns_correct_param_roll()
        {
            var die = Utilities.DiceBag.Roll(1, 5,5);

            Assert.Equal(5, die);
        }
        
        [Fact]
        public void Returns_correct_modified_roll()
        {
            var die1 = Utilities.DiceBag.Roll("1d1+5");
            var die2 = Utilities.DiceBag.Roll("1d1-5");

            Assert.Equal(6, die1);
            Assert.Equal(-4, die2);
        }
        
        [Fact]
        public void Returns_correct_advantage_roll()
        {
            var die1 = Utilities.DiceBag.Roll("+1d6");
            var die2 = Utilities.DiceBag.Roll("-1d6");

            Assert.InRange(die1, 1, 6);
            Assert.InRange(die2, 1, 6);
        }
    }
}

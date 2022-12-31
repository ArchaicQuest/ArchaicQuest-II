using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Core
{
    public class Helpers
    {
        [Fact]
        public void Returns_correct_nth_cont()
        {
            var nthObject = Utilities.Helpers.findNth("2.corpse");

            Assert.Equal(2, nthObject.Item1);

        }

        [Fact]
        public void Returns_correct_nth_object()
        {
            var nthObject = Utilities.Helpers.findNth("2.corpse");

            Assert.Equal("corpse", nthObject.Item2);

        }

        [Fact]
        public void Returns_correct_null_if_delimiter_not_found()
        {
            var nthObject = Utilities.Helpers.findNth("get apple corpse");

            Assert.Equal(-1, nthObject.Item1);

        }


        [Fact]
        public void Returns_correct_null_if_not_number()
        {
            var nthObject = Utilities.Helpers.findNth("get ab.apple corpse");

            Assert.Equal(-1, nthObject.Item1);

        }
    }
}

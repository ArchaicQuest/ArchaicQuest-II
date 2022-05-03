using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Item
{
    public interface IItemList
    {
        public IEnumerable<ItemObj> List(bool isRoom);
        public IEnumerable<ItemObj> List(IEnumerable<Item> items, params object[] isRoom);
    }
}

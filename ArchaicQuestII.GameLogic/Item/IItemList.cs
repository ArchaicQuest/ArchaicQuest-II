using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Item
{
   public interface IItemList
   {
       public IEnumerable<String> List();
       public IEnumerable<String> List(IEnumerable<Item> items);
   }
}

using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Item.RandomItemTypes
{
    public interface IRandomWeapon
    {
        public Item CreateRandomWeapon(Player player, bool legendary);
    }
}

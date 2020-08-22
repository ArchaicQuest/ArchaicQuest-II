using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
    public interface ICombat
    {
        public Player FindTarget(string target, Room room, bool isMurder);

        public Item.Item GetWeapon(Player player);

        public void HarmTarget(Player victim, int damage);

        public bool IsTargetAlive(Player victim);

        public void DisplayDamage(Player player, Player target, Room room, Item.Item weapon, int damage);

        public void Fight(Player player, string victim, Room room, bool isMurder);

       // public void AutoAttack(Player player, Player target, Room room, bool isMurder);
    }
}

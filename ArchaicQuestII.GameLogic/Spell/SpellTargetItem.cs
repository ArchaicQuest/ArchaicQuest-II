using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class SpellTargetItem
    {

        private readonly IWriteToClient _writer;
        public SpellTargetItem(IWriteToClient writer)
        {
            _writer = writer;

        }

        public Item.Item GetTargetItem(string target, Player player)
        {
            return player.Inventory.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }


        public Item.Item CheckTargetItem(Spell.Model.Spell spell, string target, Room room, Player player)
        {
            var item = GetTargetItem(target, player);

            if (item == null)
            {
                _writer.WriteLine("You can't find that item on you.", player.ConnectionId);
                return null;
            }

            return item;
        }

        /// <summary>
        /// Finds the target object of the spell
        /// </summary>
        /// <param name="spell">The Spell being cast</param>
        /// <param name="target">Target comes from the command param. cast enchant sword</param>
        /// <param name="room">Room the player is in</param>
        /// <param name="player">The PC casting the spell</param>
        /// <returns></returns>
        public Item.Item ReturnTargetObject(Spell.Model.Spell spell, string target, Room room, Player player)
        {

            // casting spell on item in inventory or worn
            // example spells, cast enchant sword, cast light sword
            if ((spell.ValidTargets & ValidTargets.TargetObjectInventory) != 0 || (spell.ValidTargets & ValidTargets.TargetObjectEquipped) != 0)
            {
                return CheckTargetItem(spell, target, room, player);
            }

            // TODO: unsure on these:

            //if ((spell.ValidTargets & ValidTargets.TargetObjectRoom) != 0 )
            //{
            //    return CheckTargetItem(spell, target, room, player);
            //}

            //if ((spell.ValidTargets & ValidTargets.TargetObjectWorld) != 0)
            //{
            //    return CheckTargetItem(spell, target, room, player);
            //}

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class SpellTargetCharacter
    {

        private readonly IWriteToClient _writer;
        public SpellTargetCharacter(IWriteToClient writer)
        {
            _writer = writer;

        }

        public Player GetTarget(string target, Room room)
        {
           return room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public Player CheckTarget(Spell.Model.Spell spell, string target, Room room, Player player)
        {
            var victim = GetTarget(target, room);

            if (victim == null)
            {
                _writer.WriteLine(
                    (spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0
                        ? "You can't find them in the realm."
                        : "You don't see them here.", player.ConnectionId);

                return null;
            }

            if (spell.StartsCombat && victim.Id == player.Id)
            {
                _writer.WriteLine("Casting this on yourself is a bad idea.", player.ConnectionId);
                return null;
            }

            return victim;
        }


        public Player ReturnTarget(Spell.Model.Spell spell, string target, Room room, Player player)
        {
            
         
            if ((spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0)
            {
                if (string.IsNullOrEmpty(target) || target == "self")
                {
                    return player;
                }

                _writer.WriteLine("You can only this spell on yourself", player.ConnectionId);
                return null;
            }

            //If no argument, target is the PC/NPC the player is fighting
            if (player.Status == CharacterStatus.Status.Fighting && (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0)
            {
                if (string.IsNullOrEmpty(target))
                {
                    return CheckTarget(spell, player.Target, room, player);
                }
            }
            // casting spell on PC/NPC in room
            // example spells, magic missile, minor wounds
            if ((spell.ValidTargets & ValidTargets.TargetPlayerRoom) != 0)
            {
                return CheckTarget(spell, target, room, player);
            }

            // casting spell on PC/NPC in the world
            // example spells, gate, summon, portal
            if ((spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0)
            {
                return CheckTarget(spell, target, room, player);
            }


            // casting spell on PC/NPC in the world
            // example spells, gate, summon, portal
            if ((spell.ValidTargets & ValidTargets.TargetObjectInventory) != 0 || (spell.ValidTargets & ValidTargets.TargetObjectEquipped) != 0)
            {
                //find victim from player cache instead
                var item = player.Inventory.FirstOrDefault(x =>
                    x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

                if (item == null)
                {
                    //target not found
                    return null;
                }


                //  return item;
            }


            return player;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    /// <summary>
    /// Random functions to go here unless there is
    /// enough related random functions to move out to their own class
    /// </summary>
   public static class Helpers
    {
        public static string GetPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "her",
                "Male" => "his",
                _ => "their",
            };
        }

        public static Player FindPlayer(string target, Room room)
        {
            return room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Player FindMob(string target, Room room)
        {
            return (Player)room.Mobs.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Item.Item FindItemInInventory(string target, Player player)
        {
            return player.Inventory.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Item.Item FindItemInRoom(string target, Room room)
        {
            return room.Items.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Player ReturnTarget(Spell.Model.Spell spell, string target, Room room, Player player)
        {

            //If no arguments, target is sell
            if ((spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0)
            {
                if (string.IsNullOrEmpty(target) || target == "self")
                {
                    return player;
                }

                //You can only cast this spell on yourself
                return null;
            }

            //If no argument, target is the PC/NPC the player is fighting
            if (player.Status == CharacterStatus.Status.Fighting && (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0)
            {
                if (string.IsNullOrEmpty(target))
                {
                    return player; //player.target
                }
            }
            // casting spell on PC/NPC in room
            // example spells, magic missile, minor wounds
            if ((spell.ValidTargets & ValidTargets.TargetPlayerRoom) != 0)
            {
                var victim = room.Players.FirstOrDefault(x =>
                    x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

                if (victim == null)
                {
                    //target not found
                    return null;
                }

                if (spell.StartsCombat && victim.Id == player.Id)
                {
                    // casting this on yourself is a bad idea
                    return null;
                }

                return victim;
            }

            // casting spell on PC/NPC in the world
            // example spells, gate, summon, portal
            if ((spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0)
            {
                //find victim from player cache instead
                var victim = room.Players.FirstOrDefault(x =>
                    x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

                if (victim == null)
                {
                    //target not found
                    return null;
                }

                if (spell.StartsCombat && victim.Id == player.Id)
                {
                    // casting this on yourself is a bad idea
                    return null;
                }

                return victim;
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

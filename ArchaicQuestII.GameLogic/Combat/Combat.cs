using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
   public class Combat
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;
        public Combat(IWriteToClient writer, IUpdateClientUI clientUi)
        {
            _writer = writer;
            _clientUi = clientUi;
        }

        public Player FindTarget(string target, Room room, bool isMurder)
        {
            // If mob
            if (!isMurder)
            {
                return (Player)room.Mobs.FirstOrDefault(x => x.Name.Contains(target));
            }

            return (Player)room.Players.FirstOrDefault(x => x.Name.StartsWith(target));
        }

        public int OffensivePoints(Player player)
        {
            var offensePoints = player.Attributes.Attribute[EffectLocation.Strength] / 5
                                   + player.Attributes.Attribute[EffectLocation.Dexterity] / 10
                                   + player.Attributes.Attribute[EffectLocation.HitRoll];
            var weaponSkill = 100; //weapon types are hardcoded so make hardcoded skills for weapon types

             return offensePoints + offensePoints * weaponSkill / 100;
        }

        public int DefensivePoints(Player player)
        {
            return player.Attributes.Attribute[EffectLocation.Dexterity] / 5 + player.Attributes.Attribute[EffectLocation.Strength] / 10;
        }

        public int BlockPoints(Player player)
        {
            var offensePoints = player.Attributes.Attribute[EffectLocation.Strength] / 5
                                + player.Attributes.Attribute[EffectLocation.Dexterity] / 10;
                                
            var blockSkill = 100; //weapon types are hardcoded so make hardcoded skills for weapon types

            return offensePoints + offensePoints * blockSkill / 100;
        }

        public int ToBlockChance(Player player, Player target)
        {
            return BlockPoints(player) * 100 / (BlockPoints(player) + DefensivePoints(target));
        }

        public int ToHitChance(Player player, Player target)
        {
            return OffensivePoints(player) * 100 / (OffensivePoints(player) + DefensivePoints(target));
        }

        public int DamageReduction(Player defender, int damage)
        {
            var ArRating = defender.ArmorRating.Armour + 1;
            var armourReduction = ArRating / (double)damage;

            if (armourReduction > 4)
            {
                armourReduction = 4;
            }
            else if (armourReduction <= 0)
            {
                armourReduction = 1;
            }

            return (int)armourReduction;
        }

        public int CalculateDamage(Player player, Player target)
        {
            var weapon = GetWeapon(player);
            var damage = 0;

            if (weapon != null)
            {
                damage = Dice.Roll(1, weapon.Damage.Minimum, weapon.Damage.Maximum);

            }
            else
            {
                //Hand to hand
                damage = Dice.Roll(1, 1, 6);
            }

            // Enhanced Damage Skill check
            // increase damage is player has enhanced damage skill
            if (false)
            {
            }

            // calculate damage reduction based on target armour
             var armourReduction = DamageReduction(target, damage);

             //refactor this shit
             var strengthMod = 0.5 + player.Attributes.Attribute[EffectLocation.Strength] / (double)100;
             var levelDif = player.Level - target.Level <= 0 ? 1 : player.Level - target.Level;
             var levelMod = levelDif / 2 <= 0 ? 1 : levelDif / 2;
             var enduranceMod = player.Attributes.Attribute[EffectLocation.Moves]/ (double)player.MaxAttributes.Attribute[EffectLocation.Moves];
             var criticalHit = 0;
            
             if (target.Status == CharacterStatus.Status.Sleeping || target.Status == CharacterStatus.Status.Stunned || target.Status == CharacterStatus.Status.Resting)
             {
                 criticalHit = 2;
             }


             int totalDamage = (int)(damage * strengthMod * criticalHit * levelMod);

             if (armourReduction > 0)
             {
                 totalDamage = totalDamage / armourReduction;
             }

             if (totalDamage <= 0)
             {
                 totalDamage = 1;
             }


             return totalDamage;
        }

        public Item.Item GetWeapon(Player player)
        {
            return player.Equipped.Wielded;
        }

        public bool IsCriticalHit()
        {
            return Dice.Roll(1, 1, 20) == 20;
        }

        public bool DoesHit(int chance)
        {
            var roll = Dice.Roll(1, 1, 100);

            return roll switch
            {
                1 => false,
                100 => true,
                _ => roll > chance
            };
        }

        public void Fight(Player player, string victim, Room room, bool isMurder)
        {
            var target = FindTarget(victim, room, isMurder);

            if (target == null)
            {
                _writer.WriteLine("They are not here.", player.ConnectionId);
                return;
            }

            var chanceToHit = ToHitChance(player, target);
            var doesHit = DoesHit(chanceToHit);

            if (doesHit)
            {
                var weapon = GetWeapon(player);
                var hasEvadedDamage = false;

                // avoidance percentage can be improved by core skills 
                // such as improved parry, acrobatic etc 
                // instead of rolling a D10, roll a D6 for a close to 15% increase in chance

                //10% chance to attempt a dodge
                if(Dice.Roll(1,1,10) == 1)
                {

                }

                //10% chance to parry
                if (!hasEvadedDamage && Dice.Roll(1, 1, 10) == 2)
                {

                }

                // Block
                if (!hasEvadedDamage && Dice.Roll(1, 1, 10) == 3)
                {
                    var chanceToBlock = ToBlockChance(target, player);
                    var doesBlock = DoesHit(chanceToBlock);

                    if (doesBlock)
                    {

                    }
                    else
                    {
                        // block fail
                    }
                }

                //dodge
                //parry

                var damage = CalculateDamage(player, target);

                if (IsCriticalHit())
                {
                    // double damage
                    damage *= 2;
                }



                 
                // do damage
                // check if target is dead or alive
                // award xp if dead
                // create corpse container holding targets inventory and money
                // end combat
            }
            else
            {
                // miss message
                // gain improvements on weapon skill
            }

        }
    }
}
 
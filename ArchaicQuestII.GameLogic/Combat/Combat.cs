using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.AttackTypes;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
   public class Combat: ICombat
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _clientUi;
        private readonly IGain _gain;
        private readonly IDamage _damage;
        private readonly IFormulas _formulas;
        private readonly ICache _cache;
        public Combat(IWriteToClient writer, IUpdateClientUI clientUi, IDamage damage, IFormulas formulas, IGain gain, ICache cache)
        {
            _writer = writer;
            _clientUi = clientUi;
            _damage = damage;
            _formulas = formulas;
            _gain = gain;
            _cache = cache;
        }

        public Player FindTarget(Player attacker, string target, Room room, bool isMurder)
        {
            // If mob
            if (!isMurder && attacker.ConnectionId != "mob")
            {
                return (Player)room.Mobs.FirstOrDefault(x => x.Name.Contains(target));
            }

            if (attacker.ConnectionId == "mob")
            {
                return (Player)room.Players.FirstOrDefault(x => x.Name.Equals(target));
            }

            return (Player)room.Players.FirstOrDefault(x => x.Name.StartsWith(target));
        }

    

        public Item.Item GetWeapon(Player player)
        {
            return player.Equipped.Wielded;
        }


        public void HarmTarget(Player victim, int damage)
        {
            victim.Attributes.Attribute[EffectLocation.Hitpoints] -= damage;

            if (victim.Attributes.Attribute[EffectLocation.Hitpoints] < 0)
            {
                victim.Attributes.Attribute[EffectLocation.Hitpoints] = 0;
            }

        }

        public bool IsTargetAlive(Player victim)
        {
           return victim.Attributes.Attribute[EffectLocation.Hitpoints] > 0;
        }

        public void DisplayDamage(Player player, Player target, Room room, Item.Item weapon, int damage)
        {
            CultureInfo cc = CultureInfo.CurrentCulture;
            var damText = _damage.DamageText(damage);
            var attackType = "";
            if (weapon == null)
            {
                attackType = target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? target.DefaultAttack?.ToLower(cc): "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)?.ToLower(cc);
            }
    
            _writer.WriteLine($"<p>Your {attackType} {damText.Value} {target.Name.ToLower(cc)}. <span class='damage'>[{damage}]</span></p>", player.ConnectionId);
            _writer.WriteLine($"<p>{target.Name} {_formulas.TargetHealth(player, target)}</p>", player.ConnectionId);

            _writer.WriteLine($"<p>{player.Name}'s {attackType} {damText.Value} you. <span class='damage'>[{damage}]</span></p></p>", target.ConnectionId);
           
          

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name}'s {attackType} {damText.Value} {target.Name.ToLower(cc)}.</p>", pc.ConnectionId);
            }
        }

        public void DisplayMiss(Player player, Player target, Room room, Item.Item weapon)
        {
            CultureInfo cc = CultureInfo.CurrentCulture;
            var attackType = "";
            if (weapon == null)
            {
                attackType = target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? target.DefaultAttack?.ToLower(cc) : "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)?.ToLower(cc);
            }
            
            _writer.WriteLine($"<p>Your {attackType} misses {target.Name.ToLower(cc)}.</p>", player.ConnectionId);
            _writer.WriteLine($"<p>{player.Name}'s {attackType} misses you.</p>", target.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name}'s {attackType} misses {target.Name.ToLower(cc)}.</p>", pc.ConnectionId);
            }
        }

        public void Fight(Player player, string victim, Room room, bool isMurder)
        {
            var target = FindTarget(player, victim, room, isMurder);

            if (target == null)
            {
                _writer.WriteLine("<p>They are not here.</p>", player.ConnectionId);
                return;
            }

            player.Target = target.Name;
            player.Status = CharacterStatus.Status.Fighting;
            target.Status = CharacterStatus.Status.Fighting;
            target.Target = string.IsNullOrEmpty(target.Target) ? player.Name : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

           if(!_cache.IsCharInCombat(player.Id.ToString()))
            {
                _cache.AddCharToCombat(player.Id.ToString(), player);
            }

            if(!_cache.IsCharInCombat(target.Id.ToString()))
            {
                _cache.AddCharToCombat(target.Id.ToString(), target);
            }
            var chanceToHit = _formulas.ToHitChance(player, target);
            var doesHit = _formulas.DoesHit(chanceToHit);
            var weapon = GetWeapon(player);
            if (doesHit)
            {
             
                var hasEvadedDamage = false;

                // avoidance percentage can be improved by core skills 
                // such as improved parry, acrobatic etc 
                // instead of rolling a D10, roll a D6 for a close to 15% increase in chance

                var avoidanceRoll = new Dice().Roll(1, 1, 10);


                //10% chance to attempt a dodge
                if (avoidanceRoll == 1)
                {

                }

                //10% chance to parry
                if (!hasEvadedDamage && avoidanceRoll == 2)
                {

                }

                // Block
                if (!hasEvadedDamage && avoidanceRoll == 3)
                {
                    var chanceToBlock = _formulas.ToBlockChance(target, player);
                    var doesBlock = _formulas.DoesHit(chanceToBlock);

                    if (doesBlock)
                    {

                    }
                    else
                    {
                        // block fail
                    }
                }
 

                var damage = _formulas.CalculateDamage(player, target, weapon);

                if (_formulas.IsCriticalHit())
                {
                    // double damage
                    damage *= 2;
                }


                HarmTarget(target, damage); 
                _writer.WriteLine("chance to hit: " + chanceToHit, player.ConnectionId);
                DisplayDamage(player, target, room, weapon, damage);

                _clientUi.UpdateHP(target);

                if (!IsTargetAlive(target))
                {
                  
                    player.Target = String.Empty;
                    player.Status = CharacterStatus.Status.Standing;
                    target.Status = CharacterStatus.Status.Ghost;
                    target.Target = string.Empty;


                    foreach (var pc in room.Players)
                    {
                        if (pc.Name.Equals(target.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            continue;
                        }
                        _writer.WriteLine($"<p>Your blood freezes as you hear {target.Name}'s death cry.</p>", pc.ConnectionId);
                    }
                    _gain.GainExperiencePoints(player, target);
                    // death cry to nearby rooms
                }

               

                // award xp if dead
                // create corpse container holding targets inventory and money
                // end combat
            }
            else
            {
                _writer.WriteLine("chance to hit: " + chanceToHit, player.ConnectionId);
                DisplayMiss(player, target, room, weapon);
                // miss message
                // gain improvements on weapon skill
            }

        }
    }
}
 
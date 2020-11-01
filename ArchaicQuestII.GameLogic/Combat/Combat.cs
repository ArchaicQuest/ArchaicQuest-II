using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.AttackTypes;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Equipment;
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
                return (Player)room.Mobs.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));
            }

            if (attacker.ConnectionId == "mob")
            {
                return (Player)room.Players.FirstOrDefault(x => x.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase));
            }

            return (Player)room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
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
                attackType = player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? player.DefaultAttack?.ToLower(cc): "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)?.ToLower(cc);
            }
    
            _writer.WriteLine($"<p class='combat'>Your {attackType} {damText.Value} {target.Name.ToLower(cc)}. <span class='damage'>[{damage}]</span></p>", player.ConnectionId);
            _writer.WriteLine($"<p class='combat'>{target.Name} {_formulas.TargetHealth(player, target)}</p>", player.ConnectionId);

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

        public SkillList GetSkill(string skillName, Player player)
        {
            //this is breaking

            var skill =  player.Skills.FirstOrDefault(x =>
                x.SkillName.Replace(" ", "").Equals(skillName.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase));
            return skill;
        }

        public void DisplayMiss(Player player, Player target, Room room, Item.Item weapon)
        {
            CultureInfo cc = CultureInfo.CurrentCulture;
            var attackType = "";
            if (weapon == null)
            {
                attackType = player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? player.DefaultAttack?.ToLower(cc) : "punch";
            }
            else
            {
                attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)?.ToLower(cc);
            }
            
            _writer.WriteLine($"<p class='combat'>Your {attackType} misses {target.Name.ToLower(cc)}.</p>", player.ConnectionId);
            _writer.WriteLine($"<p class='combat'>{player.Name}'s {attackType} misses you.</p>", target.ConnectionId);

            foreach (var pc in room.Players)
            {
                if (pc.Name == player.Name || pc.Name == target.Name)
                {
                    continue;
                }

                _writer.WriteLine($"<p>{player.Name}'s {attackType} misses {target.Name.ToLower(cc)}.</p>", pc.ConnectionId);
            }
        }

 
        public void DeathCry(Room room, Player target)
        {

            foreach (var pc in room.Players)
            {
                if (pc.Name.Equals(target.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                _writer.WriteLine($"<p class='combat'>Your blood freezes as you hear {target.Name}'s death cry.</p>", pc.ConnectionId);
            }


            // Exit checks
            var rooms = new List<Room>();


            if (room.Exits.NorthWest != null)
            {
               rooms.Add(_cache.GetRoom($"{room.Exits.NorthWest.AreaId}{room.Exits.NorthWest.Coords.X}{room.Exits.NorthWest.Coords.Y}{room.Exits.NorthWest.Coords.Z}"));
            }

            if (room.Exits.North != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.North.AreaId}{room.Exits.North.Coords.X}{room.Exits.North.Coords.Y}{room.Exits.North.Coords.Z}"));
            }

            if (room.Exits.NorthEast != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.NorthEast.AreaId}{room.Exits.NorthEast.Coords.X}{room.Exits.NorthEast.Coords.Y}{room.Exits.NorthEast.Coords.Z}"));
            }

            if (room.Exits.East != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.East.AreaId}{room.Exits.East.Coords.X}{room.Exits.East.Coords.Y}{room.Exits.East.Coords.Z}"));
            }

            if (room.Exits.SouthEast != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.SouthEast.AreaId}{room.Exits.SouthEast.Coords.X}{room.Exits.SouthEast.Coords.Y}{room.Exits.SouthEast.Coords.Z}"));
            }

            if (room.Exits.South != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.South.AreaId}{room.Exits.South.Coords.X}{room.Exits.South.Coords.Y}{room.Exits.South.Coords.Z}"));
            }

            if (room.Exits.SouthWest != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.SouthWest.AreaId}{room.Exits.SouthWest.Coords.X}{room.Exits.SouthWest.Coords.Y}{room.Exits.SouthWest.Coords.Z}"));
            }

            if (room.Exits.West != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.West.AreaId}{room.Exits.West.Coords.X}{room.Exits.West.Coords.Y}{room.Exits.West.Coords.Z}"));
            }

            if (room.Exits.Up != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.Up.AreaId}{room.Exits.Up.Coords.X}{room.Exits.Up.Coords.Y}{room.Exits.Up.Coords.Z}"));
            }

            if (room.Exits.Down != null)
            {
                rooms.Add(_cache.GetRoom($"{room.Exits.Down.AreaId}{room.Exits.Down.Coords.X}{room.Exits.Down.Coords.Y}{room.Exits.Down.Coords.Z}"));
            }

            foreach (var adjacentRoom in rooms)
            {
                foreach (var pc in adjacentRoom.Players)
                {
                    _writer.WriteLine($"<p>Your blood freezes as you hear someone's death cry.</p>", pc.ConnectionId);
                }
            }
        }

        public void Fight(Player player, string victim, Room room, bool isMurder)
        {

            try
            {

         
            var target = FindTarget(player, victim, room, isMurder);

            if (target == null)
            {
                if (player.Status == CharacterStatus.Status.Fighting)
                {
                    player.Target = "";
                    player.Status = CharacterStatus.Status.Standing;

                    _cache.RemoveCharFromCombat(player.Id.ToString());
                }

                _writer.WriteLine("<p>They are not here.</p>", player.ConnectionId);
                return;
            }

            if (player.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
            {
                _writer.WriteLine("<p>You cannot do that while dead.</p>", player.ConnectionId);
                return;
            }

            if (target.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
            {
                _writer.WriteLine("<p>They are already dead.</p>", player.ConnectionId);
                return;
            }

            // For the UI to create a nice gap between rounds of auto attacks
            _writer.WriteLine($"<p class='combat-start'></p>", player.ConnectionId);

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


                // avoidance percentage can be improved by core skills 
                // such as improved parry, acrobatic etc 
                // instead of rolling a D10, roll a D6 for a close to 15% increase in chance

                // Move to formula, needs to use _dice instead of making a new instance
                var avoidanceRoll = new Dice().Roll(1, 1, 10);


                //10% chance to attempt a dodge
                if (avoidanceRoll == 1)
                {
                    var dodge = GetSkill("dodge", player);

                    if (dodge != null)
                    {
                        _writer.WriteLine($"<p>You dodge {target.Name}'s attack.</p>", player.ConnectionId);
                        _writer.WriteLine($"<p>{player.Name} dodges your attack.</p>", target.ConnectionId);
                        return;
                    }

                   
                }

                //10% chance to parry
                if (avoidanceRoll == 2)
                {
                    var skill = GetSkill("parry", player);

                    if (skill != null)
                    {
                        _writer.WriteLine($"<p>You parry {target.Name}'s attack.</p>", player.ConnectionId);
                        _writer.WriteLine($"<p>{player.Name} parries your attack.</p>", target.ConnectionId);
                        return;
                    }
                }

                // Block
                if (avoidanceRoll == 3)
                {
                    //var chanceToBlock = _formulas.ToBlockChance(target, player);
                    //var doesBlock = _formulas.DoesHit(chanceToBlock);

                    //if (doesBlock)
                    //{
                    //    var skill = GetSkill("shieldblock", player);

                    //    if (skill != null)
                    //    {
                    //        _writer.WriteLine($"You block {target.Name}'s attack with your shield.", player.ConnectionId);
                    //        _writer.WriteLine($"{player.Name} blocks your attack with their shield.", player.ConnectionId);
                    //    }
                    //}
                    //else
                    //{
                    //    // block fail
                    //}
                }
 

                var damage = _formulas.CalculateDamage(player, target, weapon);

                if (_formulas.IsCriticalHit())
                {
                    // double damage
                    damage *= 2;
                }


                HarmTarget(target, damage); 
                
                DisplayDamage(player, target, room, weapon, damage);

                _clientUi.UpdateHP(target);

                if (!IsTargetAlive(target))
                {
                  
                    player.Target = String.Empty;
                    player.Status = CharacterStatus.Status.Standing;
                    target.Status = CharacterStatus.Status.Ghost;
                    target.Target = string.Empty;

                     DeathCry(room, target);

                    _gain.GainExperiencePoints(player, target);

                    _writer.WriteLine("<p class='dead'>You are dead. R.I.P.</p>", target.ConnectionId);

                    var targetName =  target.Name.ToLower(CultureInfo.CurrentCulture);
                    var corpse = new Item.Item()
                    {
                        Name = $"The corpse of {targetName}.",
                        Description = new Description()
                        {
                            Room = $"The corpse of {targetName} is laying here.",
                            Exam = $"The corpse of {targetName} is laying here. {target.Description}",
                            Look = $"The corpse of {targetName} is laying here. {target.Description}",

                        },
                        Slot = Equipment.EqSlot.Held,
                        Level = 1,
                        Stuck = true,
                        Container = new Container()
                        {
                            Items = new ItemList(),
                            CanLock = false,
                            IsOpen = true,
                            CanOpen = false,
                            
                        },
                        ItemType = Item.Item.ItemTypes.Container,
                        DecayTimer = 300 // 5 minutes
                    };

                    foreach (var item in target.Inventory)
                    {
                        corpse.Container.Items.Add(item);
                    }

                    // clear list
                    target.Inventory = new ItemList();
                    // clear equipped
                    target.Equipped = new Equipment();

                    // add corpse to room
                    room.Items.Add(corpse);
                    _clientUi.UpdateInventory(target);
                    _clientUi.UpdateEquipment(target);
                    _clientUi.UpdateScore(target);

                    room.Clean = false;

                    _cache.RemoveCharFromCombat(target.Id.ToString());
                    _cache.RemoveCharFromCombat(player.Id.ToString());

                    if (target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                    {
                        room.Mobs.Remove(target);
                    }
                    else
                    {
                        room.Players.Remove(target);
                    }
                    // take player to Temple / recall area
                }
 
            }
            else
            {
               
                DisplayMiss(player, target, room, weapon);
                // miss message
                // gain improvements on weapon skill


                SkillList getWeaponSkill = null;
                if (weapon != null && !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    // urgh this is ugly
                    getWeaponSkill = player.Skills.FirstOrDefault(x =>
                        x.SkillName.Replace(" ", string.Empty)
                            .Equals(Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType)));
                }

                if (weapon == null && !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    getWeaponSkill = player.Skills.FirstOrDefault(x =>
                        x.SkillName.Equals("Hand To Hand", StringComparison.CurrentCultureIgnoreCase));
                }

                if (getWeaponSkill != null)
                {
                    getWeaponSkill.Proficiency += 1;
                    _writer.WriteLine($"<p class='improve'>Your proficiency in {getWeaponSkill.SkillName} has increased.</p>");
               
                    _gain.GainExperiencePoints(player, getWeaponSkill.Level * 50);
                }

              
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
 
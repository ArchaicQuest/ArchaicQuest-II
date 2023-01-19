using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat;

public class CombatHandler : ICombatHandler
{
    private readonly ConcurrentDictionary<string, Player> _combatCache = new();
    
    private readonly ICoreHandler _coreHandler;

    public CombatHandler(ICoreHandler coreHandler)
    {
        _coreHandler = coreHandler;
    }

    public bool IsCharInCombat(string id)
    {
        return _combatCache.ContainsKey(id);
    }
    
    public bool AddCharToCombat(string id, Player character)
    {
        return _combatCache.TryAdd(id, character);
    }

    public Player GetCharFromCombat(string id)
    {
        _combatCache.TryGetValue(id, out Player character);

        return character;
    }

    public Player RemoveCharFromCombat(string id)
    {
        _combatCache.TryRemove(id, out Player character);
        return character;
    }

    public List<Player> GetCombatList()
    {
        return _combatCache.Values.ToList();
    }
    
    public Player FindTarget(Player attacker, string target, Room room, bool isMurder)
        {
            if (string.IsNullOrEmpty(target))
            {
                return null;
            }
            // If mob
            if (isMurder && attacker.ConnectionId != "mob")
            {
                return room.Players.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));
            }

            if (attacker.ConnectionId == "mob")
            {
                return room.Players.FirstOrDefault(x => x.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase));
            }
            
            return room.Mobs.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase) && x.IsHiddenScriptMob == false);
        }
        
    public Item.Item GetWeapon(Player player, bool dualWield = false)
    {
        return dualWield ? player.Equipped.Secondary : player.Equipped.Wielded;
    }

    public void HarmTarget(Player victim, int damage)
    {
        victim.Attributes.Attribute[EffectLocation.Hitpoints] -= damage;

        if (victim.Attributes.Attribute[EffectLocation.Hitpoints] < 0)
        {
            victim.Attributes.Attribute[EffectLocation.Hitpoints] = 0;
        }
        
        if (victim.Config.Wimpy > 0 && 
            victim.Attributes.Attribute[EffectLocation.Hitpoints] <= victim.Config.Wimpy)
        {
            victim.Buffer.Clear();
            victim.Buffer.Enqueue("flee");
        }
    }

    public bool IsTargetAlive(Player victim)
    {
        return victim.Attributes.Attribute[EffectLocation.Hitpoints] > 0;
    }

    public KeyValuePair<string, string> DamageText(int dam)
    {
        return Formulas.DamageText(dam);
    }

    public void DisplayDamage(Player player, Player target, Room room, Item.Item weapon, int damage)
    {

        CultureInfo cc = CultureInfo.CurrentCulture;
        var damText = Formulas.DamageText(damage);
        var attackType = "";
        var damageType = "";
        if (weapon == null)
        {
            attackType = player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? player.DefaultAttack?.ToLower(cc) : "punch";
        }
        else
        {
            attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)?.ToLower(cc); 
            damageType =  Enum.GetName(typeof(Item.Item.DamageTypes), weapon.DamageType)?.ToLower(cc);
        }

        _coreHandler.Client.WriteLine($"<p class='combat'>Your {(damageType != "none" ? damageType : "" )} {attackType} {damText.Value} {target.Name.ToLower(cc)}. <span class='damage'>[{damage}]</span></p>", player.ConnectionId);
        _coreHandler.Client.WriteLine($"<p class='combat'>{target.Name} {Formulas.TargetHealth(player, target)}.</p>", player.ConnectionId);

        _coreHandler.Client.WriteLine($"<p>{player.Name}'s {(damageType != "none" ? damageType : "" )} {attackType} {damText.Value} you. <span class='damage'>[{damage}]</span></p></p>", target.ConnectionId);


        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name && pc.Name != target.Name))
        {
            _coreHandler.Client.WriteLine($"<p>{player.Name}'s {attackType} {damText.Value} {target.Name.ToLower(cc)}.</p>", pc.ConnectionId);
        }
    }

    public SkillList GetSkill(string skillName, Player player)
    {
        //this is breaking

        var skill = player.Skills.FirstOrDefault(x =>
           x.SkillName.Replace(" ", "").Equals(skillName.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase) && player.Level >= x.Level);
        return skill;
    }

    public void DisplayMiss(Player player, Player target, Room room, Item.Item weapon)
    {
        var cc = CultureInfo.CurrentCulture;
        var attackType = "";
        if (weapon == null)
        {
            attackType = player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? player.DefaultAttack?.ToLower(cc) : "punch";
        }
        else
        {
            attackType = Enum.GetName(typeof(Item.Item.AttackTypes), weapon.AttackType)?.ToLower(cc);
        }

        _coreHandler.Client.WriteLine($"<p class='combat'>Your {attackType} misses {target.Name.ToLower(cc)}.</p>", player.ConnectionId);
        _coreHandler.Client.WriteLine($"<p class='combat'>{player.Name}'s {attackType} misses you.</p>", target.ConnectionId);

        foreach (var pc in room.Players)
        {
            if (pc.Name == player.Name || pc.Name == target.Name)
            {
                continue;
            }

            _coreHandler.Client.WriteLine($"<p>{player.Name}'s {attackType} misses {target.Name.ToLower(cc)}.</p>", pc.ConnectionId);
        }
    }

    public void DeathCry(Room room, Player target)
    {
        foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(target.Name, StringComparison.CurrentCultureIgnoreCase)))
        {
            _coreHandler.Client.WriteLine($"<p class='combat'>Your blood freezes as you hear {target.Name.ToLower()}'s death cry.</p>", pc.ConnectionId);
        }

        // Exit checks
        var rooms = new List<Room>();
        
        if (room.Exits.NorthWest != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.NorthWest.AreaId}{room.Exits.NorthWest.Coords.X}{room.Exits.NorthWest.Coords.Y}{room.Exits.NorthWest.Coords.Z}"));
        }

        if (room.Exits.North != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.North.AreaId}{room.Exits.North.Coords.X}{room.Exits.North.Coords.Y}{room.Exits.North.Coords.Z}"));
        }

        if (room.Exits.NorthEast != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.NorthEast.AreaId}{room.Exits.NorthEast.Coords.X}{room.Exits.NorthEast.Coords.Y}{room.Exits.NorthEast.Coords.Z}"));
        }

        if (room.Exits.East != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.East.AreaId}{room.Exits.East.Coords.X}{room.Exits.East.Coords.Y}{room.Exits.East.Coords.Z}"));
        }

        if (room.Exits.SouthEast != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.SouthEast.AreaId}{room.Exits.SouthEast.Coords.X}{room.Exits.SouthEast.Coords.Y}{room.Exits.SouthEast.Coords.Z}"));
        }

        if (room.Exits.South != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.South.AreaId}{room.Exits.South.Coords.X}{room.Exits.South.Coords.Y}{room.Exits.South.Coords.Z}"));
        }

        if (room.Exits.SouthWest != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.SouthWest.AreaId}{room.Exits.SouthWest.Coords.X}{room.Exits.SouthWest.Coords.Y}{room.Exits.SouthWest.Coords.Z}"));
        }

        if (room.Exits.West != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.West.AreaId}{room.Exits.West.Coords.X}{room.Exits.West.Coords.Y}{room.Exits.West.Coords.Z}"));
        }

        if (room.Exits.Up != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.Up.AreaId}{room.Exits.Up.Coords.X}{room.Exits.Up.Coords.Y}{room.Exits.Up.Coords.Z}"));
        }

        if (room.Exits.Down != null)
        {
            rooms.Add(_coreHandler.World.GetRoom($"{room.Exits.Down.AreaId}{room.Exits.Down.Coords.X}{room.Exits.Down.Coords.Y}{room.Exits.Down.Coords.Z}"));
        }

        foreach (var pc in rooms.SelectMany(adjacentRoom => adjacentRoom.Players))
        {
            _coreHandler.Client.WriteLine($"<p>Your blood freezes as you hear someone's death cry.</p>", pc.ConnectionId);
        }
    }

    public void AddCharToCombat(Player character)
    {
        if (!IsCharInCombat(character.Id.ToString()))
        {
            AddCharToCombat(character.Id.ToString(), character);
        }
    }

    public void InitFightStatus(Player player, Player target)
    {
        player.Target = string.IsNullOrEmpty(player.Target) ? target.Name : player.Target;
        player.Status = CharacterStatus.Status.Fighting;
        target.Status = (target.Status & CharacterStatus.Status.Stunned) != 0 ? CharacterStatus.Status.Stunned : CharacterStatus.Status.Fighting;
        target.Target = string.IsNullOrEmpty(target.Target) ? player.Name : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

        if (player.Target == player.Name)
        {
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        if (!IsCharInCombat(player.Id.ToString()))
        {
            AddCharToCombat(player.Id.ToString(), player);
        }

        if (!IsCharInCombat(target.Id.ToString()))
        {
            AddCharToCombat(target.Id.ToString(), target);
        }
        

    }

    // TODO refactor FIGHT init
    // currently can spam kill because of the free init hit
    public void Fight(Player player, string victim, Room room, bool isMurder)
    {
        if (victim == "k" || victim == "kill")
        {
            _coreHandler.Client.WriteLine("<p>Kill whom?<p>", player.ConnectionId);
            return;
        }

        try
        {
            
            /*if (player.Status == CharacterStatus.Status.Fighting)
            {
                _writer.WriteLine("You will try your best.", player.ConnectionId);
                return;
            }*/

            if (player.Affects.Stunned)
            {
                _coreHandler.Client.WriteLine("<p>You are too stunned to attack this round.<p>", player.ConnectionId);
                return;
            }
            // refactor this, makes no sense
            // murder command need its on'y check here should be generic find the target player or mob
            var target = FindTarget(player, victim, room, isMurder) ?? FindTarget(player, victim, room, true);

            if (target == null)
            {
                if (player.Status == CharacterStatus.Status.Fighting)
                {
                    player.Target = "";
                    player.Status = CharacterStatus.Status.Standing;

                    RemoveCharFromCombat(player.Id.ToString());
                    return;
                }
                
                _coreHandler.Client.WriteLine("<p>They are not here.</p>", player.ConnectionId);
                return;
            }

            if (target.Name == player.Name)
            {
                _coreHandler.Client.WriteLine("<p>You can't start a fight with yourself!</p>", player.ConnectionId);
                return;
            }

            if (player.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
            {
                _coreHandler.Client.WriteLine("<p>You cannot do that while dead.</p>", player.ConnectionId);
                return;
            }

            if (target.Attributes.Attribute[EffectLocation.Hitpoints] <= 0)
            {
                _coreHandler.Client.WriteLine("<p>They are already dead.</p>", player.ConnectionId);

                player.Target = String.Empty;
                return;
            }

            // For the UI to create a nice gap between rounds of auto attacks
            _coreHandler.Client.WriteLine($"<p class='combat-start'></p>", player.ConnectionId);

            player.Target = target.Name;
            player.Status = CharacterStatus.Status.Fighting;
            target.Status = CharacterStatus.Status.Fighting;
            target.Target =
                string.IsNullOrEmpty(target.Target)
                    ? player.Name
                    : target.Target; //for group combat, if target is ganged, there target should not be changed when combat is initiated.

            if (!IsCharInCombat(player.Id.ToString()))
            {
                AddCharToCombat(player.Id.ToString(), player);
            }

            if (!IsCharInCombat(target.Id.ToString()))
            {
                AddCharToCombat(target.Id.ToString(), target);
            }


            /*
             *  This section crying out for a refactor
             */


            var weapon = GetWeapon(player);
            var chanceToHit = Formulas.ToHitChance(player, target, false);

            if (chanceToHit < 5)
            {
                chanceToHit = 5;
            }

            //if player bind and don't have blind fighting 
            // reduce chance to hit by 40%
            if (player.Affects.Blind && !BlindFighting(player))
            {
                chanceToHit = (int)(chanceToHit - (chanceToHit * .70));
            }

            var doesHit = Formulas.DoesHit(chanceToHit);

            if (doesHit)
            {


                // avoidance percentage can be improved by core skills 
                // such as improved parry, acrobatic etc 
                // instead of rolling a D10, roll a D6 for a close to 15% increase in chance

                // Move to formula, needs to use _dice instead of making a new instance
                var avoidanceRoll = DiceBag.Roll(1, 1, 10);


                //10% chance to attempt a dodge
                if (avoidanceRoll == 1)
                {
                    var dodge = GetSkill("dodge", target);

                    if (dodge != null)
                    {
                        _coreHandler.Client.WriteLine($"<p>You dodge {player.Name}'s attack.</p>", target.ConnectionId);
                        _coreHandler.Client.WriteLine($"<p>{target.Name} dodges your attack.</p>", player.ConnectionId);
                        return;
                    }


                }

                //10% chance to parry
                if (avoidanceRoll == 2)
                {
                    var skill = GetSkill("parry", target);

                    if (skill != null)
                    {
                        _coreHandler.Client.WriteLine($"<p>You parry {player.Name}'s attack.</p>", target.ConnectionId);
                        _coreHandler.Client.WriteLine($"<p>{target.Name} parries your attack.</p>", player.ConnectionId);



                        var riposte = GetSkill("Riposte", target);

                        if (riposte != null)
                        {

                            _coreHandler.Client.WriteLine($"<p>You riposte {player.Name}'s attack.</p>", target.ConnectionId);
                            _coreHandler.Client.WriteLine($"<p>{target.Name} riposte's your attack.</p>", player.ConnectionId);

                            var ripDamage = Formulas.CalculateDamage(target, player, weapon);

                            ripDamage /= 3;

                            HarmTarget(player, ripDamage);

                            DisplayDamage(target, player, room, weapon, ripDamage);

                            _coreHandler.Client.UpdateHP(player);

                            if (!IsTargetAlive(player))
                            {
                                TargetKilled(target, player, room);
                            }

                        }

                        return;
                    }
                }

                // Block
                if (avoidanceRoll == 3 && player.Equipped.Shield != null)
                {


                    var chanceToBlock = Formulas.ToBlockChance(target, player);
                    var doesBlock = Formulas.DoesHit(chanceToBlock);

                    if (doesBlock)
                    {
                        var skill = GetSkill("shieldblock", target);

                        if (skill != null)
                        {
                            _coreHandler.Client.WriteLine($"You block {player.Name}'s attack with your shield.",
                                target.ConnectionId);
                            _coreHandler.Client.WriteLine($"{target.Name} blocks your attack with their shield.",
                                player.ConnectionId);
                            return;
                        }
                    }
                    else
                    {
                        // block fail
                    }


                }


                var damage = Formulas.CalculateDamage(player, target, weapon);

                var enhancedDamageChance = DiceBag.Roll(1, 1, 100);
                var hasEnhancedDamage =
                    player.Skills.FirstOrDefault(x => x.SkillName.Equals("Enhanced Damage"));



                if (Formulas.IsCriticalHit())
                {
                    // double damage
                    damage *= 2;
                }

                if (hasEnhancedDamage != null)
                {
                    if (hasEnhancedDamage.Proficiency >= enhancedDamageChance && player.Level >= hasEnhancedDamage.Level)
                    {
                        var bonusDam = Helpers.GetPercentage(15, damage);
                        damage += bonusDam;
                    }
                }

                _coreHandler.Client.PlaySound("hit", target);
                _coreHandler.Client.PlaySound("hit", player);
                HarmTarget(target, damage);

                DisplayDamage(player, target, room, weapon, damage);

                _coreHandler.Client.UpdateHP(target);

                if (!IsTargetAlive(target))
                {
                    TargetKilled(player, target, room);
                }

            }
            else
            {
                _coreHandler.Client.PlaySound("miss", target);
                _coreHandler.Client.PlaySound("miss", player);
                DisplayMiss(player, target, room, weapon);
                // miss message
                // gain improvements on weapon skill


                SkillList getWeaponSkill = null;
                if (weapon != null &&
                    !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    // urgh this is ugly
                    getWeaponSkill = player.Skills.FirstOrDefault(x =>
                        x.SkillName.Replace(" ", string.Empty)
                            .Equals(Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType)));
                }

                if (weapon == null &&
                    !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    getWeaponSkill = player.Skills.FirstOrDefault(x =>
                        x.SkillName.Equals("Hand To Hand", StringComparison.CurrentCultureIgnoreCase));
                }

                if (getWeaponSkill != null && getWeaponSkill.Proficiency < 100)
                {
                    getWeaponSkill.Proficiency += 1;
                    _coreHandler.Client.WriteLine(
                        $"<p class='improve'>Your proficiency in {getWeaponSkill.SkillName} has increased.</p>", player.ConnectionId);

                    _coreHandler.Character.GainExperiencePoints(player, getWeaponSkill.Level * 50, true);
                }


            }

            if (player.Equipped.Secondary != null)
            {
                weapon = GetWeapon(player, true);
                chanceToHit = Formulas.ToHitChance(player, target, true);

                if (player.ConnectionId == "mob" && chanceToHit < 45)
                {
                    chanceToHit = 45;
                }

                //if player bind and don't have blind fighting 
                // reduce chance to hit by 40%
                if (player.Affects.Blind && !BlindFighting(player))
                {
                    chanceToHit = (int)(chanceToHit - (chanceToHit * .40));
                }

                doesHit = Formulas.DoesHit(chanceToHit);

                if (doesHit)
                {


                    // avoidance percentage can be improved by core skills 
                    // such as improved parry, acrobatic etc 
                    // instead of rolling a D10, roll a D6 for a close to 15% increase in chance

                    // Move to formula, needs to use _dice instead of making a new instance
                    var avoidanceRoll = DiceBag.Roll(1, 1, 10);


                    //10% chance to attempt a dodge
                    if (avoidanceRoll == 1)
                    {
                        var dodge = GetSkill("dodge", target);

                        if (dodge != null)
                        {
                            _coreHandler.Client.WriteLine($"<p>You dodge {player.Name}'s attack.</p>", target.ConnectionId);
                            _coreHandler.Client.WriteLine($"<p>{target.Name} dodges your attack.</p>", player.ConnectionId);
                            return;
                        }


                    }

                    //10% chance to parry
                    if (avoidanceRoll == 2)
                    {
                        var skill = GetSkill("parry", target);

                        if (skill != null)
                        {
                            _coreHandler.Client.WriteLine($"<p>You parry {player.Name}'s attack.</p>", target.ConnectionId);
                            _coreHandler.Client.WriteLine($"<p>{target.Name} parries your attack.</p>", player.ConnectionId);



                            var riposte = GetSkill("Riposte", target);

                            if (riposte != null)
                            {

                                _coreHandler.Client.WriteLine($"<p>You riposte {player.Name}'s attack.</p>", target.ConnectionId);
                                _coreHandler.Client.WriteLine($"<p>{target.Name} riposte's your attack.</p>", player.ConnectionId);

                                var ripDamage = Formulas.CalculateDamage(target, player, weapon);

                                ripDamage /= 3;

                                HarmTarget(player, ripDamage);

                                DisplayDamage(target, player, room, weapon, ripDamage);

                                _coreHandler.Client.UpdateHP(player);

                                if (!IsTargetAlive(player))
                                {
                                    TargetKilled(target, player, room);
                                }

                            }

                            return;
                        }
                    }

                    // Block
                    if (avoidanceRoll == 3 && player.Equipped.Shield != null)
                    {


                        var chanceToBlock = Formulas.ToBlockChance(target, player);
                        var doesBlock = Formulas.DoesHit(chanceToBlock);

                        if (doesBlock)
                        {
                            var skill = GetSkill("shieldblock", target);

                            if (skill != null)
                            {
                                _coreHandler.Client.WriteLine($"You block {player.Name}'s attack with your shield.",
                                    target.ConnectionId);
                                _coreHandler.Client.WriteLine($"{target.Name} blocks your attack with their shield.",
                                    player.ConnectionId);
                                return;
                            }
                        }
                        else
                        {
                            // block fail
                        }


                    }


                    var damage = Formulas.CalculateDamage(player, target, weapon);

                    if (Formulas.IsCriticalHit())
                    {
                        // double damage
                        damage *= 2;
                    }

                    var enhancedDamageChance = DiceBag.Roll(1, 1, 100);
                    var hasEnhancedDamage =
                        player.Skills.FirstOrDefault(x => x.SkillName.Equals("Enhanced Damage"));

                    if (hasEnhancedDamage != null)
                    {
                        if (hasEnhancedDamage.Proficiency >= enhancedDamageChance)
                        {
                            var bonusDam = Helpers.GetPercentage(15, damage);
                            damage += bonusDam;
                        }
                    }

                    HarmTarget(target, damage);

                    DisplayDamage(player, target, room, weapon, damage);

                    _coreHandler.Client.UpdateHP(target);

                    if (!IsTargetAlive(target))
                    {
                        TargetKilled(player, target, room);
                    }

                }
                else
                {

                    DisplayMiss(player, target, room, weapon);
                    // miss message
                    // gain improvements on weapon skill


                    SkillList getWeaponSkill = null;
                    if (weapon != null &&
                        !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // urgh this is ugly
                        getWeaponSkill = player.Skills.FirstOrDefault(x =>
                            x.SkillName.Replace(" ", string.Empty)
                                .Equals(Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType)));
                    }

                    if (weapon == null &&
                        !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                    {
                        getWeaponSkill = player.Skills.FirstOrDefault(x =>
                            x.SkillName.Equals("Hand To Hand", StringComparison.CurrentCultureIgnoreCase));
                    }

                    if (getWeaponSkill != null)
                    {
                        getWeaponSkill.Proficiency += 1;
                        _coreHandler.Client.WriteLine(
                            $"<p class='improve'>Your proficiency in {getWeaponSkill.SkillName} has increased.</p>", player.ConnectionId);

                        _coreHandler.Character.GainExperiencePoints(player, getWeaponSkill.Level * 50, true);
                    }


                }
            }
            
            if (player.Grouped)
            {
                foreach (var follower in player.Followers)
                {
                    if (follower.Grouped && follower.Following == player.Name &&  follower.Status != CharacterStatus.Status.Fighting)
                    {
                        if (follower.Config.AutoAssist && string.IsNullOrEmpty(follower.Target))
                        {
                            follower.Buffer.Clear();
                            follower.Target = player.Target;
                            follower.Status = CharacterStatus.Status.Fighting;
                            
                            if (!IsCharInCombat(follower.Id.ToString()))
                            {
                                AddCharToCombat(follower.Id.ToString(), follower);
                            }
                        }
                    }
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public int DamageReduction(Player defender, int damage)
    {
        var ArRating = defender.ArmorRating.Armour + 1;


        decimal damageAfterReduction = damage * (damage / (damage + ArRating + (defender.Attributes.Attribute[EffectLocation.Dexterity] / 4)));

        return (int)Math.Round(damageAfterReduction, MidpointRounding.ToEven);
    }

    public int CalculateSkillDamage(Player player, Player target, int damage)
    {
        var strengthMod = Math.Round((decimal)(player.Attributes.Attribute[EffectLocation.Strength] - 20) / 2, MidpointRounding.ToEven);
        var levelDif = target.Level - player.Level <= 0 ? 0 : target.Level - player.Level;

        var totalDamage = damage + strengthMod + levelDif + player.Attributes.Attribute[EffectLocation.DamageRoll];

        var criticalHit = 1;

        if (target.Status == CharacterStatus.Status.Sleeping || target.Status == CharacterStatus.Status.Stunned || target.Status == CharacterStatus.Status.Resting)
        {
            criticalHit = 2;
        }

        totalDamage *= criticalHit;

        // calculate damage reduction based on target armour
        var DamageAfterArmourReduction = DamageReduction(target, (int)totalDamage);


        return DamageAfterArmourReduction;
    }

    public void TargetKilled(Player player, Player target, Room room)
    {
        player.Target = string.Empty;
        player.Status = CharacterStatus.Status.Standing;
        target.Status = CharacterStatus.Status.Ghost;
        target.Target = string.Empty;

        DeathCry(room, target);
        
        if (player.Grouped)
        {
            _coreHandler.Character.GroupGainExperiencePoints(player, target);

             // other group members to drop from combat if they're fighting the same target
             // other group members status set to standing
             
             var isGroupLeader = string.IsNullOrEmpty(player.Following);

             var groupLeader = player;

             if (!isGroupLeader)
             {
                 groupLeader = _coreHandler.Character.GetPlayerCache().FirstOrDefault(x => x.Value.Name.Equals(player.Following)).Value;
             }
             
             foreach (var follower in groupLeader.Followers.Where(follower => follower.Grouped && follower.Following == groupLeader.Name))
             {
                 follower.Status = CharacterStatus.Status.Standing;
                 follower.Target = string.Empty;
                 RemoveCharFromCombat(follower.Id.ToString());
             }
        }
        else
        {
            _coreHandler.Character.GainExperiencePoints(player, target);   
        }

        _coreHandler.Character.IsQuestMob(player, target.Name);

        if (target.ConnectionId != "mob")
        {
            if(_coreHandler.Config.PostToDiscord)
                Helpers.PostToDiscord($"{target.Name} got killed by {player.Name}!", _coreHandler.Config.EventsDiscordWebHookURL);

            if (player.ConnectionId != "mob")
            {
                target.PlayerDeaths += 1;
                player.PlayerKills += 1;
            }
            else
            {
                target.MobDeaths += 1;
            }
        }

        _coreHandler.Client.WriteLine("<p class='dead'>You are dead. R.I.P.</p>", target.ConnectionId);
        
        var targetName = target.Name.ToLower(CultureInfo.CurrentCulture);
        var corpse = new Item.Item
        {
            Name = $"The corpse of {targetName}",
            Description = new Description
            {
                Room = $"The corpse of {targetName} is laying here.",
                Exam = target.Description,
                Look = target.Description,

            },
            Slot = Equipment.EqSlot.Held,
            Level = 1,
            Stuck = true,
            Container = new Container
            {
                Items = new ItemList(),
                CanLock = false,
                IsOpen = true,
                CanOpen = false,

            },
            ItemType = Item.Item.ItemTypes.Container,
            Decay = target.ConnectionId.Equals("mob", StringComparison.OrdinalIgnoreCase) ? 10 : 20,
            DecayTimer = 300 // 5 minutes,
        };

        foreach (var item in target.Inventory)
        {
            item.Equipped = false;
            corpse.Container.Items.Add(item);
        }

        // clear list
        target.Inventory = new ItemList();
        // clear equipped
        target.Equipped = new Equipment();

        var mount = target.Pets.FirstOrDefault(x => x.Name.Equals(target.Mounted.Name));
        if (mount != null)
        {
            target.Pets.Remove(mount);
            target.Mounted.Name = string.Empty;
        }

        // add corpse to room
        room.Items.Add(corpse);
        _coreHandler.Client.UpdateInventory(target);
        _coreHandler.Client.UpdateEquipment(target);
        _coreHandler.Client.UpdateScore(target);
        _coreHandler.Client.UpdateScore(player);
        
        if (target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
        {
            player.MobKills += 1;

            var randomItem = _coreHandler.Item.WeaponDrop(player);

            if (randomItem != null)
            {
                corpse.Container.Items.Add(randomItem);
            }

            if (player.Config.AutoLoot)
            {
                var corpseIndex = room.Items.IndexOf(corpse) + 1;
                player.Buffer.Enqueue($"get all {corpseIndex}.corpse");
            }
        }
        
        room.Clean = false;

        RemoveCharFromCombat(target.Id.ToString());
        RemoveCharFromCombat(player.Id.ToString());

        if (target.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
        {
            var command = _coreHandler.Command.GetCommand("sacrifice");
            
            if (player.Config.AutoSacrifice && command != null)
            {
                command.Execute(player, room, new []{corpse.Name});
            }

            room.Mobs.Remove(target);
            var getTodayMobStats = _coreHandler.Pdb.GetList<MobStats>(PlayerDataBase.Collections.MobStats).FirstOrDefault(x => x.Date.Date.Equals(DateTime.Today));

            if (getTodayMobStats != null)
            {
                getTodayMobStats.MobKills += 1;
            }
            else
            {
                getTodayMobStats = new MobStats()
                {
                    MobKills = 1,
                    PlayerDeaths = 0,
                    Date = DateTime.Now,
                };
            }
            _coreHandler.Pdb.Save(getTodayMobStats, PlayerDataBase.Collections.MobStats);
        }
        else
        {
            room.Players.Remove(target);
            var getTodayMobStats = _coreHandler.Pdb.GetList<MobStats>(PlayerDataBase.Collections.MobStats).FirstOrDefault(x => x.Date.Date.Equals(DateTime.Today));

            if (getTodayMobStats != null)
            {
                getTodayMobStats.PlayerDeaths += 1;
            }
            else
            {
                getTodayMobStats = new MobStats()
                {
                    MobKills = 0,
                    PlayerDeaths = 1,
                    Date = DateTime.Now,
                };
            }
            _coreHandler.Pdb.Save(getTodayMobStats, PlayerDataBase.Collections.MobStats);
        }
        // take player to Temple / recall area

        if (target.ConnectionId != "mob")
        {
            target.Status = CharacterStatus.Status.Resting;
            var newRoom = _coreHandler.World.GetRoom(target.RecallId);

            target.Buffer = new Queue<string>();

            target.RoomId = Helpers.ReturnRoomId(newRoom);

            newRoom.Players.Add(target);
            target.Buffer.Enqueue("look");
        }
    }

    /// <summary>
    /// 40% chance to hit reduce if no blind fighting
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool BlindFighting(Player player)
    {
        var foundSkill = player.Skills.FirstOrDefault(x =>
            x.SkillName.StartsWith("blind fighting", StringComparison.CurrentCultureIgnoreCase));

        if (foundSkill == null)
        {
            return false;
        }

        var getSkill = _coreHandler.Command.GetSkill(foundSkill.SkillId);

        if (getSkill == null)
        {
            var skill = _coreHandler.Command.GetAllSkills().FirstOrDefault(x => x.Name.Equals("blind fighting", StringComparison.CurrentCultureIgnoreCase));
            foundSkill.SkillId = skill.Id;
            getSkill = skill;
        }

        var proficiency = foundSkill.Proficiency;
        var success = DiceBag.Roll(1, 1, 100);

        if (success == 1 || success == 101)
        {
            return false;
        }

        //TODO Charisma Check
        if (proficiency >= success)
        {
            return true;
        }

        if (foundSkill.Proficiency == 100)
        {
            return false;
        }

        var increase = DiceBag.Roll(1, 1, 5);

        foundSkill.Proficiency += increase;

        _coreHandler.Character.GainExperiencePoints(player, 100 * foundSkill.Level / 4, false);

        _coreHandler.Client.UpdateExp(player);

        _coreHandler.Client.WriteLine(
            $"<p class='improve'>You learn from your mistakes and gain {100 * foundSkill.Level / 4} experience points.</p>" +
            $"<p class='improve'>Your knowledge of {foundSkill.SkillName} increases by {increase}%.</p>",
            player.ConnectionId, 0);

        return false;

    }
    
    public void UpdateCombat(Player player, Player target)
    {
        if (target == null) return;
        
        if (IsTargetAlive(target))
        {
            InitFightStatus(player, target);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character;

public class CharacterHandler : ICharacterHandler
{
    private readonly ConcurrentDictionary<string, Player> _playerCache = new();
    private readonly ConcurrentDictionary<int, Quest> _questCache = new();
    private readonly Dictionary<string, Class.Class> _pcClass = new();

    private readonly IClientHandler _clientHandler;
    private readonly ICommandHandler _commandHandler;
    private readonly ICombatHandler _combatHandler;

    public Equip Equip { get; }
    public IMobScripts MobScripts { get; } 
    public PassiveSkills PassiveSkills { get; }
    public DamageSkills DamageSkills { get; }
    public UtilSkills UtilSkills { get; }
    public DamageSpells DamageSpells { get; }

    public CharacterHandler(
        IClientHandler clientHandler,
        ICommandHandler commandHandler,
        ICombatHandler combatHandler)
    {
        _clientHandler = clientHandler;
        _commandHandler = commandHandler;
        _combatHandler = combatHandler;
        
        Equip = new Equip(_clientHandler);

        PassiveSkills = new PassiveSkills(clientHandler, commandHandler, this);
        DamageSkills = new DamageSkills(clientHandler, combatHandler, this);
        UtilSkills = new UtilSkills(clientHandler, combatHandler, this);

        DamageSpells = new DamageSpells(clientHandler, null);
        
        MobScripts = new MobScripts(combatHandler, clientHandler,this);
    }

    public bool AddPlayer(string id, Player player)
    {
        return _playerCache.TryAdd(id, player);
    }

    public Player GetPlayer(string id)
    {
        _playerCache.TryGetValue(id, out Player player);

        return player;
    }

    public Player RemovePlayer(string id)
    {
        _playerCache.TryRemove(id, out Player player);
        return player;
    }
    
    public bool AddClass(string id, Class.Class pcClass)
    {
        return _pcClass.TryAdd(id, pcClass);
    }

    public Class.Class GetClass(string id)
    {
        return _pcClass[id];
    }
    
    public bool AddQuest(int id, Quest quest)
    {
        return _questCache.TryAdd(id, quest);
    }

    public Quest GetQuest(int id)
    {
        _questCache.TryGetValue(id, out var quest);

        return quest;
    }

    public ConcurrentDictionary<int, Quest> GetQuestCache()
    {
        return _questCache;
    }

    /// <summary>
    /// Only for the main loop
    /// </summary>
    /// <returns></returns>
    public ConcurrentDictionary<string, Player> GetPlayerCache()
    {
        return _playerCache;
    }

    public Player PlayerAlreadyExists(Guid id)
    {
        return _playerCache.Values.FirstOrDefault(x => x.Id.Equals(id));
    }
    
    public void GainExperiencePoints(Player player, Player target)
        {
            var expWorth = GetExpWorth(target);
            if (Math.Floor(player.Level / 2f) > target.Level)
            {
                expWorth /= 2;
            }
            player.Experience += expWorth;
            player.ExperienceToNextLevel -= expWorth;

            _clientHandler.UpdateExp(player);

            if (expWorth == 1)
            {
                _clientHandler.WriteLine(
                    $"<p class='improve'>You gain 1 measly experience point.</p>",
                    player.ConnectionId);
            }

            _clientHandler.WriteLine(
                $"<p class='improve'>You receive {expWorth} experience points.</p>",
                player.ConnectionId);

            GainLevel(player);
            _clientHandler.UpdateExp(player);
        }

        public void GroupGainExperiencePoints(Player player, Player target)
        {
            if (player.Grouped)
            {
                var isGroupLeader = string.IsNullOrEmpty(player.Following);

                var groupLeader = player;

                if (!isGroupLeader)
                {
                    groupLeader = GetPlayerCache().FirstOrDefault(x => x.Value.Name.Equals(player.Following)).Value;
                }

                if (groupLeader.RoomId == target.RoomId)
                {
                    GainExperiencePoints(groupLeader, target);
                }

                foreach (var follower in groupLeader.Followers.Where(follower => follower.Grouped && follower.Following == groupLeader.Name).Where(follower => follower.RoomId == target.RoomId))
                {
                    GainExperiencePoints(follower, target);
                }
            }
        }

        public void GainExperiencePoints(Player player, int value, bool showMessage = true)
        {
            // TODO: gain level
            player.Experience += value;
            player.ExperienceToNextLevel -= value;

            if (showMessage)
            {
                _clientHandler.WriteLine(
                    $"<p class='improve'>You receive {value} experience points.</p>",
                    player.ConnectionId);
            }

            GainLevel(player);

            _clientHandler.UpdateExp(player);

        }

        public void GainLevel(Player player)
        {
            if (player.ExperienceToNextLevel <= 0)
            {
                player.Level++;
                player.ExperienceToNextLevel = player.Level * 2000; //TODO: have class and race mod

                var hpGain = (player.MaxAttributes.Attribute[EffectLocation.Constitution] / 100m) * 20;
                var minHPGain = (hpGain / 100m) * 20;
                var totalHP = DiceBag.Roll(1, (int)minHPGain, (int)hpGain);

                var manaGain = player.MaxAttributes.Attribute[EffectLocation.Intelligence] / 100m * 20;
                var minManaGain = manaGain / 100m * 20;
                var totalMana = DiceBag.Roll(1, (int)minManaGain, (int)manaGain);

                var moveGain = player.MaxAttributes.Attribute[EffectLocation.Dexterity] / 100m * 20;
                var minMoveGain = manaGain / 100 * 20;
                var totalMove = DiceBag.Roll(1, (int)minMoveGain, (int)moveGain);

                //player.Attributes.Attribute[EffectLocation.Hitpoints] += totalHP;
                //player.Attributes.Attribute[EffectLocation.Mana] += totalMana;
                //player.Attributes.Attribute[EffectLocation.Moves] += totalMove;
                player.MaxAttributes.Attribute[EffectLocation.Hitpoints] += totalHP;
                player.MaxAttributes.Attribute[EffectLocation.Mana] += totalMana;
                player.MaxAttributes.Attribute[EffectLocation.Moves] += totalMove;

                _clientHandler.WriteLine($"<p class='improve'>You have advanced to level {player.Level}, you gain: {totalHP} HP, {totalMana} Mana, {totalMove} Moves.</p>", player.ConnectionId);

                SeedData.Classes.SetGenericTitle(player);

                _clientHandler.UpdateMana(player);
                _clientHandler.UpdateMoves(player);
                _clientHandler.UpdateHP(player);
                _clientHandler.UpdateExp(player);
                _clientHandler.UpdateScore(player);

            }
        }

        public void GainLevel(Player player, string target)
        {
            var foundTarget = GetPlayerCache()
                .FirstOrDefault(x => x.Value.Name.Equals(target, StringComparison.CurrentCultureIgnoreCase));

            if (foundTarget.Value == null)
            {
                _clientHandler.WriteLine($"Cannot find {target}.");
                return;
            }

            foundTarget.Value.ExperienceToNextLevel = 0;
            _clientHandler.WriteLine($"{player.Name} has rewarded you with a level.", foundTarget.Value.ConnectionId);
            GainLevel(foundTarget.Value);
        }

        public int GetExpWorth(Player character)
        {
            var maxEXP = 10000;
            var exp = character.Level;
            exp += DiceBag.Roll(1, 25, 275);
            exp += character.Equipped.Wielded?.Damage.Maximum ?? 6; // 6 for hand to hand
            exp += character.Attributes.Attribute[EffectLocation.DamageRoll] * 10;
            exp += character.Attributes.Attribute[EffectLocation.HitRoll] + character.Level * 10;
            exp += character.ArmorRating.Armour;

            exp += character.Attributes.Attribute[EffectLocation.Hitpoints] * 3;
            exp += character.Attributes.Attribute[EffectLocation.Mana];
            exp += character.Attributes.Attribute[EffectLocation.Strength];
            exp += character.Attributes.Attribute[EffectLocation.Dexterity];
            exp += character.Attributes.Attribute[EffectLocation.Constitution];
            exp += character.Attributes.Attribute[EffectLocation.Wisdom];
            exp += character.Attributes.Attribute[EffectLocation.Intelligence];
            exp += character.ArmorRating.Magic;
            exp += character.Level * 15;
            //exp += character.Attributes.Attribute[EffectLocation.Moves];
            // boost xp if mob is shielded

            return exp > maxEXP ? maxEXP : exp;
        }

        public void GainSkillExperience(Player character, int expGain, SkillList skill, int increase)
        {
            character.Experience += expGain;
            character.ExperienceToNextLevel -= expGain;
            skill.Proficiency += increase;

            GainLevel(character);
            _clientHandler.UpdateExp(character);

            _clientHandler.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {expGain} experience points.</p>",
                character.ConnectionId);
            _clientHandler.WriteLine(
                $"<p class='improve'>Your {skill.SkillName} skill increases by {increase}%.</p>",
                character.ConnectionId);
        }
        
        public void GainSkillProficiency(SkillList foundSkill, Player player)
        {

            var getSkill = _commandHandler.GetSkill(foundSkill.SkillId);

            if (getSkill == null)
            {
                var skill = _commandHandler.GetAllSkills().FirstOrDefault(x => x.Name.Equals(foundSkill.SkillName, StringComparison.CurrentCultureIgnoreCase));
                foundSkill.SkillId = skill.Id;
            }

            if (foundSkill.Proficiency == 100)
            {
                return;
            }

            var increase = DiceBag.Roll(1, 1, 5);

            foundSkill.Proficiency += increase;

            GainExperiencePoints(player, 100 * foundSkill.Level / 4, false);

            _clientHandler.UpdateExp(player);

            _clientHandler.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {100 * foundSkill.Level / 4} experience points.</p>" +
                $"<p class='improve'>Your knowledge of {foundSkill.SkillName} increases by {increase}%.</p>",
                player.ConnectionId, 0);
        }
        
        public string SkillLearnMistakes(Player player, string skillName, int delay = 0)
        {
            var skill = player.Skills.FirstOrDefault(x => x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase));

            if (skill == null)
            {
                return string.Empty;
            }

            if (skill.Proficiency == 100)
            {
                return string.Empty;
            }

            var increase = DiceBag.Roll(1, 1, 5);

            skill.Proficiency += increase;

            GainExperiencePoints(player, 100 * skill.Level / 4, false);

            return
                $"<p class='improve'>You learn from your mistakes and gain {100 * skill.Level / 4} experience points.</p>" +
                $"<p class='improve'>Your knowledge of {skill.SkillName} increases by {increase}%.</p>";
        }
        
        public void IsQuestMob(Player player, string mobName)
        {
            foreach (var quest in player.QuestLog)
            {
                if (quest.Type != QuestTypes.Kill)
                {
                    continue;
                }

                var questCompleted = false;

                foreach (var mob in quest.MobsToKill.Where(mob => mob.Name.Equals(mobName)))
                {
                    mob.Current = mob.Current + 1;
                    questCompleted = mob.Count == mob.Current;
                }
                
                if (questCompleted)
                {
                    quest.Completed = true;

                    _clientHandler.WriteLine($"<h3 class='gain'>{quest.Title} Completed!</h3><p>Return to the quest giver for your reward.</p>", player.ConnectionId);
                }
            }
            _clientHandler.UpdateQuest(player);
        }

        public void DoSkill(string key, string obj, Player target, string fullCommand, Player player, Room room,
            bool wearOff)
        {
            switch (key.ToLower())
            {
                case "kick":
                    DamageSkills.Kick(player, target, room);
                    break;
                case "elbow":
                    DamageSkills.Elbow(player, target, room);
                    break;
                case "trip":
                    DamageSkills.Trip(player, target, room);
                    break;
                case "headbutt":
                    DamageSkills.HeadButt(player, target, room);
                    break;
                case "charge":
                    DamageSkills.Charge(player, target, room, obj);
                    break;
                case "stab":
                    DamageSkills.Stab(player, target, room, obj);
                    break;
                case "uppercut":
                    DamageSkills.UpperCut(player, target, room, obj);
                    break;
                case "dirt kick":
                case "dirtkick":
                    DamageSkills.DirtKick(player, target, room, obj);
                    break;
                case "disarm":
                    UtilSkills.Disarm(player, target, room, obj);
                    break;
                case "lunge":
                    DamageSkills.Lunge(player, target, room, obj);
                    break;
                case "berserk":
                    UtilSkills.Berserk(player, target, room);
                    break;
                case "rescue":
                    UtilSkills.Rescue(player, target, room, obj);
                    break;
                case "mount":
                    UtilSkills.Mount(player, target, room);
                    break;
                case "shield bash":
                    DamageSkills.ShieldBash(player, target, room, obj);
                    break;
                case "war cry":
                case "warcry":
                    UtilSkills.WarCry(player, target, room);
                    break;
                case "hamstring":
                    DamageSkills.HamString(player, target, room, obj);
                    break;
                case "impale":
                    DamageSkills.Impale(player, target, room, obj);
                    break;
                case "slash":
                    DamageSkills.Slash(player, target, room, obj);
                    break;
                case "overhead crush":
                    DamageSkills.OverheadCrush(player, target, room, obj);
                    break;
                case "cleave":
                    DamageSkills.Cleave(player, target, room, obj);
                    break;
                case "second":
                    PassiveSkills.DualWield(player, target, room, obj);
                    break;
                case "lore":
                    PassiveSkills.Lore(player, room, obj);
                    break;
            }
        }
        
        public void CastSpell(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff)
        {

            switch (key.ToLower())
            {
                case "magic missile":
                    DamageSpells.MagicMissile(player, target, room);
                    break;
                case "cause light wounds":
                    DamageSpells.CauseLightWounds(player, target, room);
                    break;
                case "cure light wounds":
                    DamageSpells.CureLightWounds(player, target, room);
                    break;
                case "armour":
                case "armor":
                    DamageSpells.Armor(player, target, room, wearOff);
                    break;
                case "bless":
                    DamageSpells.Bless(player, target, room, wearOff);
                    break;
                case "identify":
                    DamageSpells.Identify(player, obj, room);
                    break;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void DoSpell(string spellName, Player origin, string targetName = "", Room room = null)
        {
            
            if (!Formulas.CheckStatusToCast(origin, out var message))
            {
                _clientHandler.WriteLine(message, origin.ConnectionId);
                return;
            }

            var spell = GetSpell(spellName, origin);

            if (spell == null)
            {
                return;
            }

            if (origin.ConnectionId != "mob" &&  origin.Attributes.Attribute[EffectLocation.Mana] > spell.Cost.Table[Cost.Mana])
            {
                _clientHandler.WriteLine("You don't have enough mana.", origin.ConnectionId);
                return;
            }

            // saves

            if (Formulas.SpellAffectsCharacter(spell))
            {
                Player target = null;
                target = ReturnTarget(spell, targetName, room, origin);

                if (target == null)
                {
                    return;
                }
                var spellSkill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));
                //saving throw
                if (spell.Type == SkillType.Affect)
                {


                    var savingThrow = target.Attributes.Attribute[EffectLocation.Intelligence] / 2;

                    var rollForSave = DiceBag.Roll(1, 1, 100);

                    if (rollForSave <= savingThrow)
                    {
                        //save
                        // half spell affect duration


                        if (rollForSave == 1)
                        {
                            // fail
                            spell.Rounds = origin.Level / 2;
                        }

                        if (rollForSave != 1)
                        {
                            spell.Rounds = origin.Level / 4;
                        }
                    }


                }

                ReciteSpellCharacter(origin, target, spell, room);
                
                //deduct mana
                origin.Attributes.Attribute[EffectLocation.Mana] -= spell.Cost.Table[Cost.Mana] == 0 ? 5 : spell.Cost.Table[Cost.Mana];
                _clientHandler.UpdateMana(origin);
                
                if (Formulas.SpellSuccess(origin, target, spell))
                {
                    
                    //if (spell.Type == SkillType.Damage)
                    //{

                    //    var savingThrow = origin.Attributes.Attribute[EffectLocation.Dexterity] / 2;

                    //    var rollForSave = _dice.Roll(1, 1, 100);

                    //    if (rollForSave <= savingThrow || rollForSave == 100)
                    //    {
                    //        if (rollForSave > 1)
                    //        {
                    //            damage /= 2;

                    //            _writer.WriteLine("You partly evade " + origin.Name + "'s " + spell.Name + " by jumping back.", target.ConnectionId);

                    //            _writer.WriteLine($"{target.Name} partly evades your " + spell.Name + " by jumping back.", origin.ConnectionId);

                    //            foreach (var pc in room.Players)
                    //            {
                    //                if (pc.ConnectionId.Equals(origin.ConnectionId) ||
                    //                    pc.ConnectionId.Equals(target.ConnectionId))
                    //                {
                    //                    continue;
                    //                }

                    //                _writer.WriteLine($"{target.Name} partly evades {origin.Name}'s " + spell.Name + " by jumping back.", pc.ConnectionId);
                    //            }
                    //        }
                    //    }
                    //}


                    var skillTarget = new SkillTarget
                    {
                        Origin = origin,
                        Target = target,
                        Room = room,
                        Skill = spell
                    };

                    if (string.IsNullOrEmpty(spell.Formula) && spell.Type == SkillType.Damage)
                    {
                        //do this for cast cure
                        CastSpell(spell.Name, "", target, "", origin, room, false);

                    }




                    if (spell.Type != SkillType.Damage)
                    {
                        CastSpell(spell.Name, "", target, "", origin, room, false);
                    }
                }
                else
                {
                    _clientHandler.WriteLine("<p>You lost concentration.</p>", origin.ConnectionId);
                    
                    var skill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));

                    if (skill == null)
                    {
                        return;
                    }

                    if (skill.Proficiency == 95)
                    {
                        return;
                    }

                    var increase = DiceBag.Roll(1, 1, 3);

                    skill.Proficiency += increase;

                    origin.Experience += 100;
                    origin.ExperienceToNextLevel -= 100;

                    _clientHandler.UpdateExp(origin);

                    _clientHandler.WriteLine(
                        $"<p class='improve'>You learn from your mistakes and gain 100 experience points.</p>",
                        origin.ConnectionId);
                    _clientHandler.WriteLine(
                        $"<p class='improve'>Your {skill.SkillName} skill increases by {increase}%.</p>",
                        origin.ConnectionId);
                }

            }
            else if ((spell.ValidTargets & ValidTargets.TargetObjectInventory) != 0 || (spell.ValidTargets & ValidTargets.TargetObjectEquipped) != 0)
            {
                //Item.Item target = null;
                //target = _spellTargetCharacter.ReturnTargetItem(spell, targetName, room, origin);

                //if (target == null)
                //{
                //    return;
                //}
                var spellSkill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));


                ReciteSpellCharacter(origin, origin, spell, room);

                //deduct mana
                origin.Attributes.Attribute[EffectLocation.Mana] -= spell.Cost.Table[Cost.Mana] == 0 ? 5 : spell.Cost.Table[Cost.Mana];
                _clientHandler.UpdateMana(origin);


                if (Formulas.SpellSuccess(origin, null, spell))
                {
                    CastSpell(spell.Name, targetName, null, "", origin, room, false);
                }
                else
                {
                    _clientHandler.WriteLine("<p>You lost concentration.</p>", origin.ConnectionId);
                    
                    var skill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));

                    if (skill == null)
                    {
                        return;
                    }

                    if (skill.Proficiency == 95)
                    {
                        return;
                    }
                    
                    var increase = DiceBag.Roll(1, 1, 3);

                    skill.Proficiency += increase;

                    origin.Experience += 100;
                    origin.ExperienceToNextLevel -= 100;

                    _clientHandler.UpdateExp(origin);

                    _clientHandler.WriteLine(
                        "<p class='improve'>You learn from your mistakes and gain 100 experience points.</p>",
                        origin.ConnectionId);
                    _clientHandler.WriteLine(
                        $"<p class='improve'>Your {skill.SkillName} skill increases by {increase}%.</p>",
                        origin.ConnectionId);
                }
            }
            
            //_writer.WriteLine(
            //    $"<p>You cannot cast this spell upon another.</p>",
            //    origin.ConnectionId);
        }


        private void ReciteSpellCharacter(Player origin, Player target, Skill.Model.Skill spell, Room room)
        {
            // not correct need to send to room 
            if (origin.Id == target.Id)
            {
                _clientHandler.WriteLine(
                    $"You close your eyes and utter the words, '{spell.Name}'.", origin.ConnectionId);

                foreach (var pc in room.Players.Where(pc => !pc.ConnectionId.Equals(origin.ConnectionId)))
                {
                    _clientHandler.WriteLine($"{origin.Name} closes {Helpers.GetPronoun(origin.Gender)} eyes and utters the words, '{Helpers.ObfuscateSpellName(spell.Name)}'.", pc.ConnectionId);
                }
            }
            else if (origin != target)
            {

                var obfuscatedSpellName = Helpers.ObfuscateSpellName(spell.Name);


                _clientHandler.WriteLine($"You look at {target.Name} and utter the words, '{spell.Name}'.", origin.ConnectionId);
                _clientHandler.WriteLine($"{origin.Name} looks at you and utters the words, '{(!Helpers.isCaster(target.ClassName) ? obfuscatedSpellName : spell.Name)}'.", target.ConnectionId);
                
                foreach (var pc in room.Players.Where(pc => !pc.ConnectionId.Equals(origin.ConnectionId) &&
                                                            !pc.ConnectionId.Equals(target.ConnectionId)))
                {
                    _clientHandler.WriteLine($"{origin.Name} looks at {target.Name} and utters the words, '{(!Helpers.isCaster(pc.ClassName) ? obfuscatedSpellName : spell.Name)}'.", pc.ConnectionId);
                }

            }
        }
        
        public Player CheckTarget(Skill.Model.Skill spell, string target, Room room, Player player)
        {

            if (string.IsNullOrEmpty(target) || target.Equals(spell.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            var victim = string.IsNullOrEmpty(target) ? player : Helpers.GetTarget(target, room);

            if (victim == null)
            {
                _clientHandler.WriteLine(
                    (spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0
                        ? "You can't find them in the realm."
                        : "You don't see them here.", player.ConnectionId);

                return null;
            }

            //if (spell.StartsCombat && victim.Id == player.Id)
            //{
            //    _writer.WriteLine("Casting this on yourself is a bad idea.", player.ConnectionId);
            //    return null;
            //}

            return victim;
        }
        
        public Player ReturnTarget(Skill.Model.Skill spell, string target, Room room, Player player)
        {


            if ((spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0)
            {
                if (string.IsNullOrEmpty(target) || target == "self")
                {
                    return player;
                }

                _clientHandler.WriteLine("You can only cast this spell on yourself", player.ConnectionId);
                return null;
            }


            if (player.Status != CharacterStatus.Status.Fighting && (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0)
            {
                if (!string.IsNullOrEmpty(target) || target.Equals(spell.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return CheckTarget(spell, target, room, player);
                }
            }

            //If no argument, target is the PC/NPC the player is fighting
            if (player.Status == CharacterStatus.Status.Fighting && (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0)
            {
                if (string.IsNullOrEmpty(target) || target.Equals(spell.Name, StringComparison.CurrentCultureIgnoreCase))
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

            return player;
        }
        
        public bool HasSkill(Player player, string skill)
        {
            return player.Skills.FirstOrDefault(x => x.SkillName.Equals(skill, StringComparison.CurrentCultureIgnoreCase) && x.Level <= player.Level) != null;
        }
        
        public void DamagePlayer(string spellName, int damage, Player player, Player target, Room room)
        {

            if (_combatHandler.IsTargetAlive(target))
            {

                var totalDam = _combatHandler.CalculateSkillDamage(player, target, damage);

                _clientHandler.WriteLine(
                    $"<p>Your {spellName} {_combatHandler.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                    player.ConnectionId);
                _clientHandler.WriteLine(
                    $"<p>{player.Name}'s {spellName} {_combatHandler.DamageText(totalDam).Value} you!  <span class='damage'>[{damage}]</span></p>",
                    target.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.ConnectionId.Equals(player.ConnectionId) ||
                        pc.ConnectionId.Equals(target.ConnectionId))
                    {
                        continue;
                    }

                    _clientHandler.WriteLine(
                        $"<p>{player.Name}'s {spellName} {_combatHandler.DamageText(totalDam).Value} {target.Name}  <span class='damage'>[{damage}]</span></p>",
                        pc.ConnectionId);

                }

                target.Attributes.Attribute[EffectLocation.Hitpoints] -= totalDam;

                if (!_combatHandler.IsTargetAlive(target))
                {
                    _combatHandler.TargetKilled(player, target, room);

                    _clientHandler.UpdateHP(target);
                    return;
                    //TODO: create corpse, refactor fight method from combat.cs
                }

                //update UI
                _clientHandler.UpdateHP(target);

                _combatHandler.AddCharToCombat(target);
                _combatHandler.AddCharToCombat(player);
            }

        }
        
        public bool AffectPlayerAttributes(string spellName, EffectLocation attribute, int value, Player player, Player target, Room room, string noAffect)
        {

            if (attribute is EffectLocation.Hitpoints or EffectLocation.Mana or EffectLocation.Moves && target.Attributes.Attribute[attribute] == target.MaxAttributes.Attribute[attribute])
            {
                _clientHandler.WriteLine(Helpers.ReplaceTargetPlaceholders(noAffect, target, false), player.ConnectionId);
                return false;
            }

            target.Attributes.Attribute[attribute] += value;

            if (attribute is EffectLocation.Hitpoints or EffectLocation.Mana or EffectLocation.Moves && target.Attributes.Attribute[attribute] > target.MaxAttributes.Attribute[attribute])
            {
                target.Attributes.Attribute[attribute] = target.MaxAttributes.Attribute[attribute];
            }

            return true;

        }
        
        /// <summary>
        /// Adds affects to player
        /// Bless
        /// HitRoll +10
        /// DamRoll + 5
        /// </summary>
        /// <param name="spellAffects"></param>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="room"></param>
        public void AddAffectToPlayer(List<Affect> spellAffects, Player player, Player target, Room room)
        {
            foreach (var affects in spellAffects)
            {
                var hasEffect = target.Affects.Custom.FirstOrDefault(x => x.Name.Equals(affects.Name));
                if (hasEffect != null)
                {
                    hasEffect.Duration = affects.Duration;
                }
                else
                {
                    target.Affects.Custom.Add(new Affect()
                    {
                        Modifier = affects.Modifier,
                        Benefits = affects.Benefits,
                        Affects = affects.Affects,
                        Duration = player.Level + player.Attributes.Attribute[EffectLocation.Intelligence] / 2,
                        Name = affects.Name
                    });

                    if (affects.Affects == DefineSpell.SpellAffect.Blind)
                    {
                        target.Affects.Blind = true;
                    }

                    //apply affects to target
                    if (affects.Modifier.Strength != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Strength] += affects.Modifier.Strength;
                    }


                    if (affects.Modifier.Dexterity != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Dexterity] += affects.Modifier.Dexterity;
                    }

                    if (affects.Modifier.Charisma != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Charisma] += affects.Modifier.Charisma;
                    }

                    if (affects.Modifier.Constitution != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Constitution] += affects.Modifier.Constitution;
                    }

                    if (affects.Modifier.Intelligence != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Intelligence] += affects.Modifier.Intelligence;
                    }

                    if (affects.Modifier.Wisdom != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Wisdom] += affects.Modifier.Wisdom;
                    }

                    if (affects.Modifier.DamRoll != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.DamageRoll] += affects.Modifier.DamRoll;
                    }

                    if (affects.Modifier.HitRoll != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.HitRoll] += affects.Modifier.HitRoll;
                    }

                    if (affects.Modifier.HP != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Hitpoints] += affects.Modifier.HP;

                        if (target.Attributes.Attribute[EffectLocation.Hitpoints] >
                            target.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                        {
                            target.Attributes.Attribute[EffectLocation.Hitpoints] =
                                target.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                        }
                    }

                    if (affects.Modifier.Mana != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Mana] += affects.Modifier.Mana;

                        if (target.Attributes.Attribute[EffectLocation.Mana] >
                            target.MaxAttributes.Attribute[EffectLocation.Mana])
                        {
                            target.Attributes.Attribute[EffectLocation.Mana] =
                                target.MaxAttributes.Attribute[EffectLocation.Mana];
                        }
                    }

                    if (affects.Modifier.Moves != 0)
                    {
                        target.Attributes.Attribute[EffectLocation.Moves] += affects.Modifier.Moves;

                        if (target.Attributes.Attribute[EffectLocation.Moves] >
                            target.MaxAttributes.Attribute[EffectLocation.Moves])
                        {
                            target.Attributes.Attribute[EffectLocation.Moves] =
                                target.MaxAttributes.Attribute[EffectLocation.Moves];
                        }
                    }

                }
            }

            _clientHandler.UpdateAffects(target);
            _clientHandler.UpdateScore(target);
        }
        
        public void EmoteAction(Player player, Player target, Room room, SkillMessage emote)
        {
            if (target.ConnectionId == player.ConnectionId)
            {
                _clientHandler.WriteLine(
                    $"<p>{Helpers.ReplaceTargetPlaceholders(emote.Hit.ToPlayer, target, true)}</p>",
                    target.ConnectionId);
            }
            else
            {
                _clientHandler.WriteLine(
                    $"<p>{Helpers.ReplaceTargetPlaceholders(emote.Hit.ToPlayer, target, false)}</p>",
                    player.ConnectionId);
            }


            if (!string.IsNullOrEmpty(emote.Hit.ToTarget))
            {
                _clientHandler.WriteLine(
                    $"<p>{emote.Hit.ToTarget}</p>",
                    target.ConnectionId);
            }

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId) ||
                    pc.ConnectionId.Equals(target.ConnectionId))
                {
                    continue;
                }

                _clientHandler.WriteLine($"<p>{Helpers.ReplaceTargetPlaceholders(emote.Hit.ToRoom, target, false)}</p>",
                    pc.ConnectionId);

            }
        }
        
        public void EmoteEffectWearOffAction(Player player, Room room, SkillMessage emote)
        {

            foreach (var pc in room.Players)
            {
                if (pc.ConnectionId.Equals(player.ConnectionId))
                {
                    _clientHandler.WriteLine($"<p>{emote.EffectWearOff.ToPlayer}</p>",
                        pc.ConnectionId);
                    continue;
                }

                _clientHandler.WriteLine($"<p>{Helpers.ReplaceTargetPlaceholders(emote.EffectWearOff.ToRoom, player, false)}</p>",
                    pc.ConnectionId);

            }
        }
        
        public Skill.Model.Skill GetSpell(string skill, Player player)
        {
            // I think this is a debug bug where the hot reload doesn't correctly run startup again 
            // and mobs loose there skills adding this hack just incase, should solve it
            if (player.Skills == null && player.ConnectionId == "mob")
            {
                player.Skills = GetClass(player.ClassName).Skills;
            }

            var foundSpell = player.Skills?.FirstOrDefault(x => x.SkillName.StartsWith(skill, StringComparison.CurrentCultureIgnoreCase) && x.Level <= player.Level);

            if (foundSpell == null)
            {
                _clientHandler.WriteLine($"You don't know a spell that begins with {skill}", player.ConnectionId);
                return null;
            }

            var spell = _commandHandler.GetSkill(foundSpell.SkillId);

            if (spell == null || spell.Name != foundSpell.SkillName)
            {
                var getSpell = _commandHandler.GetAllSkills();
                spell = getSpell.FirstOrDefault(x => x.Name.Equals(foundSpell.SkillName, StringComparison.CurrentCultureIgnoreCase));

                foundSpell.SkillId = spell.Id;
            }
            
            return spell;
        }
}
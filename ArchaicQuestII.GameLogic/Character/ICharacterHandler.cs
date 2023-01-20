using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character;

public interface ICharacterHandler
{
    void Init();
    bool AddPlayer(string id, Player player);
    Player GetPlayer(string id);
    Player RemovePlayer(string id);
    ConcurrentDictionary<string, Player> GetPlayerCache();
    Player PlayerAlreadyExists(Guid id);
    bool AddQuest(int id, Quest quest);
    Quest GetQuest(int id);
    ConcurrentDictionary<int, Quest> GetQuestCache();
    bool AddClass(string id, Class.Class pcClass);
    Class.Class GetClass(string id);
    
    void GainExperiencePoints(Player player, Player target);

    void GainExperiencePoints(Player player, int value, bool showMessage);

    void GainLevel(Player player);
    void GainLevel(Player player, string target);

    void GroupGainExperiencePoints(Player player, Player target);

    void GainSkillExperience(Player character, int expGain, SkillList skill, int increase);
    void GainSkillProficiency(SkillList foundSkill, Player player);
    string SkillLearnMistakes(Player player, string skillName, int delay = 0);
    void IsQuestMob(Player player, string mobName);
    IMobScripts MobScripts { get; }
    PassiveSkills PassiveSkills { get; }
    DamageSkills DamageSkills { get; }
    UtilSkills UtilSkills { get; }
    void DoSkill(string key, string obj, Player target, string fullCommand, Player player, Room room,
        bool wearOff);

    void DoSpell(string spellName, Player origin, string targetName = "", Room room = null);
    Player ReturnTarget(Skill.Model.Skill spell, string target, Room room, Player player);
    void CastSpell(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff);
    Equip Equip { get; }
    bool HasSkill(Player player, string skill);

    bool AffectPlayerAttributes(string spellName, EffectLocation attribute, int value, Player player, Player target,
        Room room, string noAffect);

    void AddAffectToPlayer(List<Affect> spellAffects, Player player, Player target, Room room);
    void DamagePlayer(string spellName, int damage, Player player, Player target, Room room);

    void EmoteAction(Player player, Player target, Room room, SkillMessage emote);
    void EmoteEffectWearOffAction(Player player, Room room, SkillMessage emote);

    Skill.Model.Skill GetSpell(string skill, Player player);
}
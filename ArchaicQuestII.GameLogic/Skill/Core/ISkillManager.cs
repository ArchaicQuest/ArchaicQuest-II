using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public interface ISkillManager
    {
        public void updateCombat(Player player, Player target, Room room);
        public string ReplacePlaceholders(string str, Player player, bool isTarget);

        public void DamagePlayer(string spellName, int damage, Player player, Player target, Room room);

        public bool AffectPlayerAttributes(string spellName, EffectLocation attribute, int value, Player player,
            Player target, Room room, string noAffect);

        public void UpdateClientUI(Player player);

        public void EmoteAction(Player player, Player target, Room room, SkillMessage emote);

        public void EmoteEffectWearOffAction(Player player, Room room, SkillMessage emote);

        public Player GetValidTarget(Player player, Player target, ValidTargets validTargets);

        public Player findTarget(Player player, string target, Room room, bool murder);
    }
}

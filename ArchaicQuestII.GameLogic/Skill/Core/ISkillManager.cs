using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public interface ISkillManager
    {
        public string ReplacePlaceholders(string str, Player player, bool isTarget);

        public void DamagePlayer(
            string spellName,
            int damage,
            Player player,
            Player target,
            Room room
        );

        public void UpdateClientUI(Player player);

        public void EmoteAction(Player player, Player target, Room room, SkillMessage emote);
    }
}

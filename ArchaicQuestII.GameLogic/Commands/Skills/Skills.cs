using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
   public class Skills: ISkills
    {

        private readonly IWriteToClient _writeToClient;
        private readonly IUpdateClientUI _clientUi;
        private readonly ICache _cache;
        private readonly IGain _gain;

        public Skills(IWriteToClient writeToClient, ICache cache, IUpdateClientUI clientUi, IGain gain)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _clientUi = clientUi;
            _gain = gain;
        }
        public void ShowSkills(Player player)
        {

            _writeToClient.WriteLine("Skills:");

            foreach (var skill in player.Skills)
            {
                _writeToClient.WriteLine($"Level {skill.Level} : {skill.SkillName} {skill.Proficiency}%", player.ConnectionId);
            }
        }

        public bool SuccessCheck(Player player, string skillName)
        {
            var skill = player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase));

            var chance = new Dice().Roll(1, 1, 100);

            if (skill != null && skill.Proficiency <= chance || chance == 1)
            {
                return false;
            }

            return true;
        }


        public void LearnMistakes(Player player, string skillName, int delay = 0)
        {
            var skill = player.Skills.FirstOrDefault(x => x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase));

            if (skill == null)
            {
                return;
            }

            if (skill.Proficiency == 100)
            {
                return;
            }

            var increase = new Dice().Roll(1, 1, 5);

            skill.Proficiency += increase;

            _gain.GainExperiencePoints(player, 100 * skill.Level / 4, false);

            _clientUi.UpdateExp(player);

            _writeToClient.WriteLine(
                $"<p class='improve'>You learn from your mistakes and gain {100 * skill.Level / 4} experience points.</p>" +
                $"<p class='improve'>Your knowledge of {skill.SkillName} increases by {increase}%.</p>",
                player.ConnectionId, delay);
        }
    }
}

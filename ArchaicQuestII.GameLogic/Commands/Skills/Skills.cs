using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core; 

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
   public class Skills: ISkills
    {

        private readonly IWriteToClient _writeToClient;
        private readonly ICache _cache;

        public Skills(IWriteToClient writeToClient, ICache cache)
        {
            _writeToClient = writeToClient;
            _cache = cache;
        }
        public void ShowSkills(Player player)
        {

            _writeToClient.WriteLine("Skills:");

            foreach (var skill in player.Skills)
            {
                _writeToClient.WriteLine($"Level {skill.Level} : {skill.SkillName} {skill.Proficiency}%", player.ConnectionId);
            }
        }
    }
}

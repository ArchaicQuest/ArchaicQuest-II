using System.Collections.Generic;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands
{
    public interface ICommandHandler
    {
        Task Tick();
        public void HandleCommand(Player player, Room room, string input);
        void AddCommand(string key, ICommand action);
        Dictionary<string, ICommand> GetCommands();
        bool IsCommand(string key);
        ICommand GetCommand(string key);
        void AddSocial(string key, Emote emote);
        Dictionary<string, Emote> GetSocials();
        List<Skill.Model.Skill> GetAllSkills();
        bool AddSkill(int id, Skill.Model.Skill skill);
        Skill.Model.Skill GetSkill(int id);
        List<Skill.Model.Skill> ReturnSkills();
        bool AddHelp(int id, Help help);
        Help GetHelp(int id);
        List<Help> FindHelp(string id);
        bool TargetCheck(string target, Player player, string errorMessage = "What?");
    }
}

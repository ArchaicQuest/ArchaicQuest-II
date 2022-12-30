using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class TrainCmd : ICommand
    {
        public TrainCmd(ICore core)
        {
            Aliases = new[] {"train"};
            Description = "Increase your stats.";
            Usages = new[] {"Type: train"};
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Fighting,
                CharacterStatus.Status.Ghost,
                CharacterStatus.Status.Fleeing,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Stunned
            };
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var stat = input.ElementAtOrDefault(1);

            if (room.Mobs.Find(x => x.Trainer) == null)
            {
                Core.Writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
                return;
            }

            if (player.Trains <= 0)
            {
                Core.Writer.WriteLine("<p>You have no training sessions left.</p>", player.ConnectionId);
                return;
            }

            if (string.IsNullOrEmpty(stat) || stat == "train")
            {

                Core.Writer.WriteLine(
                    $"<p>You have {player.Trains} training session{(player.Trains > 1 ? "s" : "")} remaining.<br />You can train: str dex con int wis cha hp mana move.</p>", player.ConnectionId);
            }
            else
            {
                var statName = Helpers.GetStatName(stat);
                if (string.IsNullOrEmpty(statName.Item1))
                {
                    Core.Writer.WriteLine(
                        $"<p>{stat} not found. Please choose from the following. <br /> You can train: str dex con int wis cha hp mana move.</p>", player.ConnectionId);
                    return;
                }

                player.Trains -= 1;
                if (player.Trains < 0)
                {
                    player.Trains = 0;
                }

                if (statName.Item1 is "hit points" or "moves" or "mana")
                {
                    var hitDie = Core.Cache.GetClass(player.ClassName);
                    var roll = Core.Dice.Roll(1, hitDie.HitDice.DiceMinSize, hitDie.HitDice.DiceMaxSize);

                    player.MaxAttributes.Attribute[statName.Item2] += roll;
                    player.Attributes.Attribute[statName.Item2] += roll;

                    Core.Writer.WriteLine(
                        $"<p class='gain'>Your {statName.Item1} increases by {roll}.</p>", player.ConnectionId);

                    Core.UpdateClient.UpdateHP(player);
                    Core.UpdateClient.UpdateMana(player);
                    Core.UpdateClient.UpdateMoves(player);
                }
                else
                {
                    player.MaxAttributes.Attribute[statName.Item2] += 1;
                    player.Attributes.Attribute[statName.Item2] += 1;

                    Core.Writer.WriteLine(
                        $"<p class='gain'>Your {statName.Item1} increases by 1.</p>", player.ConnectionId);
                }
                
                Core.UpdateClient.UpdateScore(player);
            }
        }
    }
}
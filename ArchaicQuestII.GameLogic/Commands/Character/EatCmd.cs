using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class EatCmd : ICommand
    {
        public EatCmd(ICoreHandler coreHandler)
        {
            Aliases = new[] {"eat"};
            Description = "Eat some food, eating is for RP purposes only. Your character does not get hungry or thirsty.";
            Usages = new[] {"Type: eat apple"};
            Title = "";
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

            Handler = coreHandler;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        
        public ICoreHandler Handler { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(1);

            if (string.IsNullOrEmpty(target))
            {
                Handler.Client.WriteLine("<p>Eat what?</p>", player.ConnectionId);
                return;
            }
            
            var findNth = Helpers.findNth(target);
            var food = Helpers.findObjectInInventory(findNth, player);

            if (food == null)
            {
                Handler.Client.WriteLine("<p>You have no food of that name.</p>", player.ConnectionId);
                return;
            }

            if (food.ItemType != Item.Item.ItemTypes.Food)
            {
                Handler.Client.WriteLine($"<p>You can't eat {food.Name.ToLower()}.</p>", player.ConnectionId);
                return;
            }

            if (player.Hunger >= 4)
            {
                Handler.Client.WriteLine("<p>You are too full to eat more.</p>", player.ConnectionId);
                return;
            }

            player.Hunger++;

            player.Inventory.Remove(food);

            Handler.Client.WriteLine($"<p>You eat {food.Name.ToLower()}.</p>", player.ConnectionId);
            Handler.Client.WriteToOthersInRoom($"<p>{player.Name} eats {food.Name.ToLower()}.</p>", room, player);

            var benefits = new StringBuilder().Append("<table>");
            var modBenefits = "";
            var hasEffect = player.Affects.Custom.FirstOrDefault(x => x.Name.Equals(food.Name));
            var newEffect = new Affect
            {
                Modifier = new Modifier()
            };
            
            if (hasEffect == null)
            {
                modBenefits = Helpers.UpdateAffect(player, food, newEffect);

                benefits.Append(
                    $"<tr><td>Food:</td><td>{food.Name}<br />{modBenefits}</td></td>");
                benefits.Append("</table>");

                newEffect.Name = food.Name;
                newEffect.Benefits = benefits.ToString();

                player.Affects.Custom.Add(newEffect);
            }
            else
            {
                modBenefits = Helpers.UpdateAffect(player, food, hasEffect);
                
                benefits.Append(
                    $"<tr><td>Food:</td><td>{food.Name}<br />{modBenefits}</td></td>");
                benefits.Append("</table>");
                
                hasEffect.Benefits = benefits.ToString();
            }

            if (player.Hunger >= 4)
            {
                Handler.Client.WriteLine("<p>You are no longer hungry.<p>", player.ConnectionId);
            }

            Handler.Client.UpdateAffects(player);
            Handler.Client.UpdateScore(player);
            Handler.Client.UpdateMoves(player);
            Handler.Client.UpdateHP(player);
            Handler.Client.UpdateMana(player);
            Handler.Client.UpdateInventory(player);
        }
    }
}
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class EatCmd : ICommand
    {
        public EatCmd(ICore core)
        {
            Aliases = new[] {"eat"};
            Description = "Consume something.";
            Usages = new[] {"Type: eat apple"};
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(1);

            if (string.IsNullOrEmpty(target))
            {
                Core.Writer.WriteLine("Eat what?", player.ConnectionId);
                return;
            }
            
            var findNth = Helpers.findNth(target);
            var food = Helpers.findObjectInInventory(findNth, player);

            if (food == null)
            {
                Core.Writer.WriteLine("You have no food of that name.", player.ConnectionId);
                return;
            }

            if (food.ItemType != Item.Item.ItemTypes.Food)
            {
                Core.Writer.WriteLine($"You can't eat {food.Name.ToLower()}.", player.ConnectionId);
                return;
            }

            if (player.Hunger >= 4)
            {
                Core.Writer.WriteLine("You are too full to eat more.", player.ConnectionId);
                return;
            }

            player.Hunger++;

            player.Inventory.Remove(food);

            Core.Writer.WriteLine($"You eat {food.Name.ToLower()}.", player.ConnectionId);
            
            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Core.Writer.WriteLine($"{player.Name} eats {food.Name.ToLower()}.", pc.ConnectionId);
            }

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
                Core.Writer.WriteLine("You are no longer hungry.", player.ConnectionId);
            }

            Core.UpdateClient.UpdateAffects(player);
            Core.UpdateClient.UpdateScore(player);
            Core.UpdateClient.UpdateMoves(player);
            Core.UpdateClient.UpdateHP(player);
            Core.UpdateClient.UpdateMana(player);
            Core.UpdateClient.UpdateInventory(player);
        }
    }
}
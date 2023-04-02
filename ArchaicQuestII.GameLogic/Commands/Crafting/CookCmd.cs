using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Crafting
{
    public class CookCmd : ICommand
    {
        public CookCmd(ICore core)
        {
            Aliases = new[] {"cook"};
            Description = "Cook food at a fire. A fire and a cook pot is needed.";
            Usages = new[] {"Type: cook"};
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
                CharacterStatus.Status.Stunned,
                CharacterStatus.Status.Resting
            };
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var pot = room.Items.FirstOrDefault(x => x.ItemType == Item.Item.ItemTypes.Cooking);

            if (pot == null)
            {
                Core.Writer.WriteLine("<p>You require a fire and a cooking pot before you can cook.</p>",
                    player.ConnectionId);

                return;
            }

            var modifiers = new Item.Modifier();
            var edibleItems = new List<Item.Item>();
            var inedibleItems = new List<Item.Item>();
            var cookTime = 0;

            foreach(var ingredient in pot.Container.Items)
            {
                switch(ingredient.ItemType)
                {
                    case Item.Item.ItemTypes.Food:
                    case Item.Item.ItemTypes.Cooked:
                    case Item.Item.ItemTypes.Drink:
                    case Item.Item.ItemTypes.Forage:
                    case Item.Item.ItemTypes.Potion:
                        edibleItems.Add(ingredient);
                        break;
                    default:
                        inedibleItems.Add(ingredient);
                        break;
                }

                cookTime += 500;
            }

            Cook(player, room, pot, edibleItems, inedibleItems, cookTime).Start();
        }

        private async Task Cook(Player player, Room room, Item.Item pot, List<Item.Item> edibleItems, List<Item.Item> inedibleItems, int cookTime)
        {
            Core.Writer.WriteLine("<p>You begin cooking.</p>",
                    player.ConnectionId);

            Core.UpdateClient.PlaySound("cooking", player);

            Core.Writer.WriteLine("<p>You stir the ingredients.</p>",
                player.ConnectionId, cookTime/3);

            Core.Writer.WriteLine("<p>You taste and season the dish.</p>",
                player.ConnectionId, cookTime/3);

            Core.Writer.WriteLine("<p>You stir the ingredients.</p>",
                player.ConnectionId, cookTime/3);

            var success = !inedibleItems.Any() && Helpers.SkillSuccessCheck(player, "Cooking");

            if(!success)
            {
                foreach(var item in edibleItems)
                {
                    pot.Container.Items.Remove(item);
                }

                foreach(var item in inedibleItems)
                {
                    if(DiceBag.FlipCoin())
                        pot.Container.Items.Remove(item);
                }

                Core.Writer.WriteLine(
                        "<p class='improve'>You failed to create something edible.</p>",
                        player.ConnectionId, cookTime + 500);

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Core.Writer.WriteLine($"<p>{player.Name} fails to cook something edible</p>",
                        pc.ConnectionId, cookTime + 500);
                }

                Helpers.SkillLearnMistakes(player, "Cooking", Core.Gain, cookTime + 500);
            }
            else
            {
                await Task.Delay(cookTime + 500);

                var cookedItem = GenerateCookedItem(edibleItems);

                pot.Container.Items.Clear();
                pot.Container.Items.Add(cookedItem);

                Core.Writer.WriteLine(
                        $"<p class='improve'>You cooked {cookedItem.Name}.</p>",
                        player.ConnectionId, cookTime + 500);

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Core.Writer.WriteLine($"<p>{player.Name} cooked {cookedItem.Name}.</p>",
                        pc.ConnectionId, cookTime + 500);
                }
            }

            Core.UpdateClient.UpdateInventory(player);
            Core.UpdateClient.UpdateScore(player);
        }

        private Item.Item GenerateCookedItem(List<Item.Item> ingredients)
        {
            var prefixes = new List<string>
            {
                "Boiled",
                "Baked",
                "Fried",
                "Toasted",
                "Smoked",
                "Roasted",
                "Poached"
            };

            var suffixes = new List<string>
            {
                "soup",
                "stew",
                "pie",
                "curry",
                "skewer"
            };

            var ingredientOrder = ingredients.OrderByDescending(item => item.Item2);
            var mainIngredient = ingredientOrder.First();

            var foodName = "";
            
            if (DiceBag.Roll(1, 1, 2) == 1)
            {
                var prefix = prefixes[DiceBag.Roll(1, 0, 6)];

                foodName = $"{prefix} {Helpers.RemoveArticle(mainIngredient.Item1.Name).ToLower()} {(ingredientOrder.Count() > 1 ? $"with {Helpers.RemoveArticle(ingredientOrder.ElementAt(1).Item1.Name).ToLower()}" : "")} {(ingredientOrder.Count() > 2 ? $"and {Helpers.RemoveArticle(ingredientOrder.ElementAt(2).Item1.Name).ToLower()}" : "")}";
            }
            else
            {
                var suffix = suffixes[DiceBag.Roll(1, 0, 4)];

                foodName = $"{Helpers.RemoveArticle(mainIngredient.Item1.Name)} {(ingredientOrder.Count() > 1 ? $"with {Helpers.RemoveArticle(ingredientOrder.ElementAt(1).Item1.Name).ToLower()}" : "")} {(ingredientOrder.Count() > 2 ? $"  {Helpers.RemoveArticle(ingredientOrder.ElementAt(2).Item1.Name).ToLower()} " : "")}{suffix}";
            }

            var food = new Item.Item()
            {
                Name = foodName,
                ArmourRating = new ArmourRating(),
                Value = 75,
                Portal = new Portal(),
                ItemType = Item.Item.ItemTypes.Cooked,
                Container = new Container(),
                Description = new Description()
                {
                    Look =
                        $"A tasty looking {foodName.ToLower()} made with {Helpers.RemoveArticle(mainIngredient.Item1.Name).ToLower()}s{(ingredientOrder.Count() > 1 ? $" and {Helpers.RemoveArticle(ingredientOrder.ElementAt(1).Item1.Name).ToLower()}." : ".")}",
                    Exam =
                        $"A tasty looking {foodName.ToLower()} made with {Helpers.RemoveArticle(mainIngredient.Item1.Name).ToLower()}s{(ingredientOrder.Count() > 1 ? $" and {Helpers.RemoveArticle(ingredientOrder.ElementAt(1).Item1.Name).ToLower()}." : ".")}"
                },
                Modifier = new Modifier()
                {
                    HP = CalculateModifer(ingredientOrder, "hp"),
                    Strength = CalculateModifer(ingredientOrder, "strength"),
                    Dexterity = CalculateModifer(ingredientOrder, "dexterity"),
                    Constitution = CalculateModifer(ingredientOrder, "constitution"),
                    Intelligence = CalculateModifer(ingredientOrder, "intelligence"),
                    Wisdom = CalculateModifer(ingredientOrder, "wisdom"),
                    Charisma = CalculateModifer(ingredientOrder, "charisma"),
                    Moves = CalculateModifer(ingredientOrder, "moves"),
                    Mana = CalculateModifer(ingredientOrder, "mana"),
                    DamRoll = CalculateModifer(ingredientOrder, "damroll"),
                    HitRoll = CalculateModifer(ingredientOrder, "hitroll"),
                    Saves = CalculateModifer(ingredientOrder, "saves"),
                },
                Level = 1,
                Slot = Equipment.EqSlot.Held,
                Uses = 1,
                Weight = 0.3F,

            };
            
            return food;
        }
    }
}
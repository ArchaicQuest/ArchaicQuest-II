using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Aliases = new[] {"Cook"};
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
            if (room.Items.FirstOrDefault(x => x.ItemType == Item.Item.ItemTypes.Cooking) == null)
            {
                Core.Writer.WriteLine("<p>You require a fire and a cooking pot before you can cook.</p>",
                    player.ConnectionId);

                return;
            }

            var pot = room.Items.FirstOrDefault(x => x.ItemType == Item.Item.ItemTypes.Cooking);

            // What happens if player throws in random shit which is not a food item
            var items = pot.Container.Items.Where(x => x.ItemType == Item.Item.ItemTypes.Food).ToList();

            if (items.Count < 3)
            {
                Core.Writer.WriteLine("<p>You need 3 raw ingredients before you can cook.</p>",
                    player.ConnectionId);
                
                if (pot.Container.Items.FirstOrDefault(x => x.ItemType != Item.Item.ItemTypes.Food) != null)
                {
                    Core.Writer.WriteLine($"<p>The following ingredients cannot be cooked with.</p>",
                        player.ConnectionId);

                    var sb = new StringBuilder();
                    sb.Append("<p>");
                    foreach (var invalidItem in pot.Container.Items.Where(x => x.ItemType != Item.Item.ItemTypes.Food))
                    {
                        sb.Append($"{invalidItem.Name}, ");
                    }
                    sb.Append("</p>");

                    Core.Writer.WriteLine(sb.ToString(),
                        player.ConnectionId);

                    return;
                }

                return;
            }

            if (items.Count > 3)
            {
                Core.Writer.WriteLine("<p>You can only cook with 3 raw food ingredients. The following ingredients are not raw food and can't be cooked.</p>",
                    player.ConnectionId);

                var sb = new StringBuilder();
                sb.Append("<p>");
                foreach (var invalidItem in pot.Container.Items.Where(x => x.ItemType != Item.Item.ItemTypes.Food))
                {
                    sb.Append($"{invalidItem.Name}, ");
                }
                sb.Append("</p>");

                Core.Writer.WriteLine(sb.ToString(),
                    player.ConnectionId);

                return;
            }

            if (items.Count == 3 && pot.Container.Items.FirstOrDefault(x => x.ItemType != Item.Item.ItemTypes.Food) != null)
            {
                Core.Writer.WriteLine($"<p>You can only cook with 3 raw ingredients. The following ingredients cannot be cooked with.</p>",
                    player.ConnectionId);

                var sb = new StringBuilder();
                sb.Append("<p>");
                foreach (var invalidItem in pot.Container.Items.Where(x => x.ItemType != Item.Item.ItemTypes.Food))
                {
                    sb.Append($"{invalidItem.Name}, ");
                }
                sb.Append("</p>");

                Core.Writer.WriteLine(sb.ToString(),
                    player.ConnectionId);

                return;
            }
            
            var ingredients = new List<Tuple<Item.Item, int>>();

            foreach (var item in items)
            {
                var ingredient = ingredients.FirstOrDefault(x => x.Item1.Name.Equals(item.Name));
                if (ingredient != null)
                {
                    var index = ingredients.FindIndex(x => x.Item1.Name.Equals(ingredient.Item1.Name));

                    var count = ingredient.Item2 + 1;
                    ingredients[index] = Tuple.Create(item, count);
                }
                else
                {
                    ingredients.Add(new Tuple<Item.Item, int>(item, 1));
                }
            }

            pot.Container.Items = new ItemList();
            var cookedItem = GenerateCookedItem(ingredients);
            Core.Writer.WriteLine("<p>You begin cooking.</p>",
                player.ConnectionId);
            Core.Writer.WriteLine("<p>You stir the ingredients.</p>",
                 player.ConnectionId, 1000);

            Core.Writer.WriteLine("<p>You taste and season the dish.</p>",
                player.ConnectionId, 2500);

            Core.Writer.WriteLine("<p>You stir the ingredients.</p>",
                player.ConnectionId, 5000);

            var success = Helpers.SkillSuccessCheck(player, "cooking");

            if (success)
            {
                Core.Writer.WriteLine(
                    $"<p class='improve'>You have successfully created {Helpers.AddArticle(cookedItem.Name).ToLower()}.</p>",
                    player.ConnectionId, 6000);

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Core.Writer.WriteLine($"<p>{player.Name} has cooked {cookedItem.Name}</p>",
                        pc.ConnectionId, 6000);
                }

                player.Inventory.Add(cookedItem);
                player.Weight += cookedItem.Weight;
            }
            else
            {
                Core.Writer.WriteLine(
                    $"<p class='improve'>You have fail to create {Helpers.AddArticle(cookedItem.Name).ToLower()}.</p>",
                    player.ConnectionId, 6000);

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Core.Writer.WriteLine($"<p>{player.Name} fails to cook {cookedItem.Name}</p>",
                        pc.ConnectionId, 6000);
                }

                Helpers.SkillLearnMistakes(player, "Cooking", Core.Gain, 6000);
            }
            Core.UpdateClient.UpdateInventory(player);
            Core.UpdateClient.UpdateScore(player);
        }

        private Item.Item GenerateCookedItem(IEnumerable<Tuple<Item.Item, int>> ingredients)
        {
            var prefixes = new List<string>
            {
                "Boiled",
                "Baked",
                "Fried",
                "Toasted",
                "Smoked",
                "Roast",
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
                var suffix = suffixes[DiceBag.Roll(1, 0, 5)];

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

        private int CalculateModifer(IOrderedEnumerable<Tuple<Item.Item, int>> ingredients, string name)
        {

            var modValue = 0;
            var mainIngredient = ingredients.First();
            var ingredient2 = ingredients.ElementAtOrDefault(1);
            var ingredient3 = ingredients.ElementAtOrDefault(2);
            switch (name)
            {
                case "strength":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Strength,
                        mainIngredient.Item1.Modifier.Strength * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Strength,
                                ingredient2.Item1.Modifier.Strength * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Strength,
                                ingredient3.Item1.Modifier.Strength * ingredient3.Item2)
                            : 0);
                    modValue *= 2;

                    break;
                case "dexterity":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Dexterity,
                        mainIngredient.Item1.Modifier.Dexterity * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Dexterity,
                                ingredient2.Item1.Modifier.Dexterity * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Dexterity,
                                ingredient3.Item1.Modifier.Dexterity * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "constitution":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Constitution,
                        mainIngredient.Item1.Modifier.Constitution * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Constitution,
                                ingredient2.Item1.Modifier.Constitution * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Constitution,
                                ingredient3.Item1.Modifier.Constitution * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "intelligence":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Intelligence,
                        mainIngredient.Item1.Modifier.Intelligence * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Intelligence,
                                ingredient2.Item1.Modifier.Intelligence * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Intelligence,
                                ingredient3.Item1.Modifier.Intelligence * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "wisdom":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Wisdom,
                        mainIngredient.Item1.Modifier.Wisdom * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Wisdom,
                                ingredient2.Item1.Modifier.Wisdom * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Wisdom,
                                ingredient3.Item1.Modifier.Wisdom * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "charisma":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Charisma,
                        mainIngredient.Item1.Modifier.Charisma * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Charisma,
                                ingredient2.Item1.Modifier.Charisma * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Charisma,
                                ingredient3.Item1.Modifier.Charisma * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "damroll":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.DamRoll,
                        mainIngredient.Item1.Modifier.DamRoll * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.DamRoll,
                                ingredient2.Item1.Modifier.DamRoll * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.DamRoll,
                                ingredient3.Item1.Modifier.DamRoll * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "hitroll":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.HitRoll,
                        mainIngredient.Item1.Modifier.HitRoll * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.HitRoll,
                                ingredient2.Item1.Modifier.HitRoll * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.HitRoll,
                                ingredient3.Item1.Modifier.HitRoll * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "mana":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Mana,
                        mainIngredient.Item1.Modifier.Mana * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Mana,
                                ingredient2.Item1.Modifier.Mana * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Mana,
                                ingredient3.Item1.Modifier.Mana * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "moves":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Moves,
                        mainIngredient.Item1.Modifier.Moves * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Moves,
                                ingredient2.Item1.Modifier.Moves * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Moves,
                                ingredient3.Item1.Modifier.Moves * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "hp":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.HP,
                        mainIngredient.Item1.Modifier.HP * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.HP,
                                ingredient2.Item1.Modifier.HP * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.HP,
                                ingredient3.Item1.Modifier.HP * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
                case "saves":
                    modValue = DiceBag.Roll(1, ingredients.First().Item1.Modifier.Saves,
                        mainIngredient.Item1.Modifier.Saves * mainIngredient.Item2);
                    modValue +=
                        (ingredient2 != null
                            ? DiceBag.Roll(1, ingredient2.Item1.Modifier.Saves,
                                ingredient2.Item1.Modifier.Saves * ingredient2.Item2)
                            : 0);
                    modValue +=
                        (ingredient3 != null
                            ? DiceBag.Roll(1, ingredient3.Item1.Modifier.Saves,
                                ingredient3.Item1.Modifier.Saves * ingredient3.Item2)
                            : 0);
                    modValue *= 2;
                    break;
            }

            return modValue;
        }
    }
}
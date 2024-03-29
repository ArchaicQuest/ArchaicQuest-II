using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Crafting
{
    public class CookCmd : ICommand
    {
        public CookCmd()
        {
            Aliases = new[] { "cook" };
            Description =
                "Cook food, type cook list to see which items you can cook. Then place items in a pot and cook 'item'";
            Usages = new[]
            {
                "Type: cook list - to view all cook-able items. \n\r cook <item> - to cook the item e.g. cook fishstew"
            };
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
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = string.Join(" ", input.Skip(1));

            // Lets find what you can cook
            var recipes = Services.Instance.Cache
                .GetCraftingRecipes()
                .Where(x => x.CreatedItem.ItemType == Item.Item.ItemTypes.Food)
                .ToList();

            if (recipes == null)
            {
                Services.Instance.Writer.WriteLine("<p>No recipes are known.</p>", player);
                return;
            }

            if (!string.IsNullOrEmpty(target) && target.Equals("list"))
            {
                ListRecipes(player, true, recipes);
                return;
            }

            if (string.IsNullOrEmpty(target))
            {
                ListRecipes(player, false, recipes);
                return;
            }

            CookItem(player, room, target, recipes);
        }

        private void ListRecipes(Player player, bool showAllRecipes, List<CraftingRecipes> recipes)
        {
            var recipeList = showAllRecipes ? recipes : ReturnValidRecipes(player, recipes);

            if (recipeList.Count == 0)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>No recipes found with the current items you have.</p>",
                    player
                );
                return;
            }

            var sb = new StringBuilder();
            sb.Append("<p>You can cook the following items:</p>");
            sb.Append("<table class='simple'>");
            sb.Append($"<tr><td>Name</td><td>Ingredients</td></tr>");
            foreach (var recipe in recipeList.Distinct())
            {
                var inredientsRequired = "";
                foreach (var ingredient in recipe.CraftingMaterials)
                {
                    inredientsRequired += $"{ingredient.Material} x{ingredient.Quantity}, ";
                }

                sb.Append($"<tr><td>{recipe.Title}</td><td>{inredientsRequired}</td></tr>");
            }

            sb.Append($"</table>");

            Services.Instance.Writer.WriteLine(sb.ToString(), player);
        }

        private List<CraftingRecipes> ReturnValidRecipes(
            Player player,
            List<CraftingRecipes> recipes
        )
        {
            var ingedients = player.Inventory
                .Where(x => x.ItemType == Item.Item.ItemTypes.Food)
                .GroupBy(y => y.Name)
                .Select(z => z.First());
            var recipeList = new List<CraftingRecipes>();
            foreach (var material in ingedients)
            {
                var quantity = player.Inventory
                    .Where(
                        x =>
                            x.ItemType == Item.Item.ItemTypes.Material
                            && x.Name.Equals(
                                material.Name,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                    .ToList();
                var canCook = recipes.Where(
                    x =>
                        x.CraftingMaterials.Any(
                            y =>
                                y.Material.Equals(
                                    material.Name,
                                    StringComparison.CurrentCultureIgnoreCase
                                )
                                && y.Quantity <= quantity.Count
                        )
                );

                recipeList.AddRange(canCook);
            }

            return recipeList;
        }

        public void CookItem(Player player, Room room, string item, List<CraftingRecipes> recipes)
        {
            var craftingRecipes = ReturnValidRecipes(player, recipes);

            var pot = room.Items.FirstOrDefault(x => x.ItemType == Item.Item.ItemTypes.Cooking);

            if (pot == null)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>To being cooking you require a fire and a cooking pot.</p>",
                    player
                );

                return;
            }

            var recipe = craftingRecipes.FirstOrDefault(
                x => x.Title.StartsWith(item, StringComparison.CurrentCultureIgnoreCase)
            );

            if (recipe == null)
            {
                Services.Instance.Writer.WriteLine("<p>You can't cook that.</p>", player);
                return;
            }

            foreach (var material in recipe.CraftingMaterials)
            {
                var craftItem = pot.Container.Items.FirstOrDefault(
                    x => x.Name.Equals(material.Material, StringComparison.CurrentCultureIgnoreCase)
                );
                var materialCount = pot.Container.Items.Count(
                    x => x.Name.Equals(material.Material, StringComparison.CurrentCultureIgnoreCase)
                );

                if (craftItem == null || material.Quantity > materialCount)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You appear to be missing required items.</p>",
                        player
                    );
                    return;
                }
            }

            Cook(player, room, pot, recipe, 4000).Start();
        }

        private async Task Cook(
            Player player,
            Room room,
            Item.Item pot,
            CraftingRecipes recipe,
            int cookTime
        )
        {
            player.Status = CharacterStatus.Status.Busy;

            Services.Instance.Writer.WriteLine("<p>You begin cooking.</p>", player);

            Services.Instance.UpdateClient.PlaySound("cooking", player);

            await Task.Delay(cookTime / 4);

            Services.Instance.Writer.WriteLine("<p>You stir the ingredients.</p>", player);

            await Task.Delay(cookTime / 4);

            Services.Instance.Writer.WriteLine("<p>You taste and season the dish.</p>", player);

            await Task.Delay(cookTime / 4);
            Services.Instance.Writer.WriteLine("<p>You stir the ingredients.</p>", player);

            await Task.Delay(cookTime / 4);

            pot.Container.Items.Clear();

            if (!player.RollSkill(SkillName.Cooking, false))
            {
                Services.Instance.Writer.WriteLine(
                    "<p class='improve'>You failed to cook something edible.</p>",
                    player
                );

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{player.Name} fails to cook something edible</p>",
                        pc
                    );
                }

                player.FailedSkill(SkillName.Cooking, true);
            }
            else
            {
                pot.Container.Items.Add(recipe.CreatedItem);

                Services.Instance.Writer.WriteLine(
                    $"<p class='improve'>You cooked {recipe.Title}.</p>",
                    player
                );

                foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{player.Name} cooked {recipe.Title}.</p>",
                        pc
                    );
                }
            }

            player.Status = CharacterStatus.Status.Standing;

            Services.Instance.UpdateClient.UpdateInventory(player);
            Services.Instance.UpdateClient.UpdateScore(player);
        }
    }
}

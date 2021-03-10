using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Alignment;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Controllers.Core
{
    [Authorize]
    public class CraftingController : Controller
    {
        private IDataBase _db { get; }
        public CraftingController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/Crafting")]
        public HttpStatusCode Post([FromBody] CraftingRecipes recipes)
        {
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid Crafting Recipes");
                throw exception;
            }

            if (recipes == null) { return HttpStatusCode.BadRequest; }

            var newRecipe = new CraftingRecipes()
            {
                CraftingMaterials = recipes.CraftingMaterials,
                Title = recipes.Title,
                Description = recipes.Description,
                CreatedItem = recipes.CreatedItem,
                CreatedItemDropsInRoom = recipes.CreatedItemDropsInRoom,
                DateUpdated = DateTime.Now
            };

            if (recipes.Id != -1)
            {

                var foundItem = _db.GetById<CraftingRecipes>(recipes.Id, DataBase.Collections.CraftingRecipes);

                if (foundItem == null)
                {
                    throw new Exception("recipe Id does not exist");
                }

                newRecipe.Id = foundItem.Id;
            }

            _db.Save(newRecipe, DataBase.Collections.CraftingRecipes);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail = $"({newRecipe.Id}) {newRecipe.Title}",
                Type = DataBase.Collections.Quests,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);
            return HttpStatusCode.OK;

        }

        [HttpGet]
        [Route("api/Crafting")]
        public CraftingRecipes Get(int id)
        {
            return _db.GetById<CraftingRecipes>(id, DataBase.Collections.CraftingRecipes);
        }

        [HttpGet]
        [Route("api/Crafting/GetCrafting")]
        public List<CraftingRecipes> GetCrafting()
        {

            return _db.GetList<CraftingRecipes>(DataBase.Collections.CraftingRecipes).ToList();

        }

    }
}

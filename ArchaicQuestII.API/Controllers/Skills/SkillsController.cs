using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Item;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Skill.Model;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers.Skills
{
    public class SkillsController : Controller
    {
        private IDataBase _db { get; }
        public SkillsController(IDataBase db)
        {
            _db = db;
        }
        [HttpPost]
        [Route("api/skill/postSkill")]
        public void PostItem([FromBody] Skill skill)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid object");
                throw exception;
            }

            var newSkill = new Skill()
            {
                Name = skill.Name,
                Cost = skill.Cost,
                Damage = skill.Damage,
                Description = skill.Description,
                Effect = skill.Effect,
                LevelBasedMessages = skill.LevelBasedMessages,
                Requirements = skill.Requirements,
                Rounds = skill.Rounds,
                SkillAction = skill.SkillAction,
                SkillEnd = skill.SkillEnd,
                SkillFailure = skill.SkillFailure,
                SkillStart = skill.SkillStart,
                Type = skill.Type
            };

            if (!string.IsNullOrEmpty(skill.Id.ToString()) && skill.Id != -1)
            {

                var foundItem = _db.GetById<Skill>(skill.Id, DataBase.Collections.Skill);

                if (foundItem == null)
                {
                    throw new Exception("Item Id does not exist");
                }

                newSkill.Id = skill.Id;
            }



            _db.Save(newSkill, DataBase.Collections.Skill);

        }


        //[HttpGet]
        //[Route("api/skill/Get")]
        //public List<Item> GetSkill()
        //{

        //    return _db.GetList<Item>(DataBase.Collections.Items).Where(x => x.Deleted == false).ToList();

        //}


        //[HttpGet]
        //[Route("api/item/FindItems")]
        //public List<Item> FindItems([FromQuery] string query)
        //{

        //    var items = _db.GetCollection<Item>(DataBase.Collections.Items).FindAll().Where(x => x.Name != null);

        //    if (string.IsNullOrEmpty(query))
        //    {
        //        return items.ToList();
        //    }

        //    return items.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        //}

        //[HttpGet]
        //[Route("api/item/FindItemsByType")]
        //public List<Item> FindItemsByType([FromQuery] Equipment.EqSlot query)
        //{

        //    var items = _db.GetCollection<Item>(DataBase.Collections.Items).FindAll().Where(x => x.Slot.Equals(query));

        //    return items.ToList();

        //}

        //[HttpGet]
        //[Route("api/item/FindItemById")]
        //public Item FindItemById([FromQuery] int id)
        //{

        //    return _db.GetCollection<Item>(DataBase.Collections.Items).FindById(id);

        //}

        //[HttpGet]
        //[Route("api/item/FindKeyById")]
        //public Item FindKeyById([FromQuery] string id)
        //{

        //    return _db.GetCollection<Item>(DataBase.Collections.Items).FindOne(x => x.KeyId.Equals(new Guid(id)) && x.ItemType == Item.ItemTypes.Key);

        //}

        //[HttpGet]
        //[Route("api/item/FindKeys")]
        //public List<Item> FindKeys([FromQuery] string query)
        //{

        //    var items = _db.GetCollection<Item>(DataBase.Collections.Items).FindAll().Where(x => x.Name != null && x.ItemType == Item.ItemTypes.Key);

        //    if (string.IsNullOrEmpty(query))
        //    {
        //        return items.ToList();
        //    }

        //    return items.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        //}

        //[HttpGet]
        //[Route("api/item/ContainerSize")]
        //public JsonResult ContainerSize()
        //{
        //    var containerSize = new List<object>();

        //    foreach (var size in Enum.GetValues(typeof(Container.ContainerSize)))
        //    {

        //        containerSize.Add(new
        //        {
        //            id = (int)size,
        //            name = size.ToString()
        //        });
        //    }
        //    return Json(containerSize);
        //}

        //[HttpGet]
        //[Route("api/item/LockStrength")]
        //public JsonResult LockStrength()
        //{
        //    var lockStrength = new List<object>();

        //    foreach (var lockDifficulty in Enum.GetValues(typeof(Item.LockStrength)))
        //    {

        //        lockStrength.Add(new
        //        {
        //            id = (int)lockDifficulty,
        //            name = lockDifficulty.ToString()
        //        });
        //    }
        //    return Json(lockStrength);
        //}

        //[HttpGet]
        //[Route("api/item/ReturnItemTypes")]
        //public JsonResult ReturnItemTypes()
        //{

        //    var itemTypes = new List<object>();

        //    foreach (var item in Enum.GetValues(typeof(Item.ItemTypes)))
        //    {

        //        itemTypes.Add(new
        //        {
        //            id = (int)item,
        //            name = item.ToString()
        //        });
        //    }
        //    return Json(itemTypes);

        //}

        //[HttpGet]
        //[Route("api/item/ReturnAttackTypes")]
        //public JsonResult ReturnAttackTypes()
        //{

        //    var itemTypes = new List<object>();

        //    foreach (var item in Enum.GetValues(typeof(Item.AttackTypes)))
        //    {

        //        itemTypes.Add(new
        //        {
        //            id = (int)item,
        //            name = item.ToString()
        //        });
        //    }
        //    return Json(itemTypes);

        //}

        //[HttpGet]
        //[Route("api/item/ReturnSlotTypes")]
        //public JsonResult ReturnSlotTypes()
        //{

        //    var itemTypes = new List<object>();

        //    foreach (var item in Enum.GetValues(typeof(Equipment.EqSlot)))
        //    {

        //        itemTypes.Add(new
        //        {
        //            id = (int)item,
        //            name = item.ToString()
        //        });
        //    }
        //    return Json(itemTypes);

        //}

        //[HttpGet]
        //[Route("api/item/ReturnFlagTypes")]
        //public JsonResult ReturnFlagTypes()
        //{

        //    var itemTypes = new List<object>();

        //    foreach (var item in Enum.GetValues(typeof(Item.ItemFlags)))
        //    {

        //        itemTypes.Add(new
        //        {
        //            id = (int)item,
        //            name = item.ToString()
        //        });
        //    }
        //    return Json(itemTypes);

        //}

        //[HttpGet]
        //[Route("api/item/ReturnDamageTypes")]
        //public JsonResult ReturnDamageTypes()
        //{

        //    var itemTypes = new List<object>();

        //    foreach (var item in Enum.GetValues(typeof(Item.DamageTypes)))
        //    {

        //        itemTypes.Add(new
        //        {
        //            id = (int)item,
        //            name = item.ToString()
        //        });
        //    }
        //    return Json(itemTypes);

        //}


        //[HttpGet]
        //[Route("api/item/ReturnWeaponTypes")]
        //public JsonResult ReturnWeaponTypes()
        //{

        //    var itemTypes = new List<object>();

        //    foreach (var item in Enum.GetValues(typeof(Item.WeaponTypes)))
        //    {

        //        itemTypes.Add(new
        //        {
        //            id = (int)item,
        //            name = item.ToString()
        //        });
        //    }
        //    return Json(itemTypes);

        //}


        //[HttpGet]
        //[Route("api/item/ReturnArmourTypes")]
        //public JsonResult ReturnArmourTypes()
        //{

        //    var itemTypes = new List<object>();

        //    foreach (var item in Enum.GetValues(typeof(Item.ArmourTypes)))
        //    {

        //        itemTypes.Add(new
        //        {
        //            id = (int)item,
        //            name = item.ToString()
        //        });
        //    }
        //    return Json(itemTypes);

        //}

        //[HttpDelete]
        //[Route("api/item/delete/{id:int}")]
        //public IActionResult Delete(int id)
        //{
        //    var item = _db.GetCollection<Item>(DataBase.Collections.Items).FindById(id);
        //    item.Deleted = true;
        //    var saved = _db.Save(item, DataBase.Collections.Items);

        //    if (saved)
        //    {
        //        return Ok(JsonConvert.SerializeObject(new { toast = $"{item.Name} deleted successfully." }));
        //    }
        //    return Ok(JsonConvert.SerializeObject(new { toast = $"{item.Name} deletion failed." }));



        //}


    }
}

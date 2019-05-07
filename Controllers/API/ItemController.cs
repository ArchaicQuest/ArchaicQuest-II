    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.Room;
using ArchaicQuestII.Engine.Item;
using ArchaicQuestII.Core.Events;
using ArchaicQuestII.Engine.Item.Enum;
using Microsoft.Azure.KeyVault.Models;
using static ArchaicQuestII.Engine.Item.Container;
using static ArchaicQuestII.Engine.Item.Item;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{
    public class ItemController : Controller
    {

        [HttpPost]
        [Route("api/item/PostItem")]
        public void PostItem([FromBody] Item item)
        {

 
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid object");
                throw exception;
            }

            var newItem = new Item()
            {
                Name = item.Name,
                Level = item.Level,
                ArmourRating = new ArmourRating()
                {
                    Armour = item.ArmourRating.Armour,
                    Magic = item.ArmourRating.Magic
                },
                ArmourType = item.ArmourType,
                AttackType = item.AttackType,
                Condition = item.Condition,
                Container = item.Container,
                Book = new Book()
                {
                    Blank = item.Book.Blank,
                    PageCount = item.Book.PageCount,
                    Pages = item.Book.Pages
                },
                DamageType = item.DamageType,
                Damage = new Damage()
                {
                    Maximum = item.Damage.Maximum,
                    Minimum = item.Damage.Minimum,
                },
                Modifier = new Modifier()
                {
                    DamRoll = item.Modifier.DamRoll,
                    HitRoll = item.Modifier.HitRoll,
                    HP = item.Modifier.HP,
                    Mana = item.Modifier.Mana,
                    Moves = item.Modifier.Moves,
                    SpellDam = item.Modifier.SpellDam,
                    Saves = item.Modifier.Saves,
                },
                DecayTimer = item.DecayTimer,
                Description = item.Description,
                Slot = item.Slot,
                ForageRank = item.ForageRank,
                Hidden = item.Hidden,
                isHiddenInRoom = item.isHiddenInRoom,
                ItemFlag = item.ItemFlag,
                Keywords = item.Keywords,
                KnownByName = item.KnownByName,
                QuestItem = item.QuestItem,
                Stuck = item.Stuck,
                ItemType = item.ItemType,
                Uses = item.Uses,
                WeaponSpeed = item.WeaponSpeed,
                WeaponType = item.WeaponType,
                Weight = item.Weight
            };

            if (item.ItemType == Item.ItemTypes.Key)
            {
                newItem.KeyId = new Guid();
            }


            if (!string.IsNullOrEmpty(item.Id.ToString()) && item.Id != -1)
            {

                var foundItem = DB.GetItems().FirstOrDefault(x => x.Id.Equals(item.Id));

                if (foundItem == null)
                {
                    throw new Exception("Item Id does not exist");
                }

                newItem.Id = item.Id;
            }
            


            DB.SaveItem(newItem);

        }

 
        [HttpGet]
        [Route("api/item/Get")]
        public List<Item> GetItem()
        {

            var items = DB.GetItems();

            return items;

        }


        [HttpGet]
        [Route("api/item/FindItems")]
        public List<Item> FindItems([FromQuery] string query)
        {

            var items = DB.GetItems().Where(x => x.Name != null);



            if (string.IsNullOrEmpty(query))
            {
                return items.ToList();
            }

            return items.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        }

        [HttpGet]
        [Route("api/item/FindItemById")]
        public Item FindItemById([FromQuery] int id)
        {

            return DB.GetItems().FirstOrDefault(x => x.Id.Equals(id));

        }

        [HttpGet]
        [Route("api/item/FindKeyById")]
        public Item FindKeyById([FromQuery] string id)
        {

            return DB.GetItems().FirstOrDefault(x => x.KeyId.Equals(new Guid(id)) && x.ItemType == ItemTypes.Key);

        }

        [HttpGet]
        [Route("api/item/FindKeys")]
        public List<Item> FindKeys([FromQuery] string query)
        {

            var items = DB.GetItems().Where(x => x.Name != null && x.ItemType == ItemTypes.Key);

            if (string.IsNullOrEmpty(query))
            {
                return items.ToList();
            }

            return items.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        }

        [HttpGet]
        [Route("api/item/ContainerSize")]
        public JsonResult ContainerSize()
        {
            var containerSize = new List<object>();

            foreach (var size in Enum.GetValues(typeof(ContainerSize)))
            {

                containerSize.Add(new
                {
                    id = (int)size,
                    name = size.ToString()
                });
            }
            return Json(containerSize);
        }

        [HttpGet]
        [Route("api/item/LockStrength")]
        public JsonResult LockStrength()
        {
            var lockStrength = new List<object>();

            foreach (var lockDifficulty in Enum.GetValues(typeof(LockStrength)))
            {

                lockStrength.Add(new
                {
                    id = (int)lockDifficulty,
                    name = lockDifficulty.ToString()
                });
            }
            return Json(lockStrength);
        }

        [HttpGet]
        [Route("api/item/ReturnItemTypes")]
        public JsonResult ReturnItemTypes()
        {
           
            var itemTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(Item.ItemTypes)))
            {

                itemTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(itemTypes);
            
        }

        [HttpGet]
        [Route("api/item/ReturnAttackTypes")]
        public JsonResult ReturnAttackTypes()
        {

            var itemTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(Item.AttackTypes)))
            {

                itemTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(itemTypes);

        }

        [HttpGet]
        [Route("api/item/ReturnSlotTypes")]
        public JsonResult ReturnSlotTypes()
        {

            var itemTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(EqSlot)))
            {

                itemTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(itemTypes);

        }

        [HttpGet]
        [Route("api/item/ReturnFlagTypes")]
        public JsonResult ReturnFlagTypes()
        {

            var itemTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(Item.ItemFlags)))
            {

                itemTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(itemTypes);

        }

        [HttpGet]
        [Route("api/item/ReturnDamageTypes")]
        public JsonResult ReturnDamageTypes()
        {

            var itemTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(Item.DamageTypes)))
            {

                itemTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(itemTypes);

        }


        [HttpGet]
        [Route("api/item/ReturnWeaponTypes")]
        public JsonResult ReturnWeaponTypes()
        {

            var itemTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(Item.WeaponTypes)))
            {

                itemTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(itemTypes);

        }


        [HttpGet]
        [Route("api/item/ReturnArmourTypes")]
        public JsonResult ReturnArmourTypes()
        {

            var itemTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(Item.ArmourTypes)))
            {

                itemTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(itemTypes);

        }

      
    }
}

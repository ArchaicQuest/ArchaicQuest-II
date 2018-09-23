using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.Room;
using ArchaicQuestII.Core.Item;
using ArchaicQuestII.Core.Events;
using Microsoft.Azure.KeyVault.Models;

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
                ArmorRating = item.ArmorRating,
                ArmourType = item.ArmourType,
                AttackType = item.AttackType,
                Condition = item.Condition,
                ContainerItems = item.ContainerItems,
                DamageType = item.DamageType,
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

            Save.SaveItem(newItem);

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

            foreach (var item in Enum.GetValues(typeof(Item.EqSlot)))
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

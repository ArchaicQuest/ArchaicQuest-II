using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Item;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
public class ItemData
{
    public Item Item { get; set; }
    public bool UpdateAllInstances { get; set; }
}

namespace ArchaicQuestII.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private IDataBase _db { get; }
        public ItemController(IDataBase db)
        {
            _db = db;
        }
        [HttpPost]
        [Route("api/item/PostItem")]
        public IActionResult PostItem([FromBody] ItemData itemData)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid object");
                throw exception;
            }
            
            
            var newItem = new Item()
            {
                Name = itemData.Item.Name,
                Level = itemData.Item.Level,
                ArmourRating = new ArmourRating()
                {
                    Armour = itemData.Item.ArmourRating.Armour,
                    Magic = itemData.Item.ArmourRating.Magic
                },
                ArmourType = itemData.Item.ArmourType,
                AttackType = itemData.Item.AttackType,
                Condition = itemData.Item.Condition,
                Container = itemData.Item.Container,
                Book = new Book()
                {
                    Blank = itemData.Item.Book.Blank,
                    PageCount = itemData.Item.Book.PageCount,
                    Pages = itemData.Item.Book.Pages
                },
                DamageType = itemData.Item.DamageType,
                Damage = new Damage()
                {
                    Maximum = itemData.Item.Damage.Maximum,
                    Minimum = itemData.Item.Damage.Minimum,
                },
                Modifier = new Modifier()
                {
                    DamRoll = itemData.Item.Modifier.DamRoll,
                    HitRoll = itemData.Item.Modifier.HitRoll,
                    HP = itemData.Item.Modifier.HP,
                    Mana = itemData.Item.Modifier.Mana,
                    Moves = itemData.Item.Modifier.Moves,
                    SpellDam = itemData.Item.Modifier.SpellDam,
                    Saves = itemData.Item.Modifier.Saves,
                    Strength = itemData.Item.Modifier.Strength,
                    Dexterity = itemData.Item.Modifier.Dexterity,
                    Constitution = itemData.Item.Modifier.Constitution,
                    Wisdom = itemData.Item.Modifier.Wisdom,
                    Intelligence = itemData.Item.Modifier.Intelligence,
                    Charisma = itemData.Item.Modifier.Charisma,
                    AcMod = itemData.Item.Modifier.AcMod,
                    AcMagicMod = itemData.Item.Modifier.AcMagicMod
                },
                DecayTimer = itemData.Item.DecayTimer,
                Description = itemData.Item.Description,
                Slot = itemData.Item.Slot,
                ForageRank = itemData.Item.ForageRank,
                Hidden = itemData.Item.Hidden,
                IsHiddenInRoom = itemData.Item.IsHiddenInRoom,
                ItemFlag = itemData.Item.ItemFlag,
                Keywords = itemData.Item.Keywords,
                KnownByName = itemData.Item.KnownByName,
                QuestItem = itemData.Item.QuestItem,
                Stuck = itemData.Item.Stuck,
                ItemType = itemData.Item.ItemType,
                Uses = itemData.Item.Uses,
                WeaponSpeed = itemData.Item.WeaponSpeed,
                WeaponType = itemData.Item.WeaponType,
                Weight = itemData.Item.Weight,
                Value = itemData.Item.Value,
                TwoHanded = itemData.Item.TwoHanded,
                SpellName = itemData.Item.SpellName,
                SpellLevel = itemData.Item.SpellLevel,
                Portal = new Portal()
                {
                    Destination = itemData.Item.Portal.Destination,
                    EnterDescription = itemData.Item.Portal.EnterDescription,
                    EnterDescriptionRoom = itemData.Item.Portal.EnterDescriptionRoom,
                    ExitDescription = itemData.Item.Portal.ExitDescription,
                    ExitDescriptionRoom = itemData.Item.Portal.ExitDescriptionRoom,
                    Name = itemData.Item.Portal.Name,
                }

            };

            if (itemData.Item.ItemType == Item.ItemTypes.Key)
            {
                newItem.KeyId = Guid.NewGuid();
            }


            if (!string.IsNullOrEmpty(itemData.Item.Id.ToString()) && itemData.Item.Id != -1)
            {

                var foundItem = _db.GetById<Item>(itemData.Item.Id, DataBase.Collections.Items);

                if (foundItem == null)
                {
                    throw new Exception("Item Id does not exist");
                }

                newItem.Id = itemData.Item.Id;
                // If you update an item, Update all the objects where it exists in a room
                // save yourself alot of work huh
                if (itemData.UpdateAllInstances)
                {
                    var rooms = _db.GetList<Room>(DataBase.Collections.Room);

                    foreach (var room in rooms)
                    {
                        foreach (var roomItem in room.Items.ToList())
                        {
                            if (roomItem.Id.Equals(newItem.Id))
                            {
                                newItem.Id = roomItem.Id;
                                room.Items.Remove(roomItem);
                                room.Items.Add(newItem);
                            }

                            if (roomItem.Container.Items.Any())
                            {
                                foreach (var containerItem in roomItem.Container.Items.ToList())
                                {
                                    if (containerItem.Id.Equals(newItem.Id))
                                    {
                                        newItem.Id = containerItem.Id;
                                        roomItem.Container.Items.Remove(containerItem);
                                        roomItem.Container.Items.Add(newItem);

                                    }
                                }
                            }
                        }

                        _db.Save(room, DataBase.Collections.Room);
                    }
                }
            }



            _db.Save(newItem, DataBase.Collections.Items);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail = $"({newItem.Id}) {newItem.Name}",
                Type = DataBase.Collections.Items,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);
            return Ok(JsonConvert.SerializeObject(new { toast = $"Item saved successfully." }));
        }


        [HttpGet]
        [Route("api/item/Get")]
        public List<Item> GetItem()
        {

            return _db.GetList<Item>(DataBase.Collections.Items).Where(x => x.Deleted == false).ToList();

        }


        [HttpGet]
        [Route("api/item/FindItems")]
        public List<Item> FindItems([FromQuery] string query)
        {

            var items = _db.GetCollection<Item>(DataBase.Collections.Items).FindAll().Where(x => x.Name != null && x.Deleted == false);

            if (string.IsNullOrEmpty(query))
            {
                return items.ToList();
            }

            return items.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        }

        [HttpGet]
        [Route("api/item/FindItemsByType")]
        public List<Item> FindItemsByType([FromQuery] Equipment.EqSlot query)
        {

            var items = _db.GetCollection<Item>(DataBase.Collections.Items).FindAll().Where(x => x.Slot.Equals(query));

            return items.ToList();

        }

        [HttpGet]
        [Route("api/item/FindItemById")]
        public Item FindItemById([FromQuery] int id)
        {

            return _db.GetCollection<Item>(DataBase.Collections.Items).FindById(id);

        }

        [HttpGet]
        [Route("api/item/FindKeyById")]
        public Item FindKeyById([FromQuery] string id)
        {

            return _db.GetCollection<Item>(DataBase.Collections.Items).FindOne(x => x.KeyId.Equals(new Guid(id)) && x.ItemType == Item.ItemTypes.Key);

        }

        [HttpGet]
        [Route("api/item/FindKeys")]
        public List<Item> FindKeys([FromQuery] string query)
        {

            var items = _db.GetCollection<Item>(DataBase.Collections.Items).FindAll().Where(x => x.Name != null && x.ItemType == Item.ItemTypes.Key);

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

            foreach (var size in Enum.GetValues(typeof(Container.ContainerSize)))
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

            foreach (var lockDifficulty in Enum.GetValues(typeof(Item.LockStrength)))
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

            foreach (var item in Enum.GetValues(typeof(Equipment.EqSlot)))
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

        [HttpDelete]
        [Route("api/item/delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var item = _db.GetCollection<Item>(DataBase.Collections.Items).FindById(id);
            item.Deleted = true;
            var saved = _db.Save(item, DataBase.Collections.Items);

            if (saved)
            {
                return Ok(JsonConvert.SerializeObject(new { toast = $"{item.Name} deleted successfully." }));
            }
            return Ok(JsonConvert.SerializeObject(new { toast = $"{item.Name} deletion failed." }));



        }


    }
}

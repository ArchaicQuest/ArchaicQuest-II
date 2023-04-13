﻿using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Rooms
    {
        internal static void Cache()
        {
            var rooms = Services.Instance.DataBase.GetList<Room>(DataBase.Collections.Room);
            var areas = Services.Instance.DataBase.GetList<Area>(DataBase.Collections.Area);
            var updatedItems = new ItemList();
            var lastRandom = 0;

            foreach (var room in rooms.Where(x => x.Deleted == false))
            {
                foreach (
                    var item in room.Items.Where(x => x.ItemType == Item.Item.ItemTypes.Forage)
                )
                {
                    updatedItems.Clear();
                    foreach (
                        var containerForageItem in item.Container.Items.Where(
                            x =>
                                x.ItemType == Item.Item.ItemTypes.Crafting
                                || x.ItemType == Item.Item.ItemTypes.Forage
                        )
                    )
                    {
                        var rnd = DiceBag.Roll(1, 1, 11);
                        if (lastRandom == rnd)
                        {
                            rnd = DiceBag.Roll(1, 1, 11);
                        }

                        lastRandom = rnd;
                        for (var i = 0; i < rnd; i++)
                        {
                            updatedItems.Add(containerForageItem);
                        }
                    }

                    item.Container.Items.AddRange(updatedItems);
                }

                AddSkillsToMobs(room);
                MapMobRoomId(room);
                Services.Instance.Cache.AddRoom(
                    $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}",
                    room
                );
                Services.Instance.Cache.AddOriginalRoom(
                    $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}",
                    JsonConvert.DeserializeObject<Room>(JsonConvert.SerializeObject(room))
                );
            }

            foreach (var area in areas)
            {
                var roomList = rooms.FindAll(x => x.AreaId == area.Id && x.Deleted == false);
                var areaByZIndex = roomList.FindAll(x => x.Coords.Z != 0).Distinct();
                foreach (var zarea in areaByZIndex)
                {
                    var roomsByZ = new List<Room>();
                    foreach (
                        var room in roomList.FindAll(
                            x => x.Coords.Z == zarea.Coords.Z && x.Deleted == false
                        )
                    )
                    {
                        roomsByZ.Add(room);
                    }

                    Services.Instance.Cache.AddMap(
                        $"{area.Id}{zarea.Coords.Z}",
                        Map.DrawMap(roomsByZ)
                    );
                }

                var rooms0index = roomList.FindAll(x => x.Coords.Z == 0 && x.Deleted == false);
                Services.Instance.Cache.AddMap($"{area.Id}0", Map.DrawMap(rooms0index));
            }
        }

        /// <summary>
        /// MOB SKILLS
        /// ----------
        /// I don't want to manually update mobs in each room
        /// if there is a change so this will run at startup to make changes
        /// to mobs. not a big deal if this takes while as it's a one time cost at startup
        ///
        /// This will mean we need special classes for mobs, can't have a shop keeper using skills
        /// that they should not know. Ok for an old mage in a magic shop casting spells at a player
        /// but can't have a mob selling socks drop kicking the player and going HAM with 2nd, 3rd attack
        /// and finishing off with a bash and a cleave to the skull. A tad unrealistic.
        ///
        /// Basic Mob class should be added with the essentials
        /// dodge, blunt, staves, and short blade skills
        /// probably others to add. maybe parry
        /// </summary>
        /// <param name="room"></param>
        private static void AddSkillsToMobs(Room room)
        {
            try
            {
                foreach (var mob in room.Mobs)
                {
                    mob.Skills = new List<SkillList>();

                    mob.AddSkills(Enum.Parse<ClassName>(mob.ClassName));

                    foreach (var skill in mob.Skills)
                    {
                        skill.Proficiency = 100;
                    }

                    //set mob armor
                    mob.ArmorRating = new ArmourRating()
                    {
                        Armour = mob.Level > 5 ? mob.Level * 3 : 1,
                        Magic = mob.Level * 3 / 4,
                    };

                    if (mob.Equipped.Light != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Light.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Light.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Finger != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Finger.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Finger.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Finger2 != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Finger2.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Finger2.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Neck != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Neck.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Neck.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Neck2 != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Neck2.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Neck2.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Face != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Face.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Face.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Head != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Head.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Head.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Torso != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Torso.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Torso.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Legs != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Legs.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Legs.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Feet != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Feet.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Feet.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Hands != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Hands.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Hands.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Arms != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Arms.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Arms.ArmourRating.Magic;
                    }

                    if (mob.Equipped.AboutBody != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.AboutBody.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.AboutBody.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Waist != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Waist.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Waist.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Wrist != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Wrist.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Wrist.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Wrist2 != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Wrist2.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Wrist2.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Wielded != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Wielded.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Wielded.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Secondary != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Secondary.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Secondary.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Shield != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Shield.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Shield.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Held != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Held.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Held.ArmourRating.Magic;
                    }

                    if (mob.Equipped.Floating != null)
                    {
                        mob.ArmorRating.Armour += mob.Equipped.Floating.ArmourRating.Armour;
                        mob.ArmorRating.Magic += mob.Equipped.Floating.ArmourRating.Magic;
                    }

                    //give mob unique IDs
                    mob.UniqueId = Guid.NewGuid();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void MapMobRoomId(Room room)
        {
            foreach (var mob in room.Mobs)
            {
                mob.UniqueId = Guid.NewGuid();
                mob.RoomId = $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
            }
        }
    }
}

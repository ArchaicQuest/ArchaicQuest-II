using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Rooms
    {
        internal static void Cache(IDataBase db, ICache cache)
        {
            var rooms = db.GetList<Room>(DataBase.Collections.Room);
            var areas = db.GetList<Area>(DataBase.Collections.Area);

            foreach (var room in rooms)
            {
                AddSkillsToMobs(db, room);
                MapMobRoomId(room);
                cache.AddRoom($"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}", room);
                cache.AddOriginalRoom($"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}", JsonConvert.DeserializeObject<Room>(JsonConvert.SerializeObject(room)));
            }

            foreach (var area in areas)
            {
                var roomList = rooms.FindAll(x => x.AreaId == area.Id);
                var areaByZIndex = roomList.FindAll(x => x.Coords.Z != 0).Distinct();
                foreach (var zarea in areaByZIndex)
                {
                    var roomsByZ = new List<Room>();
                    foreach (var room in roomList.FindAll(x => x.Coords.Z == zarea.Coords.Z))
                    {
                        roomsByZ.Add(room);
                    }

                    cache.AddMap($"{area.Id}{zarea.Coords.Z}", Map.DrawMap(roomsByZ));
                }

                var rooms0index = roomList.FindAll(x => x.Coords.Z == 0);
                cache.AddMap($"{area.Id}0", Map.DrawMap(rooms0index));
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
        private static void AddSkillsToMobs(IDataBase db, Room room)
        {
            foreach (var mob in room.Mobs)
            {

                mob.Skills = new List<SkillList>();

                var classSkill = db.GetCollection<Class>(DataBase.Collections.Class).FindOne(x =>
                    x.Name.Equals(mob.ClassName, StringComparison.CurrentCultureIgnoreCase));

                foreach (var skill in classSkill.Skills)
                {
                    // skill doesn't exist and should be added
                    if (mob.Skills.FirstOrDefault(x =>
                        x.SkillName.Equals(skill.SkillName, StringComparison.CurrentCultureIgnoreCase)) == null)
                    {
                        mob.Skills.Add(
                            new SkillList()
                            {
                                Proficiency = 100,
                                Level = skill.Level,
                                SkillName = skill.SkillName,
                                SkillId = skill.SkillId
                            }
                        );
                    }

                    mob.Skills.FirstOrDefault(x => x.SkillName.Equals(skill.SkillName, StringComparison.CurrentCultureIgnoreCase)).SkillId = skill.SkillId;
                }

                //set mob armor
                mob.ArmorRating = new ArmourRating()
                {
                    Armour = mob.Level > 5 ? mob.Level * 3 : 1,
                    Magic = mob.Level * 3 / 4,
                };

                //give mob unique IDs
                mob.UniqueId = Guid.NewGuid();
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

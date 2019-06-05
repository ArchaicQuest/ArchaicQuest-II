using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.Core.Events;
using ArchaicQuestII.Engine.World.Room;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers.API
{
   
    public class RoomController : Controller
    {
        [HttpPost]
        [Route("api/room/post")]
        public void Post(Room room)
        {
            var newRoom = new Room()
            {
                Title = room.Title,
                Description = room.Description,
               // Area = room.Area,
                Coords = new Coordinates()
                {
                    X = room.Coords.X,
                    Y = room.Coords.Y,
                    Z = room.Coords.Z
                },
                Exits = new RoomExits(),
                Emotes = new List<string>(),
                InstantRePop = room.InstantRePop,
                UpdateMessage = room.UpdateMessage,
                Mobs = new List<Player>(),
                RoomObjects = new List<RoomObject>(),
                //Terrain = room.Terrain,
                Type = room.Type,
              //  Modified = DateTime.Now
            };

           
            if (room.Emotes != null)
            {
                foreach (var emote in room.Emotes)
                {
                    newRoom.Emotes.Add(emote);
                }
            }

            if (room.Mobs != null)
            {
                foreach (var mob in room.Mobs)
                {
                    newRoom.Mobs.Add(mob);
                }
            }

            if (room.RoomObjects != null)
            {
                foreach (var keyword in room.RoomObjects)
                {
                    newRoom.RoomObjects.Add(keyword);
                }
            }

            DB.SaveRoom(newRoom);

        }
    }
}

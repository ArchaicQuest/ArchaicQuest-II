using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.Room;
using ArchaicQuestII.Core.Events;

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
                Area = room.Area,
                AreaId = room.AreaId,
                Region = room.Region,
                Coords = new Coordinates()
                {
                    X = room.Coords.X,
                    Y = room.Coords.Y,
                    Z = room.Coords.Z
                },
                Exits = new List<Exit>(),
                Emotes = new List<string>(),
                InstantRePop = room.InstantRePop,
                UpdateMessage = room.UpdateMessage,
                Mobs = new List<Core.Player.Player>(),
                Keywords = new List<RoomObject>(),
                Terrain = room.Terrain,
                Type = room.Type,
                Modified = DateTime.Now
            };

            if (room.Exits != null)
            {
                foreach (var exit in room.Exits)
                {
                    newRoom.Exits.Add(exit);
                }
            }

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

            if (room.Keywords != null)
            {
                foreach (var keyword in room.Keywords)
                {
                    newRoom.Keywords.Add(keyword);
                }
            }

            Save.SaveRoom(newRoom);

        }
    }
}

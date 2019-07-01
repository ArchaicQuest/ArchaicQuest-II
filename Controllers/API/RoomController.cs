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
        public void Post([FromBody] Room room)
        {
            var newRoom = new Room()
            {
                Title = room.Title,
                Description = room.Description,
             AreaId = room.AreaId,
                Coords = new Coordinates()
                {
                    X = room.Coords.X,
                    Y = room.Coords.Y,
                    Z = room.Coords.Z
                },
                Exits = room.Exits,
                Emotes =room.Emotes,
                InstantRePop = room.InstantRePop,
                UpdateMessage = room.UpdateMessage,
                Items = room.Items,
                Mobs = room.Mobs,
                RoomObjects = room.RoomObjects,
                //Terrain = room.Terrain,
                Type = room.Type,
              //  Modified = DateTime.Now
            };

           
           

            DB.SaveRoom(newRoom);

        }
    }
}

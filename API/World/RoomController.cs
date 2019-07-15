using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.Core.Events;
using ArchaicQuestII.Engine.World.Room.Commands;
using ArchaicQuestII.Engine.World.Room.Model;
using ArchaicQuestII.Engine.World.Room.Queries;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.API.World
{
   
    public class RoomController : Controller
    {
        [HttpPost]
        [Route("api/World/Room")]
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

           
           

           DB.Save(newRoom, "Room");

        }


        [HttpGet]
        [Route("api/World/Room/{id:int}")]
        public Room Get(int id)
        {

            return new GetRoomQuery().GetRoom(id);

        }

        [HttpPut]
        [Route("api/World/Room/{id:int}")]
        public void Put([FromBody] Room data)
        {

            new UpdateRoomCommand().UpdateRoom(data);

        }
    }
}

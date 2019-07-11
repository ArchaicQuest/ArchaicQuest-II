using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.Engine.Character.Model;

namespace ArchaicQuestII.Engine.World.Room.Model
{
    public class RoomExits
    {
       
        public Exit North { get; set; } = new Exit()
        {
            Name = "North"
        };
        public Exit NorthEast { get; set; } = new Exit()
        {
            Name = "North East"
        };
        public Exit East { get; set; } = new Exit()
        {
            Name = "East"
        };
        public Exit SouthEast { get; set; } = new Exit()
        {
            Name = "South East"
        };
        public Exit South { get; set; } = new Exit()
        {
            Name = "South"
        };
        public Exit SouthWest { get; set; } = new Exit()
        {
            Name = "South West"
        };
        public Exit West { get; set; } = new Exit()
        {
            Name = "West"
        };
        public Exit NorthWest { get; set; } = new Exit()
        {
            Name = "North West"
        };
        public Exit Up { get; set; } = new Exit()
        {
            Name = "Up"
        };
        public Exit Down { get; set; } = new Exit()
        {
            Name = "Down"
        };

    }
}

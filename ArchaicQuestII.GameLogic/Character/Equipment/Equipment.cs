using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.Equipment
{
    public class Equipment
    {

        public Item.Item Light { get; set; }
        public Item.Item Finger { get; set; }
        public Item.Item Finger2 { get; set; }
        public Item.Item Neck { get; set; }
        public Item.Item Neck2 { get; set; }
        public Item.Item Eyes { get; set; }
        public Item.Item Face { get; set; }
        public Item.Item Head { get; set; }
        public Item.Item Ear { get; set; }
        public Item.Item Torso { get; set; }
        public Item.Item Legs { get; set; }
        public Item.Item Feet { get; set; }
        public Item.Item Hands { get; set; }
        public Item.Item Arms { get; set; }
        public Item.Item AboutBody { get; set; }
        public Item.Item OnBack { get; set; }
        public Item.Item Waist { get; set; }
        public Item.Item Wrist { get; set; }
        public Item.Item Wrist2 { get; set; }
        public Item.Item Wielded { get; set; }
        public Item.Item Secondary { get; set; }
        public Item.Item Shield { get; set; }
        public Item.Item Held { get; set; }
        public Item.Item Floating { get; set; }
        public Item.Item Quiver { get; set; }

        public enum EqSlot
        {
            [Description("Arms")]
            Arms = 0,
            [Description("Body")]
            Body = 1,
            [Description("Face")]
            Face = 2,
            [Description("Feet")]
            Feet = 3,
            [Description("Finger")]
            Finger = 4,
            [Description("Floating")]
            Floating = 5,
            [Description("Hands")]
            Hands = 6,
            [Description("Head")]
            Head = 7,
            [Description("Held")]
            Held = 8,
            [Description("Legs")]
            Legs = 9,
            [Description("Light")]
            Light = 10,
            [Description("Neck")]
            Neck = 11,
            [Description("Shield")]
            Shield = 12,
            [Description("Torso")]
            Torso = 13,
            [Description("Waist")]
            Waist = 14,
            [Description("Wielded")]
            Wielded = 15,
            [Description("Wrist")]
            Wrist = 16,
            [Description("Secondary")]
            Secondary = 17,

        }


    }
}

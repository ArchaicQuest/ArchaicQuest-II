using System;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Item;
using LiteDB;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Config;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;
using Money = ArchaicQuestII.GameLogic.Item.Money;

namespace ArchaicQuestII.GameLogic.Character
{
    public class MobEvents
    {
        //        public string Act { get; set; } = @"
        //		                    -- defines a function
        //function act(room, player, mob, text)
        //    if string.match(text, ' pokes you in the') then


        //        if obj.HasEventState(player, 'larisaPoke')
        //            then
        //          state =  obj.ReadEventState(player, 'larisaPoke')
        //          state = state + 1
        // obj.UpdateEventState(player, 'larisaPoke', state)

        //           else
        //            obj.AddEventState(player, 'larisaPoke', 1)
        //            end

        //            if state > 3
        //            then
        // obj.Say(obj.getName(mob) .. ' says GRRRRR!! I WARNED YOU!!', 0, room, player)
        //            obj.AttackPlayer(room, player, mob)
        //        end


        //    end

        //end

        //  act(room, player, mob, text)";
        public string Act { get; set; }
        public string Look { get; set; }
        public string Enter { get; set; }
        public string Leave { get; set; }
        public string Emote { get; set; }
        public string Give { get; set; }
        public string UponDeath { get; set; }
        public string CombatMessages { get; set; }
    }

    public class Mount
    {
        public string Name { get; set; } = String.Empty;
        public string MountedBy { get; set; } = String.Empty;
        public bool IsMount { get; set; }
    }

    /// <summary>
    /// Spells for mob to cast upon user
    /// </summary>
    public class MobSpellList
    {
        public string Name { get; set; }
        public int Cost { get; set; }
    }

    public class Character
    {
        /// <summary>
        /// Assigned when player logs in.
        /// used to find player in cached dictionary and to send data directly to player
        /// </summary>
        [BsonIgnore]
        [JsonProperty("connectionId")]
        public string ConnectionId { get; set; } = "mob";

        /// <summary>
        /// To be assigned to mobs on start up,
        /// used to repop correct mobs
        /// </summary>
        [BsonIgnore]
        public Guid UniqueId { get; set; } = Guid.Empty;

        /// <summary>
        /// Associated Account Id
        /// </summary>
        public Guid AccountId { get; set; } = Guid.Empty;

        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Displays short description of the character in the room
        /// For Players this would work as a 'Pose' and for mobs
        /// will be something like 'A bat flaps around the cavern above.'
        /// </summary>
        [JsonProperty("longName")]
        public string LongName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("subClassName")]
        public string SubClassName { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("alignmentScore")]
        public int AlignmentScore { get; set; } = 0;

        [JsonProperty("totalExperience")]
        public int TotalExperience { get; set; }

        [JsonProperty("experience")]
        public int Experience { get; set; }

        [JsonProperty("experienceToNextLevel")]
        public int ExperienceToNextLevel { get; set; } = 1000;

        [JsonProperty("equipped")]
        public Equipment Equipped { get; set; } = new Equipment();

        [JsonProperty("inventory")]
        public ItemList Inventory { get; set; } = new ItemList();

        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        [JsonProperty("status")]
        public CharacterStatus.Status Status { get; set; }

        [JsonProperty("maxStats")]
        public Stats MaxStats { get; set; }

        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }

        [JsonProperty("maxAttributes")]
        public Attributes MaxAttributes { get; set; }

        [BsonIgnore]
        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("armorRating")]
        public ArmourRating ArmorRating { get; set; } = new ArmourRating();

        [JsonProperty("money")]
        public Money Money { get; set; }

        [JsonProperty("affects")]
        public Affects Affects { get; set; } = new Affects();

        [JsonProperty("config")]
        public PlayerConfig Config { get; set; } = new PlayerConfig();

        [JsonProperty("roomId")]
        /// arearID + X + Y + z e,g "1000"
        public string RoomId { get; set; }

        [JsonProperty("roomType")]
        /// arearID + X + Y + z e,g "1000"
        public Room.RoomType? RoomType { get; set; } = Room.RoomType.Standard;

        [JsonProperty("recallId")]
        public string RecallId { get; set; }

        [JsonProperty("defaultAttack")]
        public string DefaultAttack { get; set; }

        [JsonIgnore]
        public Queue<string> Buffer { get; set; } = new Queue<string>();

        public List<Spell.Model.Spell> Spells { get; set; } = new List<Spell.Model.Spell>();
        public List<SkillList> Skills { get; set; } = new List<SkillList>();
        public bool Deleted { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; } = DateTime.Now;
        public List<string> Emotes { get; set; } = new List<string>();

        /// <summary>
        /// for Mob path, e.g n,e,s,w
        /// </summary>
        public string Commands { get; set; }

        public string EnterEmote { get; set; } = String.Empty;
        public string LeaveEmote { get; set; } = String.Empty;

        /// <summary>
        /// moves around randomly
        /// </summary>
        public bool Roam { get; set; }
        public bool Shopkeeper { get; set; }
        public bool Trainer { get; set; }

        /// <summary>
        /// Use for when you want scripting but not for the mob to be visible
        /// </summary>
        public bool IsHiddenScriptMob { get; set; }
        public MobEvents Events { get; set; } = new MobEvents();
        public Dictionary<string, int> EventState { get; set; } = new Dictionary<string, int>();
        public List<Quest> QuestLog { get; set; } = new List<Quest>();

        [JsonProperty("weight")]
        public double Weight { get; set; } = 0;

        // Full at 4
        public int Hunger { get; set; } = 0;
        public int Lag { get; set; } = 0;
        public Mount Mounted { get; set; } = new Mount();
        public List<Player> Pets { get; set; } = new List<Player>(); //maybe just ID will suffice?
        public List<MobSpellList> SpellList { get; set; } = new List<MobSpellList>();
        public bool Aggro { get; set; } = false;

        [JsonProperty("flags")]
        public List<CharacterFlags> Flags { get; set; } = new();
    }
}

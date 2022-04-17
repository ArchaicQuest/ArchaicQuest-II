using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Commands;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Core
{

    public class GameLoop : IGameLoop
    {


        private IWriteToClient _writeToClient;
        private ICache _cache;
        private ICommands _commands;
        private ICombat _combat;
        private IDataBase _db;
        private IDice _dice;
        private IUpdateClientUI _client;
        private ITime _time;
        private ICore _core;
        private ISpellList _spellList;
        private IWeather _weather;

        public GameLoop(IWriteToClient writeToClient, ICache cache, ICommands commands, ICombat combat, IDataBase database, IDice dice, IUpdateClientUI client, ITime time, ICore core, ISpellList spelllist, IWeather weather)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _commands = commands;
            _combat = combat;
            _db = database;
            _dice = dice;
            _client = client;
            _time = time;
            _core = core;
            _spellList = spelllist;
            _weather = weather;

        }

        public int GainAmount(int value, Player player)
        {
            return player.Status switch
            {
                CharacterStatus.Status.Sleeping => value *= 3,
                CharacterStatus.Status.Resting => value *= 2,
                _ => value
            };
        }

        public void UpdateCorpse(Item.Item corpse, Room room)
        {
            
            switch (corpse.Decay)
            {
                case 10:
                case 9:
                case 8:
                case 7:
                case 6:
                    corpse.Description.Room = $"The corpse of {corpse.Name.ToLower()} lies here.";
                    break;
                case 5:
                case 4:
                    corpse.Description.Room = $"The corpse of {corpse.Name.ToLower()} is buzzing with flies.";
                    break;
                case 3:
                    corpse.Description.Room = $"The corpse of {corpse.Name.ToLower()} fills the air with a foul stench.";
                    break;
                case 2:
                    corpse.Description.Room = $"The corpse of {corpse.Name.ToLower()} is crawling with vermin.";
                    break;
                case 1:
                    corpse.Description.Room = $"The corpse of {corpse.Name.ToLower()} is in the last stages of decay.";
                    break;
                case 0:
                  
                    foreach (var pc in room.Players)
                    {
                        _writeToClient.WriteLine($"<p>A quivering horde of maggots consumes {corpse.Name.ToLower()}.</p>", pc.ConnectionId);
                    }
                    room.Items.Remove(corpse);
                    break;
                default:
                    corpse.Description.Room = $"The corpse of {corpse.Name.ToLower()} lies here.";
                    break;
            }
            
            corpse.Decay--;
      
        }

        public async Task UpdateTime()
        {   


            Console.WriteLine("started loop");
            while (true)
            {
                //2 mins
                await Task.Delay(30000);
                var rooms = _cache.GetAllRoomsToRepop();
                var players = _cache.GetPlayerCache().Values.ToList();

             
              
                foreach (var room in rooms)
                {
                    var originalRoom = JsonConvert.DeserializeObject<Room>(JsonConvert.SerializeObject(_cache.GetOriginalRoom(Helpers.ReturnRoomId(room))));

                    foreach (var mob in originalRoom.Mobs)
                    {
                
                        var mobExist = rooms.Find(x => x.Mobs.Any(y => y.UniqueId.Equals(mob.UniqueId)))
                            ?.Mobs.FirstOrDefault(z => z.UniqueId.Equals(mob.UniqueId));
                       
                        if (mobExist == null)
                        {
                            room.Mobs.Add(mob);
                        }
                        else
                        {
                            if (mob.Status != CharacterStatus.Status.Fighting)
                            {

                                mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                                    _dice.Roll(1, 2, 5) + mobExist.Level;
                                mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                                    _dice.Roll(1, 2, 5) + mobExist.Level;
                                mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                                    _dice.Roll(1, 2, 5) + mobExist.Level;
                            }
                            else
                            {
                                mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                                    (_dice.Roll(1, 1, 5) + mobExist.Level) / 2;
                                mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                                    (_dice.Roll(1, 1, 5) + mobExist.Level) / 2;
                                mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                                    (_dice.Roll(1, 1, 5) + mobExist.Level) / 2;
                            }

                            if (mobExist.Attributes.Attribute[EffectLocation.Hitpoints] >
                                mobExist.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                            {
                                mobExist.Attributes.Attribute[EffectLocation.Hitpoints] =
                                    mobExist.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                            }

                            if (mobExist.Attributes.Attribute[EffectLocation.Mana] >
                                mobExist.MaxAttributes.Attribute[EffectLocation.Mana])
                            {
                                mobExist.Attributes.Attribute[EffectLocation.Mana] =
                                    mobExist.MaxAttributes.Attribute[EffectLocation.Mana];
                            }

                            if (mobExist.Attributes.Attribute[EffectLocation.Moves] >
                                mobExist.MaxAttributes.Attribute[EffectLocation.Moves])
                            {
                                mobExist.Attributes.Attribute[EffectLocation.Moves] =
                                    mobExist.MaxAttributes.Attribute[EffectLocation.Moves];
                            }
                        }
                    }
                    
                    //get corpse and remove
                    var corpses = room.Items.FindAll(x => x.Description.Room.Contains("corpse", StringComparison.CurrentCultureIgnoreCase));

                    foreach (var corpse in corpses)
                    {
                        UpdateCorpse(corpse, room);
                    }



                    foreach (var item in originalRoom.Items)
                    {
                        // need to check if item exists before adding
                        var itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));

                        if (itemExist == null)
                        {
                            room.Items.Add(item);
                        }
                          itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));

                        if (itemExist.Container.Items.Count < item.Container.Items.Count)
                        {
                            itemExist.Container.Items = item.Container.Items;
                            itemExist.Container.IsOpen = item.Container.IsOpen;
                            itemExist.Container.IsLocked = item.Container.IsLocked;
                        }

                    }

                    // reset doors
                    room.Exits = originalRoom.Exits;

                    //set room clean
                    if (room.Players.Count == 0)
                    {
                        room.Clean = true;

                        foreach (var player in room.Players)
                        {
                            _writeToClient.WriteLine("<p>The hairs on your neck stand up.</p>", player.ConnectionId);
                        }
                    }
                    

                }

                foreach (var player in players)
                {
          
                  //  IdleCheck(player);

                    var hP = (_dice.Roll(1, 2, 5));
                    var mana = (_dice.Roll(1, 2, 5));
                    var moves = (_dice.Roll(1, 2, 5));

                    // if player has fast healing add the bonus here
                  var hasFastHealing =  player.Skills.FirstOrDefault(x =>
                        x.SkillName.Equals("Fast Healing", StringComparison.CurrentCultureIgnoreCase) && player.Level >= x.Level);

         

                  if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
                  {
                      hP *= 2;
                      mana *= 2;
                      moves *= 2;
                    }

                  if ((player.Status & CharacterStatus.Status.Resting) != 0)
                  {
                      hP *= (int)1.5;
                      mana *= (int)1.5;
                      moves *= (int)1.5;
                    }

                    if (player.Attributes.Attribute[EffectLocation.Hitpoints] <
                        player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                    {

                        if (hasFastHealing != null)
                        {
                            if (_core.SkillCheckSuccesful(hasFastHealing))
                            {
                                hP *= 2;
                            }
                            else
                            {
                                _core.GainSkillProficiency(hasFastHealing, player);
                            }
                        }

                        player.Attributes.Attribute[EffectLocation.Hitpoints] += GainAmount(hP, player);
                        if (player.Attributes.Attribute[EffectLocation.Hitpoints] > player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                        {
                            player.Attributes.Attribute[EffectLocation.Hitpoints] = player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                        }
                    }

                    if (player.Attributes.Attribute[EffectLocation.Mana] <
                        player.MaxAttributes.Attribute[EffectLocation.Mana])
                    {
                        player.Attributes.Attribute[EffectLocation.Mana] += GainAmount(mana, player);

                        if (player.Attributes.Attribute[EffectLocation.Mana] > player.MaxAttributes.Attribute[EffectLocation.Mana])
                        {
                            player.Attributes.Attribute[EffectLocation.Mana] = player.MaxAttributes.Attribute[EffectLocation.Mana];
                        }
                    }

                    if (player.Attributes.Attribute[EffectLocation.Moves] <
                        player.MaxAttributes.Attribute[EffectLocation.Moves])
                    {
                        player.Attributes.Attribute[EffectLocation.Moves] += GainAmount(moves, player);
                        if (player.Attributes.Attribute[EffectLocation.Moves] > player.MaxAttributes.Attribute[EffectLocation.Moves])
                        {
                            player.Attributes.Attribute[EffectLocation.Moves] = player.MaxAttributes.Attribute[EffectLocation.Moves];
                        }
                    }

                 


                 

                    _client.UpdateHP(player);
                    _client.UpdateMana(player);
                    _client.UpdateMoves(player);
                    _client.UpdateScore(player);

                }

            }
        }

        public async Task UpdateCombat()
        {
            // create a combat cache to add mobs too so they can fight back
            // block movement while fighting
            // end fight if target is not there / dead
            // create flee commant
            Console.WriteLine("started combat loop");
            while (true)
            {

                try
                {
                    await Task.Delay(4000);
                  

                    var players = _cache.GetCombatList();
                    var validPlayers = players.Where(x => x.Status == CharacterStatus.Status.Fighting);
                   
                    foreach (var player in validPlayers)
                    {
                        if (player.Lag > 0 &&
                            player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                        {
                            player.Lag -= 1;
                            continue;
                        }

                        var attackCount = 1;

                        var hasSecondAttack = player.Skills.FirstOrDefault(x =>
                            x.SkillName.Equals("Second Attack", StringComparison.CurrentCultureIgnoreCase));
                        if (hasSecondAttack != null)
                        {
                            hasSecondAttack = player.Level >= hasSecondAttack.Level ? hasSecondAttack : null;
                        }
                      
                        var hasThirdAttack = player.Skills.FirstOrDefault(x =>
                            x.SkillName.Equals("Third Attack", StringComparison.CurrentCultureIgnoreCase));
                        if (hasThirdAttack != null)
                        {
                            hasThirdAttack = player.Level >= hasThirdAttack.Level ? hasThirdAttack : null;
                        }

                        var hasFouthAttack = player.Skills.FirstOrDefault(x =>
                            x.SkillName.Equals("Fourth Attack", StringComparison.CurrentCultureIgnoreCase));
                        if (hasFouthAttack != null)
                        {
                            hasFouthAttack = player.Level >= hasFouthAttack.Level ? hasFouthAttack : null;
                        }
                        var hasFithAttack = player.Skills.FirstOrDefault(x =>
                            x.SkillName.Equals("Fith Attack", StringComparison.CurrentCultureIgnoreCase));

                        if (hasFithAttack != null)
                        {
                            hasFithAttack = player.Level >= hasFithAttack.Level ? hasFithAttack : null;
                        }

                        if (hasSecondAttack != null)
                        {
                            attackCount += 1;
                        }

                        if (hasThirdAttack != null)
                        {
                            attackCount += 1;
                        }

                        if (hasFouthAttack != null)
                        {
                            attackCount += 1;
                        }

                        if (hasFithAttack != null)
                        {
                            attackCount += 1;
                        }

                        if (player.Affects.Haste)
                        {
                            attackCount += 1;
                        }


                        for (var i = 0; i < attackCount; i++)
                        {
                            _combat.Fight(player, player.Target, _cache.GetRoom(player.RoomId), false);
                        }
                       
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task UpdateWorldTime()
        {
            Console.WriteLine("started world time loop");
            while (true)
            {
                try
                {
                    await Task.Delay(60000);
                    _time.DisplayTimeOfDayMessage(_time.UpdateTime());
                  

                    var weather = $"<span class='weather'>{_weather.SimulateWeatherTransitions()}</span>";

                    foreach (var player in _cache.GetPlayerCache().Values.ToList())
                    {
                        //check if player is not indoors
                        // TODO:
                        _client.UpdateTime(player);
                        var room = _cache.GetRoom(player.RoomId);

                        if (room.Terrain != Room.TerrainType.Inside && room.Terrain != Room.TerrainType.Underground)
                        {
                            _writeToClient.WriteLine(weather, player.ConnectionId);

                        }

                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public async Task Tick()
        {


            while (true)
            {
                try
                {
                    await Task.Delay(60000);
                    var players = _cache.GetPlayerCache().Values;

                    foreach (var pc in players)
                    {
                        pc.Hunger--;
                        foreach (var aff in pc.Affects.Custom.ToList())
                        {
                            aff.Duration--;

                            if (aff.Duration <= 0)
                            {
                                if (aff.Modifier.Strength != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.Strength] -= aff.Modifier.Strength;
                                }

                                if (aff.Modifier.Dexterity != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.Dexterity] -= aff.Modifier.Dexterity;
                                }

                                if (aff.Modifier.Constitution != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.Constitution] -= aff.Modifier.Constitution;
                                }

                                if (aff.Modifier.Intelligence != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.Intelligence] -= aff.Modifier.Intelligence;
                                }

                                if (aff.Modifier.Wisdom != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.Wisdom] -= aff.Modifier.Wisdom;
                                }

                                if (aff.Modifier.Charisma != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.Charisma] -= aff.Modifier.Charisma;
                                }

                                if (aff.Modifier.HitRoll != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.HitRoll] -= aff.Modifier.HitRoll;
                                }

                                if (aff.Modifier.DamRoll != 0)
                                {
                                    pc.Attributes.Attribute[EffectLocation.DamageRoll] -= aff.Modifier.DamRoll;
                                }
                                if (aff.Modifier.Armour != 0)
                                {
                                    pc.ArmorRating.Armour -= aff.Modifier.Armour;
                                    pc.ArmorRating.Magic -= aff.Modifier.Armour;
                                }


                                pc.Affects.Custom.Remove(aff);

                                _spellList.CastSpell(aff.Name, "", pc, "", pc, _cache.GetRoom(pc.RoomId), true);

                                if (aff.Affects == DefineSpell.SpellAffect.Blind)
                                {
                                    pc.Affects.Blind = false;
                                    _writeToClient.WriteLine("You are no longer blinded.", pc.ConnectionId);
                                }
                                if (aff.Affects == DefineSpell.SpellAffect.Berserk)
                                {
                                    pc.Affects.Berserk = false;
                                }
                                if (aff.Affects == DefineSpell.SpellAffect.NonDetect)
                                {
                                    pc.Affects.NonDectect = false;
                                }
                                if (aff.Affects == DefineSpell.SpellAffect.Invis)
                                {
                                    pc.Affects.Invis = false;
                                }
                                if (aff.Affects == DefineSpell.SpellAffect.DetectInvis)
                                {
                                    pc.Affects.DetectInvis = false;
                                }
                                if (aff.Affects == DefineSpell.SpellAffect.DetectHidden)
                                {
                                    pc.Affects.DetectHidden = false;
                                }
                                if (aff.Affects == DefineSpell.SpellAffect.Poison)
                                {
                                    pc.Affects.Poisoned = false;
                                }
                                if (aff.Affects == DefineSpell.SpellAffect.Haste
                                )
                                {
                                    pc.Affects.Haste = false;
                                }
                            }
                            _client.UpdateAffects(pc);
                        }
                        
                        this.IdleCheck(pc);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void IdleCheck(Player player)
        {
            var idleTime5Mins = player.LastCommandTime.AddMinutes(6) <= DateTime.Now;

            if (!player.Idle && idleTime5Mins)
            {
                _writeToClient.WriteLine("You enter the void.", player.ConnectionId);
                player.Idle = true;
                return;
            }

            var idleTime10Mins = player.LastCommandTime.AddMinutes(11) <= DateTime.Now;
            var idleTime15Mins = player.LastCommandTime.AddMinutes(16) <= DateTime.Now;
            if (idleTime10Mins && !idleTime15Mins)
            {
                _writeToClient.WriteLine("You go deeper into the void.", player.ConnectionId);
            }

            if (idleTime15Mins)
            {
                _core.Quit(player, _cache.GetRoom(player.RoomId));
            }
        }

        public async Task UpdateRoomEmote()
        {

            while (true)
            {
                try
                {
                    await Task.Delay(45000).ConfigureAwait(false);

                    var rooms = _cache.GetAllRooms().Where(x => x.Players.Any());

                    if (!rooms.Any())
                    {
                        continue;
                    }

                    foreach (var room in rooms)
                    {
                        if (!room.Emotes.Any() || _dice.Roll(1, 1, 10) < 7)
                        {
                            continue;
                        }

                        var emote = room.Emotes[_dice.Roll(1, 0, room.Emotes.Count - 1)];

                        foreach (var player in room.Players)
                        {
                            _writeToClient.WriteLine($"<p class='room-emote'>{emote}</p>",
                                player.ConnectionId);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }

        }

        public async Task UpdateMobEmote()
        {


            while (true)
            {
                try
                {
                    await Task.Delay(30000).ConfigureAwait(false);

                    var rooms = _cache.GetAllRooms();

                    if (!rooms.Any())
                    {
                        continue;
                    }
                    var mobIds = new List<Guid>();
                    foreach (var room in rooms.Where(x => x.Mobs.Any()))
                    {

                        foreach (var mob in room.Mobs.Where(x => x.Status != CharacterStatus.Status.Fighting).ToList())
                        {
                            if (mob.Emotes.Any() && mob.Emotes[0] != null && _dice.Roll(1,0,1) == 1)
                            {

                                var emote = mob.Emotes[_dice.Roll(1, 0, mob.Emotes.Count - 1)];
                                foreach (var player in room.Players)
                                {
                                    //example mob emote: Larissa flicks through her journal.
                                    if (emote == null)
                                    {
                                        continue;
                                    }
                                    _writeToClient.WriteLine($"<p class='mob-emote'>{mob.Name} {emote}</p>",
                                        player.ConnectionId);
                                }
                            }

                            if (mobIds.Contains(mob.Id))
                            {
                                continue;
                            }


                            if (mob.Roam && _dice.Roll(1, 1, 100) >= 50)
                            {
                                var exits = Helpers.GetListOfExits(room.Exits);
                                if (exits != null)
                                {
                                    var direction = exits[_dice.Roll(1, 0, exits.Count - 1)];

                                    mob.Buffer.Enqueue(direction);
                                }
                            }

                            if (!string.IsNullOrEmpty(mob.Commands) && mob.Buffer.Count == 0)
                            {
                                mob.RoomId = $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
                                var commands = mob.Commands.Split(";");

                                foreach (var command in commands)
                                {
                                    mob.Buffer.Enqueue(command);
                                }

                            }

                            if (mob.Buffer.Count > 0)
                            {
                                var mobCommand = mob.Buffer.Dequeue();

                                _commands.ProcessCommand(mobCommand, mob, room);
                            }

                            mobIds.Add(mob.Id);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }

        }

        public async Task UpdatePlayers()
        {
            while (true)
            {

                try
                {
                    await Task.Delay(125);
                    var players = _cache.GetPlayerCache();
                    var validPlayers = players.Where(x => x.Value.Buffer.Count > 0);

                    foreach (var player in validPlayers)
                    {
                        // don't action commands if player is lagged
                        if (player.Value.Lag > 0)
                        {
                            continue;
                        }

                        var command = player.Value.Buffer.Dequeue();
                        var room = _cache.GetRoom(player.Value.RoomId);
                        player.Value.LastCommandTime = DateTime.Now;
                        
                        if (player.Value.CommandLog.Count >= 1000)
                        {
                            player.Value.CommandLog = new List<string>();
                        }
                        
                        player.Value.CommandLog.Add($"{string.Format("{0:f}", DateTime.Now)} - {command}");
                        _commands.ProcessCommand(command, player.Value, room);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task UpdatePlayerLag()
        {
            while (true)
            {

                try
                {
                    await Task.Delay(4000);
                    var players = _cache.GetPlayerCache();
                    var validPlayers = players.Where(x => x.Value.Lag > 0);

                    foreach (var player in validPlayers)
                    {
                         
                        player.Value.Lag -= 1;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        public List<string> Hints()
        {
            var hints = new List<string>()
            {
               "If you get lost, enter recall to return to the starting room.",
               "If you need help use newbie to send a message. newbie help me",
               "ArchaicQuest is a new game so might be quite, join the discord to chat to others https://discord.gg/QVF6Uutt",
               "To communicate enter say then the message to speak. such as say hello there"
            };

            return hints;
        }


    }


}
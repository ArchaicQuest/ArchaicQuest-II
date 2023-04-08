using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Gain;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class MiscLoop : ILoop
    {
        public int TickDelay => 120000;
        public bool ConfigureAwait => true;
        private ICore _core;
        private List<Room> _rooms;
        private List<Player> _players;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
        }

        public void PreTick()
        {
            _rooms = _core.Cache.GetAllRoomsToRepop();
            _players = _core.Cache.GetPlayerCache().Values.ToList();
        }

        public void Tick()
        {
            foreach (var room in _rooms)
            {
                //max 187MB allocated type: string too much memory used here
                var originalRoom = JsonConvert.DeserializeObject<Room>(
                        JsonConvert.SerializeObject(_core.Cache.GetOriginalRoom(Helpers.ReturnRoomId(room))));


                foreach (var mob in originalRoom.Mobs)
                {

                    var mobExist = _rooms.Find(x => x.Mobs.Any(y => y.UniqueId.Equals(mob.UniqueId)))
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
                                DiceBag.Roll(1, 2, 5) + mobExist.Level;
                            mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                                DiceBag.Roll(1, 2, 5) + mobExist.Level;
                            mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                                DiceBag.Roll(1, 2, 5) + mobExist.Level;
                        }
                        else
                        {
                            mobExist.Attributes.Attribute[EffectLocation.Hitpoints] +=
                                (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                            mobExist.Attributes.Attribute[EffectLocation.Mana] +=
                                (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
                            mobExist.Attributes.Attribute[EffectLocation.Moves] +=
                                (DiceBag.Roll(1, 1, 5) + mobExist.Level) / 2;
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


                foreach (var item in originalRoom.Items)
                {
                    // need to check if item exists before adding
                    var itemExist = room.Items.FirstOrDefault(x => x.Id.Equals(item.Id));
                    var itemCount = room.Items.FindAll(x => x.Id.Equals(item.Id));
                    var originalItemCount = originalRoom.Items.FindAll(x => x.Id.Equals(item.Id));

                    if (itemCount.Count < originalItemCount.Count)
                    {
                        room.Items.Add(item);
                    }

                    if (itemExist != null && itemExist.ItemType == Item.Item.ItemTypes.Container || itemExist.ItemType == Item.Item.ItemTypes.Forage)
                    {
                        itemExist.Container.IsOpen = item.Container.IsOpen;
                        itemExist.Container.IsLocked = item.Container.IsLocked;
                        itemExist.Container.GoldPieces = item.Container.GoldPieces;
                        foreach (var items in item.Container.Items)
                        {
                            if (itemExist.Container.Items.Count < item.Container.Items.Count)
                            {
                                itemExist.Container.Items.Add(items);
                            }
                        }

                    }


                }

                // reset doors
                room.Exits = originalRoom.Exits;

                //set room clean
                // if (room.Players.Count == 0)
                //   {
                room.Clean = true;

                foreach (var player in room.Players)
                {
                    _core.Writer.WriteLine("<p>The hairs on your neck stand up.</p>",
                        player.ConnectionId);
                }
                //  }


            }

            foreach (var player in _players)
            {

                //  IdleCheck(player);

                var hP = (DiceBag.Roll(1, 2, 5));
                var mana = (DiceBag.Roll(1, 2, 5));
                var moves = (DiceBag.Roll(1, 2, 5));

                // if player has fast healing add the bonus here
                var hasFastHealing = player.Skills.FirstOrDefault(x =>
                    x.Name == SkillName.FastHealing &&
                    player.Level >= x.Level);


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
                        if (Helpers.SkillSuccessCheck(hasFastHealing))
                        {
                            hP *= 2;
                        }
                        else
                        {
                            player.FailedSkill(SkillName.FastHealing, out _);
                        }
                    }

                    player.Attributes.Attribute[EffectLocation.Hitpoints] += GainAmount(hP, player);
                    if (player.Attributes.Attribute[EffectLocation.Hitpoints] >
                        player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                    {
                        player.Attributes.Attribute[EffectLocation.Hitpoints] =
                            player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                    }
                }

                if (player.Attributes.Attribute[EffectLocation.Mana] <
                    player.MaxAttributes.Attribute[EffectLocation.Mana])
                {
                    player.Attributes.Attribute[EffectLocation.Mana] += GainAmount(mana, player);

                    if (player.Attributes.Attribute[EffectLocation.Mana] >
                        player.MaxAttributes.Attribute[EffectLocation.Mana])
                    {
                        player.Attributes.Attribute[EffectLocation.Mana] =
                            player.MaxAttributes.Attribute[EffectLocation.Mana];
                    }
                }

                if (player.Attributes.Attribute[EffectLocation.Moves] <
                    player.MaxAttributes.Attribute[EffectLocation.Moves])
                {
                    player.Attributes.Attribute[EffectLocation.Moves] += GainAmount(moves, player);
                    if (player.Attributes.Attribute[EffectLocation.Moves] >
                        player.MaxAttributes.Attribute[EffectLocation.Moves])
                    {
                        player.Attributes.Attribute[EffectLocation.Moves] =
                            player.MaxAttributes.Attribute[EffectLocation.Moves];
                    }
                }

                _core.UpdateClient.UpdateHP(player);
                _core.UpdateClient.UpdateMana(player);
                _core.UpdateClient.UpdateMoves(player);
                _core.UpdateClient.UpdateScore(player);

            }
        }

        public void PostTick()
        {
            _rooms.Clear();
            _players.Clear();
        }

        private int GainAmount(int value, Player player)
        {
            return player.Status switch
            {
                CharacterStatus.Status.Sleeping => value *= 3,
                CharacterStatus.Status.Resting => value *= 2,
                _ => value
            };
        }
    }
}


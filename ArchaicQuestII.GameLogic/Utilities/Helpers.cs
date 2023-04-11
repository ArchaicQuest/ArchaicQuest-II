using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;
using Discord;
using Discord.Rest;

namespace ArchaicQuestII.GameLogic.Utilities
{
    /// <summary>
    /// Random functions to go here unless there is
    /// enough related random functions to move out to their own class
    /// </summary>
    public static class Helpers
    {
        public static Tuple<int, string> findNth(string command)
        {
            var splitCommand = command.Split('.');
            var isnumber = int.TryParse(splitCommand[0], out int nth);
            if (splitCommand[0].Equals(command) || !isnumber)
            {
                return new Tuple<int, string>(-1, command);
            }

            var target = splitCommand[1];

            return new Tuple<int, string>(nth, target);
        }

        public static Item.Item findRoomObject(Tuple<int, string> keyword, Room room)
        {
            return keyword.Item1 == -1
                ? room.Items.FirstOrDefault(
                    x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)
                )
                : room.Items
                    .FindAll(
                        x =>
                            x.Name.Contains(
                                keyword.Item2,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                    .Skip(keyword.Item1 - 1)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Find matching player or mob
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public static Player findPlayerObject(Tuple<int, string> keyword, Room room)
        {
            return keyword.Item1 == -1
                ? room.Players.FirstOrDefault(
                    x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)
                )
                : room.Players
                    .FindAll(
                        x =>
                            x.Name.Contains(
                                keyword.Item2,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                    .Skip(keyword.Item1 - 1)
                    .FirstOrDefault()
                    ?? (
                        keyword.Item1 == null
                            ? room.Mobs.FirstOrDefault(
                                x =>
                                    x.Name.Contains(
                                        keyword.Item2,
                                        StringComparison.CurrentCultureIgnoreCase
                                    )
                            )
                            : room.Mobs
                                .FindAll(
                                    x =>
                                        x.Name.Contains(
                                            keyword.Item2,
                                            StringComparison.CurrentCultureIgnoreCase
                                        )
                                )
                                .Skip(keyword.Item1 - 1)
                                .FirstOrDefault()
                    );
        }

        public static Player FindPlayer(Tuple<int, string> keyword, Room room)
        {
            return keyword.Item1 == -1
                ? room.Players.FirstOrDefault(
                    x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)
                )
                : room.Players
                    .FindAll(
                        x =>
                            x.Name.Contains(
                                keyword.Item2,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                    .Skip(keyword.Item1 - 1)
                    .FirstOrDefault();
        }

        public static Player FindMob(Tuple<int, string> keyword, Room room)
        {
            return keyword.Item1 == -1
                ? room.Mobs
                    .Where(x => x.IsHiddenScriptMob == false)
                    .FirstOrDefault(
                        x =>
                            x.Name.Contains(
                                keyword.Item2,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                    ?? room.Mobs.FirstOrDefault(
                        x =>
                            x.LongName.Contains(
                                keyword.Item2,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                : room.Mobs
                    .FindAll(
                        x =>
                            x.Name.Contains(
                                keyword.Item2,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                    .Skip(keyword.Item1 - 1)
                    .FirstOrDefault()
                    ?? room.Mobs
                        .FindAll(
                            x =>
                                x.LongName.Contains(
                                    keyword.Item2,
                                    StringComparison.CurrentCultureIgnoreCase
                                )
                        )
                        .Skip(keyword.Item1 - 1)
                        .FirstOrDefault();
        }

        public static string ReturnRoomId(Room room)
        {
            return $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
        }

        /// <summary>
        /// Use to remove A / An from word
        /// </summary>
        /// <param name="word"></param>
        /// <returns> string</returns>
        public static string RemoveArticle(string word)
        {
            var wordWithOutArticle = word;
            var splitWord = word.Split(" ");

            if (
                splitWord[0].Equals("a", StringComparison.CurrentCultureIgnoreCase)
                || splitWord[0].Equals("an", StringComparison.CurrentCultureIgnoreCase)
            )
            {
                wordWithOutArticle = word.Substring(word.IndexOf(" ") + 1);
            }

            return wordWithOutArticle;
        }

        /// <summary>
        /// Adds A / An to word, should work in most cases as
        /// we are dealing with nouns but this doesn't work for
        /// words like hour where it should be An Hour and not a
        /// hour.
        /// </summary>
        /// <param name="word"></param>
        /// <returns> string</returns>
        public static string AddArticle(string word)
        {
            var startsWithVowel = "aeiou".Contains(
                word[0],
                StringComparison.CurrentCultureIgnoreCase
            );
            var newWord = string.Empty;

            if (startsWithVowel)
            {
                newWord = "An " + word;
            }
            else
            {
                newWord = "A " + word;
            }
            return newWord;
        }

        public static string DisplayDoor(Exit exit)
        {
            var exitName = "";
            if (exit.Door && exit.Closed)
            {
                exitName = $"[{exit.Name}]";
            }
            else
            {
                exitName = exit.Name;
            }
            return exitName;
        }

        public static Exit IsExit(string exit, Room room)
        {
            if (
                room.Exits.North != null
                && room.Exits.North.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase)
            )
            {
                return room.Exits.North;
            }

            if (
                room.Exits.East != null
                && room.Exits.East.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase)
            )
            {
                return room.Exits.East;
            }

            if (
                room.Exits.South != null
                && room.Exits.South.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase)
            )
            {
                return room.Exits.South;
            }

            if (
                room.Exits.West != null
                && room.Exits.West.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase)
            )
            {
                return room.Exits.West;
            }

            if (
                room.Exits.NorthWest != null
                && room.Exits.NorthWest.Name.StartsWith(
                    exit,
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                return room.Exits.NorthWest;
            }

            if (
                room.Exits.NorthEast != null
                && room.Exits.NorthEast.Name.StartsWith(
                    exit,
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                return room.Exits.NorthEast;
            }

            if (
                room.Exits.SouthEast != null
                && room.Exits.SouthEast.Name.StartsWith(
                    exit,
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                return room.Exits.SouthEast;
            }
            if (
                room.Exits.SouthWest != null
                && room.Exits.SouthWest.Name.StartsWith(
                    exit,
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            {
                return room.Exits.SouthWest;
            }
            if (
                room.Exits.Down != null
                && room.Exits.Down.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase)
            )
            {
                return room.Exits.Down;
            }
            if (
                room.Exits.Up != null
                && room.Exits.Up.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase)
            )
            {
                return room.Exits.Up;
            }

            return null;
        }

        public static List<string> GetListOfExits(ExitDirections exits)
        {
            var exitList = new List<string>();

            if (exits.North != null)
            {
                exitList.Add(exits.North.Name);
            }

            if (exits.East != null)
            {
                exitList.Add(exits.East.Name);
            }

            if (exits.South != null)
            {
                exitList.Add(exits.South.Name);
            }

            if (exits.West != null)
            {
                exitList.Add(exits.West.Name);
            }

            if (exits.Up != null)
            {
                exitList.Add(exits.Up.Name);
            }

            if (exits.Down != null)
            {
                exitList.Add(exits.Down.Name);
            }

            if (exits.NorthEast != null)
            {
                exitList.Add(exits.NorthEast.Name);
            }

            if (exits.NorthWest != null)
            {
                exitList.Add(exits.NorthWest.Name);
            }

            if (exits.SouthEast != null)
            {
                exitList.Add(exits.SouthEast.Name);
            }

            if (exits.SouthWest != null)
            {
                exitList.Add(exits.SouthWest.Name);
            }

            return exitList;
        }

        public static async void PostToDiscordBot(string message, ulong channelId, string token)
        {
            // Create a new instance of the DiscordRestClient
            var client = new DiscordRestClient();

            // Login to Discord using your bot token
            await client.LoginAsync(TokenType.Bot, token);

            // Get the channel you want to post the message to
            var channel = await client.GetChannelAsync(channelId); // replace with the channel ID

            // Send the message to the channel
            await (channel as IMessageChannel).SendMessageAsync(message);
        }

        public static async void PostToDiscord(string botToSay, string eventName, Config config)
        {
            if (!config.PostToDiscord)
            {
                return;
            }
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("content", botToSay) }
            );

            content.Headers.ContentType = new MediaTypeHeaderValue(
                "application/x-www-form-urlencoded"
            );

            switch (eventName)
            {
                case "event":
                    if (!string.IsNullOrEmpty(config.EventsDiscordWebHookURL))
                    {
                        await client.PostAsync(config.EventsDiscordWebHookURL, content);
                    }

                    break;

                case "error":
                    if (!string.IsNullOrEmpty(config.ErrorDiscordWebHookURL))
                    {
                        await client.PostAsync(config.ErrorDiscordWebHookURL, content);
                    }

                    break;
                default:
                    break;
            }

            client.Dispose();
        }

        public static int GetPercentage(int percentage, int value)
        {
            if (percentage == 0)
            {
                return percentage;
            }

            return (int)Math.Floor((double)(percentage * value / 100m));
        }

        public static int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string ReturnOpositeExitName(string direction)
        {
            switch (direction)
            {
                case "North":
                    return "South";
                case "North East":
                    return "South West";
                case "East":
                    return "West";
                case "South East":
                    return "North West";
                case "South":
                    return "North";
                case "South West":
                    return "North East";
                case "West":
                    return "East";
                case "North West":
                    return "South East";
                case "Down":
                    return "Up";
                case "Up":
                    return "Down";
                default:
                {
                    return direction;
                }
            }
        }

        /// <summary>
        /// Adds flags to display of items
        /// e.g (glow) A Long sword
        /// </summary>
        /// <param name="item"></param>
        /// <returns>name of the weapon with prefixed flags</returns>
        public static string DisplayEQNameWithFlags(Item.Item item)
        {
            if (item == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            if ((item.ItemFlag & Item.Item.ItemFlags.Glow) != 0)
            {
                sb.Append("({teal}Glowing{/}) ");
            }

            if ((item.ItemFlag & Item.Item.ItemFlags.Hum) != 0)
            {
                sb.Append("({yellow}Humming{/}) ");
            }

            if ((item.ItemFlag & Item.Item.ItemFlags.Holy) != 0)
            {
                sb.Append("(Holy) ");
            }

            sb.Append(item.Name);

            return sb.ToString();
        }

        public static Tuple<string, EffectLocation> GetStatName(string name)
        {
            return name switch
            {
                "str" => new Tuple<string, EffectLocation>("strength", EffectLocation.Strength),
                "dex" => new Tuple<string, EffectLocation>("dexterity", EffectLocation.Dexterity),
                "con"
                    => new Tuple<string, EffectLocation>(
                        "constitution",
                        EffectLocation.Constitution
                    ),
                "int"
                    => new Tuple<string, EffectLocation>(
                        "intelligence",
                        EffectLocation.Intelligence
                    ),
                "wis" => new Tuple<string, EffectLocation>("wisdom", EffectLocation.Wisdom),
                "cha" => new Tuple<string, EffectLocation>("charisma", EffectLocation.Charisma),
                "hp" => new Tuple<string, EffectLocation>("hit points", EffectLocation.Hitpoints),
                "move" => new Tuple<string, EffectLocation>("moves", EffectLocation.Moves),
                "mana" => new Tuple<string, EffectLocation>("mana", EffectLocation.Mana),
                _ => new Tuple<string, EffectLocation>("", EffectLocation.None)
            };
        }

        public static string Replace(
            string source,
            string oldString,
            string newString,
            StringComparison comparison,
            bool recursive = false
        )
        {
            var index = source.IndexOf(oldString, comparison);

            while (index > -1)
            {
                source = source.Remove(index, oldString.Length);
                source = source.Insert(index, newString);

                if (!recursive)
                {
                    return source;
                }
                index = source.IndexOf(oldString, index + newString.Length, comparison);
            }

            return source;
        }

        public static string ReplaceSocialTags(string text, Player player, Player target)
        {
            if (text == "null")
            {
                return "You can't do that.";
            }
            var newText = text.Replace("#player#", player.Name)
                .Replace("#pgender#", player.ReturnPronoun())
                .Replace("#pgender2#", player.ReturnSubjectPronoun())
                .Replace("#pgender3#", player.ReturnObjectPronoun())
                .Replace("#pgender#", player.ReturnPronoun());

            if (target != null)
            {
                newText = newText
                    .Replace("#target#", target.Name)
                    .Replace("#tgender#", target.ReturnPronoun())
                    .Replace("#tgender2#", target.ReturnSubjectPronoun())
                    .Replace("#tgender3#", target.ReturnObjectPronoun());
            }

            return newText;
        }

        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
            {
                return str;
            }

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}

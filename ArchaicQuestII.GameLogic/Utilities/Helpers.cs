using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;

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
                ? room.Items.FirstOrDefault(x =>
                    x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                : room.Items.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        public static Item.Item findObjectInInventory(Tuple<int, string> keyword, Player player)
        {
            if (keyword.Item2.Equals("book"))
            {
                return keyword.Item1 == -1 ? player.Inventory.FirstOrDefault(x => x.ItemType == Item.Item.ItemTypes.Book) :
                player.Inventory.FindAll(x => x.ItemType == Item.Item.ItemTypes.Book).Skip(keyword.Item1 - 1).FirstOrDefault();
            }

            return keyword.Item1 == -1 ? player.Inventory.FirstOrDefault(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)) :
                player.Inventory.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)).Skip(keyword.Item1 - 1).FirstOrDefault();
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
                ? room.Players.FirstOrDefault(x =>
                    x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                : room.Players.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault() ?? room.Mobs
                    .FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        public static Player FindPlayer(Tuple<int, string> keyword, Room room)
        {
            return keyword.Item1 == -1
                ? room.Players.FirstOrDefault(x =>
                    x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                : room.Players.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        public static Player FindMob(Tuple<int, string> keyword, Room room)
        {

            return keyword.Item1 == -1
                ? room.Mobs.Where(x => x.IsHiddenScriptMob == false).FirstOrDefault(x =>
                    x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase)) ?? room.Mobs.FirstOrDefault(x =>
                    x.LongName.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                : room.Mobs.FindAll(x => x.Name.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault() ?? room.Mobs.FindAll(x => x.LongName.Contains(keyword.Item2, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(keyword.Item1 - 1).FirstOrDefault();
        }

        public static string ReturnRoomId(Room room)
        {
            return $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
        }

        public static SkillList FindSkill(string skillName, Player player)
        {

            return player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase) && player.Level >= x.Level);
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

            if (splitWord[0].Equals("a", StringComparison.CurrentCultureIgnoreCase) || splitWord[0].Equals("an", StringComparison.CurrentCultureIgnoreCase))
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

            var startsWithVowel = "aeiou".Contains(word[0], StringComparison.CurrentCultureIgnoreCase);
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

        /// <summary>
        /// Her / His
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "her",
                "Male" => "his",
                _ => "their",
            };
        }
        /// <summary>
        /// She / He
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetSubjectPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "she",
                "Male" => "he",
                _ => "it",
            };
        }

        /// <summary>
        /// Her / Him
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetObjectPronoun(string gender)
        {
            return gender switch
            {
                "Female" => "her",
                "Male" => "him",
                _ => "it",
            };
        }

        public static bool isCaster(string classname)
        {
            return classname switch
            {
                "Mage" => true,
                "Cleric" => true,
                "Druid" => true,
                _ => false,
            };
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
            if (room.Exits.North != null && room.Exits.North.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.North;
            }

            if (room.Exits.East != null && room.Exits.East.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.East;
            }

            if (room.Exits.South != null && room.Exits.South.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.South;
            }

            if (room.Exits.West != null && room.Exits.West.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.West;
            }

            if (room.Exits.NorthWest != null && room.Exits.NorthWest.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.NorthWest;
            }

            if (room.Exits.NorthEast != null && room.Exits.NorthEast.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.NorthEast;
            }

            if (room.Exits.SouthEast != null && room.Exits.SouthEast.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.SouthEast;
            }
            if (room.Exits.SouthWest != null && room.Exits.SouthWest.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.SouthWest;
            }
            if (room.Exits.Down != null && room.Exits.Down.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
            {
                return room.Exits.Down;
            }
            if (room.Exits.Up != null && room.Exits.Up.Name.StartsWith(exit, StringComparison.CurrentCultureIgnoreCase))
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

        public static async void PostToDiscord(string botToSay, string discordWebHookUrl)
        {
            if (!string.IsNullOrEmpty(discordWebHookUrl)) return;
            
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("content", botToSay)
            });

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            
            await client.PostAsync(discordWebHookUrl, content);

            client.Dispose();
        }


        public static int GetPercentage(int percentage, int value)
        {
            if (percentage == 0)
            {
                return percentage;
            }

            return (int)percentage * value / 100;
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
        
        public static int GetWeaponSkill(Item.Item weapon, Player player)
        {

            var weaponTypeString = Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType);

            var weaponSkill = player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals(weaponTypeString, StringComparison.CurrentCultureIgnoreCase));

            return (int)(weaponSkill == null ? 0 : weaponSkill.Proficiency);
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
                default: { return direction; }
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

        /// <summary>
        /// Applies bonus affects to player
        /// </summary>
        /// <param name="direction"></param>
        public static void ApplyAffects(Affect affect, Player player)
        {

            {
                if (affect.Modifier.Strength != 0)
                {
                    player.Attributes.Attribute[EffectLocation.Strength] += affect.Modifier.Strength;
                }

                if (affect.Modifier.Dexterity != 0)
                {
                    player.Attributes.Attribute[EffectLocation.Dexterity] += affect.Modifier.Dexterity;
                }

                if (affect.Modifier.Constitution != 0)
                {
                    player.Attributes.Attribute[EffectLocation.Constitution] += affect.Modifier.Constitution;
                }

                if (affect.Modifier.Intelligence != 0)
                {
                    player.Attributes.Attribute[EffectLocation.Intelligence] += affect.Modifier.Intelligence;
                }

                if (affect.Modifier.Wisdom != 0)
                {
                    player.Attributes.Attribute[EffectLocation.Wisdom] += affect.Modifier.Wisdom;
                }

                if (affect.Modifier.Charisma != 0)
                {
                    player.Attributes.Attribute[EffectLocation.Charisma] += affect.Modifier.Charisma;
                }

                if (affect.Modifier.HitRoll != 0)
                {
                    player.Attributes.Attribute[EffectLocation.HitRoll] += affect.Modifier.HitRoll;
                }

                if (affect.Modifier.DamRoll != 0)
                {
                    player.Attributes.Attribute[EffectLocation.DamageRoll] += affect.Modifier.DamRoll;
                }

                if (affect.Modifier.Armour != 0)
                {
                    player.ArmorRating.Armour += affect.Modifier.Armour;
                    player.ArmorRating.Magic += affect.Modifier.Armour;
                }

                if (affect.Affects == DefineSpell.SpellAffect.Blind)
                {
                    player.Affects.Blind = true;
                }
                if (affect.Affects == DefineSpell.SpellAffect.Berserk)
                {
                    player.Affects.Berserk = true;
                }
                if (affect.Affects == DefineSpell.SpellAffect.NonDetect)
                {
                    player.Affects.NonDectect = true;
                }
                if (affect.Affects == DefineSpell.SpellAffect.Invis)
                {
                    player.Affects.Invis = true;
                }
                if (affect.Affects == DefineSpell.SpellAffect.DetectInvis)
                {
                    player.Affects.DetectInvis = true;
                }
                if (affect.Affects == DefineSpell.SpellAffect.DetectHidden)
                {
                    player.Affects.DetectHidden = true;
                }
                if (affect.Affects == DefineSpell.SpellAffect.Poison)
                {
                    player.Affects.Poisoned = true;
                }
                if (affect.Affects == DefineSpell.SpellAffect.Haste
                )
                {
                    player.Affects.Haste = true;
                }

            }
        }
        
        public static bool SkillSuccessCheck(Player player, string skillName)
        {
            var skill = player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals(skillName, StringComparison.CurrentCultureIgnoreCase));

            var chance = DiceBag.Roll(1, 1, 100);

            return (skill == null || !(skill.Proficiency <= chance)) && chance != 1 && chance != 101;
        }
        
        public static bool SkillSuccessCheck(SkillList skill)
        {
            var proficiency = skill.Proficiency;
            var success = DiceBag.Roll(1, 1, 100);

            if (success == 1 || success == 101)
            {
                return false;
            }

            return proficiency >= success;
        }
        
        public static bool LoreSuccess(int? skillLevel)
        {
            var chance = DiceBag.Roll(1, 1, 100);

            return skillLevel >= chance;
        }

        public static string UpdateAffect(Player player, Item.Item item, Affect affect)
        {
            var modBenefits = string.Empty;

            if (item.Modifier.Strength != 0)
            {

                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Strength] += item.Modifier.Strength;

                affect.Modifier.Strength = item.Modifier.Strength;
                modBenefits = $"modifies STR by {item.Modifier.Strength} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.Dexterity != 0)
            {
                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Dexterity] += item.Modifier.Dexterity;

                affect.Modifier.Dexterity = item.Modifier.Dexterity;
                modBenefits = $"modifies DEX by {item.Modifier.Dexterity} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.Constitution != 0)
            {
                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Constitution] += item.Modifier.Constitution;

                affect.Modifier.Constitution = item.Modifier.Constitution;
                modBenefits = $"modifies CON by {item.Modifier.Constitution} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.Intelligence != 0)
            {
                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Intelligence] += item.Modifier.Intelligence;
                affect.Modifier.Intelligence = item.Modifier.Intelligence;
                modBenefits = $"modifies INT by {item.Modifier.Intelligence} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.Wisdom != 0)
            {
                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Wisdom] += item.Modifier.Wisdom;

                affect.Modifier.Wisdom = item.Modifier.Wisdom;
                modBenefits = $"modifies WIS by {item.Modifier.Wisdom} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.Charisma != 0)
            {
                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.Charisma] += item.Modifier.Charisma;

                affect.Modifier.Charisma = item.Modifier.Charisma;
                modBenefits = $"modifies CHA by {item.Modifier.Charisma} for { affect.Duration} minutes<br />";

            }

            if (item.Modifier.HP != 0)
            {
                player.Attributes.Attribute[EffectLocation.Hitpoints] += item.Modifier.HP;

                if (player.Attributes.Attribute[EffectLocation.Hitpoints] >
                    player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                {
                    player.Attributes.Attribute[EffectLocation.Hitpoints] =
                        player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                }
            }

            if (item.Modifier.Mana != 0)
            {
                player.Attributes.Attribute[EffectLocation.Mana] += item.Modifier.Mana;

                if (player.Attributes.Attribute[EffectLocation.Mana] >
                    player.MaxAttributes.Attribute[EffectLocation.Mana])
                {
                    player.Attributes.Attribute[EffectLocation.Mana] =
                        player.MaxAttributes.Attribute[EffectLocation.Mana];
                }
            }

            if (item.Modifier.Moves != 0)
            {
                player.Attributes.Attribute[EffectLocation.Moves] += item.Modifier.Moves;

                if (player.Attributes.Attribute[EffectLocation.Moves] >
                    player.MaxAttributes.Attribute[EffectLocation.Moves])
                {
                    player.Attributes.Attribute[EffectLocation.Moves] =
                        player.MaxAttributes.Attribute[EffectLocation.Moves];
                }
            }

            if (item.Modifier.HitRoll != 0)
            {
                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.HitRoll] += item.Modifier.HitRoll;
                affect.Modifier.HitRoll = item.Modifier.HitRoll;

                modBenefits = $"modifies Hit Roll by {item.Modifier.HitRoll} for { affect.Duration} minutes<br />";
            }

            if (item.Modifier.DamRoll != 0)
            {
                affect.Duration = 5;
                player.Attributes.Attribute[EffectLocation.DamageRoll] += item.Modifier.DamRoll;

                affect.Modifier.DamRoll = item.Modifier.DamRoll;
                modBenefits = $"modifies Dam Roll by {item.Modifier.DamRoll} for { affect.Duration} minutes<br />";

            }

            // saves / saving spell

            return modBenefits;
        }
        
        public static Tuple<string, EffectLocation> GetStatName(string name)
        {
            return name switch
            {
                "str" => new Tuple<string, EffectLocation>("strength", EffectLocation.Strength),
                "dex" => new Tuple<string, EffectLocation>("dexterity", EffectLocation.Dexterity),
                "con" => new Tuple<string, EffectLocation>("constitution", EffectLocation.Constitution),
                "int" => new Tuple<string, EffectLocation>("intelligence", EffectLocation.Intelligence),
                "wis" => new Tuple<string, EffectLocation>("wisdom", EffectLocation.Wisdom),
                "cha" => new Tuple<string, EffectLocation>("charisma", EffectLocation.Charisma),
                "hp" => new Tuple<string, EffectLocation>("hit points", EffectLocation.Hitpoints),
                "move" => new Tuple<string, EffectLocation>("moves", EffectLocation.Moves),
                "mana" => new Tuple<string, EffectLocation>("mana", EffectLocation.Mana),
                _ => new Tuple<string, EffectLocation>("", EffectLocation.None)
            };
        }
        
        public static string Replace(string source, string oldString,
            string newString, StringComparison comparison,
            bool recursive = false)
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
            var newText = text.Replace("#player#", player.Name).Replace("#pgender#", Helpers.GetPronoun(player.Gender))
                .Replace("#pgender2#", Helpers.GetSubjectPronoun(player.Gender))
                .Replace("#pgender3#", Helpers.GetObjectPronoun(player.Gender))
                .Replace("#pgender#", Helpers.GetPronoun(player.Gender));

            if (target != null)
            {
                newText = newText.Replace("#target#", target.Name)
                    .Replace("#tgender#", Helpers.GetPronoun(target.Gender))
                    .Replace("#tgender2#", Helpers.GetSubjectPronoun(target.Gender))
                    .Replace("#tgender3#", Helpers.GetObjectPronoun(target.Gender));
            }

            return newText;
        }

        /// <summary>
        /// Logs and error to the database
        /// </summary>
        /// <param name="db"></param>
        /// <param name="file">which file its in</param>
        /// <param name="error">the error message</param>
        /// <param name="priority"></param>
        public static void LogError(IDataBase db, string file, string error, ErrorPriority priority)
        {
            var e = new ErrorInfo
            {
                File = file,
                Error = error,
                Priority = priority
            };

            db.Save(e, DataBase.Collections.ErrorLog);
        }
        
        public static void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
        {
            player.Status = status;
            player.LongName = longName;
            player.Pose = "";
        }
        
        public static string ObfuscateSpellName(string spellName)
        {
            var magicWords = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(" ", " "),
                new Tuple<string, string>("ar", "abra"),
                new Tuple<string, string>("au", "kada"),
                new Tuple<string, string>("bless", "fido"),
                new Tuple<string, string>("blind", "nose"),
                new Tuple<string, string>("bur", "mosa"),
                new Tuple<string, string>("cu", "judi"),
                new Tuple<string, string>("de", "oculo"),
                new Tuple<string, string>("en", "unso"),
                new Tuple<string, string>("light", "dies"),
                new Tuple<string, string>("lo", "hi"),
                new Tuple<string, string>("mor", "zak"),
                new Tuple<string, string>("move", "sido"),
                new Tuple<string, string>("ness", "lacri"),
                new Tuple<string, string>("ning", "illa"),
                new Tuple<string, string>("per", "duda"),
                new Tuple<string, string>("ra", "gru"),
                new Tuple<string, string>("fresh", "ima"),
                new Tuple<string, string>("re", "candus"),
                new Tuple<string, string>("son", "sabru"),
                new Tuple<string, string>("tect", "infra"),
                new Tuple<string, string>("tri", "cula"),
                new Tuple<string, string>("ven", "nofo"),
                new Tuple<string, string>("a", "a"), new Tuple<string, string>("b", "b"),
                new Tuple<string, string>("c", "q"), new Tuple<string, string>("d", "e"),
                new Tuple<string, string>("e", "z"), new Tuple<string, string>("f", "y"),
                new Tuple<string, string>("g", "o"), new Tuple<string, string>("h", "p"),
                new Tuple<string, string>("i", "u"), new Tuple<string, string>("j", "y"),
                new Tuple<string, string>("k", "t"), new Tuple<string, string>("l", "r"),
                new Tuple<string, string>("m", "w"), new Tuple<string, string>("n", "i"),
                new Tuple<string, string>("o", "a"), new Tuple<string, string>("p", "s"),
                new Tuple<string, string>("q", "d"), new Tuple<string, string>("r", "f"),
                new Tuple<string, string>("s", "g"), new Tuple<string, string>("t", "h"),
                new Tuple<string, string>("u", "j"), new Tuple<string, string>("v", "z"),
                new Tuple<string, string>("w", "x"), new Tuple<string, string>("x", "n"),
                new Tuple<string, string>("y", "l"), new Tuple<string, string>("z", "k"),
                new Tuple<string, string>("", "")
            };

            var buff = new StringBuilder();
            var pos = 0;
            var length = 0;

            while (pos < spellName.Length)
            {

                foreach (var magicWord in magicWords)
                {
                    var prefix = magicWord.Item1;
                    var len = prefix.Length;

                    if (pos + len <= spellName.Length && spellName.IndexOf(prefix, pos, len, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        buff.Append(magicWord.Item2);
                        length = len;
                        break;
                    }
                }

                if (length > 0)
                {
                    pos += length;
                }
            }

            return buff.ToString();
        }
        
        public static Item.Item ReturnTargetItem(Skill.Model.Skill spell, string target, Room room, Player player)
        {

            if ((spell.ValidTargets & ValidTargets.TargetObjectInventory) != 0 || (spell.ValidTargets & ValidTargets.TargetObjectEquipped) != 0)
            {
                //find victim from player cache instead
                var item = player.Inventory.FirstOrDefault(x =>
                    x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

                if (item == null)
                {
                    //target not found
                    return null;
                }


                return item;
            }


            return null;
        }
        
        public static Player GetTarget(string target, Room room)
        {
            if (string.IsNullOrEmpty(target))
            {
                return null;
            }

            return room.Mobs.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ??
                   room.Players.FirstOrDefault(
                       x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
        }
        
        public static Player GetValidTarget(Player player, Player target, ValidTargets validTargets)
        {

            var setTarget = target;
            if (validTargets.HasFlag(ValidTargets.TargetFightSelf) && player.Status == CharacterStatus.Status.Fighting)
            {
                setTarget = player;
            }

            if (validTargets.HasFlag(ValidTargets.TargetFightVictim) && player.Status == CharacterStatus.Status.Fighting)
            {
                setTarget = target;
            }

            if (validTargets == ValidTargets.TargetIgnore)
            {
                setTarget = player;
            }

            return setTarget;
        }
        
        public static string ReplaceTargetPlaceholders(string str, Player player, bool isTarget)
        {
            string newString;
            
            if (isTarget)
            {
                newString = str.Replace("#target#", "You");

                return newString;
            }

            newString = str.Replace("#target#", player.Name);

            return newString;

        }
    }
}

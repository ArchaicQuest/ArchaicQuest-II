using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Emote;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Commands;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Socials
    {
        private static Dictionary<string, Emote> SeedData()
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            System.IO.StreamReader file =
                new System.IO.StreamReader(directory + "/socials.txt");
            string line;

            // var socials = new List<Emote>();
            var socialObject = new Dictionary<string, Emote>();
            var emote = new Emote();
            var key = string.Empty;
            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);

                var x = line.Split(" ");

                if (x[0] == "Name")
                {
                    key = x[8].Replace("~", "");
                }

                if (x[0] == "OthersNoArg")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");

                    emote.RoomNoTarget = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }

                if (x[0] == "CharNoArg")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");

                    emote.CharNoTarget = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }


                if (x[0] == "CharFound")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");
                    emote.TargetFound = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }

                if (x[0] == "OthersFound")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");
                    emote.RoomTarget = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }

                if (x[0] == "VictFound")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");
                    emote.ToTarget = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }

                if (x[0] == "CharAuto")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");
                    emote.TargetSelf = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }

                if (x[0] == "CharAuto")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");
                    emote.TargetSelf = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }

                if (x[0] == "OthersAuto")
                {
                    var s = string.Join(" ", x);
                    var indexOfFirstSpace = s.IndexOf(" ");
                    emote.RoomSelf = s.Substring(indexOfFirstSpace + 1).Replace("$n", "#player#")
                        .Replace("$N", "#target#")
                        .Replace("$n", "#player#")
                        .Replace("$e", "#pgender2#")
                        .Replace("$E", "#tgender2#")
                        .Replace("$s", "#pgender#")
                        .Replace("$S", "#tgender#")
                        .Replace("$M", "#tgender3#")
                        .Replace("$m", "#pgender3#").Replace("~", "").Trim();
                }

                if (x[0] == "End")
                {
                    socialObject.Add(key, emote);
                    emote = new Emote();
                }
            }
            file.Close();

            return socialObject;
        }

        internal static void SeedAndCache(IDataBase db, ICommandHandler commandHandler)
        {
            var seedData = SeedData();

            if (!db.DoesCollectionExist(DataBase.Collections.Socials))
            {
                foreach (var socialSeed in seedData)
                {
                    db.Save(socialSeed, DataBase.Collections.Socials);
                }
            }

            foreach (var socialSeed in seedData)
            {
                commandHandler.AddSocial(socialSeed.Key, socialSeed.Value);
            }
        }
    }
}

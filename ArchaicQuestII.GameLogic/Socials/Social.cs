using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Socials
{
    public class Social : ISocials
    {

        private readonly IWriteToClient _writeToClient;
        private readonly ICache _cache;
        private readonly IMobScripts _mobScripts;
        public Social(IWriteToClient writeToClient, ICache cache, IMobScripts mobScripts)
        {
            _writeToClient = writeToClient;
            _cache = cache;
            _mobScripts = mobScripts;
        }

        public string ReplaceSocialTags(string text, Player player, Player target)
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
        public void EmoteSocial(Player player, Room room, Emote social, string target)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                _writeToClient.WriteLine("You can't do this while asleep.", player.ConnectionId);
                return;
            }


            if (string.IsNullOrEmpty(target))
            {
                _writeToClient.WriteLine($"<p>{social.CharNoTarget}</p>", player.ConnectionId);

                foreach (var pc in room.Players)
                {
                    if (pc.Id == player.Id)
                    {
                        continue;
                    }
                    _writeToClient.WriteLine($"<p>{ReplaceSocialTags(social.RoomNoTarget, player, null)}</p>", pc.ConnectionId);
                }

                return;
            }

            var getTarget = target.Equals("self", StringComparison.CurrentCultureIgnoreCase) ? player : room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
            if (getTarget == null)
            {
                getTarget = room.Mobs.FirstOrDefault(x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));
            }
            if (getTarget != null)
            {
                if (getTarget.Id == player.Id)
                {
                    foreach (var pc in room.Players)
                    {
                        if (pc.Id == player.Id)
                        {
                            _writeToClient.WriteLine($"<p>{ReplaceSocialTags(social.TargetSelf, player, getTarget)}</p>", player.ConnectionId);
                            continue;
                        }
                        _writeToClient.WriteLine($"<p>{ReplaceSocialTags(social.RoomSelf, player, getTarget)}</p>", pc.ConnectionId);
                    }
                    return;
                }
                _writeToClient.WriteLine($"<p>{ReplaceSocialTags(social.TargetFound, player, getTarget)}<p>", player.ConnectionId);
                _writeToClient.WriteLine($"<p>{ReplaceSocialTags(social.ToTarget, player, getTarget)}</p>", getTarget.ConnectionId);
                foreach (var pc in room.Players)
                {
                    if (pc.Id == player.Id || pc.Id == getTarget.Id)
                    {
                        continue;
                    }
                    _writeToClient.WriteLine($"<p>{ReplaceSocialTags(social.RoomTarget, player, getTarget)}</p>", pc.ConnectionId);
                }

                if (!string.IsNullOrEmpty(getTarget.Events.Act))
                {
                    UserData.RegisterType<MobScripts>();

                    Script script = new Script();

                    DynValue obj = UserData.Create(_mobScripts);
                    script.Globals.Set("obj", obj);
                    UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                    UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));


                    script.Globals["room"] = room;
                    script.Globals["player"] = player;
                    script.Globals["mob"] = getTarget;
                    script.Globals["text"] = ReplaceSocialTags(social.ToTarget, player, getTarget);


                    DynValue res = script.DoString(getTarget.Events.Act);
                }


            }
            else
            {
                _writeToClient.WriteLine("<p>They are not here.</p>", player.ConnectionId);
            }
        }

        public void DisplaySocials(Player player)
        {
            StringBuilder table = new StringBuilder("<table>");
            int count = 0;
            foreach (var social in _cache.GetSocials())
            {
                count++;

                if (count == 1)
                {
                    table.Append("<tr>");
                }

                table.Append($"<td>{social.Key}</td>");

                if (count == 10)
                {
                    table.Append("</tr>");
                    count = 0;
                }


            }

            table.Append("</table>");

            _writeToClient.WriteLine("<h3>Socials</h3> <p>Available socials:</p>" + table.ToString(), player.ConnectionId);
        }
    }
}

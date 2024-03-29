using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using MoonSharp.Interpreter;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class SocialCmd : ICommand
    {
        public SocialCmd()
        {
            Aliases = new[] { "social" };
            Description = "List prebuilt social emotes that you can use";
            Usages = new[] { "Type: social" };
            Title = "";
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Fighting,
                CharacterStatus.Status.Ghost,
                CharacterStatus.Status.Fleeing,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Stunned,
            };
            UserRole = UserRole.Player;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(2);
            var socialName = input.ElementAtOrDefault(1);

            var getSocial = Services.Instance.Cache
                .GetSocials()
                .Keys.FirstOrDefault(x => x.Equals(socialName));
            if (getSocial == null)
            {
                return;
            }
            target = socialName == target ? "" : target;
            Emote social = Services.Instance.Cache.GetSocials()[getSocial];

            if (string.IsNullOrEmpty(socialName) || socialName == "list")
            {
                var table = new StringBuilder("<table>");
                var count = 0;

                foreach (var s in Services.Instance.Cache.GetSocials())
                {
                    count++;

                    if (count == 1)
                    {
                        table.Append("<tr>");
                    }

                    table.Append($"<td>{s.Key}</td>");

                    if (count == 10)
                    {
                        table.Append("</tr>");
                        count = 0;
                    }
                }

                table.Append("</table>");

                Services.Instance.Writer.WriteLine(
                    "<h3>Socials</h3> <p>Available socials:</p>" + table,
                    player
                );
            }

            if (string.IsNullOrEmpty(target))
            {
                Services.Instance.Writer.WriteLine($"<p>{social.CharNoTarget}</p>", player);

                foreach (var pc in room.Players.Where(pc => pc.Id != player.Id))
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{Helpers.ReplaceSocialTags(social.RoomNoTarget, player, null)}</p>",
                        pc
                    );
                }

                return;
            }

            var getTarget = target.Equals("self", StringComparison.CurrentCultureIgnoreCase)
                ? player
                : room.Players.FirstOrDefault(
                    x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase)
                );

            if (getTarget == null)
            {
                getTarget = room.Mobs.FirstOrDefault(
                    x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            if (getTarget != null)
            {
                if (getTarget.Id == player.Id)
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{Helpers.ReplaceSocialTags(social.TargetSelf, player, getTarget)}</p>",
                        player
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{Helpers.ReplaceSocialTags(social.RoomSelf, player, getTarget)}</p>",
                        room,
                        player
                    );
                }

                if (getTarget.Id != player.Id)
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{Helpers.ReplaceSocialTags(social.TargetFound, player, getTarget)}<p>",
                        player
                    );
                    Services.Instance.Writer.WriteLine(
                        $"<p>{Helpers.ReplaceSocialTags(social.ToTarget, player, getTarget)}</p>",
                        getTarget
                    );
                }

                foreach (
                    var pc in room.Players.Where(pc => pc.Id != player.Id && pc.Id != getTarget.Id)
                )
                {
                    Services.Instance.Writer.WriteLine(
                        $"<p>{Helpers.ReplaceSocialTags(social.RoomTarget, player, getTarget)}</p>",
                        pc
                    );
                }

                if (!string.IsNullOrEmpty(getTarget.Events.Act))
                {
                    UserData.RegisterType<MobScripts>();

                    var script = new Script();

                    var obj = UserData.Create(Services.Instance.MobScripts);
                    script.Globals.Set("obj", obj);
                    UserData.RegisterProxyType<MyProxy, Room>(r => new MyProxy(room));
                    UserData.RegisterProxyType<ProxyPlayer, Player>(r => new ProxyPlayer(player));

                    script.Globals["room"] = room;
                    script.Globals["player"] = player;
                    script.Globals["mob"] = getTarget;
                    script.Globals["text"] = Helpers.ReplaceSocialTags(
                        social.ToTarget,
                        player,
                        getTarget
                    );

                    var res = script.DoString(getTarget.Events.Act);
                }
            }
            else
            {
                Services.Instance.Writer.WriteLine("<p>They are not here.</p>", player);
            }
        }
    }
}

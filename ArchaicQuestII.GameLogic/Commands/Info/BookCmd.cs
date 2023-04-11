using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using Markdig;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class BookCmd : ICommand
    {
        public BookCmd()
        {
            Aliases = new[] { "book" };
            Description =
                @"Books are useful for information, stories and recording progress. The commands below will tell you how to use books. <br />
<table class='simple'>
    <tr>
        <td style='min-width: 230px;'>book</td>
        <td>Shows what books you have in your inventory</td>
    </tr>
 <tr>
        <td>book open &lt;title&gt;</td>
        <td>Opens the book specified, once a book is opened you can now read, rename or write in it</td>
    </tr>
 <tr>
        <td>book close</td>
        <td>Close the open book</td>
    </tr>
 <tr>
        <td>book read &lt;page number&gt; </td>
        <td>Read the specified page number</td>
    </tr>
 <tr>
        <td>book rename &lt;new title&gt; </td>
        <td>renames the book to the given title</td>
    </tr>
 <tr>
        <td>book write &lt;page number&gt;</td>
        <td>specifies which page to write in, this opens the writing modal which accepts markdown</td>
    </tr>
</table>";
            Usages = new[]
            {
                "commands: book, book open, book close, book read 2, book write 3, book rename My Diary"
            };
            Title = "";
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Fleeing,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Stunned,
                CharacterStatus.Status.Fighting,
                CharacterStatus.Status.Ghost,
                CharacterStatus.Status.Resting
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
            var target1 = input.ElementAtOrDefault(1)?.ToLower();
            var target2 = input.ElementAtOrDefault(2)?.ToLower();

            if (string.IsNullOrEmpty(target1) && player.OpenedBook == null)
            {
                var sb = new StringBuilder();

                sb.Append("<ul>Current Books:");

                foreach (
                    var item in player.Inventory.Where(
                        item => item.ItemType == Item.Item.ItemTypes.Book
                    )
                )
                {
                    sb.Append($"<li>{item.Name}</li>");
                }

                sb.Append("</ul>");

                Services.Instance.Writer.WriteLine(sb.ToString(), player);
                return;
            }

            if (string.IsNullOrEmpty(target1) && player.OpenedBook != null)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>You currently have {player.OpenedBook.Name} opened.</p>",
                    player
                );
                return;
            }

            if (player.OpenedBook == null)
            {
                OpenBook(player, room, target1);
                return;
            }

            int page;
            switch (target1)
            {
                case "close":
                    CloseBook(player, room);
                    break;
                case "read":
                    int.TryParse(target2, out page);
                    ReadBook(player, page);
                    break;
                case "rename":
                    RenameBook(player, target2);
                    break;
                case "write":

                    if (string.IsNullOrEmpty(target2))
                    {
                        Services.Instance.Writer.WriteLine(
                            "<p>What page do you want to write on?</p>",
                            player
                        );
                        return;
                    }

                    if (!int.TryParse(target2, out page))
                    {
                        Services.Instance.Writer.WriteLine(
                            "<p>Page number must be a number.</p>",
                            player
                        );
                        return;
                    }

                    WriteBook(player, page, "");
                    break;
                default:
                    Services.Instance.Writer.WriteLine(
                        "<p>What are you trying to do? Valid options: Close, Read, Rename, and Write.</p>",
                        player
                    );
                    break;
            }
        }

        private void OpenBook(Player player, Room room, string book)
        {
            var nthTarget = Helpers.findNth(book);
            var item = player.FindObjectInInventory(nthTarget);

            if (item == null)
            {
                Services.Instance.Writer.WriteLine("<p>You dont have that book.</p>", player);
                return;
            }

            if (item.ItemType != Item.Item.ItemTypes.Book)
            {
                Services.Instance.Writer.WriteLine($"<p>{item.Name} is not a book.</p>", player);
                return;
            }

            Services.Instance.Writer.WriteLine(
                "<p>You open the book and prepare to 'read' or 'write' in it.</p>",
                player
            );

            foreach (var pc in room.Players)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name} gets out a book and opens it.</p>",
                    pc
                );
            }

            player.OpenedBook = item;
        }

        private void CloseBook(Player player, Room room)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You close {player.OpenedBook.Name} and put it away.</p>",
                player
            );

            foreach (var pc in room.Players.Where(x => x.Id != player.Id))
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name} closes a book and puts it away.</p>",
                    pc
                );
            }

            player.OpenedBook = null;
        }

        private void ReadBook(Player player, int pageNum)
        {
            if (pageNum == player.OpenedBook.Book.Pages.Count)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>That exceeds the page count of {player.OpenedBook.Book.Pages.Count}.</p>",
                    player
                );
                return;
            }

            if (pageNum >= player.OpenedBook.Book.PageCount)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.OpenedBook.Name} does not contain that many pages.</p>",
                    player
                );

                return;
            }

            if (string.IsNullOrEmpty(player.OpenedBook.Book.Pages[pageNum]))
            {
                Services.Instance.Writer.WriteLine("<p>This page is blank.</p>", player);
                return;
            }

            var result = Markdown.ToHtml(player.OpenedBook.Book.Pages[pageNum]);
            Services.Instance.Writer.WriteLine($"<p>{result}</p>", player);
        }

        private void WriteBook(Player player, int pageNum, string writing)
        {
            if (pageNum > player.OpenedBook.Book.PageCount)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.OpenedBook.Name} does not contain that many pages.</p>",
                    player
                );
                return;
            }

            if (player.OpenedBook.Book.PageCount > player.OpenedBook.Book.Pages.Count)
            {
                var diff = player.OpenedBook.Book.PageCount - player.OpenedBook.Book.Pages.Count;

                for (var i = 0; i < diff; i++)
                {
                    player.OpenedBook.Book.Pages.Add(string.Empty);
                }
            }

            var bookContent = new WriteBook
            {
                Title = player.OpenedBook.Name,
                Description = player.OpenedBook.Book.Pages[pageNum],
                PageNumber = pageNum
            };

            Services.Instance.UpdateClient.UpdateContentPopUp(player, bookContent);

            Services.Instance.Writer.WriteLine(
                $"<p>You begin writing in {player.OpenedBook.Name}.</p>",
                player
            );
        }

        private void RenameBook(Player player, string title)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{player.OpenedBook.Name} has now been titled {title}.</p>",
                player
            );
            var originalBook = player.Inventory.FirstOrDefault(
                x => x.Name.Equals(player.OpenedBook.Name)
            );

            if (originalBook != null)
            {
                originalBook.Name = title;
            }

            player.OpenedBook.Name = title;

            Services.Instance.UpdateClient.UpdateInventory(player);
        }
    }
}

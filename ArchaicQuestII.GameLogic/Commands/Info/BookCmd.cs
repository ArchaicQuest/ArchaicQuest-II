using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Markdig;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class BookCmd : ICommand
    {
        public BookCmd(ICore core)
        {
            Aliases = new[] {"book"};
            Description = "Write in or Read from a book.";
            Usages = new[] {"Type: book title read"};
            DeniedStatus = default;
            UserRole = UserRole.Player;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target1 = input.ElementAtOrDefault(1)?.ToLower();
            var target2 = input.ElementAtOrDefault(2)?.ToLower();
            var target3 = input.ElementAtOrDefault(3)?.ToLower();
            
            if (string.IsNullOrEmpty(target1) && player.OpenedBook == null)
            {
                var sb = new StringBuilder();
                
                sb.Append("<ul>Current Books:");
                
                foreach (var item in player.Inventory.Where(item => item.ItemType == Item.Item.ItemTypes.Book))
                {
                    sb.Append($"<li>{item.Name}</li>");
                }
                
                sb.Append("</ul>");
                
                Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
                return;
            }
            
            if (string.IsNullOrEmpty(target1) && player.OpenedBook != null)
            {
                Core.Writer.WriteLine($"<p>You currently have {player.OpenedBook} opened.</p>", player.ConnectionId);
                return;
            }

            if (player.OpenedBook == null)
            {
                OpenBook(player, room, target1);
                return;
            }
            
            if (player.OpenedBook != null && target1 == "close")
            {
                CloseBook(player, room);
            }

            if (player.OpenedBook != null && target1 == "read")
            {
                int.TryParse(target2, out var page);
                ReadBook(player, page);
            }
            
            if (player.OpenedBook != null && target1 == "rename")
            {
                RenameBook(player, target2);
            }

            if (player.OpenedBook != null && target1 == "write")
            {
                if (string.IsNullOrEmpty(target2))
                {
                    Core.Writer.WriteLine("<p>What page do you want to write on?</p>", player.ConnectionId);
                    return;
                }
                
                if (string.IsNullOrEmpty(target3))
                {
                    Core.Writer.WriteLine("<p>What do you want to write?</p>", player.ConnectionId);
                    return;
                }

                if (!int.TryParse(target2, out var page))
                {
                    Core.Writer.WriteLine("<p>Page number must be a number.</p>", player.ConnectionId);
                    return;
                }
                
                var words = string.Join(" ", input.Skip(2));
                WriteBook(player, page, words);
            }
        }

        private void OpenBook(Player player, Room room, string book)
        {
            var nthTarget = Helpers.findNth(book);
            var item = Helpers.findObjectInInventory(nthTarget, player);

            if (item == null)
            {
                Core.Writer.WriteLine("<p>You dont have that book.</p>", player.ConnectionId);
                return;
            }
                
            if (item.ItemType != Item.Item.ItemTypes.Book)
            {
                Core.Writer.WriteLine($"<p>{item.Name} is not a book.</p>", player.ConnectionId);
                return;
            }

            Core.Writer.WriteLine("<p>You open the book and prepare to 'read' or 'write' in it.</p>", player.ConnectionId);
            
            foreach (var pc in room.Players)
            {
                Core.Writer.WriteLine($"<p>{player} gets out a book and opens it.</p>", pc.ConnectionId);
            }
            
            player.Status = CharacterStatus.Status.Busy;
            player.OpenedBook = item;
        }

        private void CloseBook(Player player, Room room)
        {
            Core.Writer.WriteLine($"<p>You close {player.OpenedBook.Name} and put it away.</p>", player.ConnectionId);

            foreach (var pc in room.Players)
            {
                Core.Writer.WriteLine($"<p>{player} closes a book and puts it away.</p>", pc.ConnectionId);
            }
            
            player.Status = CharacterStatus.Status.Standing;
            player.OpenedBook = null;
        }

        private void ReadBook(Player player, int pageNum)
        {
            if (pageNum == player.OpenedBook.Book.Pages.Count)
            {
                Core.Writer.WriteLine($"That exceeds the page count of {player.OpenedBook.Book.Pages.Count}", player.ConnectionId);
                return;
            }

            if (pageNum >= player.OpenedBook.Book.PageCount)
            {

                Core.Writer.WriteLine($"{player.OpenedBook.Name} does not contain that many pages.", player.ConnectionId);

                return;
            }

            if (string.IsNullOrEmpty(player.OpenedBook.Book.Pages[pageNum]))
            {
                Core.Writer.WriteLine("This page is blank.", player.ConnectionId);
                return;
            }

            var result = Markdown.ToHtml(player.OpenedBook.Book.Pages[pageNum]);
            Core.Writer.WriteLine($"{result}", player.ConnectionId);
        }

        private void WriteBook(Player player, int pageNum, string writing)
        {
            if (pageNum == 0)
            {
                Core.Writer.WriteLine($"{player.OpenedBook.Name} has now been titled {writing}.", player.ConnectionId);
                player.OpenedBook.Name = writing;
                Core.UpdateClient.UpdateInventory(player);
                return;
            }

            if (pageNum > player.OpenedBook.Book.PageCount)
            {
                Core.Writer.WriteLine($"{player.OpenedBook.Name} does not contain that many pages.", player.ConnectionId);
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

            Core.UpdateClient.UpdateContentPopUp(player, bookContent);

            Core.Writer.WriteLine($"You begin writing in {player.OpenedBook.Name}.", player.ConnectionId);
        }

        private void RenameBook(Player player, string title)
        {
            Core.Writer.WriteLine($"{player.OpenedBook.Name} has now been titled {title}.", player.ConnectionId);
            player.OpenedBook.Name = title;
            Core.UpdateClient.UpdateInventory(player);
        }
    }
}
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;
using Markdig;

namespace ArchaicQuestII.GameLogic.Commands.Info;

public class BookCmd : ICommand
{
    public BookCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"book"};
        Description = @"Books are useful for information, stories and recording progress. The commands below will tell you how to use books. <br />
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
        Usages = new[] {"commands: book, book open, book close, book read 2, book write 3, book rename My Diary"};
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

        Handler = coreHandler;
    }
        
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICoreHandler Handler { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target1 = input.ElementAtOrDefault(1)?.ToLower();
        var target2 = input.ElementAtOrDefault(2)?.ToLower();

        if (string.IsNullOrEmpty(target1) && player.OpenedBook == null)
        {
            var sb = new StringBuilder();
                
            sb.Append("<ul>Current Books:");
                
            foreach (var item in player.Inventory.Where(item => item.ItemType == Item.Item.ItemTypes.Book))
            {
                sb.Append($"<li>{item.Name}</li>");
            }
                
            sb.Append("</ul>");
                
            Handler.Client.WriteLine(sb.ToString(), player.ConnectionId);
            return;
        }
            
        if (string.IsNullOrEmpty(target1) && player.OpenedBook != null)
        {
            Handler.Client.WriteLine($"<p>You currently have {player.OpenedBook.Name} opened.</p>", player.ConnectionId);
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
                    Handler.Client.WriteLine("<p>What page do you want to write on?</p>", player.ConnectionId);
                    return;
                }
                
                
                if (!int.TryParse(target2, out page))
                {
                    Handler.Client.WriteLine("<p>Page number must be a number.</p>", player.ConnectionId);
                    return;
                }

                WriteBook(player, page, "");
                break;
            default:
                Handler.Client.WriteLine("<p>What are you trying to do? Valid options: Close, Read, Rename, and Write.</p>", player.ConnectionId);
                break;
        }
    
    }

    private void OpenBook(Player player, Room room, string book)
    {
        var nthTarget = Helpers.findNth(book);
        var item = Helpers.findObjectInInventory(nthTarget, player);

        if (item == null)
        {
            Handler.Client.WriteLine("<p>You dont have that book.</p>", player.ConnectionId);
            return;
        }
                
        if (item.ItemType != Item.Item.ItemTypes.Book)
        {
            Handler.Client.WriteLine($"<p>{item.Name} is not a book.</p>", player.ConnectionId);
            return;
        }

        Handler.Client.WriteLine("<p>You open the book and prepare to 'read' or 'write' in it.</p>", player.ConnectionId);
            
        foreach (var pc in room.Players)
        {
            Handler.Client.WriteLine($"<p>{player.Name} gets out a book and opens it.</p>", pc.ConnectionId);
        }
            
        player.OpenedBook = item;
    }

    private void CloseBook(Player player, Room room)
    {
        Handler.Client.WriteLine($"<p>You close {player.OpenedBook.Name} and put it away.</p>", player.ConnectionId);

        foreach (var pc in room.Players.Where(x => x.Id != player.Id))
        {
            Handler.Client.WriteLine($"<p>{player.Name} closes a book and puts it away.</p>", pc.ConnectionId);
        }
            
        player.OpenedBook = null;
    }

    private void ReadBook(Player player, int pageNum)
    {
        if (pageNum == player.OpenedBook.Book.Pages.Count)
        {
            Handler.Client.WriteLine($"<p>That exceeds the page count of {player.OpenedBook.Book.Pages.Count}.</p>", player.ConnectionId);
            return;
        }

        if (pageNum >= player.OpenedBook.Book.PageCount)
        {

            Handler.Client.WriteLine($"<p>{player.OpenedBook.Name} does not contain that many pages.</p>", player.ConnectionId);

            return;
        }

        if (string.IsNullOrEmpty(player.OpenedBook.Book.Pages[pageNum]))
        {
            Handler.Client.WriteLine("<p>This page is blank.</p>", player.ConnectionId);
            return;
        }

        var result = Markdown.ToHtml(player.OpenedBook.Book.Pages[pageNum]);
        Handler.Client.WriteLine($"<p>{result}</p>", player.ConnectionId);
    }

    private void WriteBook(Player player, int pageNum, string writing)
    {

        if (pageNum > player.OpenedBook.Book.PageCount)
        {
            Handler.Client.WriteLine($"<p>{player.OpenedBook.Name} does not contain that many pages.</p>", player.ConnectionId);
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

        Handler.Client.UpdateContentPopUp(player, bookContent);

        Handler.Client.WriteLine($"<p>You begin writing in {player.OpenedBook.Name}.</p>", player.ConnectionId);
    }

    private void RenameBook(Player player, string title)
    {
        Handler.Client.WriteLine($"<p>{player.OpenedBook.Name} has now been titled {title}.</p>", player.ConnectionId);
        var originalBook = player.Inventory.FirstOrDefault(x => x.Name.Equals(player.OpenedBook.Name));

        if (originalBook != null)
        {
            originalBook.Name = title;
        }
            
        player.OpenedBook.Name = title;
           
        Handler.Client.UpdateInventory(player);
    }
}
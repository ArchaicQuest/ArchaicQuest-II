using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArchaicQuestII.API.Controllers.Discord;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
 
namespace ArchaicQuestII.DiscordBot;

public class Bot
{
    private static ICache _cache;
    private static IHubContext<GameLogic.Hubs.GameHub> _hubContext;
    private DiscordSocketClient _client;
    private static readonly HttpClient httpClient = new HttpClient();
    public Bot(ICache cache, IHubContext<GameLogic.Hubs.GameHub> hubContext, DiscordSocketClient client)
    {
        _cache = cache;
        _hubContext = hubContext;
        _client = client;
    }
    
     public async Task MainAsync()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.All
        };
        _client = new DiscordSocketClient(config);
        _client.Log += Log;

       //TODO: Rename to Discord TOKEN here and in the admin project
        var token = _cache.GetConfig().ChannelDiscordWebHookURL;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        _client.Ready += async () =>
        {
            _client.MessageCommandExecuted += MessageCommandHandler;
            _client.UserCommandExecuted += UserCommandHandler;
            _client.MessageReceived += ClientOnMessageReceived;
            
        };
        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    public async Task UserCommandHandler(SocketUserCommand arg)
    {
        Console.WriteLine("User command received!", arg.CommandName);
    }

    public async Task MessageCommandHandler(SocketMessageCommand arg)
    {
        Console.WriteLine("Message command received!", arg.CommandName);
    }
    private async Task ClientOnMessageReceived(SocketMessage socketMessage)
    {
        await Task.Run(async () =>
        {
            //Activity is not from a Bot.
 
            if (!socketMessage.Author.IsBot)
            {

                var username = socketMessage.Author.Username;
                var authorId = socketMessage.Author.Id;
                var channelId  = socketMessage.Channel.Id.ToString();
                var messageId = socketMessage.Id;
                var message = socketMessage.Content;
                
                var channel = _client.GetChannel(Convert.ToUInt64(channelId));
                var socketChannel = (ISocketMessageChannel)channel;

                var discordBotdata = new DiscordBotData() { Channel = "", Message = message, Username = username};
                if (socketChannel.Name == "newbie-chat")
                {
                    discordBotdata.Channel = "newbie";
                }
                
                if (socketChannel.Name == "ooc-chat")
                {
                    discordBotdata.Channel = "ooc";
                }
                
                if (socketChannel.Name == "gossip-chat")
                {
                    discordBotdata.Channel = "gossip";
                }

                if (!string.IsNullOrEmpty(discordBotdata.Channel))
                {
                    await PostToNewbieChannel(discordBotdata);
                }
            }
        });
    }
    
    public async Task<string> PostDataAsync(string url, DiscordBotData data)
    {
        
        var jsonData = JsonConvert.SerializeObject(data);
        
        // create a StringContent object with the JSON data
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        // send a POST request using the static HttpClient instance
        var response = await httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return responseContent;
    }
    
    public async Task PostToNewbieChannel(DiscordBotData data)
    {
        var message = $"<p class='newbie'>[<span>Newbie</span>] {data.Username}: {data.Message}</p>";

        if (data.Channel == "ooc")
        {
            message = $"<p class='ooc'>[<span>OOC</span>] {data.Username}: {data.Message}</p>";
        }
        
        if (data.Channel == "gossip")
        {
            message = $"<p class='gossip'>[<span>Gossip</span>] {data.Username}: {data.Message}</p>";
        }

        foreach (var pc in _cache.GetAllPlayers().Where(x => x.Config.NewbieChannel))
        {
           await SendMessageToClient(message, pc.ConnectionId);
          await UpdateCommunicationAll(message, data.Channel, pc.ConnectionId);
        }
       
    }

    private async Task SendMessageToClient(string message, string id)
    {
        try
        {
            await _hubContext.Clients.Client(id).SendAsync("SendMessage", message, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public async Task UpdateCommunicationAll(string message, string type, string id)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }
        try
        {
            await _hubContext.Clients.Client(id).SendAsync("CommUpdate", message, type);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Config;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ArchaicQuestII.API.Controllers.Discord;

public class DiscordBotData
{
    public string Channel { get; set; }
    public string Message { get; set; }
    public string Username { get; set; }
}


public class DiscordController : Controller
{
    //private  IHubContext<GameHub> _gameHubContext;
    private ICore Core { get; }
    private ICache Cache { get; }
    public DiscordController(ICore core, ICache cache)
    {
        Core = core;
        Cache = cache;
    }
    
           
    [HttpPost]
    [AllowAnonymous]
    [Route("api/discord/updateChannel")]
    public Task<IActionResult> Post([FromBody] DiscordBotData data)
    {
        if (ModelState.IsValid)
        {
            PostToNewbieChannel(data);

        }
        
        return Task.FromResult<IActionResult>(Ok());
    }

    public void PostToNewbieChannel(DiscordBotData data)
    {
        var message = $"<p class='newbie'>[<span>Newbie</span>] {data.Username}: {data.Message}</p>";

        foreach (var pc in Core.Cache.GetAllPlayers().Where(x => x.Config.NewbieChannel))
        {
            Core.Writer.WriteLine(message, pc.ConnectionId);
            Core.UpdateClient.UpdateCommunication(pc, message, data.Channel);
        }
       
    }

 
}
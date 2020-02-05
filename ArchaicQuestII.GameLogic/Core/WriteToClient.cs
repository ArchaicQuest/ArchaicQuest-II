using System;

using ArchaicQuestII.GameLogic.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ArchaicQuestII.GameLogic.Core
{
    public class WriteToClient : IWriteToClient
    {
        private readonly IHubContext<GameHub> _hubContext;
      

        public WriteToClient(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

    

        public async void WriteLine(string message, string id)
        {
          
            try
            {
                
                await _hubContext.Clients.Client(id).SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
    
 
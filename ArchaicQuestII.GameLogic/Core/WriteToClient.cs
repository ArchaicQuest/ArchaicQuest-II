using System;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Hubs;
using ArchaicQuestII.GameLogic.Hubs.Telnet;
using Microsoft.AspNetCore.SignalR;

namespace ArchaicQuestII.GameLogic.Core
{
    public class WriteToClient : IWriteToClient
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly TelnetHub _telnetHub;
      

        public WriteToClient(IHubContext<GameHub> hubContext, TelnetHub telnetHub)
        {
            _hubContext = hubContext;
            _telnetHub = telnetHub;
        }

    

        public async void WriteLine(string message, string id)
        {
          
            try
            {
                if (id.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    return;

                }
 
                await _hubContext.Clients.Client(id).SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void WriteLine(string message, string id, int delay)
        {

            try
            {
               await Task.Delay(delay);
                await  _hubContext.Clients.Client(id).SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async void WriteLine(string message)
        {

            try
            {

                await _hubContext.Clients.All.SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
    
 
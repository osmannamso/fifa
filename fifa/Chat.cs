using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace fifa
{
    public class Chat : Hub
    {      
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Send", message);
        }
    }
}
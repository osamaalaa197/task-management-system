using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.Hubs
{
    public class SendMessageHub:Hub
    {
        public async Task ReceiveMessage(List<string> userIds, string message)
        {
            await Clients.Users(userIds).SendAsync("sendMessage", message);
        }
    }
}

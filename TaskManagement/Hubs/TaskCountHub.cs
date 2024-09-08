using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.Hubs
{
    public class TaskCountHub:Hub
    {
        public async Task UpdateTaskCounter(string userId, int totalCount)
        {
            await Clients.User(userId).SendAsync("ReceiveTaskUpdate", totalCount);
        }
    }
}

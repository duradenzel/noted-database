using Microsoft.AspNetCore.SignalR;

namespace noted_database.Hubs{
    public class NotificationHub : Hub
    {
      public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

    }
}
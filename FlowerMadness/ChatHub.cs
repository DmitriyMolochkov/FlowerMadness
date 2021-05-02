using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;


namespace FlowerMadness
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        static List<Messages> CurrentMessage = new List<Messages>();

        private const string MANAGER = "manager";
        private const string MANAGERS_GROUP = "managers";

        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
        public async Task SendFromManager(string message, string to)
        {
            var userName = Context.UserIdentifier;
            var time = DateTime.Now.ToString();
            if (Context.UserIdentifier != to) // если получатель и текущий пользователь не совпадают
                await Clients.Groups(MANAGERS_GROUP).SendAsync("Receive", message, userName, to, time);
            await Clients.User(to).SendAsync("Receive", message, userName, to, time);

            AddMessageinCache(userName, to, message, time);
        }

        public async Task SendFromUser(string message)
        {
            var to = MANAGER;
            var userName = Context.User.Identity.Name;
            var time = DateTime.Now.ToString();
            
            await Clients.User(Context.UserIdentifier).SendAsync("Receive", message, userName, to, time);
            await Clients.Groups(MANAGERS_GROUP).SendAsync("Receive", message, userName, to, time);

            AddMessageinCache(userName, to, message, time);
        }

        private void AddMessageinCache(string userName, string to, string message, string time)
        {
            CurrentMessage.Add(new Messages { UserName = userName, To = to, Message = message, Time = time});
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var isManager = user.IsInRole("manager") || user.IsInRole("administrator");
            if (isManager)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, MANAGERS_GROUP);
            }
            
            // send to caller
            foreach (var message in CurrentMessage.Where(x => isManager || x.To == Context.UserIdentifier || x.UserName == Context.UserIdentifier))
            {
                await Clients.Caller.SendAsync("Receive", message.Message, message.UserName, message.To, message.Time);
            }

            await Clients.User(Context.UserIdentifier).SendAsync("Notify", $"Привет, {Context.UserIdentifier}!", isManager);
            await base.OnConnectedAsync();
        }
    }
    
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity.Name;
        }
    }

    public class Messages
    {

        public string UserName { get; set; }

        public string To { get; set; }

        public string Message { get; set; }

        public string Time { get; set; }

        public string UserImage { get; set; }

    }
}

using Microsoft.AspNet.SignalR;
using NaftanRailway.BLL.DTO.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaftanRailway.WebUI.Hubs {
    //[HubName("adminHub")]
    public class AdminHub : Hub {
        private static List<UserDTO> Users = new List<UserDTO>();

        public AdminHub() {
        }

        public void Send(string message) {
            var msg = new MessageDTO() {
                MsgText = message,
                SendTime = DateTime.Now,
                User = new UserDTO() { Name = Context.User.Identity.Name, ConnectionId = Context.User.Identity.Name }
            };

            Clients.All.newMessage(msg);
        }

        // Подключение нового пользователя
        public void Connect(string userName) {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id)) {
                Users.Add(new UserDTO { ConnectionId = id, Name = userName });

                // Посылаем сообщение текущему пользователю
                Clients.Caller.onConnected(id, userName, Users);

                // Посылаем сообщение всем пользователям, кроме текущего
                Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
        }

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled) {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null) {
                Users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
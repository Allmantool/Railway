using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaftanRailway.WebUI.Hubs {
    [HubName("adminHub")]
    public class AdminHub : Hub {
        private static List<UserDTO> Users = new List<UserDTO>();
        private readonly IAuthorizationEngage _authLogic;

        public AdminHub(IAuthorizationEngage authLogic) {
            _authLogic = authLogic;
        }

        public void Send(string message) {
            var msg = new MessageDTO() {
                MsgText = message,
                SendTime = DateTime.Now,
                User = new UserDTO() {
                    Name = _authLogic.AdminPrincipal(Context.User.Identity.Name).FullName,
                    ConnectionId = Context.ConnectionId
                }
            };

            Clients.All.newMessage(msg);
        }

        // Подключение нового пользователя
        public override Task OnConnected() {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id)) {
                Users.Add(new UserDTO {
                    ConnectionId = id,
                    Name = _authLogic.AdminPrincipal(Context.User.Identity.Name).FullName
                });

                // Посылаем сообщение текущему пользователю
                Clients.Caller.onConnected(id, Context.User.Identity.Name, Users);

                // Посылаем сообщение всем пользователям, кроме текущего
                Clients.AllExcept(id).onNewUserConnected(id, Context.User.Identity.Name);
            }

            return base.OnConnected();
        }

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled) {
            var id = Context.ConnectionId;
            var item = Users.FirstOrDefault(x => x.ConnectionId == id);

            if (item != null) {
                Users.Remove(item);
                //client side
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
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
    [Authorize(RequireOutgoing = false)]
    public class AdminHub : Hub {
        private static readonly HashSet<UserDTO> Users = new HashSet<UserDTO>();
        private readonly IAuthorizationEngage _authLogic;

        public AdminHub(IAuthorizationEngage authLogic) {
            this._authLogic = authLogic;
        }

        public void Send(string message) {
            var msg = new MessageDTO() {
                MsgText = message,
                SendTime = DateTime.Now,
                User = new UserDTO() {
                    Name = this._authLogic.AdminPrincipal(this.Context.User.Identity.Name).FullName,
                    ConnectionId = this.Context.ConnectionId
                }
            };

            this.Clients.All.newMessage(msg);
            //Clients.Caller.doWork();
            //Clients.Others.doWork();
            //Clients.Users("Rob").doWork();
        }

        // Подключение нового пользователя
        public override Task OnConnected() {
            var id = this.Context.ConnectionId;
            var principalName = this._authLogic.AdminPrincipal(this.Context.User.Identity.Name).FullName;

            if (Users.All(x => x.ConnectionId != id)) {
                Users.Add(new UserDTO {
                    ConnectionId = id,
                    Name = principalName
                });

                // Посылаем сообщение текущему пользователю
                this.Clients.Caller.onConnected(id, principalName, Users);
                 
                // Посылаем сообщение всем пользователям, кроме текущего
                this.Clients.AllExcept(id).onNewUserConnected(id, principalName);
            }

            return base.OnConnected();
        }

        public override Task OnReconnected() {
            var id = this.Context.ConnectionId;
            var principalName = this._authLogic.AdminPrincipal(this.Context.User.Identity.Name).FullName;

            if (Users.All(x => x.ConnectionId != id)) {
                Users.Add(new UserDTO {
                    ConnectionId = id,
                    Name = principalName
                });

                // Посылаем сообщение текущему пользователю
                this.Clients.Caller.onConnected(id, principalName, Users);

                // Посылаем сообщение всем пользователям, кроме текущего
                this.Clients.AllExcept(id).onNewUserConnected(id, principalName);
            }

            return base.OnConnected();
        }

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled) {
            var id = this.Context.ConnectionId;
            var item = Users.FirstOrDefault(x => x.ConnectionId == id);

            if (item != null) {
                Users.Remove(item);
                //client side
                this.Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaftanRailway.WebUI.Hubs {
    //[HubName("adminHub")]
    public class AdminHub : Hub {
        private static List<User> Users = new List<User>();

        public AdminHub() {
        }

        public void Send(string message) {
            Clients.All.newMessage(string.Format("{0} says: {1}", Context.User.Identity.Name, message));
        }

        // Подключение нового пользователя
        public void Connect(string userName) {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id)) {
                Users.Add(new User { ConnectionId = id, Name = userName });

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

    public class User {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
    }
}
using System;
using System.Web;

namespace NaftanRailway.WebUI.Infrastructure {
    public enum SessionStateKeys {
        NAME
    }

    /// <summary>
    ///  It's a class for work with state data (HttpSessionState)
    /// </summary>
    public static class SessionStateHelper {
        public static object Get(SessionStateKeys key) {
            string keyString = Enum.GetName(typeof(SessionStateKeys), key);

            return HttpContext.Current.Session[keyString];
        }

        public static object Set(SessionStateKeys key, object value) {
            string keyString = Enum.GetName(typeof(SessionStateKeys), key);
            var sessionState = HttpContext.Current.Session;

            return sessionState[keyString] = value;
        }
    }
}
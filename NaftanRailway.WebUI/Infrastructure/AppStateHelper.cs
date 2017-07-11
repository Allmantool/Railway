using System;
using System.Web;

namespace NaftanRailway.WebUI.Infrastructure {
    public enum AppStateKeys {
        ONLINE
    }

    public static class AppStateHelper {
        public static object Get(AppStateKeys key, object defaultValue = null) {
            string keyString = Enum.GetName(typeof(AppStateKeys), key);

            if (HttpContext.Current.Application[keyString] == null && defaultValue != null) {
                HttpContext.Current.Application[keyString] = defaultValue;
            }

            return HttpContext.Current.Application[keyString];
        }

        public static object Set(AppStateKeys key, object value) {
            return HttpContext.Current.Application[Enum.GetName(typeof(AppStateKeys), key)] = value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;

namespace NaftanRailway.WebUI.Infrastructure {
    public enum AppStateKeys {
        /// <summary>
        /// It shows how many users are online
        /// </summary>
        ONLINE,
        LAST_REQUEST_TIME,
        LAST_REQUEST_URL
    }

    /// <summary>
    /// It's a class for work with state data (HttpApplicationState)
    /// </summary>
    public static class AppStateHelper {
        public static object Get(AppStateKeys key, object defaultValue = null) {
            var appState = HttpContext.Current.Application;
            string keyString = Enum.GetName(typeof(AppStateKeys), key);

            appState.Lock();

            if (appState[keyString] == null && defaultValue != null) {
                appState[keyString] = defaultValue;
            }

            appState.UnLock();

            return appState[keyString];
        }

        public static object Set(AppStateKeys key, object value) {
            var appState = HttpContext.Current.Application;

            appState.Lock();

            appState[Enum.GetName(typeof(AppStateKeys), key)] = value;

            appState.UnLock();

            return appState[Enum.GetName(typeof(AppStateKeys), key)];
        }

        public static IDictionary<AppStateKeys, object> GetMultiple(params AppStateKeys[] keys) {
            Dictionary<AppStateKeys, object> results = new Dictionary<AppStateKeys, object>();
            var appState = HttpContext.Current.Application;

            appState.Lock();
            foreach (var key in keys) {
                string keyString = Enum.GetName(typeof(AppStateKeys), key);
                results.Add(key, appState[keyString]);
            }
            appState.UnLock();

            return results;
        }

        public static void SetMultiple(IDictionary<AppStateKeys, object> data) {
            var appState = HttpContext.Current.Application;
            appState.Lock();
            foreach (var key in data.Keys) {
                string keyString = Enum.GetName(typeof(AppStateKeys), key);
                appState[keyString] = data[key];
            }
            appState.UnLock();
        }
    }
}
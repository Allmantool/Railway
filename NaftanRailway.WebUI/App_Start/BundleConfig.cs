﻿using System.Web.Optimization;

namespace NaftanRailway.WebUI {
    public static class  BundleConfig {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new ScriptBundle("~/bundles/JQuery1")
                .Include("~/Scripts/jquery-1.9.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQuery2")
                .Include("~/Scripts/jquery-2.1.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Jquery")
                .Include("~/Scripts/jquery-ui-1.11.4.min.js",
                         "~/Scripts/moment.min.js",
                         "~/Scripts/moment-with-locales.min.js",
                         "~/Scripts/bootstrap.min.js",
                         "~/Scripts/bootstrap-datetimepicker.min.js",
                         "~/Scripts/jquery.unobtrusive-ajax.js",
                         "~/Scripts/jquery.validate.min.js",
                         "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserFunctions")
                .Include("~/Scripts/UI_user.js"));

            bundles.Add(new StyleBundle("~/Content/cssbundle")
                .Include("~/Content/font/glyphicons-halflings-regular.woff",new CssRewriteUrlTransform())
                .Include("~/Content/bootstrap.min.css",
                         "~/Content/bootstrap-theme.min.css",
                         "~/Content/bootstrap-datetimepicker.min.css",
                         "~/Content/Bootstrap_AutoComplete.css",
                         "~/Content/jquery.ui.theme.css",
                         "~/Content/jquery.ui.theme.font-awesome.css",
                         "~/Content/font/glyphicons-halflings-regular.eot",
                         "~/Content/font/glyphicons-halflings-regular.svg",
                         "~/Content/font/glyphicons-halflings-regular.ttf",
                         "~/Content/font/glyphicons-halflings-regular.woff",
                         "~/Content/font/glyphicons-halflings-regular.woff2",
                         "~/Content/ErrorStyles.css"));
        }
    }
}

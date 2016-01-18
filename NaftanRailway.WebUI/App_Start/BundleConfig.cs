using System.Web.Optimization;

namespace NaftanRailway.WebUI {
    public static class  BundleConfig {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new ScriptBundle("~/bundles/JQuery1")
                .Include("~/Scripts/jquery-1.9.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQuery2")
                .Include("~/Scripts/jquery-2.2.0.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Jquery")
                .Include("~/Scripts/moment.min.js",
                         "~/Scripts/moment-with-locales.min.js",
                         "~/Scripts/bootstrap.min.js",
                         "~/Scripts/bootstrap-datetimepicker.min.js",
                         "~/Scripts/jquery-ui-1.11.4.min.js",
                         "~/Scripts/jquery.unobtrusive-ajax.js",
                         "~/Scripts/jquery.validate.min.js",
                         "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/BootstrapIE8")
                .Include("~/Scripts/html5shiv.js",
                         "~/Scripts/respond.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/UserFunctions")
                .Include("~/Scripts/UI_user.js"));

            bundles.Add(new StyleBundle("~/Content/cssbundle")
                .Include("~/Content/bootstrap.min.css",
                         "~/Content/bootstrap-theme.min.css",
                         "~/Content/bootstrap-datetimepicker.min.css",
                         "~/Content/Bootstrap_AutoComplete.css",
                         "~/Content/jquery.ui.theme.css",
                         "~/Content/jquery.ui.theme.font-awesome.css",
                         "~/Content/ErrorStyles.css"));
        }
    }
}

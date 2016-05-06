using System.Web.Optimization;

namespace NaftanRailway.WebUI {
    public static class BundleConfig {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new ScriptBundle("~/bundles/JQuery1")
                .Include("~/Scripts/jquery-1.11.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQuery2")
                .Include("~/Scripts/jquery-2.2.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Jquery")
                .Include("~/Scripts/jquery.cookie-1.4.1.min.js",
                         "~/Scripts/bootstrap.min.js",
                         "~/Scripts/bootstrap-datepicker.min.js",
                         "~/Content/locales/bootstrap-datepicker.ru.min.js",
                         "~/Scripts/bootstrap-multiselect.js",
                         "~/Scripts/moment-with-locales.min.js",
                         "~/Scripts/jquery-ui-1.11.4.min.js",
                         "~/Scripts/jquery.validate.min.js",
                         "~/Scripts/jquery.validate.unobtrusive.min.js",
                         "~/Scripts/jquery.unobtrusive-ajax.min.js",
                         "~/Scripts/bundle/html5/jquery.history.js"
                         ));

            bundles.Add(new ScriptBundle("~/bundles/BootstrapIE8")
                .Include("~/Scripts/modernizr-custom.js",
                          "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserFunctions")
                .Include("~/Scripts/UI_user.js"));

            bundles.Add(new StyleBundle("~/Content/cssbundle")
                .Include("~/Content/bootstrap.min.css",
                         "~/Content/bootstrap-multiselect.css",
                         "~/Content/bootstrap-theme.min.css",
                         "~/Content/bootstrap-datetimepicker.min.css",
                         "~/Content/bootstrap-datepicker3.min.css",
                         "~/Content/Bootstrap_AutoComplete.css",
                         "~/Content/jquery.ui.theme.css",
                         "~/Content/jquery.ui.theme.font-awesome.css",
                         "~/Content/ErrorStyles.css",
                         "~/Content/CustomUser.css"));
        }
    }
}
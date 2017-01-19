using System.Collections.Generic;
using System.Web.Optimization;

namespace NaftanRailway.WebUI {
    public static class BundleConfig {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.IgnoreList.Clear();

            //*************************************************************** JS scripts *****************************************************************/
            bundles.Add(new ScriptBundle("~/bundles/Modernizn", "//cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js")
                .Include("~/Scripts/modernizr.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQuery1", "//code.jquery.com/jquery-1.11.3.min.js")
                .Include("~/Scripts/jquery-1.11.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQuery2", "//code.jquery.com/jquery-2.2.4.min.js")
                .Include("~/Scripts/jquery-2.2.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQueryUI").NonOrdering()
                .Include("~/Scripts/jquery.cookie-{version}.min.js",
                         "~/Scripts/jquery-ui-{version}.min.js",
                         "~/Scripts/jquery.validate.min.js",
                         "~/Scripts/jquery.validate.unobtrusive.min.js",
                         "~/Scripts/jquery.unobtrusive-ajax.min.js",
                         "~/Scripts/bundle/html5/jquery.history.js"));

            bundles.Add(new ScriptBundle("~/bundles/BootsrapUI").NonOrdering()
                .Include("~/Scripts/bootstrap.min.js",
                         "~/Scripts/bootstrap-datepicker.min.js",
                         "~/Content/locales/bootstrap-datepicker.ru.min.js",
                         "~/Scripts/bootstrap-multiselect.js",
                         "~/Scripts/moment-with-locales.min.js",
                         "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/BootstrapIE8").NonOrdering()
                .Include("~/Scripts/modernizr-custom.js",
                         "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Knockout", "//ajax.aspnetcdn.com/ajax/knockout/knockout-3.4.0.js").NonOrdering()
                .Include("~/Scripts/knockout-{version}.js",
                         "~/Scripts/knockout.validation.min.js",
                         "~/Scripts/knockout.mapping-latest.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserRail").NonOrdering()
                .Include("~/Scripts/sammy-{version}.js")
                .Include("~/Scripts/GeneralJs/_General.js",
                         "~/Scripts/RailwayJs/_Guild18.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserNomenclature").NonOrdering()
                .Include("~/Scripts/sammy-{version}.js")
                .Include("~/Scripts/GeneralJs/_General.js",
                         "~/Scripts/NomenclatureJs/DataContext.js")
                .IncludeDirectory("~/Scripts/NomenclatureJs/Models/", "*.js")
                .IncludeDirectory("~/Scripts/NomenclatureJs/ViewModels/", "*.js")
                .Include("~/Scripts/NomenclatureJs/MainVM.js")
                .Include("~/Scripts/NomenclatureJs/CustomBindings/koBindings.js")
                .Include("~/Scripts/NomenclatureJs/koExtenders/koExtenders.js")
                .Include("~/Scripts/NomenclatureJs/koComponentsAndCustomElement/koComponents.js")
                .Include("~/Scripts/NomenclatureJs/_Scrolls.js"));

            //*************************************************************** CSS styles *****************************************************************/
            bundles.Add(new StyleBundle("~/Content/CSSbundle").NonOrdering()
                .Include("~/Content/bootstrap.min.css",
                         "~/Content/bootstrap-multiselect.css",
                         "~/Content/bootstrap-theme.min.css",
                         "~/Content/bootstrap-datetimepicker.min.css",
                         "~/Content/bootstrap-datepicker3.min.css",
                         "~/Content/Bootstrap_AutoComplete.css",
                         "~/Content/jquery.ui.theme.css",
                         //conflict with jquery ui icons (dialog close btn)
                         //"~/Content/jquery.ui.theme.font-awesome.css",
                         "~/Content/GeneralCustomCSS/ErrorStyles.css"));

            bundles.Add(new StyleBundle("~/Content/ScrollCSS").NonOrdering()
                .Include("~/Content/GeneralCustomCSS/_General.css",
                         "~/Content/NomenclatureCSS/_Scrolls.css",
                         "~/Content/NomenclatureCSS/CustomJQueryUI.css"));

            bundles.Add(new StyleBundle("~/Content/Rail").NonOrdering()
                .Include("~/Content/GeneralCustomCSS/_General.css",
                         "~/Content/Guild18CSS/_Guild18.css"));

            //Set EnableOptimizations to false for debugging. For more information visit: http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
            bundles.UseCdn = true;
        }
    }

    /// <summary>
    /// To reduce codes during creating bundles, i suggest you create an extension method. Require infrastructure classes:
    /// </summary>
    static class BundleExtentions {
        public static Bundle NonOrdering(this Bundle bundle) {
            bundle.Orderer = new NonOrderingBundleOrderer();

            return bundle;
        }
    }
    /// <summary>
    /// Set up order in bundles
    /// </summary>
    internal class NonOrderingBundleOrderer : IBundleOrderer {
        /// <summary>
        /// Order by existing in bundles tree
        /// </summary>
        /// <param name="context"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files) {
            return files;
        }
    }
}
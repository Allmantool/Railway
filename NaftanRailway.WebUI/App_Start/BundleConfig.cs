using System.Collections.Generic;
using System.Web.Optimization;

namespace NaftanRailway.WebUI {
    public static class BundleConfig {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725

        //Tips:
        //Microsoft.AspNet.Web.Optimization not include *.min.js in budle by default
        //By default, MVC will search for a matching file with .min.js and include that  (not entirely sure if it trys to minify further).
        //If not, it creates a minified version. You can test this by adding the following to BundleConfig.cs
        //You can skip minification simply by creating bundles with no transform, i.e.don't create ScriptBundles, just normal Bundles.
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.IgnoreList.Clear();

//*************************************************************** JS scripts *****************************************************************/
            #region JavaScript Bundles

            bundles.Add(new ScriptBundle("~/bundles/CrossBr", "//cdnjs.cloudflare.com/ajax/libs/modernizr/{version}/modernizr.min.js").NonOrdering()
                .Include(
                    "~/Scripts/modernizr-{version}.js",
                    "~/Scripts/respond.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/JQuery1", "//code.jquery.com/jquery-1.11.3.min.js")
                .Include("~/Scripts/jquery-1.11.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQuery2", "//code.jquery.com/jquery-{version}.min.js")
                .Include("~/Scripts/jquery-2.2.4.js"));

            bundles.Add(new ScriptBundle("~/bundles/JQueryUI").NonOrdering()
                .Include(
					    //~/Scripts/jquery-migrate-{version}.js",
						 "~/Scripts/jquery-ui-{version}.js",
                         "~/Scripts/jquery.validate.js",
                         "~/Scripts/jquery.validate.unobtrusive.js",
                         "~/Scripts/jquery.unobtrusive-ajax.js",
                         "~/Scripts/moment-with-locales.js"
                         //"~/Scripts/bundle/html5/jquery.history.js"
                         ));

            bundles.Add(new ScriptBundle("~/bundles/BootsrapUI").NonOrdering()
                .Include("~/Scripts/bootstrap.js",
                         "~/Scripts/bootstrap-datepicker.js",
                         "~/Content/locales/bootstrap-datepicker.ru.js",
                         "~/Scripts/bootstrap-multiselect.js"));

            //cnd link means replace all bundle ( in this case it will not valid = > because link to one file <> 3 links in bundle
            bundles.Add(new ScriptBundle("~/bundles/Knockout", "//ajax.aspnetcdn.com/ajax/knockout/knockout-{version}.js").NonOrdering()
                .Include("~/Scripts/knockout-{version}.js",
                         //"~/Scripts/knockout.validation.min.js",
                         "~/Scripts/knockout.mapping-latest.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserRail").NonOrdering()
                //.Include("~/Scripts/sammy-{version}.js")
                .Include("~/Scripts/GeneralJs/_General.js",
                         "~/Scripts/RailwayJs/DataContext.js")
                .IncludeDirectory("~/Scripts/RailwayJs/Models/", "*.js")
                .IncludeDirectory("~/Scripts/RailwayJs/ViewModels", "*.js")
                .Include("~/Scripts/RailwayJs/MainVM.js")
                .Include("~/Scripts/RailwayJs/koExtenders/koExtenders.js")
                .Include("~/Scripts/RailwayJs/_Guild18.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserNomenclature").NonOrdering()
                //.Include("~/Scripts/sammy-{version}.js")
                .Include(
                         "~/Scripts/GeneralJs/_General.js",
                         "~/Scripts/NomenclatureJs/DataContext.js")
                .IncludeDirectory("~/Scripts/NomenclatureJs/Models/", "*.js")
                .IncludeDirectory("~/Scripts/NomenclatureJs/ViewModels/", "*.js")
                .Include("~/Scripts/NomenclatureJs/MainVM.js")
                .Include("~/Scripts/NomenclatureJs/CustomBindings/koBindings.js")
                .Include("~/Scripts/NomenclatureJs/koExtenders/koExtenders.js")
                .Include("~/Scripts/NomenclatureJs/koComponentsAndCustomElement/koComponents.js")
                .Include("~/Scripts/NomenclatureJs/_Scrolls.js"));

            #endregion
//*************************************************************** CSS styles *****************************************************************/
            #region CSS Bundles

            bundles.Add(new StyleBundle("~/Content/CSSbundle").NonOrdering()
                .Include("~/Content/bootstrap.css",
                         "~/Content/bootstrap-multiselect.css",
                         "~/Content/bootstrap-theme.css",
                         //"~/Content/bootstrap-datetimepicker.css",
                         "~/Content/bootstrap-datepicker3.css",
                         "~/Content/Bootstrap_AutoComplete.css",
                         "~/Content/jquery.ui.theme.css",
                         //conflict with jquery ui icons (dialog close btn)
                         //"~/Content/jquery.ui.theme.font-awesome.css",
                         "~/Content/GeneralCustomCSS/ErrorStyles.css"));

            bundles.Add(new StyleBundle("~/Content/ScrollCSS").NonOrdering()
                .Include("~/Content/GeneralCustomCSS/_General.css",
                         "~/Content/NomenclatureCSS/_Scrolls.css",
                         "~/Content/NomenclatureCSS/CustomJQueryUI.css"));

            bundles.Add(new StyleBundle("~/Content/RailCSS").NonOrdering()
                .Include("~/Content/GeneralCustomCSS/_General.css",
                         "~/Content/Guild18CSS/_Guild18.css"));

            #endregion

            //Set EnableOptimizations to false for debugging. For more information visit: http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
            bundles.UseCdn = false;
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
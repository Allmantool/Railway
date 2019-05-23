using System.Web.Mvc;
using NaftanRailway.WebUI.ViewModels;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Services;
using log4net;

namespace NaftanRailway.WebUI.Controllers {
    public class NavigationController : BaseController {
        private readonly IRailwayModule _bussinesEngage;

        public NavigationController(IRailwayModule bussinesEngage, ILog logger) : base(logger) {
            this._bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// Menu type operation
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="menuView"></param>
        /// <param name="operationCategory"></param>
        /// <returns></returns>
        public PartialViewResult MenuTypeOperations(SessionStorage storage, InputMenuViewModel menuView, string operationCategory = null) {
            //reload page (save select report date)
            //menuView.ReportPeriod = _businessProvider.SyncActualDate(storage, menuView.ReportPeriod);

            //Передаем динамические типы операций
            //menuView.SelectedOperCategory = operationCategory;
            //menuView.TypesOfOperation = _businessProvider.GetTypeOfOpers(menuView.ReportPeriod);

            return this.PartialView("FlexMenu");
        }

        /// <summary>
        /// Filtering by Date and template shipping number
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="menuView"></param>
        /// <param name="asService"></param>
        /// <returns></returns>
        public PartialViewResult GeneralMenu(SessionStorage storage, InputMenuViewModel menuView, bool asService = false) {
            menuView.ReportPeriod = storage.ReportPeriod;

            //Base controller info
            this.ViewBag.UserName = this.CurrentADUser.Name;
            this.ViewBag.BrowserInfo = this.BrowserInfo;

            return this.PartialView("ComplexNavbarMenu", menuView);
        }
    }
}
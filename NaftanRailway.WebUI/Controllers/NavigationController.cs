using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels.SessionLogic;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    public class NavigationController : Controller {
        private readonly IRailwayModule _bussinesEngage;

        public NavigationController(IRailwayModule bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// Menu type operasion
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="menuView"></param>
        /// <param name="operationCategory"></param>
        /// <returns></returns>
        public PartialViewResult MenuTypeOperations(SessionStorage storage, InputMenuViewModel menuView, string operationCategory = null) {
            //reload page (save select report date)
            menuView.ReportPeriod = _bussinesEngage.SyncActualDate(storage, menuView.ReportPeriod);

            //Передаем динамически типы операций
            menuView.SelectedOperCategory = operationCategory;
            menuView.TypesOfOperation = _bussinesEngage.GetTypeOfOpers(menuView.ReportPeriod);

            return PartialView("FlexMenu", menuView);
        }
        /// <summary>
        /// Filtering by Date and temlate shipping number
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="menuView"></param>
        /// <returns></returns>
        public PartialViewResult GeneralMenu(SessionStorage storage, InputMenuViewModel menuView) {
            menuView.ReportPeriod = storage.ReportPeriod;

            return PartialView("ComplexNavbarMenu", menuView);
        }
    }
}

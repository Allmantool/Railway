using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    public class NavigationController : Controller {
        private readonly IBussinesEngage _bussinesEngage;

        public NavigationController(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// Menu type operasion
        /// </summary>
        /// <param name="menuView"></param>
        /// <param name="operationCategory"></param>
        /// <returns></returns>
        public PartialViewResult MenuTypeOperations(InputMenuViewModel menuView, string operationCategory = null) {
            ViewBag.SelectedCategory = operationCategory;

            var typeOperations = _bussinesEngage.GetGroup<v_otpr, short?>(x => x.oper); 
            //Передаем динамически типы операций
            ViewBag.TypeOperation = typeOperations.AsEnumerable();

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

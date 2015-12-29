﻿using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    public class NavigationController : Controller {
        private readonly IDocumentsRepository _documentRepository;

        public NavigationController(IDocumentsRepository documentRepository) {
            _documentRepository = documentRepository;
        }

        /// <summary>
        /// Menu type operasion
        /// </summary>
        /// <param name="menuView"></param>
        /// <param name="operationCategory"></param>
        /// <returns></returns>
        public PartialViewResult MenuTypeOperations(InputMenuViewModel menuView, string operationCategory = null) {
            ViewBag.SelectedCategory = operationCategory;

            IQueryable<short?> typeOperations = (_documentRepository.ShippinNumbers)
                            .Select(x => x.oper)
                            .Distinct()
                            .OrderBy(x => x);

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

using System;
using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.WebUI.Models;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    [Authorize]
    public class Ceh18Controller : Controller {
        private readonly IDocumentsRepository _documentRepository;
        /// <summary>
        /// Count visible elemnts per one page
        /// </summary>
        private const int PageSize = 8;
        private const int ShiftDay = 3;
        private readonly BussinesEngage _bussinesEngage;

        public Ceh18Controller(IDocumentsRepository documentRepository) {
            _documentRepository = documentRepository;
            _bussinesEngage = new BussinesEngage(PageSize, ShiftDay);
        }

        /// <summary>
        /// Main page with summary information
        /// </summary>
        /// <param name="storage">session storage</param>
        /// <param name="menuView">input menu</param>
        /// <param name="operationCategory">filter category</param>
        /// <param name="page">current page</param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Index(SessionStorage storage, InputMenuViewModel menuView,EnumOperationType operationCategory = EnumOperationType.All, int page = 1) {
            menuView.ShippingChoise = menuView.ShippingChoise ?? "";

            if(menuView.ReportPeriod == null) {
                menuView.ReportPeriod = storage.ReportPeriod;
            }else {storage.ReportPeriod = menuView.ReportPeriod.Value;}

            DateTime chooseDate = new DateTime(menuView.ReportPeriod.Value.Year, menuView.ReportPeriod.Value.Month, 1);

            DispatchListViewModel model = new DispatchListViewModel() {
                Dispatchs = _bussinesEngage.ShippingsViews(_documentRepository, menuView.ShippingChoise, operationCategory, chooseDate, page),
                PagingInfo = new PagingInfo() {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = _bussinesEngage.ShippingsViewsCount(_documentRepository, menuView.ShippingChoise, operationCategory, chooseDate)
                },
                OperationCategory = operationCategory,
                Menu = menuView
            };

            return View(model);
        }

        /// <summary>
        /// Action to responde to post request (for routing system actualy display selecting month)
        /// </summary>
        /// <param name="menuView"></param>
        /// <returns></returns>
        [HttpPost]
        public RedirectToRouteResult Index(InputMenuViewModel menuView) {
            if(menuView.ShippingChoise == "") {
                return RedirectToRoute("Period", new {
                    reportPeriod = menuView.ReportPeriod != null ? menuView.ReportPeriod.Value.ToString("MMyyyy") : null,
                    page =1
                });
            }

            return RedirectToRoute("Path_Full", new {
                reportPeriod = menuView.ReportPeriod != null ? menuView.ReportPeriod.Value.ToString("MMyyyy") : null,
                templateNumber = menuView.ShippingChoise == "" ? null : menuView.ShippingChoise,
                page = 1
            });
        }

        /// <summary>
        /// For shipping number autoComplete
        /// </summary>
        /// <param name="menuView"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchNumberShipping(InputMenuViewModel menuView) {
            if(menuView.ReportPeriod != null) {
                DateTime chooseDate = new DateTime(menuView.ReportPeriod.Value.Year, menuView.ReportPeriod.Value.Month, 1);

                var result = (_documentRepository
                        .ShippinNumbers)
                        .Where(p => p.n_otpr.StartsWith(menuView.ShippingChoise) && 
                            (p.date_oper >= chooseDate.AddDays(-ShiftDay) && p.date_oper <= chooseDate.AddMonths(1).AddDays(ShiftDay)))
                        .GroupBy(g => new { g.n_otpr })
                        .OrderByDescending(p => p.Key.n_otpr)
                        .Select(m => m.Key.n_otpr)
                        .Take(PageSize);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return grouping by oper result
        /// </summary>
        /// <param name="menuView"></param>
        /// <param name="operationCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BadgesCount(InputMenuViewModel menuView, EnumOperationType operationCategory) {
            if(menuView.ReportPeriod != null) {
                DateTime chooseDate = new DateTime(menuView.ReportPeriod.Value.Year, menuView.ReportPeriod.Value.Month, 1);

                var resultGroup = _documentRepository.ShippinNumbers
                    .Where(p => p.n_otpr.StartsWith(menuView.ShippingChoise)
                            && (p.date_oper >= chooseDate.AddDays(-ShiftDay) && p.date_oper <= chooseDate.AddMonths(1).AddDays(ShiftDay))
                                && ((int)operationCategory == 0 || p.oper == (int)operationCategory))
                    .GroupBy(x => new { x.oper })
                    .Select(g => new { g.Key.oper, operCount = g.Count() });

                return Json(resultGroup, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Services;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewsModels;
using NaftanRailway.WebUI.ViewModels;
using System.Web.SessionState;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.BLL.POCO;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewModels;
using System.Linq.Expressions;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    //[ExceptionFilter]
    [SessionState(SessionStateBehavior.Disabled)]
    public class ScrollController : BaseController {
        private readonly INomenclatureModule _bussinesEngage;
        public ScrollController(INomenclatureModule bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// View table krt_Naftan (with infinite scrolling)
        /// For increase jquery perfomance in IE8 method apply paging instead of ajax infinite scrolling
        /// (IsAjaxRequest leave for compability with older version (ajax)
        /// </summary>
        /// <returns></returns>
        [HttpGet, OutputCache(CacheProfile = "AllEvents")]
        //[ActionName("Enumerate")]
        public ActionResult Index(DateTime? period = null, int page = 1, bool asService = false, ushort initialSizeItem = 15) {
            if (Request.IsAjaxRequest()) {
                long recordCount;

                //period = period ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //if period == null => all records
                Expression<Func<ScrollLineDTO, bool>> predicate = x => x.DTBUHOTCHET == period || ((period == null) == true);

                var result = new IndexMV() {
                    ListKrtNaftan = _bussinesEngage.SkipTable<ScrollLineDTO>(page, initialSizeItem, out recordCount, predicate),
                    ReportPeriod = DateTime.Now,
                    PagingInfo = new PagingInfo {
                        CurrentPage = page,
                        ItemsPerPage = initialSizeItem,
                        TotalItems = recordCount,
                        RoutingDictionary = Request.RequestContext.RouteData.Values
                    },
                    RangePeriod = _bussinesEngage.GetListPeriod()
                };

                if (asService) {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return PartialView("_AjaxTableKrtNaftan", result);
            }
            
            //AD DS
            ViewBag.UserName = ADUserName;

            return View();
        }

        /// <summary>
        /// Change Buh Data
        /// </summary>
        /// <param name="model"></param>
        /// <param name="asService"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeDate(PeriodModalMV model, bool asService = false) {
            //Custom value provider binding => TryUpdateModel(model, new FormValueProvider(ControllerContext));
            if (Request.IsAjaxRequest()) {
                var result = _bussinesEngage.ChangeBuhDate(model.Period, model.Item.KEYKRT, model.Multimode);

                if (asService) {
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                return PartialView("_KrtNaftanRows", result);
            }

            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        /// <summary>
        /// Get nomenclature from ORC
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdmitScroll(bool asService = false) {
            if (Request.IsAjaxRequest() && ModelState.IsValid) {
                _bussinesEngage.SyncWithOrc();

                return Index(asService: asService);
            }
            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        /// <summary>
        /// Request from ajax-link and then response json to JqueryFunction(UpdateData)
        /// </summary>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Confirmed(int numberScroll, int reportYear, bool asService = false) {
            string msgError = "";

            if (Request.IsAjaxRequest() && ModelState.IsValid) {
                var result = _bussinesEngage.AddKrtNaftan(numberScroll, reportYear, out msgError);

                if (asService) {
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                return PartialView("_KrtNaftanRows", result);
            }

            TempData["message"] = String.Format(@"Ошибка добавления переченя № {0}. {1}", numberScroll, msgError);

            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        [HttpPost]
        public ActionResult Delete(int numberScroll, int reportYear, bool asService = false) {
            if (Request.IsAjaxRequest() && ModelState.IsValid) {
                var result = _bussinesEngage.DeleteNomenclature(numberScroll, reportYear);

                if (asService) {
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                return PartialView("_KrtNaftanRows", result);
            }

            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// Detail gathering of one scroll 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScrollDetails(int numberScroll, int reportYear, IList<CheckListFilter> filters, int page = 1, byte initialSizeItem = 15, bool asService = false) {
            if (Request.IsAjaxRequest() && ModelState.IsValid) {
                var findKrt = _bussinesEngage.GetNomenclatureByNumber(numberScroll, reportYear);

                if (findKrt != null) {
                    var result = new DetailModelView() {
                        Scroll = findKrt,
                        Filters = filters ?? _bussinesEngage.InitNomenclatureDetailMenu(findKrt.KEYKRT),
                        PagingInfo = new PagingInfo {
                            CurrentPage = page,
                            ItemsPerPage = initialSizeItem,
                            TotalItems = findKrt.RecordCount,
                            RoutingDictionary = Request.RequestContext.RouteData.Values
                        },
                        ListDetails = _bussinesEngage.ApplyNomenclatureDetailFilter(findKrt.KEYKRT, filters, page, initialSizeItem)
                };

                    if (result.ListDetails.Any() && asService) {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    } else {
                        return PartialView("_AjaxTableKrtNaftan_ORC_SAPOD", result);
                    }
                }
            }

            return View();
        }

        //[HttpPost]
        //public ActionResult ScrollDetails(int numberScroll, int reportYear, IList<CheckListFilter> filters, int page = 1, byte initialSizeItem = 15, bool asService = false) {
        //    if (Request.IsAjaxRequest() && ModelState.IsValid) {
        //        var findKrt = _bussinesEngage.GetNomenclatureByNumber(numberScroll, reportYear);

        //        long recordCount;
        //        var srcRows = _bussinesEngage.ApplyNomenclatureDetailFilter(findKrt.KEYKRT, filters, page, initialSizeItem, out recordCount);

        //        var result = new DetailModelView() {
        //            Scroll = findKrt,
        //            Filters = filters,
        //            PagingInfo = new PagingInfo {
        //                CurrentPage = page,
        //                ItemsPerPage = initialSizeItem,
        //                TotalItems = recordCount,
        //                RoutingDictionary = Request.RequestContext.RouteData.Values
        //            },
        //            ListDetails = srcRows
        //        };

        //        if (asService) {
        //            return Json(result, JsonRequestBehavior.DenyGet);
        //        }

        //        return PartialView("_AjaxTableKrtNaftan", result);
        //    }

        //    return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", page } });
        //}

        /// <summary>
        /// General method for donwload files or display report throught SSRS
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
        //[FileDownloadCompleteFilter]
        public ActionResult Reports(string reportName, int numberScroll, int reportYear) {
            const string serverName = @"DB2";
            const string folderName = @"Orders";

            //link to SSRS buil-in repors (default for integer)
            if (numberScroll == 0 || reportYear == 0) {

                return View("Reports", (object)string.Format(@"http://{0}/ReportServer/Pages/ReportViewer.aspx?/{1}/{2}&{3}", serverName, folderName, reportName, @"rs:Command=Render"));
            }

            //get report with parameters
            try {
                return File(_bussinesEngage.GetNomenclatureReports(this, numberScroll, reportYear, serverName, folderName, reportName), @"application/vnd.ms-excel");

            } catch (Exception) {
                TempData[@"message"] = (@"Невозможно вывести отчёт. Ошибка! Возможно не указан перечень");

                return RedirectToAction("Index", "Scroll");
            }
        }

        /// <summary>
        /// Work with JQuery dialog menu
        /// </summary>
        /// <param name="operation">type of operation</param>
        /// <param name="feeKey">KEYSBOR</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GeneralCorrection(EnumMenuOperation operation, long feeKey) {

            if (Request.IsAjaxRequest() && ModelState.IsValid) {
                var result = _bussinesEngage.OperationOnScrollDetail(feeKey, operation);

                switch (operation) {
                    case EnumMenuOperation.Join: return PartialView("_JoinRowsModal", result);
                    case EnumMenuOperation.Edit: return PartialView("_EditRowsModal");
                    case EnumMenuOperation.Delete: break;
                    default: return new EmptyResult();
                }
            }

            return new EmptyResult();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using NaftanRailway.WebUI.Controllers;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Services;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewsModels;
using NaftanRailway.WebUI.ViewModels;
using System.Web.SessionState;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.BLL.POCO;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewModels;
using System.Linq.Expressions;
using NaftanRailway.WebUI.Infrastructure.Filters;
using System.Web;
using NaftanRailway.BLL.DTO.General;
using log4net;
using System.Threading.Tasks;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    //[AllowAnonymous]
    //[AuthorizeAD(Groups = "Rail_Developers, Rail_Users"/*, Users = @"lan\cpn"*/)]
    [SessionState(SessionStateBehavior.Disabled)]
    public class ScrollController : BaseController {
        private readonly INomenclatureModule _bussinesEngage;
        public ScrollController(INomenclatureModule bussinesEngage, ILog log) : base(log) {
            this._bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// View table krt_Naftan (with infinite scrolling)
        /// For increase jquery performance in IE8 method apply paging instead of ajax infinite scrolling
        /// (IsAjaxRequest leave for compatibility with older version (ajax)
        /// </summary>
        /// <returns></returns>
        [HttpGet, OutputCache(CacheProfile = "AllEvents")]
        //[ActionName("Enumerate")]
        public ActionResult Index(DateTime? period = null, int page = 1, bool asService = false, ushort initialSizeItem = 15) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                long recordCount;

                //await _bussinesEngage.SyncWithOrc();

                //period = period ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //if period == null => all records
                Expression<Func<ScrollLineDTO, bool>> predicate = x => x.DTBUHOTCHET == period || ((period == null) == true);

                var result = new IndexMV() {
                    ListKrtNaftan = this._bussinesEngage.SkipTable(page, initialSizeItem, out recordCount, predicate),
                    ReportPeriod = DateTime.Now,
                    PagingInfo = new PagingInfo {
                        CurrentPage = page,
                        ItemsPerPage = initialSizeItem,
                        TotalItems = recordCount,
                        RoutingDictionary = this.Request.RequestContext.RouteData.Values
                    },
                    RangePeriod = this._bussinesEngage.GetListPeriod()
                };

                if (asService) {
                    return this.Json(result, JsonRequestBehavior.AllowGet);
                }

                return this.PartialView("_AjaxTableKrtNaftan", result);
            }

            //Base controller info
            this.ViewBag.UserName = this.CurrentADUser.FullName;
            this.ViewBag.BrowserInfo = this.BrowserInfo;

            return this.View();
        }

        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// Detail gathering of one scroll
        /// </summary>
        /// <returns></returns>
        public ActionResult ScrollDetails(int numberScroll, int reportYear, IList<CheckListFilter> filters, int page = 1, int initialSizeItem = 20, bool viewWrong = false, bool asService = false) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                var findKrt = this._bussinesEngage.GetNomenclatureByNumber(numberScroll, reportYear);

                if (findKrt != null) {
                    long recordCount;
                    var chargeRows = this._bussinesEngage.ApplyNomenclatureDetailFilter(findKrt.KEYKRT, filters, page, initialSizeItem, out recordCount, viewWrong);

                    var result = new DetailModelView() {
                        Scroll = findKrt,
                        Filters = filters ?? this._bussinesEngage.InitNomenclatureDetailMenu(findKrt.KEYKRT),
                        PagingInfo = new PagingInfo {
                            CurrentPage = page,
                            ItemsPerPage = initialSizeItem,
                            TotalItems = recordCount,
                            RoutingDictionary = this.Request.RequestContext.RouteData.Values
                        },
                        ListDetails = chargeRows
                    };

                    if (asService) {
                        return this.Json(result, JsonRequestBehavior.AllowGet);
                    } else {
                        return this.PartialView("_AjaxTableKrtNaftan_ORC_SAPOD", result);
                    }
                }
            }

            return this.RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", page } });
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
            if (this.Request.IsAjaxRequest()) {
                var result = this._bussinesEngage.ChangeBuhDate(model.Period, model.Item.KEYKRT, model.Multimode);

                if (asService) {
                    return this.Json(result, JsonRequestBehavior.DenyGet);
                }

                return this.PartialView("_KrtNaftanRows", result);
            }

            return this.RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        /// <summary>
        /// Get nomenclature from ORC
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AdmitScroll(bool asService = false) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                await this._bussinesEngage.SyncWithOrc();

                return this.Index(asService: asService);
            }
            return this.RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        /// <summary>
        /// Request from Ajax-link and then response json to JqueryFunction(UpdateData)
        /// </summary>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Confirmed(int numberScroll, int reportYear, bool asService = false) {
            string msgError = "";

            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                var result = this._bussinesEngage.AddKrtNaftan(numberScroll, reportYear, out msgError);

                if (asService) {
                    return this.Json(result, JsonRequestBehavior.DenyGet);
                }

                return this.PartialView("_KrtNaftanRows", result);
            }

            this.TempData["message"] = String.Format(@"Ошибка добавления переченя № {0}. {1}", numberScroll, msgError);

            return this.RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        [HttpPost]
        public ActionResult EditCharge(ScrollDetailDTO charge, bool asService = false) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                var result = this._bussinesEngage.EditKrtNaftanOrcSapod(charge);

                if (asService) {
                    return this.Json(result, JsonRequestBehavior.DenyGet);
                }
            }

            Log.Debug($"EditCharge method isn't valid: {this.ModelErrors}.");
            return this.RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        [HttpPost]
        public ActionResult Delete(int numberScroll, int reportYear, bool asService = false) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                var result = this._bussinesEngage.DeleteNomenclature(numberScroll, reportYear);

                if (asService) {
                    return this.Json(result, JsonRequestBehavior.DenyGet);
                }

                return this.PartialView("_KrtNaftanRows", result);
            }

            return this.RedirectToAction("Index", "Scroll", new { page = 1 });
        }

        /// <summary>
        /// General method for donwload files or display report throught SSRS
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
        //[FileDownloadCompleteFilter]
        public async Task<ActionResult> Reports(string reportName, int numberScroll, int reportYear) {
            const string serverName = @"DB2";
            const string folderName = @"Orders";
            var browsInfo = new BrowserInfoDTO() { Name = this.Request.Browser.Browser, Version = this.Request.Browser.Version };

            //link to SSRS buil-in repors (default for integer)
            if (numberScroll == 0 || reportYear == 0) {
                return this.View("Reports", (object)string.Format(
                    @"http://{0}/ReportServer/Pages/ReportViewer.aspx?/{1}/{2}&{3}",
                    serverName, folderName, reportName,
                    @"rs:Command=Render")
                );
            }

            //get report with parameters
            try {
                var typle = await this._bussinesEngage.GetNomenclatureReports(browsInfo, numberScroll, reportYear, serverName, folderName, reportName);
                //name file (with encoding)
                this.Response.AddHeader("Content-Disposition", typle.Item2);
                //For js spinner and complete download callback
                this.Response.Cookies.Clear();
                this.Response.AppendCookie(new HttpCookie("SSRSfileDownloadToken", "true"));

                return this.File(typle.Item1, @"application/vnd.ms-excel");
            } catch (Exception exc) {
                //TempData[@"message"] = (@"Невозможно вывести отчёт. Ошибка! Возможно не указан перечень: " + exc.Message);
                Log.DebugFormat(@"Ошибка при получении отчёта {0}. Oшибка: {1}", reportName, exc.Message);

                //it returns log txt file with error description
                return this.GetLog();
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

            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                var result = this._bussinesEngage.OperationOnScrollDetail(feeKey, operation);

                switch (operation) {
                    //case EnumMenuOperation.Join: return PartialView("_JoinRowsModal", result);
                    case EnumMenuOperation.Edit: return this.PartialView("_EditRowsModal");
                    case EnumMenuOperation.Delete: break;
                    default: return new EmptyResult();
                }
            }

            return new EmptyResult();
        }
    }
}
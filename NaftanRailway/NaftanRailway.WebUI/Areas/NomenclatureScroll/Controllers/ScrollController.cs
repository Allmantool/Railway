using System;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    public class ScrollController : Controller {
        private readonly IBussinesEngage _bussinesEngage;

        public ScrollController(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }

        [HttpGet]
        public ViewResult Index() {
            return View(new IndexModelView() {
                ListKrtNaftan = _bussinesEngage.GetKrt_Naftans,
                ReportPeriod = DateTime.Now
            });
        }

        [HttpPost]
        public RedirectToRouteResult Add(IndexModelView model) {
            if(ModelState.IsValid) {

            }
            else {
                ModelState.AddModelError("Error", @"Неверно указаные значения");
            }

            return RedirectToAction("Index", "Scroll");
        }
    }
}

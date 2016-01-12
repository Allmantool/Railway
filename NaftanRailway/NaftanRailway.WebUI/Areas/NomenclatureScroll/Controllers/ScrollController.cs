using System;
using System.Linq;
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
                ListKrtNaftan = _bussinesEngage.GetKrtNaftans,
                ReportPeriod = DateTime.Now
            });
        }

        [HttpPost]
        public RedirectToRouteResult Add(IndexModelView model) {
            if(ModelState.IsValid && model.ReportPeriod != null && _bussinesEngage.AddKrtNaftan(model.ReportPeriod.Value, model.ListKrtNaftan.FirstOrDefault().KEYKRT)) {
            } else {
                ModelState.AddModelError("Error", @"Неверно указаны значения");
            }

            return RedirectToAction("Index", "Scroll");
        }
    }
}

using System;
using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    public class ScrollController : Controller {
        private readonly IBussinesEngage _bussinesEngage;

        public ScrollController(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }
        /// <summary>
        /// View table krt_Naftan
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Index() {
            return View(new IndexModelView() {
                ListKrtNaftan = _bussinesEngage.GetTable<krt_Naftan>().Take(30).OrderByDescending(x => x.KEYKRT),
                ReportPeriod = DateTime.Now
            });
        }
        /// <summary>
        /// Add scroll in krt_Naftan_ORC_Sopod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public RedirectToRouteResult Add(IndexModelView model) {
            var firstOrDefault = model.ListKrtNaftan.FirstOrDefault();

            if (firstOrDefault != null && (ModelState.IsValid && model.ReportPeriod != null && _bussinesEngage.AddKrtNaftan(model.ReportPeriod.Value, firstOrDefault.KEYKRT))) {
            } else {
                ModelState.AddModelError("Error", @"Неверно указаны значения");
            }

            return RedirectToAction("Index", "Scroll");
        }
        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult ScrollsDetails() {
            return View(_bussinesEngage.GetTable<krt_Naftan_orc_sapod>().Take(100));
        }
    }
}

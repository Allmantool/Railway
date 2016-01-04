using System;
using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    public class ScrollController : Controller {
        private readonly IDocumentsRepository _documentRepository;

        public ScrollController(IDocumentsRepository documentRepository) {
            _documentRepository = documentRepository;
        }

        [HttpGet]
        public ViewResult Index() {
            return View(new IndexModelView() {
                ListKrtNaftan = _documentRepository.KrtNaftans.OrderByDescending(x => x.KEYKRT).ToList(),
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

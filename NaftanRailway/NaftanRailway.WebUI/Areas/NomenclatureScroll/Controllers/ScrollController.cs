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
                ListKrtNaftan = _documentRepository.KrtNaftans.Take(21).OrderByDescending(x => x.KEYKRT).ToList(),
                FilterScroll = ""
            });
        }

        //[HttpPost]
        //public RedirectToRouteResult Add(IndexModelView model) {
        //    if(ModelState.IsValid) {
        //        krt_Naftan krtNaftan = (_documentRepository.OrcKrts).AsEnumerable()
        //            .Where(ok => ok.NKRT.Value == Int32.Parse(model.InputMenu.NumberScroll)).Select(x => new krt_Naftan() {
        //                KEYKRT = x.KEYKRT, DATE_OBRABOT = x.DATE_OBRABOT.Value, NKRT = x.NKRT.Value,
        //                NTREB = x.NTREB.Value, DTTREB = x.DTTREB.Value, DTOPEN = x.DTOPEN, DTCLOSE = x.DTCLOSE,
        //                SMTREB = x.SMTREB.Value, NDSTREB = x.NTREB.Value, U_KOD =(short)x.U_KOD, P_TYPE = x.P_TYPE,
        //                IN_REAL = x.IN_REAL, DTBUHOTCHET = model.InputMenu.PeriodScroll,
        //                RecordCount = (_documentRepository.OrcSbors).Count(os => os.KEYKRT == x.KEYKRT),
        //                StartDate_PER = (_documentRepository.OrcSbors).Where(os => os.KEYKRT == x.KEYKRT).Min(os => os.DT).Value,
        //                EndDate_PER = (_documentRepository.OrcSbors).Where(os => os.KEYKRT == x.KEYKRT).Max(os => os.DT).Value,
        //                SignAdjustment_list = ((_documentRepository.OrcSbors).Where(os => os.NKRT.Contains("S")).Count(os => os.KEYKRT == x.KEYKRT) > 0)? true: false,
        //                Scroll_Sbor = (_documentRepository.OrcSbors).Where(os => os.KEYKRT == x.KEYKRT).ToList().Select(os => string.Join(", ", os.VIDSBR.ToString())).FirstOrDefault()
        //            }).FirstOrDefault();

        //        _documentRepository.AddKrtNaftan(krtNaftan);

        //        return RedirectToAction("Index");
        //    }
        //    else {
        //        ModelState.AddModelError("Error", @"Неверно указаные значения");
        //        return RedirectToAction("Index", "Scroll");
        //    }
        //}
    }
}

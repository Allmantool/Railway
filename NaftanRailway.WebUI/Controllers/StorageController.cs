using System.Web.Mvc;
using NaftanRailway.WebUI.ViewModels;
using NaftanRailway.BLL.Abstract;

namespace NaftanRailway.WebUI.Controllers {
    //[Authorize]
    public class StorageController : Controller {
        private readonly IBussinesEngage _bussinesEngage;
        
        public StorageController(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }
        
        /// <summary>
        /// View PreReport
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ViewResult Index(ISessionStorage storage, string returnUrl) {
            return View(new SessionStorageViewModel {
                Storage = storage,
                ReturnUrl = returnUrl ?? (string)TempData["returnUrl"]
            });
        }
        
        /// <summary>
        /// Add one infoline to session storage
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public RedirectToRouteResult AddRow(ISessionStorage storage, int id, string returnUrl) {
            //ShippingInfoLine line = storage.Lines.FirstOrDefault(sh => sh.Shipping.id == id);
            //if (line == null) {
            //    v_otpr shipping = _bussinesEngage.GetTable<v_otpr,bool>(x=>x.id == id).FirstOrDefault();

            //    if (shipping != null) {
            //        //temp variant(time period)
            //        storage.ReportPeriod = shipping.date_oper ?? DateTime.Today;

            //        short recordCount;
            //        //ShippingInfoLine packDocument = _bussinesEngage.PackDocuments(shipping.n_otpr, out recordCount);

            //        //storage.AddItem(packDocument);
            //    }
            //}
            TempData["returnUrl"] = returnUrl;

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Remove one line from session storage
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public RedirectToRouteResult RemoveRow(ISessionStorage storage, int id, string returnUrl) {
            //ShippingInfoLine line = storage.Lines.FirstOrDefault(sh => sh.Shipping.id == id);

            //if (line != null) {
            //    TempData["message"] = string.Format("Отправка {0} успешно удалена",line.Shipping.n_otpr);
            //    storage.RemoveLine(line.Shipping);
            //}

            return RedirectToAction("Index", new { returnUrl });
        }
        
        /// <summary>
        /// Summary in navigation bar
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public PartialViewResult Summary(ISessionStorage storage, string returnUrl) {
            ViewBag.returnUrl = returnUrl;
            return PartialView(storage);
        }

        [HttpGet]
        public ViewResult EditRow(ISessionStorage storage, int id, string returnUrl) {
            //ShippingInfoLine line = storage.Lines.FirstOrDefault(sh => sh.Shipping.id == id);

            return View(new InfoLineViewModel() {
                DocumentPackLine = null,
                ReturnUrl = returnUrl
            });
        }
        
        /// <summary>
        /// Save edition 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditRow(ISessionStorage storage, InfoLineViewModel line) {
            //storage.Update(line.DocumentPackLine);

            if (ModelState.IsValid) {
                //storage.SaveLine(line.DocumentPackLine);
                TempData["message"] = string.Format("Отправка {0} успешно отредактирована", line.DocumentPackLine.Shipping.n_otpr);

                return RedirectToAction("Index", new { line.ReturnUrl });
            } else {
                // there is something wrong with the data values + mistake
                return View(line);
            }
        }
    }
}
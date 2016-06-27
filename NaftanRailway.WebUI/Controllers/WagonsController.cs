using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.WebUI.Controllers {
    //[Authorize]
    public class WagonsController :Controller {
        /// <summary>
        /// List wagons in current shipping
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult Index(SessionStorage storage,int id) {
            return PartialView(storage.Lines.FirstOrDefault(m => m.Shipping.id == id));
        }
        /// <summary>
        /// Some problem wiht prefix collection item
        /// he must be wagonsNumbers[0...n], but bind prefix don't dimanic
        /// temp solusition pass string paramenter bindPrefix and then updateModel
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="wagon"></param>
        /// <returns></returns>
        public RedirectToRouteResult RemoveWagons(SessionStorage storage,v_o_v wagon) {
            //TryUpdateModel(wagon,bindPrefix);

            ShippingInfoLine line = storage.Lines.FirstOrDefault(l => l.Shipping.id == wagon.id_otpr);

            if (line != null) {
                //line.WagonsNumbers.Remove(line.WagonsNumbers.FirstOrDefault(w => w.id == wagon.id));

                //replace(update)
                storage.RemoveLine(line.Shipping);
                storage.SaveLine(line);
            }

            return RedirectToAction("EditRow","Storage",new {id = wagon.id_otpr });
        }
        /// <summary>
        /// Add wagon
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public RedirectToRouteResult AddWagons(SessionStorage storage) {
            return RedirectToAction("Index");
        }
    }
}

using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.BusinessModels;

namespace NaftanRailway.WebUI.Controllers {
    [Authorize]
    public class ActsController : Controller {
        /// <summary>
        /// List Acts in current shipping
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult Index(SessionStorage storage, int id) {
            return PartialView(storage.Lines.FirstOrDefault(m => m.Shipping.id == id));
        }
    }
}

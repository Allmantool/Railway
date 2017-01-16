namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewsModels {
    /// <summary>
    /// Model for properties configuration view
    /// </summary>
    public class CheckCharges {
        public string Cod { get; set; }
        public string Name { get; set; }
        public bool ChkState { get; set; }

        public CheckCharges() {
            this.ChkState = false;
        }
    }
}
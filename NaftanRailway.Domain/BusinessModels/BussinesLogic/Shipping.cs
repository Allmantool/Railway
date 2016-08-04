using System.Collections.Generic;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.BusinessModels.BussinesLogic {
    /// <summary>
    /// Return general information about despatch (v_otpr,v_o_v,entsg)
    /// </summary>
    public class Shipping 
    {
        //[Column(TypeName = "varchar")]
        private bool IsSelected { get; set; }
        public v_otpr VOtpr { get; set; }
        public etsng Etsng { get; set; }
        public krt_Guild18 Guild18 { get; set; }
        public IEnumerable<v_o_v> Vovs { get; set; }
        public IEnumerable<v_pam> VPams { get; set; }
        public IEnumerable<v_akt> VAkts { get; set; }
        public IEnumerable<v_kart> VKarts { get; set; }
        public IEnumerable<krt_Naftan> KNaftan { get; set; }
        public Shipping() {
            IsSelected = true;
        }
        public override string ToString() {
            return VOtpr.nam_otpr;
        }
    }
}